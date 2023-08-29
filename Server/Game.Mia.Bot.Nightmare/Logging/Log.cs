using System;


namespace Game.Mia.Bot.Nightmare.Logging
{
    public class Log
    {
        public static void Write(string message)
        {
            string timeValue = DateTime.Now.ToString("MM/dd/yy HH:mm:ss.fff");
            Console.WriteLine($"{timeValue} {message}");
        }
    }
}
