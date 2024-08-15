using System;
using TestTask.Logging;
using TestTask.FolderManagement;

namespace TestTask
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WriteInstructions();
            Logger.StartLogging();
            Console.WriteLine("Enter the main folder path:");
            string mainFolderPath = Console.ReadLine();
            Console.WriteLine("Enter the copy folder path:");
            string copyFolderPath = Console.ReadLine();
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
            Console.WriteLine($"3.The log file can be found at {Logger.LOG_PATH}, and it will be cleared after restarting the program");
            Console.WriteLine("4.If the main folder already exists, all existing items within it will be copied to the copy folder");
            Console.WriteLine("5.If the copy folder already exists, all existing items within it will be DELETED");
            Console.WriteLine("6.If the synchronization interval is not between 5 and 180 seconds or is invalid it will be defaulted to 30 seconds");
            Console.WriteLine("7.The checking interval (it checks for added files in the main directory without copying them) is 1 second by default");
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
    }
}
