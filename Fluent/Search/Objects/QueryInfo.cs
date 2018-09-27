using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using vb.Elastic.Fluent.Enums;
using vb.Elastic.Fluent.Metadata;
using vb.Elastic.Fluent.Search.Objects;

namespace vb.Elastic.Fluent.Search.Objects
{
    /// <summary>
    /// Query Information Class Used in order to build a query for the index of T entity
    /// </summary>
    /// <typeparam name="T">The indexed entity</typeparam>
    internal class QueryInfo<T> where T : class
    {
        internal IList<EsField<T>> Fields { get; set; }
        internal IList<EsSort<T>> Sort { get; set; }
        internal IList<Field> Source { get; set; }
        internal AggregationDictionary Aggregations;
        internal Dictionary<string, TermsAggregation> termDictionary;

        /// <summary>
        /// Query Information Model Constractor
        /// </summary>
        internal QueryInfo()
        {
            Fields = new List<EsField<T>>();
            Sort = new List<EsSort<T>>();
            Source=new List<Field>();
            Aggregations = new AggregationDictionary();
            termDictionary = new Dictionary<string, TermsAggregation>();
        }
    }
}
