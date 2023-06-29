using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.CodeTalk.Profilepoints;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using EnvDTE80;

namespace Microsoft.CodeTalk
{
    public class DataChannelWithVisualStudio : WebSocketBehavior
    {
        public static SortedDictionary<int, long> recordedTimeSlice;

        public DataChannelWithVisualStudio()
        {
            recordedTimeSlice = new SortedDictionary<int, long>();
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            var jsonArray = JsonConvert.DeserializeObject<ResourceWebSocketData>(e.Data).p;
            int updateSize = jsonArray.Count();
            recordedTimeSlice.Add(updateSize, DateTimeOffset.Now.ToUnixTimeSeconds());
            CodeTalkWebSocketServer.referenceSize = updateSize;
            Sessions.Broadcast(e.Data);
        }
    }

    public class DataChannelWithWebViews : WebSocketBehavior
    {
        internal SVsServiceProvider ServiceProvider = null;
        protected override void OnMessage(MessageEventArgs e)
        {
            var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
            Debugger debugger = dte.Debugger;
            JObject receivedData = JObject.Parse(e.Data);
            string controlChannelCommand = (string)receivedData["command"];
            string controlChannelCommandPayload = (string)receivedData["payload"];
            switch (controlChannelCommand)
            {
                case "RequestFunctionDetails":
                    ResourceConsumptionTimeSeries resourceConsumptionTimeSeries = JsonConvert.DeserializeObject<ResourceConsumptionTimeSeries>(controlChannelCommandPayload);
                    TimeSeriesAdapter timeSeriesAdapter = new TimeSeriesAdapter(resourceConsumptionTimeSeries, resourceConsumptionTimeSeries.timeslice);
                    string webSocketObject = VSOperations.functionLevelDetailsHandler.MakeFunctionsLevelDetailsJSON(timeSeriesAdapter);
                    Sessions.Broadcast(webSocketObject);
                    break;
                case "pause":
                    debugger.Break(false);
                    break;
                case "resume":
                    debugger.Go(false);
                    break;
                case "stop":
                    debugger.Stop(false);
                    break;
                case "RequestTimeslice":
                    string timesliceObject = JsonConvert.SerializeObject(DataChannelWithVisualStudio.recordedTimeSlice);
                    Sessions.Broadcast(timesliceObject);
                    break;
                default:
                    break;
            }
        }
    }

    public class CodeTalkWebSocketServer
    {
        static WebSocketServer codeTalkwebSocketServer;
        public static long referenceSize { get; set; }
        public static long referenceTick { get; set; }

        public CodeTalkWebSocketServer()
        {
            referenceSize = long.MinValue; referenceTick = long.MaxValue;

            IPAddress webSocketServerIP = IPAddress.Parse("127.0.0.1");
            codeTalkwebSocketServer = new WebSocketServer(webSocketServerIP, 5667);

            codeTalkwebSocketServer.AddWebSocketService<DataChannelWithVisualStudio>("/cpu");
            codeTalkwebSocketServer.AddWebSocketService<DataChannelWithWebViews>("/cpu");

            codeTalkwebSocketServer.AddWebSocketService<DataChannelWithVisualStudio>("/memory");
            codeTalkwebSocketServer.AddWebSocketService<DataChannelWithWebViews>("/memory");

            codeTalkwebSocketServer.AddWebSocketService<DataChannelWithWebViews>("/control");
            codeTalkwebSocketServer.AddWebSocketService<DataChannelWithWebViews>("/timeslice");

            codeTalkwebSocketServer.Start();

        }
    }
}
