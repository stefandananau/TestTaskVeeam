using System;
using TestTask.Logging;
using TestTask.FolderManagement;
using TestTask.Extensions;

namespace TestTask
{
    internal class Program
    {
        const string BASE_LOG_PATH = "C:\\CopyAppLog.txt";
        const string BASE_MAIN_PATH = "C:\\CopyAppMainFolder";
        const string BASE_COPY_PATH = "C:\\CopyAppMainFolder";
        static void Main(string[] args)
        {
            WriteInstructions();
            Console.WriteLine("Enter the log path:");
            string logPath = ReadPathWithCheck(BASE_LOG_PATH).AddTxtExtension();
            Logger.StartLogging(logPath);
            Console.WriteLine("Enter the main folder path:");
            string mainFolderPath = ReadPathWithCheck(BASE_MAIN_PATH);
            Console.WriteLine("Enter the copy folder path:");
            string copyFolderPath = ReadPathWithCheck(BASE_COPY_PATH);
            Console.WriteLine("Enter the synchronization interval (in seconds):");
            int syncInterval = ReadSyncInterval();
            FolderManager folderManager = new FolderManager(mainFolderPath, copyFolderPath, syncInterval);
            Console.WriteLine("\nIf the main folder already contained files, a synchronization was done before starting real-time synchronization");
            Console.WriteLine("Press any key to start real-time synchronization...");
            Console.ReadKey();
            Console.WriteLine("\nREAL TIME SYNCHRONIZATION STARTED");
            folderManager.Start();
            Console.WriteLine("Press any key to stop the application...");
            Console.ReadKey();
        }

        private static void WriteInstructions()
        {
            Console.WriteLine("WELCOME TO THE DIRECTORY SYNCHRONIZATION PROGRAM");
            Console.WriteLine("--> Created by Dananau Stefan <--");
            Console.WriteLine("\nINSTRUCTIONS:");
            Console.WriteLine("1.Visual Studio should be opened with administrator rights");
            Console.WriteLine("2.Paths should be of format 'Disk:\\folder1\\folder2\\etc..'");
            Console.WriteLine("3.If the main folder already exists, all existing items within it will be copied to the copy folder");
            Console.WriteLine("4.If the copy folder already exists, all existing items within it will be DELETED");
            Console.WriteLine("5.If the synchronization interval is not between 5 and 180 seconds or is invalid it will be defaulted to 30 seconds");
            Console.WriteLine("6.The checking interval (it checks for added files in the main directory without copying them) is 1 second by default");
            Console.WriteLine("\nBy pressing any key you agree that you read all the instructions... (press any key to continue)");
            Console.ReadKey();
            Console.Clear();
        }

        private static int ReadSyncInterval()
        {
            int interval;
            try
            {
                interval = Convert.ToInt32(Console.ReadLine());
                if (interval < 5 || interval > 180)
                {
                    throw new Exception("Interval should be between 5 and 180 seconds");
                }
            }
            catch (Exception)
            {
                interval = 30;
                Logger.Log("Synchronization Interval was defaulted to 30 seconds", LogLevel.Warning);
            }
            return interval;
        }

        private static string ReadPathWithCheck(string basePath)
        {
            string path = Console.ReadLine();
            path = path.IsPath() ? path : basePath;
            return path;
        }
    }
}
