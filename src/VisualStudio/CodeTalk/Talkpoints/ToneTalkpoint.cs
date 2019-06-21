//using System;
//using static Microsoft.CodeTalk.Constants;
//using System.Diagnostics;
//using System.Threading;
//using Microsoft.VisualStudio.Shell.Interop;
//using ReactWebViewContainer;

//namespace Microsoft.CodeTalk.Talkpoints
//{
//    public class ToneTalkpoint : Talkpoint
//    {
//        Tones talkpointTone;
//        bool isCustomTone;
//        CustomTone customTalkpointTone;
//        bool continueSession;
//        IVsOutputWindowPane customPane;
//        IVsOutputWindow outWindow;

//        public ToneTalkpoint(string filePath, CursorPos position, bool doesContinue, Tones tone) : base(filePath, position, doesContinue)
//        {
//            this.talkpointTone = tone;
//            this.isCustomTone = false;
//            this.continueSession = true;
//            this.outWindow = VisualStudio.Shell.Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;

//            // Use e.g. Tools -> Create GUID to make a stable, but unique GUID for your pane.
//            // Also, in a real project, this should probably be a static constant, and not a local variable
//            //Guid customGuid = new Guid("0F44E2D1-F5FA-4d2d-AB30-22BE8ECD9789");
//            //string customTitle = "Profilepoints";
//            //this.outWindow.CreatePane(ref customGuid, customTitle, 1, 1);

//            //this.outWindow.GetPane(ref customGuid, out this.customPane);
//            //this.customPane.OutputString("Hello, Custom World!");
//            //customPane.Activate(); // Brings this pane into view
//        }

//        public ToneTalkpoint(string filePath, CursorPos position, bool doesContinue, CustomTone customTone) : base(filePath, position, doesContinue)
//        {
//            this.customTalkpointTone = customTone;
//            this.isCustomTone = true;
//        }

//        private void endDiagnosticsSession(object sender, System.EventArgs e)
//        {
//            Debug.WriteLine("Ending Session; Event Exited");
//            this.continueSession = false;
//        }

//        private void GetSingleProcessData()
//        {
//            Debug.WriteLine("Starting Diag for single process");
//            var p = System.Diagnostics.Process.GetCurrentProcess();
//            //p.Exited += new EventHandler(endDiagnosticsSession);
//            var counterC = ProcessCpuCounter.GetCPUPerfCounterForProcessId(p.Id);
//            var counterM = ProcessCpuCounter.GetMemoryPerfCounterForProcessId(p.Id);
//            Debug.WriteLine("Current Process Profiling ID: ", p.Id);
//            // start capturing

//            while (continueSession)
//            {
//                Thread.Sleep(200);
//                var cpu = counterC.NextValue() / (float)Environment.ProcessorCount;
//                customPane.OutputString(counterC.InstanceName + " -  Cpu: " + cpu + "\n");
//                Debug.WriteLine(counterC.InstanceName + " -  Cpu: " + cpu);

//                customPane.OutputString(counterM.InstanceName + " -  Memory: " + counterM.NextValue() + "\n");
//                Debug.WriteLine(counterM.InstanceName + " -  Memory: " + counterM.NextValue());
//            }            
//        }

//        public override void Execute()
//        {
//            Debug.WriteLine("Executing Void TonePoint");
//            //Thread thread = new Thread(GetSingleProcessData);
//            //thread.Start();

//            if (isCustomTone)
//            {
//                if (null != customTalkpointTone)
//                {
//                    VSOperations.PlaySound(customTalkpointTone);
//                }
//            }
//            else
//            {
//                VSOperations.PlaySound(talkpointTone);

//            }
//            CodeTalkProfiler ctp = new CodeTalkProfiler();
//            ctp.Show();
//        }


//    }
//}


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.CodeTalk.Constants;
using Microsoft.CodeTalk.Profilepoints;

namespace Microsoft.CodeTalk.Talkpoints
{
    public class ToneTalkpoint : Talkpoint
    {
        Tones talkpointTone;
        bool isCustomTone;
        CustomTone customTalkpointTone;
        FunctionLevelDetailsHandler breakpointHandler;
        readonly int id;

        public ToneTalkpoint(string filePath, CursorPos position, bool doesContinue, Tones tone) : base(filePath, position, doesContinue)
        {
            this.talkpointTone = tone;
            this.isCustomTone = false;
        }

        public ToneTalkpoint(string filePath, CursorPos position, bool doesContinue, CustomTone customTone) : base(filePath, position, doesContinue)
        {
            this.customTalkpointTone = customTone;
            this.isCustomTone = true;
        }

        //private void GetSingleProcessData()
        //{
        //    Debug.WriteLine("Starting Diag for single process");
        //    var pId = new VSOperations().GetProcessId();
        //    //p.Exited += new EventHandler(endDiagnosticsSession);
        //    var counterC = ProcessCpuCounter.GetCPUPerfCounterForProcessId(pId);
        //    var counterM = ProcessCpuCounter.GetMemoryPerfCounterForProcessId(pId);
        //    Debug.WriteLine("Current Process Profiling ID: ", pId);
        //    // start capturing
        //    var initialM = 0.0;
        //    while (true)
        //    {
        //        Thread.Sleep(200);
        //        //customPane.OutputString(counterC.InstanceName + " -  Cpu: " + cpu + "\n");
        //        //Debug.WriteLine(" Cpu: " + counterC.NextValue() + " " + pId);

        //        //customPane.OutputString(counterM.InstanceName + " -  Memory: " + counterM.NextValue() + "\n");
        //        var newM = counterM.NextValue();
        //        Debug.WriteLine("  Memory change: " + (32000 - newM) + " " + pId);
        //        int[] ar = new int[100000];
        //        //initialM = newM;

        //    }
        //}

        public override void Execute()

        {
            if (isCustomTone)
            {
                if (null != customTalkpointTone)
                {
                    VSOperations.PlaySound(customTalkpointTone);
                }
            }
            else
            {
                VSOperations.PlaySound(talkpointTone);
            }
        }
    }
}


