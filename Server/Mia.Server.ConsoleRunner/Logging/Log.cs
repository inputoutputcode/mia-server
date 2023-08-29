using System;


namespace Mia.Server.ConsoleRunner.Logging
{
    public class Log : ILog
    {
        public void Message(string message)
        {
            string timeValue = DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff");
            Console.WriteLine($"{timeValue} { message}");
        }
    }
}
