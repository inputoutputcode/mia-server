namespace Game.Server.Console.Config
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

        public int InitializeDelayTime { get; set; }

        public string ConnectionKey { get; set; }
        
        public bool LogFileCreation { get; set; }
     
        public string LogFilePath { get; set; }
    }
}
