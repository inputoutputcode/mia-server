﻿using Mia.Server.Game.Interface;


namespace Mia.Server.Game.PlayEngine
{
    public class Player : IPlayer
    {
        #region Members

        private PlayerState currentState;
        private string name;
        private int score;

        #endregion Members


        #region Properties

        public bool IsSpectator
        {
            get { return currentState == PlayerState.Spectator; }
        }

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
        }

        #endregion Properties


        #region Constructor

        public Player(string name, bool isSpectator, int score = 0)
        {
            this.name = name;
            this.currentState = isSpectator ? PlayerState.Spectator : PlayerState.Inactive;
            this.score = score;
        }

        #endregion Constructor


        #region Methods

        public void ChangeState(PlayerState state)
        {
            if (state == PlayerState.Active || state == PlayerState.Inactive)
                currentState = state;
        }

        public string GetDebuggerDisplay()
        {
            return this.Name;
        }

        #endregion
    }
}
