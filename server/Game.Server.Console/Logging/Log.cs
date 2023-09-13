using System;


namespace Game.Server.Console.Logging
{
    public class Log : ILog
    {
        public void Message(string message)
        {
            string timeValue = DateTime.Now.ToString("yyMMdd HH:mm:ss.fff");
            System.Console.WriteLine($"{timeValue} {message}");
        }
    }
}
