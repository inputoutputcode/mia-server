using System;


namespace Mia.Server.Game.Logging
{
    public class Log
    {
        private static readonly object lockObject = new object();

        public static void Write(string message)
        {
            string timeValue = DateTime.Now.ToString("MM/dd/yy HH:mm:ss.fff");

            lock (lockObject)
            {
                Console.WriteLine($"{timeValue} {message}");
            }
        }
    }
}
