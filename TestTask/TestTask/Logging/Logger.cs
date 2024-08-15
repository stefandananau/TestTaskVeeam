using System;
using System.IO;

namespace TestTask.Logging
{
    internal static class Logger
    {
        public const string LOG_PATH = "C:\\CopyFolderLog.txt";

        public static void StartLogging()
        {
            if(File.Exists(LOG_PATH)) 
            {  
                File.Delete(LOG_PATH); 
            }
            Log("Logging started", LogLevel.Info);
        }

        public static void Log(string message, LogLevel level)
        {
            string logMessage = $"[{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}][{level}]:{message}";
            try
            {
                using (StreamWriter w = File.AppendText(LOG_PATH))
                {
                    WriteLogToFile(logMessage, w);
                    Console.WriteLine(logMessage);
                }
            }
            catch (Exception)
            {
            }
        }

        private static void WriteLogToFile(string message, TextWriter writer)
        {
            
            try
            {
                writer.WriteLine(message);
            }
            catch (Exception)
            {
            }
        }
    }
}
