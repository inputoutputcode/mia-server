namespace Game.Server.Config
{
    public class AppSettings
    {
        public int RoundsPerGame { get; set; }

        public int MinimumPlayerCount { get; set; }

        public int RegistrationTimeOut { get; set; }

        public int JoinTimeOut { get; set; }

        public int TurnTimeOut { get; set; }

        public int InitializeDelayTime { get; set; }

        public string ScoreMode { get; set; }

        public int MaximumActivePlayers { get; set; }

        public int MaximumSpectactors { get; set; }

        public int ScoreIncrement { get; set; }

        public int ScoreDecrement { get; set; }

        public string ConnectionKey { get; set; }

        public bool UniqueIpAddressPerClientRequired { get; set; }

        public int DisconnectTimeoutInMs { get; set; }
    }
}
