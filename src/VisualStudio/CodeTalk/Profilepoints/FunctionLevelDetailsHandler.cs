using Newtonsoft.Json;
using System.Collections.Generic;
using WebSocketSharp;
using System;
using System.Windows.Forms;
using Microsoft.CodeTalk.LanguageService;
using System.Resources;
using Microsoft.CodeTalk.Properties;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace Microsoft.CodeTalk.Profilepoints
{
    public class FunctionLevelDetailsHandler
    {
        static SortedDictionary<int, List<long>> timestamps;
        CodeTalkWebSocketServer codeTalkWebSocketServer;
        public IEnumerable<ISyntaxEntity> functionsList;
        public SortedDictionary<long, FunctionDetailsObject> functionLevelDetails;
        //public List<FunctionDetailsObject> functionLevelDetails;

        public FunctionLevelDetailsHandler(CodeTalkWebSocketServer codeTalkWebSocketServer)
        {
            timestamps = new SortedDictionary<int, List<long>>();
            this.codeTalkWebSocketServer = codeTalkWebSocketServer;
            functionLevelDetails = new SortedDictionary<long, FunctionDetailsObject>();
            //functionLevelDetails = new List<FunctionDetailsObject>();
        }

        public int AddNewProfilePoint(int lineNumber)
        {
            timestamps.Add(lineNumber, new List<long>());
            return lineNumber;
        }

        public void AddEnd(int handlerkey, long timestamp)
        {
            if (timestamps.ContainsKey(handlerkey))
            {
                timestamps[handlerkey].Add(timestamp);
            }
            else
            {
                throw new NullReferenceException("Line number has not been encountered earlier.");
            }
        }

        public IEnumerable<ISyntaxEntity> GetFunctions()
        {
            ResourceManager rm = new ResourceManager(typeof(Resources));
            System.Diagnostics.Debug.WriteLine("Get Functions");

            IEnumerable<ISyntaxEntity> functions = new List<ISyntaxEntity>();
            CodeFile codeFile;

            //Getting the code text from the active document
            var path = TalkCodePackage.vsOperations.GetActiveDocumentPath();
            var codeText = TalkCodePackage.vsOperations.GetActiveDocumentCode();

            //Creating a language service
            var lService = new Language(path);
            try
            {
                //Parsing the code
                codeFile = lService.Parse(codeText, path);
            }
            catch (CodeTalkLanguageServiceException)
            {
                MessageBox.Show(rm.GetString("CompilationErrorMessage", CultureInfo.CurrentCulture), rm.GetString("CodeTalkString", CultureInfo.CurrentCulture), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return functions;
            }

            //Creating a function collector for getting all the functions
            var functionCollector = new FunctionCollector();
            functionCollector.VisitCodeFile(codeFile);

            //Getting all the functions
            functions = functionCollector.FunctionsInFile;
            return functions;
        }

        public string MakeFunctionsLevelDetailsJSON(TimeSeriesAdapter timeSeriesAdapter)
        {
            functionsList = GetFunctions();
            functionLevelDetails = new SortedDictionary<long, FunctionDetailsObject>();
            foreach (var function in functionsList)
            {
                try
                {
                    long functionHash = new FunctionSignature(function).GetHashCode();
                    FunctionActivationRecords records = new FunctionActivationRecords(timestamps[function.Location.StartLineNumber + 1], timestamps[function.Location.EndLineNumber]);
                    GenerateFunctionDetails generatorObject = new GenerateFunctionDetails(records.GetInvocationDetails(), timestamps, timeSeriesAdapter, function);
                    FunctionDetailsObject functionDetailsObject = generatorObject.GetFunctionDetailsObject();
                    functionLevelDetails.Add(functionHash, functionDetailsObject);
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
            return JsonConvert.SerializeObject(functionLevelDetails);
        }
    }
}
