using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vb.Elastic.Fluent.Objects
{
    /// <summary>
    /// Agregator dictionaries
    /// </summary>
    public class Aggregator
    {
        public Dictionary<string, object> Aggregations = new Dictionary<string, object>();
        public Dictionary<string, Dictionary<string, AggregatorMetrics>> Grouped = new Dictionary<string, Dictionary<string, AggregatorMetrics>>();
    }
}
