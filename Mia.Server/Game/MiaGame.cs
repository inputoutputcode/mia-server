using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Linq;
using System.Threading;

using Mia.Server.Command;
using Mia.Server.Logging;
using Mia.Server.Messaging;


namespace Mia.Server
{
    public class MiaGame
    {
        #region Members

        private int roundCount;
        private bool stopped;
        private PlayerList players = new PlayerList();
        private DiceRoller diceRoller = new DiceRoller();
        private Dice actualDice;
        private Dice announcedDice;
        private CurrentPlayers currentPlayers;
        private ScoreList scoreList;
        private Turn currentTurn;
        private Guid token;
        private MessageServer messageServer;

        #endregion Members


        #region Properties

        public int RoundCount
        {
            get { return roundCount; }
        }

        public bool Stopped
        {
            get { return stopped; }
        }

        public PlayerList Players
        {
            get { return players; }
        }

        public Guid Token
        {
            get { return token; }
        }

        public Turn Turn
        {
            get { return currentTurn; }
        }

        #endregion Properties


        #region Constructor

        public MiaGame(MessageServer messageServer, ScoreList scoreList)
        {
            this.messageServer = messageServer;
            this.scoreList = scoreList;
            players = scoreList.PlayerScores;

            token = Guid.NewGuid();
        }

        #endregion Constructor


        #region Methods

        public void Start()
        {
            JoinStarting();
            JoinStarted();
            GameStarting();
        }

        public void JoinStarting()
        {
            messageServer.SendMessageToAll(ServerCommand.ROUND_STARTING + ";" + token);

            var tracker = new TimeOutTracker(Config.JoinTimeOut);
            while (tracker.IsValid)
            {
                Thread.Sleep(20);
            }
        }

        public void JoinStarted()
        {
            string message = string.Empty;

            if (players.RealPlayers.Count == 0)
            {
                message = ServerCommand.ROUND_CANCELED + ";" + ServerCommandArguments.NO_PLAYERS;
                stopped = true;
            }
            else if (players.RealPlayers.Count < 2)
            {
                message = ServerCommand.ROUND_CANCELED + ";" + ServerCommandArguments.ONLY_ONE_PLAYER;
                stopped = true;
            }
            else
            {
                string playerNames = string.Empty;
                foreach (Player player in players.RealPlayers)
                    playerNames += player.Name + ",";
                playerNames = playerNames.Remove(playerNames.Length - 1);

                message = ServerCommand.ROUND_STARTED + ";" + scoreList.GameCount + ";" + playerNames;
            }

            messageServer.SendMessageToAll(message);
        }

        public void GameStarting()
        {
            if (stopped)
                return;

            GameStarted();
        }

        public void GameStarted()
        {
            // count game
            scoreList.AddGameCount();

            // generate new order
            players.Permute();

            // reset dices
            actualDice = null;
            announcedDice = null;

            // save point for attending the game
            foreach (Player player in players.RealPlayers)
            {
                scoreList.IncreaseFor(player);
            }

            while (!Stopped)
            {
                if (RoundCount == 0)
                {
                    FirstTurn();
                }
                else
                {
                    NextTurn();
                }

                var player = currentPlayers.Current;
                var timeOut = new TimeOutTracker(Config.TurnTimeOut);
                while (timeOut.IsValid && player.CurrentState == PlayerState.Active)
                {
                    Thread.Sleep(20);
                }

                if (player.CurrentState == PlayerState.Active && !Stopped)
                {
                    var failure = Turn.HasAnnounced ? FailureCommand.DID_NOT_ANNOUNCE : FailureCommand.DID_NOT_TAKE_TURN;

                    messageServer.SendMessageToAll(ServerCommand.PLAYER_LOST + ";" + player.Name + ";" + failure);

                    scoreList.DecreaseFor(player);

                    BroadCastScore();
                    stopped = true;
                }
            }
        }

        private void FirstTurn()
        {
            currentPlayers = new CurrentPlayers() { Current = players.RealPlayers[0], Last = null };

            StartTurn();
        }

        private void NextTurn()
        {
            currentPlayers = players.NextPlayer();

            if (currentPlayers.Current == players.RealPlayers[0])
            {
                roundCount++;
            }

            StartTurn();
        }

        private void StartTurn()
        {
            var player = currentPlayers.Current;

            currentTurn = new Turn(player);
            player.ChangeState(PlayerState.Active);
            player.YourTurn(currentTurn.Token);
        }

        public void See(Player player)
        {
            if (!Stopped)
            {
                string message = ServerCommand.PLAYER_WANTS_TO_SEE + ";" + player.Name;
                messageServer.SendMessageToAll(message);

                if (actualDice == null)
                {
                    messageServer.SendMessageToAll(ServerCommand.PLAYER_LOST + ";" + player.Name + ";" + FailureCommand.SEE_BEFORE_FIRST_ROLL);

                    scoreList.DecreaseFor(player);
                }
                else
                {
                    message = ServerCommand.ACTUAL_DICE + ";" + actualDice.ToString();
                    messageServer.SendMessageToAll(message);

                    if (announcedDice.IsHigherThan(actualDice))
                    {
                        message = ServerCommand.PLAYER_LOST + ";" + currentPlayers.Last.Name + ";" + FailureCommand.CAUGHT_BLUFFING;
                        messageServer.SendMessageToAll(message);

                        scoreList.IncreaseFor(currentPlayers.Current);
                        scoreList.DecreaseFor(currentPlayers.Last);
                    }
                    else
                    {
                        message = ServerCommand.PLAYER_LOST + ";" + currentPlayers.Current.Name + ";" + FailureCommand.SEE_FAILED;
                        messageServer.SendMessageToAll(message);

                        scoreList.IncreaseFor(currentPlayers.Last);
                        scoreList.DecreaseFor(currentPlayers.Current);
                    }
                }

                BroadCastScore();
                stopped = true;
            }
        }

        public void Announce(Player player, Dice dice)
        {
            announcedDice = dice;

            string messageToAll = ServerCommand.ANNOUNCED + ";" + player.Name + ";" + dice;
            messageServer.SendMessageToAll(messageToAll);

            if (actualDice.IsHigherThan(announcedDice))
            {
                messageServer.SendMessageToAll(ServerCommand.PLAYER_LOST + ";" + player.Name + ";" + FailureCommand.ANNOUNCED_LOSING_DICE);
                scoreList.DecreaseFor(player);

                BroadCastScore();
                stopped = true;
            }
            else
            {
                if (dice.IsMia)
                {
                    MiaIsAnnounced(player);
                }
            }

            player.ChangeState(PlayerState.Inactive);
        }

        public void Roll(Player player)
        {
            if (player == currentPlayers.Current && currentTurn.RollCount < 3)
            {
                currentTurn.AddRollCount();

                // PLAYER_ROLLS;playerName
                string broadCastMessage = ServerCommand.PLAYER_ROLLS.ToString() + ";" + player.Name;
                messageServer.SendMessageToAll(broadCastMessage);

                actualDice = diceRoller.Roll();

                // ROLLED;dice;token
                string message = ServerCommand.ROLLED.ToString() + ";" + actualDice.DieOne + actualDice.DieTwo + ";" + currentTurn.Token;
                messageServer.SendAndWaitForAnswer(message, currentTurn.Token, player.UdpState);
            }
            else
            { 
                InvalidTurn(player);
            }
        }

        private void MiaIsAnnounced(Player sender)
        {
            if (actualDice.IsMia)
            {
                messageServer.SendMessageToAll(ServerCommand.PLAYER_LOST + ";" + currentPlayers.Last + ";" + FailureCommand.MIA);

                var loosers = players.RealPlayers.FindAll(x => x.Name != currentPlayers.Current.Name);
                var winners = new List<Player> { currentPlayers.Current };
                ScoreWinnersAndLoosers(FailureCommand.MIA, loosers, winners);
            }
            else
            {
                messageServer.SendMessageToAll(ServerCommand.PLAYER_LOST + ";" + currentPlayers.Current + ";" + FailureCommand.LIED_ABOUT_MIA);

                var winners = players.RealPlayers.FindAll(x => x.Name != currentPlayers.Last.Name);
                var loosers = new List<Player> { currentPlayers.Last };
                ScoreWinnersAndLoosers(FailureCommand.LIED_ABOUT_MIA, loosers, winners);
            }

            stopped = true;
        }

        private void BroadCastScore()
        {
            messageServer.SendMessageToAll(ServerCommand.SCORE + ";" + scoreList.AllScores());

            stopped = true;
        }

        private void ScoreWinnersAndLoosers(FailureCommand failureCommand, List<Player> loosers, List<Player> winners)
        {
            foreach (Player player in loosers)
            {
                scoreList.DecreaseFor(player);
            }

            foreach (Player player in winners)
            {
                scoreList.IncreaseFor(player);
            }

            BroadCastScore();
        }

        public void InvalidTurn(Player player)
        {
            Logger.Log(string.Format("Invalid turn by {0}. Decreasing one point.", player.Name));
            messageServer.SendInfo(ServerCommand.INVALID_TURN.ToString(), player.UdpState);
            scoreList.DecreaseFor(player);
        }

        #endregion Methods
    }
}
