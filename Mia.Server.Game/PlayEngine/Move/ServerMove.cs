using System;
using Mia.Server.Game.PlayEngine.Move.Interface;
using Mia.Server.Game.Interface;


namespace Mia.Server.Game.PlayEngine.Move
{
    /// <summary>
    /// 
    /// </summary>
    public class ServerMove : IServerMove
    {
        private ServerMoveCode code;

        private string value;

        private ServerFailureReasonCode failureCode;

        private Guid token;

        private IPlayer[] players;

       
        public ServerMoveCode Code
        {
            get { return code; }
        }

        public string Value
        {
            get { return value; }
        }

        public ServerFailureReasonCode FailureReasonCode
        {
            get { return failureCode; }
        }

        public IPlayer[] Players
        {
            get { return players; }
        }

        public Guid Token
        {
            get { return token; }
        }

        public ServerMove(ServerMoveCode commandCode, string commandValue, ServerFailureReasonCode failureCode, IPlayer[] players, Guid token)
        {
            this.code = commandCode;
            this.value = commandValue;
            this.failureCode = failureCode;
            this.players = players;
            this.token = token;
        }

        public ServerMove(ServerMoveCode commandCode, Guid token)
        {
            this.code = commandCode;
            this.token = token;
        }

    }
}
