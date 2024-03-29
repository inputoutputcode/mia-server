﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Game.Server.Engine.Mia.Interface;
using Game.Server.Engine.Mia.Move;
using Game.Server.Scoring;
using Game.Server.Scoring.Interface;
using Game.Server.Register.Interface;
using Game.Server.Logging;
using Game.Server.Engine.Mia.Move.Interface;


namespace Game.Server.Engine.Mia
{
    public class Game : IGame
    {
        #region Members

        private int gameNumber;
        private int turnCount;
        private IPlayerList playerList;
        private IGameScorer gameScorer;
        private IDice currentDice;
        private IDice announcedDice;
        private IDice lastAnnouncedDice;
        private ITurn currentTurn;
        private Guid token;
        private IGameManager gameManager;
        private GamePhase gamePhase;
        private TaskCompletionSource<bool> gameOverCompletion = null;
        private EventHistory eventHistory = new EventHistory();

        private static readonly object lockObject = new object();

        #endregion Members


        #region Properties

        public ITurn CurrentTurn
        {
            get { return currentTurn; }
            private set { }
        }

        public int TurnCount
        {
            get { return turnCount; }
        }

        public List<IPlayer> Players
        {
            get { return playerList.ActivePlayers; }
        }

        public Guid Token
        {
            get { return token; }
        }

        public int GameNumber
        {
            get { return gameNumber; }
        }

        #endregion Properties


        #region Constructor

        public Game(int gameNumber, ScoreMode scoreMode, IGameManager gameManager, IDice dice = null, IGameScorer gameScorer = null)
        {
            this.gameNumber = gameNumber;
            this.gameScorer = gameScorer ?? GameScoreFactory.Create(scoreMode);
            this.gameManager = gameManager;
            currentDice = dice == null ? new Dice() : dice;

            playerList = new PlayerList(Config.Config.Settings.MaximumActivePlayers, Config.Config.Settings.MaximumSpectactors);
            token = Guid.NewGuid();
        }

        #endregion


        #region Methods

        public async Task StartAsync()
        {
            gamePhase = GamePhase.Starting;

            // TODO: Clean up as rounds are handled by GameManager currently. Review multi-tenant design.
            //playerList.RoundReset();
            playerList.Permute();

            Log.Write($"Round '{gameNumber}' starting");

            var players = playerList.RegisteredPlayers.ToArray();
            if (players.Length >= 1)
            {
                // Send ROUND_STARTING
                var serverMove = new ServerMove(ServerMoveCode.ROUND_STARTING, string.Empty, ServerFailureReasonCode.None, players, token);
                eventHistory.Add(serverMove);
                SendServerMessage(serverMove);

                HandleJoinTimeoutAsync();

                gameOverCompletion = new TaskCompletionSource<bool>();
                await gameOverCompletion.Task;

                //GameOver();
            }
            else
            {
                HandleNextRoundWaitTimeAsync();
            }
        }

        public bool Register(IPlayer player)
        {
            bool playerAccepted = false;
            
            // TODO: Fix join procedure
            //if (gamePhase == GamePhase.Starting)
            {
                playerAccepted = playerList.Register(player);

                if (player.CurrentState != PlayerState.Spectator)
                    playerAccepted = playerList.Join(player);
            }

            return playerAccepted;
        }

        public void RoundStarted()
        {
            gamePhase = GamePhase.Started;
            gameScorer.SetActivePlayers(playerList.ActivePlayers);

            var activePlayers = playerList.ActivePlayers.ToArray();
            if (activePlayers.Length < 2)
            {
                ServerFailureReasonCode failureCode;
                if (activePlayers.Length == 0)
                    failureCode = ServerFailureReasonCode.NO_PLAYERS;
                else
                    failureCode = ServerFailureReasonCode.ONLY_ONE_PLAYER;

                // Send ROUND_CANCELLED
                var spectators = playerList.RegisteredPlayers.ToArray();
                var serverMove = new ServerMove(ServerMoveCode.ROUND_CANCELLED, string.Empty, failureCode, spectators, token);
                eventHistory.Add(serverMove);
                SendServerMessage(serverMove);

                gameOverCompletion?.TrySetResult(true);
            }
            else
            {
                var serverMove = new ServerMove(ServerMoveCode.ROUND_STARTED, string.Empty, ServerFailureReasonCode.None, activePlayers, token);
                eventHistory.Add(serverMove);
                SendServerMessage(serverMove);

                // Send YOUR_TURN
                SendYourTurn(playerList.First());
            }
        }

        public virtual void SendServerMessage(IServerMove serverMove)
        {
            string flexibleValuePart = string.Empty;

            if (!string.IsNullOrEmpty(serverMove.Value))
                flexibleValuePart += $"{serverMove.Value};";

            if (serverMove.FailureReasonCode != ServerFailureReasonCode.None)
                flexibleValuePart += $"{serverMove.FailureReasonCode};";

            string eventMessage = $"{serverMove.Code};{flexibleValuePart}{serverMove.Token}";

            gameManager.SendEvent(eventMessage, serverMove.Players);
        }

        public void ReceiveClientEvent(string eventMessage, string eventValue, IPlayer player, Guid token)
        {
            ClientMoveCode moveCode;
            ServerMove serverMove = null;
            Enum.TryParse(eventMessage, out moveCode);
            var playerMove = new ClientMove(moveCode, eventValue, player, token);
            eventHistory.Add(playerMove);

            lock (lockObject)
            {
                // TODO: Validate token, consider implementing turn token
                if (player.CurrentState == PlayerState.Active)
                {
                    switch (playerMove.Code)
                    {
                        case ClientMoveCode.JOIN_ROUND:
                            if (gamePhase == GamePhase.Starting)
                            {
                                bool operationResult = playerList.Join(playerMove.Player);

                                if (!operationResult)
                                {
                                    SendPlayerLost(playerMove.Player, ServerFailureReasonCode.INVALID_TURN);
                                }
                            }
                            break;

                        case ClientMoveCode.ROLL:
                            if (playerMove.Player.Name == playerList.Current().Name && currentTurn.RollCount < 2)
                            {
                                serverMove = new ServerMove(ServerMoveCode.PLAYER_ROLLS, playerMove.Player.Name, ServerFailureReasonCode.None, playerList.RegisteredPlayers.ToArray(), this.token);
                                eventHistory.Add(serverMove);
                                SendServerMessage(serverMove);

                                currentDice.Shake();
                                lastAnnouncedDice = announcedDice;
                                currentTurn.AddRollCount();

                                if (currentTurn.RollCount == 1)
                                {
                                    if (currentDice.IsMia)
                                    {
                                        // TODO: Considering move the decision of raising MIA to the player
                                        var loosingPlayer = playerList.Previous();
                                        serverMove = new ServerMove(ServerMoveCode.PLAYER_LOST, loosingPlayer.Name, ServerFailureReasonCode.MIA, playerList.RegisteredPlayers.ToArray(), this.token);
                                        eventHistory.Add(serverMove);
                                        SendServerMessage(serverMove);

                                        gameScorer.Winner(playerMove.Player);
                                        gameScorer.Lost(loosingPlayer);

                                        GameOver();
                                    }
                                    else
                                    {
                                        var currentPlayer = new IPlayer[] { playerMove.Player };
                                        serverMove = new ServerMove(ServerMoveCode.ROLLED, currentDice.ToString(), ServerFailureReasonCode.None, currentPlayer, this.token);
                                        eventHistory.Add(serverMove);
                                        SendServerMessage(serverMove);
                                    }
                                }
                                else if (currentTurn.RollCount == 2)
                                {
                                    var currentPlayer = new IPlayer[] { playerMove.Player };
                                    serverMove = new ServerMove(ServerMoveCode.ROLLED, "", ServerFailureReasonCode.None, currentPlayer, this.token);
                                    eventHistory.Add(serverMove);
                                    SendServerMessage(serverMove);
                                }

                                HandleTurnTimeoutAsync(playerMove.Player);
                            }
                            else
                            {
                                SendPlayerLost(playerMove.Player, ServerFailureReasonCode.INVALID_TURN);
                            }
                            break;

                        case ClientMoveCode.SEE:
                            if (playerMove.Player.Name == playerList.Current().Name && currentTurn.RollCount == 0)
                            {
                                if (announcedDice == null)
                                {
                                    SendPlayerLost(playerMove.Player, ServerFailureReasonCode.SEE_BEFORE_FIRST_ROLL);
                                }
                                else
                                {
                                    serverMove = new ServerMove(ServerMoveCode.PLAYER_WANTS_TO_SEE, playerMove.Player.Name, ServerFailureReasonCode.None, playerList.RegisteredPlayers.ToArray(), this.token);
                                    eventHistory.Add(serverMove);
                                    SendServerMessage(serverMove);

                                    serverMove = new ServerMove(ServerMoveCode.ACTUAL_DICE, currentDice.ToString(), ServerFailureReasonCode.None, playerList.RegisteredPlayers.ToArray(), this.token);
                                    eventHistory.Add(serverMove);
                                    SendServerMessage(serverMove);

                                    IPlayer looserPlayer;
                                    IPlayer winnerPlayer;
                                    ServerFailureReasonCode reasonCode = ServerFailureReasonCode.None;

                                    if (announcedDice.IsHigherOrEqual(currentDice))
                                    {
                                        looserPlayer = playerList.Previous();
                                        winnerPlayer = playerMove.Player;
                                        reasonCode = ServerFailureReasonCode.CAUGHT_BLUFFING;
                                    }
                                    else
                                    {
                                        looserPlayer = playerMove.Player;
                                        winnerPlayer = playerList.Previous();
                                    }

                                    gameScorer.Lost(looserPlayer);
                                    gameScorer.Winner(winnerPlayer);

                                    serverMove = new ServerMove(ServerMoveCode.PLAYER_LOST, looserPlayer.Name, reasonCode, playerList.RegisteredPlayers.ToArray(), this.token);
                                    eventHistory.Add(serverMove);
                                    SendServerMessage(serverMove);

                                    GameOver();
                                }
                            }
                            else
                            {
                                SendPlayerLost(playerMove.Player, ServerFailureReasonCode.INVALID_TURN);
                            }
                            break;

                        case ClientMoveCode.ANNOUNCE:
                            announcedDice = currentDice.Parse(playerMove.Value);
                            if (playerMove.Player.Name == playerList.Current().Name &&
                                currentTurn.RollCount <= 2 &&
                                announcedDice != null &&
                                announcedDice.IsValid)
                            {
                                string broadcastValue = $"{playerMove.Player.Name};{announcedDice}";
                                serverMove = new ServerMove(ServerMoveCode.ANNOUNCED, broadcastValue, ServerFailureReasonCode.None, playerList.RegisteredPlayers.ToArray(), this.token);
                                eventHistory.Add(serverMove);
                                SendServerMessage(serverMove);

                                if (announcedDice.IsMia)
                                {
                                    // TODO: Implement if next player does not want to SEE for Mia to get less point reduction
                                    if (currentDice.IsMia)
                                    {
                                        var loosingPlayer = playerList.Previous();

                                        var failureReasonCode = ServerFailureReasonCode.MIA;
                                        serverMove = new ServerMove(ServerMoveCode.PLAYER_LOST, loosingPlayer.Name, failureReasonCode, playerList.RegisteredPlayers.ToArray(), this.token);
                                        eventHistory.Add(serverMove);
                                        SendServerMessage(serverMove);

                                        gameScorer.Winner(playerMove.Player);
                                        gameScorer.Lost(loosingPlayer);

                                        GameOver();
                                    }
                                    else
                                    {
                                        var nextPlayer = playerList.Next();

                                        var failureReasonCode = ServerFailureReasonCode.LIED_ABOUT_MIA;
                                        serverMove = new ServerMove(ServerMoveCode.PLAYER_LOST, playerMove.Player.Name, failureReasonCode, playerList.RegisteredPlayers.ToArray(), this.token);
                                        eventHistory.Add(serverMove);
                                        SendServerMessage(serverMove);

                                        gameScorer.Winner(nextPlayer);
                                        gameScorer.Lost(playerMove.Player);

                                        GameOver();
                                    }
                                }
                                else if (lastAnnouncedDice == null)
                                {
                                    var nextPlayer = playerList.Next();
                                    SendYourTurn(nextPlayer);
                                }
                                else  if (announcedDice.IsHigherThan(lastAnnouncedDice))
                                {
                                    var nextPlayer = playerList.Next();
                                    SendYourTurn(nextPlayer);
                                }
                                else
                                {
                                    SendPlayerLost(playerMove.Player, ServerFailureReasonCode.ANNOUNCED_LOSING_DICE);
                                }
                            }
                            else
                            {
                                SendPlayerLost(playerMove.Player, ServerFailureReasonCode.INVALID_TURN);
                            }
                            break;
                    }
                }
            }
        }

        private async void HandleNextRoundWaitTimeAsync()
        {
            Log.Write($"Not enough players, wait till next round (wait time: {Config.Config.Settings.RegistrationTimeOut}).");

            await Task.Delay(Config.Config.Settings.RegistrationTimeOut);
        }

        private async void HandleJoinTimeoutAsync()
        {
            int joinTimeOut = Config.Config.Settings.JoinTimeOut;
            await Task.Delay(joinTimeOut);

            RoundStarted();
        }

        private async void HandleTurnTimeoutAsync(IPlayer player)
        {
            int turnTimeOut = Config.Config.Settings.TurnTimeOut;
            await Task.Delay(turnTimeOut);

            lock (lockObject)
            {
                var currentPlayer = playerList.Current();
                if (currentPlayer != null && player.Name == currentPlayer.Name && player.CurrentState == PlayerState.Active)
                {
                    SendPlayerLost(player, ServerFailureReasonCode.DID_NOT_TAKE_TURN);
                }
            }
        }

        private void SendYourTurn(IPlayer player)
        {
            NewTurn(player);

            var singlePlayerList = new IPlayer[] { player };
            var serverMove = new ServerMove(ServerMoveCode.YOUR_TURN, string.Empty, ServerFailureReasonCode.None, singlePlayerList, token);
            eventHistory.Add(serverMove);
            SendServerMessage(serverMove);

            // TODO: Send data to spectators

            HandleTurnTimeoutAsync(player);
        }

        private void NewTurn(IPlayer player)
        {
            currentTurn = new Turn(player);
            turnCount += 1;
            token = Guid.NewGuid();
        }

        private void SendPlayerLost(IPlayer player, ServerFailureReasonCode reasonCode)
        {
            gameScorer.Lost(player);
            var serverMove = new ServerMove(ServerMoveCode.PLAYER_LOST, player.Name, reasonCode, playerList.RegisteredPlayers.ToArray(), token);
            eventHistory.Add(serverMove);
            SendServerMessage(serverMove);

            playerList.Kick(player);
            if (playerList.ActivePlayers.Count < Config.Config.Settings.MinimumPlayerCount)
            {
                GameOver();
            }
            else
            {
                var nextPlayer = playerList.Current();
                SendYourTurn(nextPlayer);
            }
        }

        public List<IPlayer> GetScore()
        {
            return gameScorer.GetScores();
        }

        public void GameOver()
        {
            playerList.RoundReset();
            var scoreValues = gameScorer.GetScoreValues();

            var serverMove = new ServerMove(ServerMoveCode.SCORE, scoreValues, ServerFailureReasonCode.None, playerList.RegisteredPlayers.ToArray(), token);
            eventHistory.Add(serverMove);
            SendServerMessage(serverMove);

            gameOverCompletion?.TrySetResult(true);
        }

        #endregion
    }
}
