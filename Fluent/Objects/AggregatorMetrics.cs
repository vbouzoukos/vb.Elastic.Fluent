using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vb.Elastic.Fluent.Objects
{
    public class AggregatorMetrics
    {
        public object Sum { get; set; }
        public object Average { get; set; }
        public object Max { get; set; }
        public object Min { get; set; }
        public object Count { get; set; }
    }
}
