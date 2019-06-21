using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeTalk.Profilepoints
{
    public class ResourceConsumptionStatistics
    {
        public double mean { get; set; }
        public double max { get; set; }
        public double standardDeviation { get; set; }
    }

    public class FunctionLevelResourceConsumptionStatistics
    {
        public ResourceConsumptionStatistics CPUConsumptionStatistics { get; set; }
        public ResourceConsumptionStatistics memoryConsumptionStatistics { get; set; }

        public FunctionLevelResourceConsumptionStatistics(ResourceConsumptionStatistics CPUConsumptionStatistics, ResourceConsumptionStatistics memoryConsumptionStatistics)
        {
            this.CPUConsumptionStatistics = CPUConsumptionStatistics;
            this.memoryConsumptionStatistics = memoryConsumptionStatistics;
        }
    }

    class FunctionLevelStatistics
    {
        public TimeSeriesAdapter timeSeriesAdapter;
        public InvocationDetails invocationDetails;

        public FunctionLevelStatistics(InvocationDetails invocationDetails, TimeSeriesAdapter timeSeriesAdapter)
        {
            this.invocationDetails = invocationDetails;
            this.timeSeriesAdapter = timeSeriesAdapter;
        }

        public double FindMax(List<long> values)
        {
            return values.Max();
        }

        public double FindMean(List<long> values)
        {
            return values.Count > 0 ? values.Average() : 0.0; ;
        }

        public double FindStandardDeviation(List<long> values)
        {
            double standardDeviation = 0;
            if (values.Count() > 0)
            {
                double avg = values.Average();
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                standardDeviation = Math.Sqrt((sum) / (values.Count() - 1));
            }
            return standardDeviation;
        }

        public ResourceConsumptionStatistics ComputeStatistics(InvocationDetails invocationDetails, List<long> resourceConsumptionTimestamps, List<long> resourceConsumptionValues)
        {
            int startIndex = resourceConsumptionTimestamps.IndexOf(resourceConsumptionTimestamps.OrderBy(x => Math.Abs(x - invocationDetails.entryTime)).First());
            int endIndex = resourceConsumptionTimestamps.IndexOf(resourceConsumptionTimestamps.OrderBy(x => Math.Abs(x - invocationDetails.exitTime)).First());
            int sliceSize = endIndex - startIndex + 1;
            List<long> slicedConsumptionValues = resourceConsumptionValues.GetRange(startIndex, sliceSize > 0 ? sliceSize : 1);
            ResourceConsumptionStatistics resourceConsumptionStatistics = new ResourceConsumptionStatistics();
            resourceConsumptionStatistics.max = this.FindMax(slicedConsumptionValues);
            resourceConsumptionStatistics.mean = this.FindMean(slicedConsumptionValues);
            //resourceConsumptionStatistics.standardDeviation = this.FindStandardDeviation(slicedConsumptionValues);
            return resourceConsumptionStatistics;
        }

        public FunctionLevelResourceConsumptionStatistics FunctionLevelResourceConsumptionStatistics()
        {
            ResourceConsumptionStatistics CPUConsumptionStatistics = ComputeStatistics(invocationDetails, timeSeriesAdapter.CPU.timestamps, timeSeriesAdapter.CPU.values);
            ResourceConsumptionStatistics MempryConsumptionStatistics = ComputeStatistics(invocationDetails, timeSeriesAdapter.Memory.timestamps, timeSeriesAdapter.Memory.values);
            FunctionLevelResourceConsumptionStatistics functionLevelResourceConsumptionStatistics = new FunctionLevelResourceConsumptionStatistics(CPUConsumptionStatistics, MempryConsumptionStatistics);
            return functionLevelResourceConsumptionStatistics;
        }
    }
}
