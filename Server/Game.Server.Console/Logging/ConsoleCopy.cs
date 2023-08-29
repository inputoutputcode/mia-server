﻿using System;
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
            private object lockObject = new object();

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
                lock (lockObject)
                {
                    one.Write(value);
                    two.Write(value);
                }
            }

        }

        public ConsoleCopy()
        {
            string logFileDirectory = Config.Config.Settings.LogFilePath;
            Directory.CreateDirectory(logFileDirectory);
            string logFilePath = string.Format(@$"{logFileDirectory}\Maxle_{0}.log", DateTime.Now.Ticks);

            oldOut = System.Console.Out;

            try
            {
                fileStream = File.Create(logFilePath);

                fileWriter = new StreamWriter(fileStream);
                fileWriter.AutoFlush = true;

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