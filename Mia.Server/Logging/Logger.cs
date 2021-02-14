using System;


namespace Mia.Server.Logging
{
    public class Logger
    {
        public static void Log(string message)
        {
            string timeValue = DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff");
            Console.WriteLine($"{timeValue} { message}");
        }
    }
}
