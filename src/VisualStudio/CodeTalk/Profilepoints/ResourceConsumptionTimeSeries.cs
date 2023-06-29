using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeTalk.Profilepoints
{
    public class ClockTick
    {
        public long h;
        public long l;
    }

    public class ResourceConsumptionDetail
    {
        public ClockTick t;
        public long v;
    }

    public class ResourceConsumptionTimeSeries
    {
        public List<ResourceConsumptionDetail> CPUConsumptionTimeSeries;
        public List<ResourceConsumptionDetail> memoryConsumptionTimeSeries;
        public SortedDictionary<int, long> timeslice;
    }

    public class ResourceWebSocketData
    {
        public List<ResourceConsumptionDetail> p;
    }
}
