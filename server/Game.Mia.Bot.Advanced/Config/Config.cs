﻿using System.IO;

using Microsoft.Extensions.Configuration;


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
                    string configFileName = "Game.Mia.Bot.Advanced.Config.json";
                    var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(configFileName, optional: false);
                    config = builder.Build();
                }

                return config.GetSection("appSettings").Get<AppSettings>();
            }
        }

    }
}
