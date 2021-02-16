using System;
using Mia.Server.Game.Register.Interface;


namespace Mia.Server.Game.Register
{
    public class GameInstance : IGameInstance
    {
        private string name;

        private Guid gameToken;

        public Guid GameToken
        {
            get { return gameToken; }
        }

        public string Name
        {
            get { return name; }
        }

        public GameInstance(string name, Guid gameToken)
        {
            this.name = name;
            this.gameToken = gameToken;
        }
    }
}
