using Game.Server.Engine.Mia.Interface;

namespace Game.Server.Engine.Mia
{
    public class Player : IPlayer
    {
        #region Members

        private PlayerState currentState;
        private string name;
        private int score;

        #endregion Members


        #region Properties

        public PlayerState CurrentState
        {
            get { return currentState; }
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

        #endregion Properties


        #region Constructor

        public Player(string name, bool isSpectator = false, int score = 0)
        {
            this.name = name;
            currentState = isSpectator ? PlayerState.Spectator : PlayerState.Inactive;
            this.score = score;
        }

        #endregion Constructor


        #region Methods

        /// <summary>
        /// Only for Active or Inactive, Spectator must register again.
        /// </summary>
        /// <param name="state"></param>
        public void SetActive()
        {
            currentState = PlayerState.Active;
        }

        public void Kick()
        {
            currentState = PlayerState.Inactive;
        }

        public string GetDebuggerDisplay()
        {
            return Name;
        }

        #endregion
    }
}
