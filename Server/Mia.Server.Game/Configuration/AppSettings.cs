﻿namespace Mia.Server.ConsoleRunner.Configuration
{
    public class AppSettings
    {
        public int RoundsPerGame { get; set; }

        public int MinimumPlayerCount { get; set; }

        public int RegistrationTimeOut { get; set; }

        public int JoinTimeOut { get; set; }

        public int TurnTimeOut { get; set; }

        public string ScoreMode { get; set; }
    }
}
