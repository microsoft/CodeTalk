using System;
using System.Collections.Generic;
using Microsoft.CodeTalk.LanguageService;
using System.Resources;
using Microsoft.CodeTalk.Properties;

namespace Microsoft.CodeTalk.Profilepoints
{
    public class InvocationDetails
    {
        public long entryTime { get; set; }
        public long exitTime { get; set; }
        public long ThreadId { get; set; }

        public FunctionLevelResourceConsumptionStatistics functionLevelResourceConsumptionStatistics { get; set; }

        public InvocationDetails(long entryTime, long exitTime)
        {
            this.entryTime = entryTime;
            this.exitTime = exitTime;
        }
        //public InvocationDetails(int entryTime, int exitTime, int ThreadId)
        //{
        //    this.entryTime = entryTime;
        //    this.exitTime = exitTime;
        //    this.ThreadId = ThreadId;
        //}
    }

    class FunctionActivationRecords
    {
        Stack<long> invocationCounter;
        List<InvocationDetails> functionInvocationList;
        List<long> entryTimes, exitTimes;

        public FunctionActivationRecords(List<long> entryTimes, List<long> exitTimes)
        {
            this.entryTimes = entryTimes;
            this.exitTimes = exitTimes;
            this.invocationCounter = new Stack<long>();
            this.functionInvocationList = new List<InvocationDetails>(entryTimes.Capacity);
        }

        public List<InvocationDetails> GetInvocationDetails()
        {
            try
            {
                int minCount = Math.Min(entryTimes.Capacity, exitTimes.Capacity);
                if (entryTimes.Capacity > 0 && exitTimes.Capacity > 0 )
                {
                    int entryTimesListIndex = 0, exitTimesListIndex = 0;

                    while (exitTimesListIndex < minCount)
                    {
                        if(entryTimesListIndex != entryTimes.Count)
                        {
                            invocationCounter.Push(entryTimes[entryTimesListIndex]);
                            entryTimesListIndex += 1;
                        }
                        if (entryTimesListIndex == entryTimes.Count || exitTimes[exitTimesListIndex] <= entryTimes[entryTimesListIndex])
                        {
                            long entryTime = invocationCounter.Pop();
                            InvocationDetails invocationDetail = new InvocationDetails(entryTime, exitTimes[exitTimesListIndex]);
                            functionInvocationList.Add(invocationDetail);
                            exitTimesListIndex += 1;
                        }
                    }
                }
            }
            catch(Exception e) { }
           
            return functionInvocationList;
        }
    }
}
