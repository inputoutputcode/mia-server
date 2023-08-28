namespace Mia.Server.ConsoleRunner.Configuration
{
    public class AppSettings
    {
        public string ServerName { get; set; }

        public int RoundsPerRun { get; set; }

        public int MinimumPlayerCount { get; set; }

        public int ServerPort { get; set; }

        public int RegistrationTimeOut { get; set; }

        public int JoinTimeOut { get; set; }

        public int TurnTimeOut { get; set; }

        public string ConnectionKey { get; set; }
    }
}
