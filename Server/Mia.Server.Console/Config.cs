using Microsoft.Extensions.Configuration;
using System.IO;


namespace Mia.Server.Exe
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
                    var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appSettings.json", optional: false);
                    config = builder.Build();
                }

                return config.GetSection("appSetting").Get<AppSettings>();
            }
        }

    }
}
