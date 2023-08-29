using Microsoft.Extensions.Configuration;
using System.IO;


namespace Game.Mia.Bot.Advanced.Configuration
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
                    string configFileName = "Game.Mia.Bot.Nightmare.Config.json";
                    var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(configFileName, optional: false);
                    config = builder.Build();
                }

                return config.GetSection("appSettings").Get<AppSettings>();
            }
        }

    }
}
