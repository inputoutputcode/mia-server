using System.Diagnostics;


namespace Mia.Server.Game.PlayEngine
{
    public class TimeOutTracker
    {
        private Stopwatch stopwatch;
        private int milliSeconds;

        public bool IsValid
        {
            get
            {
                bool isValid = true;

                if (stopwatch.ElapsedMilliseconds > milliSeconds)
                {
                    isValid = false;
                    stopwatch.Stop();
                }

                return isValid;
            }
        }

        public TimeOutTracker(int milliSeconds)
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();

            this.milliSeconds = milliSeconds;
        }
    }
}
