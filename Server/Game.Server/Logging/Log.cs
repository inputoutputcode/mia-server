using System;


namespace Game.Server.Logging
{
    public class Log
    {
        private static readonly object lockObject = new object();

        public static void Write(string message)
        {
            string timeValue = DateTime.Now.ToString("MM/dd/yy HH:mm:ss.fff");

            lock (lockObject)
            {
                // TODO: Implement logging for multi-tenant
            }
        }
    }
}
