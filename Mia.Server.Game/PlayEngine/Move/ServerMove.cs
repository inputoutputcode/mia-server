using System;
using Mia.Server.Game.PlayEngine.Move.Interface;
using Mia.Server.Game.Interface;


namespace Mia.Server.Game.PlayEngine.Move
{
    /// <summary>
    /// 
    /// </summary>
    public class ServerMove : IServerTurn
    {
        private ServerMoveCode code;

        private string value;

        private ServerMoveFailureCode failureCode;

        private Guid gameToken;

        private IPlayer player;

       
        public ServerMoveCode Code
        {
            get { return code; }
        }

        public string Value
        {
            get { return value; }
        }

        public ServerMoveFailureCode FailureCode
        {
            get { return failureCode; }
        }

        public IPlayer Player
        {
            get { return player; }
        }

        public Guid GameToken
        {
            get { return gameToken; }
        }

        public ServerMove(ServerMoveCode commandCode, string commandValue, ServerMoveFailureCode failureCode, IPlayer player, Guid gameToken)
        {
            this.code = commandCode;
            this.value = commandValue;
            this.failureCode = failureCode;
            this.player = player;
            this.gameToken = gameToken;
        }

        public ServerMove(ServerMoveCode commandCode, Guid gameToken)
        {
            this.code = commandCode;
            this.gameToken = gameToken;
        }

    }
}
