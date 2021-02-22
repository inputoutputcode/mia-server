using System;
using Mia.Server.Game.Register.Interface;


namespace Mia.Server.Game.Register
{
    public class GameInstance : IGameInstance
    {
        private int gameNumber;

        private Guid gameToken;

        public Guid GameToken
        {
            get { return gameToken; }
        }

        public int GameNumber
        {
            get { return gameNumber; }
        }

        public GameInstance(int gameNumber, Guid gameToken)
        {
            this.gameNumber = gameNumber;
            this.gameToken = gameToken;
        }
    }
}
