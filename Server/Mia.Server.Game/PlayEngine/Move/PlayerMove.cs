using System;

using Mia.Server.Game.PlayEngine.Move.Interface;
using Mia.Server.Game.Interface;


namespace Mia.Server.Game.PlayEngine.Move
{
    /// <summary>
    /// 
    /// </summary>
    public class PlayerMove : IPlayerMove
    {
        public PlayerMoveCode code;

        public string value;

        public Guid token;

        public IPlayer player;

        public PlayerMoveCode Code
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

        public PlayerMove()
        {
            code = PlayerMoveCode.None;
        }

        public PlayerMove(PlayerMoveCode code, string value, IPlayer player, Guid token)
        {
            this.code = code;
            this.value = value;
            this.player = player;
            this.token = token;
        }
    }
}
