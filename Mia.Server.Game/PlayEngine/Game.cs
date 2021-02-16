using System;
using System.Threading;
using Mia.Server.Game.PlayEngine.Move.Interface;
using Mia.Server.Game.Interface;
using Mia.Server.Game.Scoring;
using Mia.Server.Game.Scoring.Interface;
using Mia.Server.Game.Register.Interface;
using Mia.Server.Game.PlayEngine.Move;

namespace Mia.Server.Game.PlayEngine
{
    public class Game : IGame
    {
        #region Members

        private string name;
        private int turnCount;
        private bool turnFinished;
        private bool roundClosed;
        private int currentRoundNumber = 0;
        private int roundLimit;
        private Guid roundToken;
        private IPlayerList playerList;
        private IGameScorer gameScorer;
        private IDice currentDice;
        private IDice announcedDice;
        private ITurn currentTurn;
        private Guid token;
        private bool isSimulation;
        private IGameManager gameManager;
        private GamePhase gamePhase;

        #endregion Members


        #region Properties

        public bool RoundFinished => throw new NotImplementedException();

        public int TurnCount => throw new NotImplementedException();

        public bool TurnFinished => throw new NotImplementedException();

        public IPlayerList Players => throw new NotImplementedException();

        public ITurn CurrentTurn => throw new NotImplementedException();

        public Guid Token
        {
            get { return token; }
        }

        public bool IsSimulation
        {
            get { return isSimulation; }
        }

        public string Name
        {
            get { return name; }
        }

        #endregion Properties


        #region Constructor

        public Game(string name, int roundLimit, ScoreMode scoreMode, IGameManager gameManager)
        {
            this.name = name;
            this.roundLimit = roundLimit;
            this.gameScorer = new GameScorer(scoreMode);
            this.gameManager = gameManager;

            playerList = new PlayerList();
            token = new Guid();
        }

        #endregion

        public void CreateSimulation(IPlayerList players, ITurn turn, IDice dice)
        {
            this.playerList = players;
            this.currentTurn = turn;
            this.currentDice = dice;

            this.isSimulation = true;
        }

        #region Methods
        public void StartRound()
        {
            while (roundLimit < currentRoundNumber)
            {
                RoundStarting();
                currentRoundNumber += 1;
            }
        }

        public void Move(IPlayerMove playerMove)
        {
            switch (playerMove.Code)
            {
                case PlayerMoveCode.JOIN_ROUND:
                    if (gamePhase == GamePhase.Starting)
                    {
                        


                    }

                    break;
                case PlayerMoveCode.ROLL:
                    break;
                case PlayerMoveCode.SEE:
                    break;
                case PlayerMoveCode.ANNOUNCE:
                    break;
            }
        }

        public bool JoinGame(IPlayer player)
        {
            return playerList.JoinGame(player);
        }

        public void RoundStarting()
        {
            gamePhase = GamePhase.Starting;
            playerList.RoundReset();
            playerList.PermutePlayers();

            roundToken = Guid.NewGuid();
            var players = playerList.ActivePlayers.ToArray();
            var serverMove = new ServerMove(ServerMoveCode.ROUND_STARTING, string.Empty, ServerFailureReasonCode.None, players, roundToken);
            gameManager.ProcessMove(serverMove);

            var tracker = new TimeOutTracker(200);
            while (tracker.IsValid)
            {
                // TODO: Is this blocking the incoming moves?
                Thread.Sleep(20);
            }

            RoundStarted();
        }

        public void RoundStarted()
        {
            gamePhase = GamePhase.Started;

            // Send ROUND_STARTED
            var players = playerList.ActivePlayers.ToArray();
            var serverMove = new ServerMove(ServerMoveCode.ROUND_STARTED, string.Empty, ServerFailureReasonCode.None, players, roundToken);
            gameManager.ProcessMove(serverMove);

            // Send YOUR_TURN
            var firstPlayer = new IPlayer[] { playerList.FirstPlayer() };
            var serverMoveTurn = new ServerMove(ServerMoveCode.YOUR_TURN, string.Empty, ServerFailureReasonCode.None, firstPlayer, roundToken);
            gameManager.ProcessMove(serverMoveTurn);
        }

        private IDice Parse(string value)
        {
            string[] diceValues = value.Split(',');

            int dieOne;
            int dieTwo;

            int.TryParse(diceValues[0], out dieOne);
            int.TryParse(diceValues[1], out dieTwo);

            if (dieOne > 0 && dieTwo > 0)
                return new Dice(dieOne, dieTwo);

            return null;
        }

        #endregion
    }
}
