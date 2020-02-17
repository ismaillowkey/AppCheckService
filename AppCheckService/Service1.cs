using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;

namespace MorirokuVDOCheckService
{
    public partial class Service1 : ServiceBase
    {
        private CancellationTokenSource cts;
        FileIniDataParser parser = new FileIniDataParser();
        IniData data;

        public Service1()
        {
            InitializeComponent();

        }

        protected async override void OnStart(string[] args)
        {
            WriteToEventLog("IsmailLowkey", "IsmailLowkey.Logging", "Startup: Run service AppCheckService", EventLogEntryType.Information);
            var FolderPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);


            data = parser.ReadFile(FolderPath + @"\Configuration.ini");

            // Create a cancellation token source to get a cancellation token
            cts = new CancellationTokenSource();

            // Create a ConsumeTasks task
            // It's probably running right now...
            var task = ConsumeTasks(cts.Token);
            await task;
        }

        private async Task ConsumeTasks(CancellationToken cancel)
        {
            foreach (var task in ProduceForever(cancel))
            {
                await task;
            }
        }

        private IEnumerable<Task> ProduceForever(CancellationToken cancel)
        {
            do
            {
                yield return DoTheThing();
            } while (!cancel.IsCancellationRequested);
        }

        private async Task DoTheThing()
        {
            await Task.Delay(5000);
            await ProcessTask();
        }

        private Task ProcessTask()
        {
            var ProcessName = data["App"]["AppNameRunning"].ToString(); //process name without .exe
            var ExeRun = data["App"]["PathRunExe"].ToString();

            if (Process.GetProcessesByName(ProcessName).Length > 0)
            {
                // Is running
            }
            else
            {
                try
                {
                    ProcessHandler.CreateProcessAsUser(@ExeRun, null);
                }
                catch(Exception ex)
                {
                    WriteToEventLog("IsmailLowkey", "IsmailLowkey.Logging", "Error : " + ex.Message , EventLogEntryType.Error);
                }
            }

            return Task.CompletedTask;
        }


        protected override void OnStop()
        {
            // We quit by cancelling the task
            cts.Cancel();
            WriteToEventLog("IsmailLowkey", "IsmailLowkey.Logging", "Startup: Stop service AppCheckService", EventLogEntryType.Warning);
        }



        public void WriteToEventLog(string sLog, string sSource, string message, EventLogEntryType level)
        {
            RegistryPermission regPermission = new RegistryPermission(PermissionState.Unrestricted);  
            regPermission.Assert();

            // Check if the event source exists. If not create it.
            if (!System.Diagnostics.EventLog.SourceExists(sSource))
            {
                System.Diagnostics.EventLog.CreateEventSource(sSource, sLog);
            }

            EventLog.WriteEntry(sSource, message, level);
        }
    
    
    }
}
