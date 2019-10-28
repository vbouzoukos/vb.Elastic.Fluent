using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vb.Elastic.Fluent.Objects
{
    /// <summary>
    /// Results data of Execute call on FindRequest
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryResult<T> where T : class
    {
        /// <summary>
        /// The search results
        /// </summary>
        public IEnumerable<T> Documents { get; set; }
        /// <summary>
        /// Aggregation Data
        /// </summary>
        public Aggregator Aggregations { get; set; }
        /// <summary>
        /// Scores list
        /// </summary>
        public IEnumerable<double?> Scores { get; set; }
        /// <summary>
        /// Total Results found
        /// </summary>
        public long TotalResuts { get; set; }
        public QueryResult()
        {
            Aggregations = new Aggregator();
        }
    }
}
