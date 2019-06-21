using Newtonsoft;
using System.Collections.Generic;
using Microsoft.CodeTalk.LanguageService;
using Newtonsoft.Json;

namespace Microsoft.CodeTalk.Profilepoints
{
    public class FunctionDetailsObject
    {
        public int invocationCounter { get; set; }
        public InvocationDetails overviewInvocationDetails { get; set; }
        public List<InvocationDetails> functionActivationRecords { get; set; }
        public string name { get; set; }

        public FunctionDetailsObject(int invocationCounter, InvocationDetails overviewInvocationDetails, List<InvocationDetails> functionActivationRecords, string name)
        {
            this.invocationCounter = invocationCounter;
            this.overviewInvocationDetails = overviewInvocationDetails;
            this.functionActivationRecords = functionActivationRecords;
            this.name = name;
        }
    }

    internal class GenerateFunctionDetails
    {
        List<InvocationDetails> functionActivationRecords;
        SortedDictionary<int, List<long>> timestamps;
        ISyntaxEntity function;
        InvocationDetails functionOverviewInvocationDetails;
        TimeSeriesAdapter timeSeriesAdapter;

        public GenerateFunctionDetails(List<InvocationDetails> functionActivationRecords, SortedDictionary<int, List<long>> timestamps, TimeSeriesAdapter timeSeriesAdapter, ISyntaxEntity function)
        {
            this.functionActivationRecords = functionActivationRecords;
            this.timestamps = timestamps;
            this.function =  function;
            this.timeSeriesAdapter = timeSeriesAdapter;
            AverageInvocationRecords();
            GetFunctionLevelStatistics();
        }

        public void AverageInvocationRecords()
        {
            List<long> entryTimeStamps = timestamps[function.Location.StartLineNumber + 1];
            List<long> exitTimeStamps = timestamps[function.Location.EndLineNumber];
            if(entryTimeStamps.Count == exitTimeStamps.Count && exitTimeStamps.Count > 0)
            {
                this.functionOverviewInvocationDetails = new InvocationDetails(entryTimeStamps[0], exitTimeStamps[exitTimeStamps.Count - 1]);
            }
            else
            {
                this.functionOverviewInvocationDetails = new InvocationDetails(0, 0);
            }

            this.functionOverviewInvocationDetails.functionLevelResourceConsumptionStatistics = new FunctionLevelStatistics(functionOverviewInvocationDetails, this.timeSeriesAdapter).FunctionLevelResourceConsumptionStatistics();
        }

        public void GetFunctionLevelStatistics()
        {
            functionActivationRecords.ForEach(activationRecord => {
                activationRecord.functionLevelResourceConsumptionStatistics = new FunctionLevelStatistics(activationRecord, this.timeSeriesAdapter).FunctionLevelResourceConsumptionStatistics();
                }
            );
        }

        public FunctionDetailsObject GetFunctionDetailsObject()
        {
            return new FunctionDetailsObject(
                functionActivationRecords.Count, this.functionOverviewInvocationDetails, this.functionActivationRecords, this.function.Name);
        }
    }
}
