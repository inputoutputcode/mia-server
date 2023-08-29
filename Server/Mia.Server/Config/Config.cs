using Microsoft.Extensions.Configuration;
using System.IO;


namespace Game.Server.Console.Config
{
    public static class Config
    {
        private static IConfigurationRoot config;

        public static AppSettings Settings
        {
            get 
            {
                if (config == null)
                {
                    string configFileName = "Game.Server.Game.Config.json";
                    var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(configFileName, optional: false);
                    config = builder.Build();
                }

                return config.GetSection("appSettings").Get<AppSettings>();
            }
        }

    }
}
