using Nest;
using System;
using System.Linq.Expressions;

namespace vb.Elastic.Fluent.Search.Objects
{
    /// <summary>
    /// Sorting Option for a Field
    /// </summary>
    /// <typeparam name="T">The Class of the entity we going to search</typeparam>
    class EsSort<T> where T : class
    {
        public EsSort(Expression<Func<T, object>> pField, bool pAscending = true)
        {
            Field = pField;
            Ascending = pAscending;
        }
        /// <summary>
        /// Sorting Field
        /// </summary>
        public Field Field { get; set; }
        /// <summary>
        /// Sorting Order
        /// </summary>
        public bool Ascending { get; set; }
    }
}
