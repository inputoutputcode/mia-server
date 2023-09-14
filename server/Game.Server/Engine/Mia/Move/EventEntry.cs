using System;

using Game.Server.Engine.Mia.Move.Interface;


namespace Game.Server.Engine.Mia.Move
{
    internal class EventEntry : IEventEntry
    {
        public string Command { get; set; }

        public string Value { get; set; }

        public string SecondValue { get; set; }

        public string Player { get; set; }

        public string Token { get; set; }

        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return string.Format("Time={5};Command={0};Value={1};Players={2};Failure={3};Token={4};", Command, Value, Player, SecondValue, Token, Timestamp.ToString("mm:fff"));
        }
    }
}
