using System.Configuration;


namespace Mia.Server.Game
{
    public class Config
    {
        public static string ServerName
        {
            get { return ConfigurationManager.AppSettings["ServerName"]; }
        }

        public static int RoundsPerRun
        {
            get { return int.Parse(ConfigurationManager.AppSettings["RoundsPerRun"]); }
        }

        public static int ServerPort
        {
            get { return int.Parse(ConfigurationManager.AppSettings["ServerPort"]); }
        }

        public static int MinimumPlayerCount
        {
            get { return int.Parse(ConfigurationManager.AppSettings["MinimumPlayerCount"]); }
        }

        public static int RoundTimeOut
        {
            get { return int.Parse(ConfigurationManager.AppSettings["RoundTimeOut"]); }
        }

        public static int JoinTimeOut
        {
            get { return int.Parse(ConfigurationManager.AppSettings["JoinTimeOut"]); }
        }

        public static int RegistrationTimeOut
        {
            get { return int.Parse(ConfigurationManager.AppSettings["RegistrationTimeOut"]); }
        }

        public static int TurnTimeOut
        {
            get { return int.Parse(ConfigurationManager.AppSettings["TurnTimeOut"]); }
        }
    }
}
