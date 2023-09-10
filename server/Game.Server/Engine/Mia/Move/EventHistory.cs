using System;
using System.Collections.Generic;

using Game.Server.Engine.Mia.Move.Interface;


namespace Game.Server.Engine.Mia.Move
{
    internal class EventHistory
    {
        private List<IEventEntry> events = new List<IEventEntry>();

        public void Add(IServerMove serverMove)
        {
            string playerNames = string.Empty;
            foreach (var player in serverMove.Players)
                playerNames += player.Name + ",";

            events.Add(new EventEntry()
            {
                Command = serverMove.Code.ToString(),
                Value = serverMove.Value,
                SecondValue = serverMove.FailureReasonCode.ToString(),
                Player = playerNames.TrimEnd(','),
                Token = serverMove.Token.ToString(),
                Timestamp = DateTime.Now
            });
        }

        public void Add(IClientMove clientMove)
        {
            events.Add(new EventEntry()
            {
                Command = clientMove.Code.ToString(),
                Value = clientMove.Value,
                Player = clientMove.Player.Name,
                Token = clientMove.Token.ToString(),
                Timestamp = DateTime.Now
            });
        }
    }
}
