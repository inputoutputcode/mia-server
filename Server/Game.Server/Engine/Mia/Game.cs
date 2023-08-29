using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Game.Server.Config;
using Game.Server.Engine.Mia.Interface;
using Game.Server.Engine.Mia.Move;
using Game.Server.Engine.Mia.Move.Interface;
using Game.Server.Scoring;
using Game.Server.Scoring.Interface;
using Game.Server.Register.Interface;
using Game.Server.Logging;


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
        private ITurn currentTurn;
        private Guid token;
        private bool isSimulation;
        private IGameManager gameManager;
        private GamePhase gamePhase;
        private TaskCompletionSource<bool> gameOverCompletion = null;

        #endregion Members


        #region Properties

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

        public bool IsSimulation
        {
            get { return isSimulation; }
        }

        public int GameNumber
        {
            get { return gameNumber; }
        }

        #endregion Properties


        #region Constructor

        public Game(int gameNumber, ScoreMode scoreMode, IGameManager gameManager, bool isSimulation = false)
        {
            this.gameNumber = gameNumber;
            gameScorer = GameScoreFactory.Create(scoreMode);
            this.gameManager = gameManager;
            currentDice = new Dice();

            playerList = new PlayerList(Config.Config.Settings.MaximumActivePlayers, Config.Config.Settings.MaximumSpectactors);
            token = Guid.NewGuid();
        }

        #endregion

        public void CreateSimulation(IPlayerList players, ITurn turn, IDice dice)
        {
            playerList = players;
            currentTurn = turn;
            currentDice = dice;
        }

        #region Methods

        public async Task StartAsync()
        {
            gamePhase = GamePhase.Starting;

            // TODO: Clean up as rounds are handled by GameManager currently. Review multi-tenant design.
            //playerList.RoundReset();
            playerList.Permute();

            Log.Write($"Round '{gameNumber}' starting");

            var players = playerList.RegisteredPlayers.ToArray();
            if (players.Length > 1)
            {
                // Send ROUND_STARTING
                var serverMove = new ServerMove(ServerMoveCode.ROUND_STARTING, string.Empty, ServerFailureReasonCode.None, players, token);
                gameManager.ProcessMove(serverMove);

                HandleJoinTimeoutAsync();

                gameOverCompletion = new TaskCompletionSource<bool>();
                await gameOverCompletion.Task;
            }
            else
            {
                Log.Write($"Not enough players, wait till next round.");

                HandleNextRoundWaitTimeAsync();
            }
        }

        public bool Register(IPlayer player)
        {
            bool isRegistered = playerList.Register(player);
            isRegistered = playerList.Join(player);

            Log.Write($"Player '{player.Name}' registered for the game.");

            return isRegistered;
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
                gameManager.ProcessMove(serverMove);
                Log.Write($"Round '{gameNumber}' cancelled");
            }
            else
            {
                var serverMove = new ServerMove(ServerMoveCode.ROUND_STARTED, string.Empty, ServerFailureReasonCode.None, activePlayers, token);
                gameManager.ProcessMove(serverMove);
                Log.Write($"Round '{gameNumber}' started ");

                // Send YOUR_TURN
                SendPlayerTurn(playerList.First());
            }
        }

        public void Move(IPlayerMove playerMove)
        {
            switch (playerMove.Code)
            {
                case PlayerMoveCode.JOIN_ROUND:
                    if (gamePhase == GamePhase.Starting)
                    {
                        bool operationResult = playerList.Join(playerMove.Player);

                        if (!operationResult)
                        {
                            SendPlayerLost(playerMove.Player, ServerFailureReasonCode.INVALID_TURN);
                        }
                    }
                    break;

                case PlayerMoveCode.ROLL:
                    if (playerMove.Player.Name == playerList.Current().Name && currentTurn.RollCount <= 1)
                    {
                        var serverMoveRolls = new ServerMove(ServerMoveCode.PLAYER_ROLLS, playerMove.Player.Name, ServerFailureReasonCode.None, playerList.RegisteredPlayers.ToArray(), token);
                        gameManager.ProcessMove(serverMoveRolls);

                        currentDice.Shake();
                        currentTurn.AddRollCount();

                        if (currentTurn.RollCount == 1)
                        {
                            if (currentDice.IsMia)
                            {
                                var loosingPlayer = playerList.Previous();
                                var serverMoveMia = new ServerMove(ServerMoveCode.PLAYER_LOST, loosingPlayer.Name, ServerFailureReasonCode.MIA, playerList.RegisteredPlayers.ToArray(), token);
                                gameManager.ProcessMove(serverMoveMia);

                                gameScorer.Winner(playerMove.Player);
                                gameScorer.Looser(loosingPlayer);

                                GameOver();
                            }
                            else
                            {
                                var currentPlayer = new IPlayer[] { playerMove.Player };
                                var serverMoveRolled = new ServerMove(ServerMoveCode.ROLLED, currentDice.ToString(), ServerFailureReasonCode.None, currentPlayer, token);
                                gameManager.ProcessMove(serverMoveRolled);
                            }
                        }

                        HandleTurnTimeoutAsync(playerMove.Player);
                    }
                    else
                    {
                        SendPlayerLost(playerMove.Player, ServerFailureReasonCode.INVALID_TURN);
                    }
                    break;

                case PlayerMoveCode.SEE:
                    if (playerMove.Player.Name == playerList.Current().Name)
                    {
                        if (announcedDice == null)
                        {
                            SendPlayerLost(playerMove.Player, ServerFailureReasonCode.SEE_BEFORE_FIRST_ROLL);
                        }
                        else
                        {
                            var serverMoveSee = new ServerMove(ServerMoveCode.PLAYER_WANTS_TO_SEE, playerMove.Player.Name, ServerFailureReasonCode.None, playerList.RegisteredPlayers.ToArray(), token);
                            gameManager.ProcessMove(serverMoveSee);

                            var serverMoveDice = new ServerMove(ServerMoveCode.ACTUAL_DICE, currentDice.ToString(), ServerFailureReasonCode.None, playerList.RegisteredPlayers.ToArray(), token);
                            gameManager.ProcessMove(serverMoveDice);

                            IPlayer looserPlayer;
                            IPlayer winnerPlayer;
                            ServerFailureReasonCode reasonCode = ServerFailureReasonCode.None;

                            if (announcedDice.IsHigherThan(currentDice))
                            {
                                looserPlayer = playerMove.Player;
                                winnerPlayer = playerList.Previous();
                            }
                            else
                            {
                                looserPlayer = playerList.Previous();
                                winnerPlayer = playerMove.Player;
                                reasonCode = ServerFailureReasonCode.CAUGHT_BLUFFING;
                            }

                            gameScorer.Lost(looserPlayer);
                            gameScorer.Winner(winnerPlayer);

                            var serverMoveTurn = new ServerMove(ServerMoveCode.PLAYER_LOST, looserPlayer.Name, reasonCode, playerList.RegisteredPlayers.ToArray(), token);
                            gameManager.ProcessMove(serverMoveTurn);
                            Log.Write($"Send PLAYER_LOST for '{looserPlayer.Name}'");

                            GameOver();
                        }
                    }
                    else
                    {
                        SendPlayerLost(playerMove.Player, ServerFailureReasonCode.INVALID_TURN);
                    }
                    break;

                case PlayerMoveCode.ANNOUNCE:
                    if (playerMove.Player.Name == playerList.Current().Name && currentTurn.RollCount <= 2)
                    {
                        var announcedDice = currentDice.Parse(playerMove.Value);

                        if (announcedDice.IsMia)
                        {
                            if (currentDice.IsMia)
                            {
                                var loosingPlayer = playerList.Previous();
                                var serverMoveMia = new ServerMove(ServerMoveCode.PLAYER_LOST, loosingPlayer.Name, ServerFailureReasonCode.MIA, playerList.RegisteredPlayers.ToArray(), token);
                                gameManager.ProcessMove(serverMoveMia);

                                gameScorer.Winner(playerMove.Player);
                                gameScorer.Looser(loosingPlayer);

                                GameOver();
                            }
                            else
                            {
                                var nextPlayer = playerList.Next();
                                var serverMoveMia = new ServerMove(ServerMoveCode.PLAYER_LOST, playerMove.Player.Name, ServerFailureReasonCode.LIED_ABOUT_MIA, playerList.RegisteredPlayers.ToArray(), token);
                                gameManager.ProcessMove(serverMoveMia);

                                gameScorer.Winner(nextPlayer);
                                gameScorer.Looser(playerMove.Player);

                                GameOver();
                            }
                        }
                        else if (announcedDice.IsHigherThan(currentDice))
                        {
                            string broadcastValue = $"{playerMove.Player.Name};{announcedDice}";
                            var serverMoveAnnounced = new ServerMove(ServerMoveCode.ANNOUNCED, broadcastValue, ServerFailureReasonCode.None, playerList.RegisteredPlayers.ToArray(), token);
                            gameManager.ProcessMove(serverMoveAnnounced);

                            var nextPlayer = playerList.Next();
                            SendPlayerTurn(nextPlayer);
                        }
                        else
                        {
                            SendPlayerLost(playerMove.Player, ServerFailureReasonCode.ANNOUNCED_LOSING_DICE);

                            var nextPlayer = playerList.Next();
                            SendPlayerTurn(nextPlayer);
                        }
                    }
                    else
                    {
                        SendPlayerLost(playerMove.Player, ServerFailureReasonCode.INVALID_TURN);
                    }
                    break;
            }
        }

        private async void HandleNextRoundWaitTimeAsync()
        {
            await Task.Delay(Config.Config.Settings.RegistrationTimeOut);
        }

        private async void HandleJoinTimeoutAsync()
        {
            await Task.Delay(Config.Config.Settings.JoinTimeOut);

            RoundStarted();
        }

        private async void HandleTurnTimeoutAsync(IPlayer player)
        {
            await Task.Delay(Config.Config.Settings.TurnTimeOut);

            if (player.Name == playerList.Current().Name)
            {
                var nextPlayer = playerList.Next();
                SendPlayerLost(player, ServerFailureReasonCode.DID_NOT_TAKE_TURN);
                SendPlayerTurn(nextPlayer);
            }
        }

        private void SendPlayerTurn(IPlayer player)
        {
            var singlePlayerList = new IPlayer[] { player };
            var serverMoveTurn = new ServerMove(ServerMoveCode.YOUR_TURN, string.Empty, ServerFailureReasonCode.None, singlePlayerList, token);
            gameManager.ProcessMove(serverMoveTurn);
            NewTurn(player);
            Log.Write($"Send YOUR_TURN to '{player.Name}'");

            // TODO: Send data to spectators

            HandleTurnTimeoutAsync(player);
        }

        private void NewTurn(IPlayer player)
        {
            currentTurn = new Turn(player);
            turnCount += 1;
        }

        private void SendPlayerLost(IPlayer player, ServerFailureReasonCode reasonCode)
        {
            gameScorer.Lost(player);
            player.Kick();

            var serverMoveTurn = new ServerMove(ServerMoveCode.PLAYER_LOST, player.Name, reasonCode, playerList.RegisteredPlayers.ToArray(), token);
            gameManager.ProcessMove(serverMoveTurn);
            Log.Write($"Send PLAYER_LOST for '{player.Name}'");

            if (playerList.ActivePlayers.Count < Config.Config.Settings.MinimumPlayerCount)
            {
                gameOverCompletion?.TrySetResult(true);
            }
        }

        public List<IPlayer> GetScore()
        {
            return gameScorer.GetScores();
        }

        public void GameOver()
        {
            var serverMoveTurn = new ServerMove(ServerMoveCode.SCORE, gameScorer.GetScoreValues(), ServerFailureReasonCode.None, playerList.RegisteredPlayers.ToArray(), token);
            gameManager.ProcessMove(serverMoveTurn);
            Log.Write($"Send SCORE");

            gameOverCompletion?.TrySetResult(true);
        }

        #endregion
    }
}
