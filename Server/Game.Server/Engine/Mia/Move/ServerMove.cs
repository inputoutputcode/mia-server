using System;
using Game.Server.Engine.Mia.Interface;
using Game.Server.Engine.Mia.Move.Interface;

namespace Game.Server.Engine.Mia.Move
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
            code = commandCode;
            value = commandValue;
            this.failureCode = failureCode;
            this.players = players;
            this.token = token;
        }
    }
}
