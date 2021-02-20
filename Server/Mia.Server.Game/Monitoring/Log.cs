using System;


namespace Mia.Server.Game.Monitoring
{
    public class Log
    {
        public static void Message(string message)
        {
            string timeValue = DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff");
            Console.WriteLine($"{timeValue} { message}");
        }
    }
}
