using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeTalk.Profilepoints
{
    public class TimeSeriesQueryableObject
    {
        public List<long> timestamps;
        public List<long> values;
    }

    public class TimeSeriesAdapter
    {
        public TimeSeriesQueryableObject CPU;
        public TimeSeriesQueryableObject Memory;

        public TimeSeriesAdapter(ResourceConsumptionTimeSeries resourceConsumptionTimeSeries, SortedDictionary<int, long> recordedTimeSlices)
        {
            var unixMilliseconds = convertToUnixMilliseconds(recordedTimeSlices);

            CPU = new TimeSeriesQueryableObject();
            CPU.timestamps = new List<long>(unixMilliseconds);
            CPU.values = new List<long>(resourceConsumptionTimeSeries.CPUConsumptionTimeSeries.Select(resourceConsumptionDetail => resourceConsumptionDetail.v));
            var CPUsizeLimiter = Math.Min(CPU.timestamps.Count, CPU.values.Count);
            CPU.timestamps = CPU.timestamps.GetRange(0, CPUsizeLimiter);
            CPU.values = CPU.values.GetRange(0, CPUsizeLimiter);

            Memory = new TimeSeriesQueryableObject();
            Memory.timestamps = new List<long>(unixMilliseconds);
            Memory.values = new List<long>(resourceConsumptionTimeSeries.memoryConsumptionTimeSeries.Select(resourceConsumptionDetail => resourceConsumptionDetail.v));
            var MemorysizeLimiter = Math.Min(Memory.timestamps.Count, Memory.values.Count);
            Memory.timestamps = Memory.timestamps.GetRange(0, MemorysizeLimiter);
            Memory.values = Memory.values.GetRange(0, MemorysizeLimiter);

        }

        public List<long> convertToUnixMilliseconds(SortedDictionary<int, long> recordedTimeSlices)
        {
            var unixMilliseconds = Enumerable.Repeat(recordedTimeSlices.ElementAt(0).Value, recordedTimeSlices.ElementAt(0).Key).ToList();
            for (int i = 1; i < recordedTimeSlices.Count() - 1; i++)
            {
                var kvp = recordedTimeSlices.ElementAt(i) ;
                var prev = recordedTimeSlices.ElementAt(i - 1);
                var sizeDiff = kvp.Key - prev.Key + 1;
                var timeDiff = kvp.Value;
                if (sizeDiff > 0 && timeDiff > 0)
                {
                    unixMilliseconds.AddRange(Enumerable.Repeat(timeDiff, sizeDiff).ToList());
                }
            }
            return unixMilliseconds;
        }
    }
}
