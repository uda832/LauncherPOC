using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Launcher.Parent
{
    class Parent
    {
        static void Main(string[] args)
        {

            List<Task> tasksToStart = new List<Task>();
            List<Task> tasksToEnd = new List<Task>();
            List<ChildProcess> processes = new List<ChildProcess>();

            for(int i = 0; i < 10; ++i)
            {
                Thread.Sleep(1000);
                string ProcessId = DateTime.UtcNow.ToString("MM_dd_yyyy_HHmmss");
                ChildProcess process = new ChildProcess(ProcessId);
                var task = process.Start();
                tasksToStart.Add(task);
            }
            Task.WaitAll(tasksToStart.ToArray());


            foreach (ChildProcess process in processes)
            {
                var task = process.Cleanup();
                tasksToEnd.Add(task);
            }
            Task.WaitAll(tasksToEnd.ToArray());

        }


    }
    public class ChildProcess
    {
        public string ProcessId { get; set; }
        public static string ExecutionDirRoot = @"C:\Software\Apps\LauncherPOC";
        public static string BinSourceRoot = @"C:\Users\u4096\source\repos\Launcher.Parent\Launcher.Child\bin\Debug\netcoreapp3.1";

        public ChildProcess(string pid)
        {
            ProcessId = pid;
        }




        public async Task Start()
        {
            string ChildExePath = PrepChildProcessInstance(BinSourceRoot, ProcessId);

            StartChildProcess(ChildExePath, ProcessId);

        }



        public string PrepChildProcessInstance(string BinSourceRoot, string ProcessId)
        {
            //Create the new execution directory
            //---------------------------------------------
            string ExecutionDir = Path.Combine(ExecutionDirRoot, ProcessId);
            Directory.CreateDirectory(ExecutionDir);

            //Copy all assemblies into the new execution dir
            //--------------------------------------------------------------
            foreach (string sourceFile in Directory.GetFiles(BinSourceRoot))
            {
                string currentFileName = Path.GetFileName(sourceFile);
                string destFile = Path.Combine(ExecutionDir, currentFileName);
                File.Copy(sourceFile, destFile);
            }

            //Return the path of the executable
            //--------------------------------------------------------------
            string ChildExePath = Directory.GetFiles(ExecutionDir, "*.exe").FirstOrDefault();
            return ChildExePath;
        }

        public void StartChildProcess(string ChildExePath, string ProcessId)
        {
            Process process = new Process();

            // Configure the process using the StartInfo properties.
            process.StartInfo.FileName = ChildExePath;
            process.StartInfo.Arguments = ProcessId;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
        }

        public async Task Cleanup()
        {
            //Create the new execution directory
            //---------------------------------------------
            string ExecutionDir = Path.Combine(ExecutionDirRoot, ProcessId);
            Directory.Delete(ExecutionDir, recursive: true);
        }


    }
}
