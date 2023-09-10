namespace Game.Server.Engine.Mia.Move.Interface
{
    internal interface IEventEntry
    {
        string Command { get; set; }

        string Value { get; set; }

        string SecondValue { get; set; }

        string Player { get; set; }

        string Token { get; set; }
    }
}
