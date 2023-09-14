using System;

using Game.Server.Engine.Mia.Interface;
using Game.Server.Engine.Mia.Move.Interface;


namespace Game.Server.Engine.Mia.Move
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientMove : IClientMove
    {
        public ClientMoveCode code;

        public string value;

        public Guid token;

        public IPlayer player;

        public ClientMoveCode Code
        {
            get { return code; }
        }

        public string Value
        {
            get { return value; }
        }

        public Guid Token
        {
            get { return token; }
        }

        public IPlayer Player
        {
            get { return player; }
        }

        public ClientMove()
        {
            code = ClientMoveCode.None;
        }

        public ClientMove(ClientMoveCode code, string value, IPlayer player, Guid token)
        {
            this.code = code;
            this.value = value;
            this.player = player;
            this.token = token;
        }
    }
}
