using System;
using System.IO;

namespace TestTask.Logging
{
    internal static class Logger
    {
        public static string logPath;

        public static void StartLogging(string path)
        {
            logPath = path;
            if(File.Exists(logPath)) 
            {  
                File.Delete(logPath); 
            }
            Log($"Logging started, logs can be found at {logPath}", LogLevel.Info);
        }

        public static void Log(string message, LogLevel level)
        {
            string logMessage = $"[{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}][{level}]:{message}";
            try
            {
                using (StreamWriter w = File.AppendText(logPath))
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
