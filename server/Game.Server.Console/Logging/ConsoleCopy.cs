using System;
using System.IO;
using System.Text;


namespace Game.Server.Console.Logging
{
    class ConsoleCopy : IDisposable
    {
        FileStream fileStream;
        StreamWriter fileWriter;
        TextWriter doubleWriter;
        TextWriter oldOut;

        class DoubleWriter : TextWriter
        {
            TextWriter one;
            TextWriter two;

            public DoubleWriter(TextWriter one, TextWriter two)
            {
                this.one = one;
                this.two = two;
            }

            public override Encoding Encoding
            {
                get { return one.Encoding; }
            }

            public override void Flush()
            {
                one.Flush();
                two.Flush();
            }

            public override void Write(char value)
            {
                try
                {
                    one.Write(value);
                    two.Write(value);
                }
                catch (ObjectDisposedException ex)
                {
                    // BUG (it might be easier to keep the data in memory per game round)
                    System.Console.WriteLine(ex.Message);
                }
            }
        }

        public ConsoleCopy()
        {
            string logFileDirectory = Config.Config.Settings.LogFilePath;
            Directory.CreateDirectory(logFileDirectory);
            string logFilePath = string.Format(@$"{logFileDirectory}\Maxle_{0}.log", DateTime.Now.Ticks.ToString());

            oldOut = System.Console.Out;

            try
            {
                fileStream = File.Create(logFilePath);
                fileWriter = new StreamWriter(fileStream) { AutoFlush = false };
                doubleWriter = new DoubleWriter(fileWriter, oldOut);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Cannot open file for writing");
                System.Console.WriteLine(e.Message);
                return;
            }
            System.Console.SetOut(doubleWriter);
        }

        public void Dispose()
        {
            System.Console.SetOut(oldOut);
            if (fileWriter != null)
            {
                fileWriter.Flush();
                fileWriter.Close();
                fileWriter = null;
            }
            if (fileStream != null)
            {
                fileStream.Close();
                fileStream = null;
            }
        }

    }
}
