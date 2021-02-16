using System;
using Mia.Server.Game.PlayEngine.Move.Interface;
using Mia.Server.Game.Interface;
using Mia.Server.Game.Scoring;
using Mia.Server.Game.Scoring.Interface;
using Mia.Server.Game.Register.Interface;
using System.Threading;

namespace Mia.Server.Game.PlayEngine
{
    public class Game : IGame
    {
        #region Members

        private int turnCount;
        private bool turnFinished;
        private bool roundClosed;
        private string name;
        private int rounds;
        private IPlayerList playerList;
        private IGameScorer gameScorer;
        private IDice currentDice;
        private IDice announcedDice;
        private ITurn currentTurn;
        private Guid token;
        private bool isSimulation;
        private IGameManager gameManager;

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

        public Game(string name, int rounds, ScoreMode scoreMode, IGameManager gameManager)
        {
            this.name = name;
            this.rounds = rounds;
            this.gameScorer = new GameScorer(scoreMode);
            this.gameManager = gameManager;

            token = new Guid();
        }

        #endregion

        public void CreateSimulation(IPlayerList players, ITurn turn, IDice dice)
        {
            this.playerList = players;
            this.currentTurn = turn;
            this.currentDice = dice;
        }

        #region Methods
        public void StartRound()
        {
            throw new NotImplementedException();
        }

        public void Move(IPlayerMove playerMove)
        {
            throw new NotImplementedException();
        }

        public bool Register(IPlayer player)
        {
            return playerList.AddPlayer(player);
        }

        public void RoundStarting()
        {
            throw new NotImplementedException();

            var tracker = new TimeOutTracker(200);
            while (tracker.IsValid)
            {
                Thread.Sleep(20);
            }
        }

        public void JoinRound(IPlayer player)
        {
            throw new NotImplementedException();
        }

        public void RoundStarted()
        {
            throw new NotImplementedException();
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
