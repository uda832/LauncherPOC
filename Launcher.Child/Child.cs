using System;
using System.IO;
using System.Threading;

namespace Launcher.Child
{
    class Child
    {
        public const string TestRootPath = @"C:\Software\Test\LauncherPOC";

        static void Main(string[] args)
        {
            Console.WriteLine("Child process " + args[0]);
            CreateNewTestFile(args[0]);
        }

        static void CreateNewTestFile(string ProcessId)
        {
            string currentFileName = DateTime.UtcNow.ToString("MM_dd_yyyy_HHmmss") + ".txt";
            string newFilePath = Path.Combine(TestRootPath, currentFileName);

            File.AppendAllText(newFilePath, "Child task started" + Environment.NewLine);
            File.AppendAllText(newFilePath, "   child process id: " + ProcessId + Environment.NewLine);
            Thread.Sleep(15000);
            File.AppendAllText(newFilePath, "Child task completed");
        }
    }
}
