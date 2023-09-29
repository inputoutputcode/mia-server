using System;

using Game.Server.Register.Interface;


namespace Game.Server.Register
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
