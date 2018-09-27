using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vb.Elastic.Fluent.Objects
{
    public class QueryResult<T> where T : class
    {
        public IEnumerable<T> Documents { get; set; }
        public Aggregator Aggregations { get; set; }

        public QueryResult()
        {
            Aggregations = new Aggregator();
        }
    }
}
