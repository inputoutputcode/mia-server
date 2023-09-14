using Microsoft.Extensions.Configuration;
using System.IO;


<<<<<<< HEAD:Server/Mia.Server.Game/Configuration/Config.cs
namespace Mia.Server.Game.Configuration
=======
namespace Game.Mia.Bot.Nightmare.Config
>>>>>>> main:server/Game.Mia.Bot.Nightmare/Config/Config.cs
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
