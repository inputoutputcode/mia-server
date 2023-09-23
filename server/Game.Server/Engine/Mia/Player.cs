using Game.Server.Engine.Mia.Interface;


namespace Game.Server.Engine.Mia
{
    public class Player : IPlayer
    {
        #region Members

        private PlayerState currentState;
        private string name;
        private int score;
        private string ipAddress;

        #endregion Members


        #region Properties

        public PlayerState CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }

        public string Name
        {
            get { return name; }
        }

        public int Score
        {
            get { return score; }
            set { score = value; }
        }

        public string IPAddress => ipAddress;

        #endregion Properties


        #region Constructor

        public Player(string name, bool isSpectator = false, string ipAddress = "", int score = 0)
        {
            this.name = name;
            currentState = isSpectator ? PlayerState.Spectator : PlayerState.Inactive;
            this.score = score;
            this.ipAddress = ipAddress;
        }

        #endregion Constructor


        #region Methods

        public string GetDebuggerDisplay()
        {
            return Name;
        }

        #endregion
    }
}
