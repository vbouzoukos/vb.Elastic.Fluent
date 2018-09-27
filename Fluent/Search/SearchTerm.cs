using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using vb.Elastic.Fluent.Enums;
using vb.Elastic.Fluent.Objects.Document;
using vb.Elastic.Fluent.Search.Objects;

namespace vb.Elastic.Fluent.Search
{
    public class SearchTerm<T> where T : EsDocument, new()
    {
        internal EsField<T> Field;

        /// <summary>
        /// Creates a match term upon mutiple fields
        /// </summary>
        /// <param name="query">Search term</param>
        /// <param name="boost">Boost used on results</param>
        /// <param name="multiFields">Fields to search for the term</param>
        public static SearchTerm<T> MultiMatch(object query, double? boost, params Expression<Func<T, object>>[] multiFields)
        {
            return new SearchTerm<T>()
            {
                Field = new EsField<T>(new EsValue(query), boost, multiFields)
            };
        }
        /// <summary>
        /// Match term
        /// </summary>
        /// <param name="field">The field where we search for matching terms</param>
        /// <param name="query">Terms query in case you with to search for exact phrase include this phrase between ""</param>
        /// <param name="nestedField">The nested field in case we want to search on a nested field in the field given</param>
        /// <param name="boost">Boost used on results</param>
        /// <returns></returns>
        public static SearchTerm<T> Match(Expression<Func<T, object>> field, object query, Expression<Func<T, object>> nestedField = null, double? boost = null)
        {
            return new SearchTerm<T>()
            {
                Field = new EsField<T>(field, new EsValue(query), EnQueryOperator.And, EnQueryType.Match, nestedField, boost)
            };
        }
        /// <summary>
        /// Common term search where stop words are included with lower priority
        /// </summary>
        /// <param name="field">The field where we search for matching terms</param>
        /// <param name="query">The search term</param>
        /// <param name="nestedField">The nested field in case we want to search on a nested field in the field given</param>
        /// <param name="boost">Boost used on results</param>
        public static SearchTerm<T> CommonTerm(Expression<Func<T, object>> field, object query, Expression<Func<T, object>> nestedField = null, double? boost = null)
        {
            return new SearchTerm<T>()
            {
                Field = new EsField<T>(field, new EsValue(query), EnQueryOperator.And, EnQueryType.CommonTerm, nestedField, boost)
            };
        }
        /// <summary>
        /// Search for keyword
        /// </summary>
        /// <param name="field">The field where we search for matching terms</param>
        /// <param name="query">The search term</param>
        /// <param name="nestedField">The nested field in case we want to search on a nested field in the field given</param>
        /// <param name="boost">Boost used on results</param>
        /// <returns></returns>
        public static SearchTerm<T> Term(Expression<Func<T, object>> field, object query, Expression<Func<T, object>> nestedField = null, double? boost = null)
        {
            return new SearchTerm<T>()
            {
                Field = new EsField<T>(field, new EsValue(query), EnQueryOperator.And, EnQueryType.Term, nestedField, boost)
            };
        }
        /// <summary>
        /// Search for documents starting with prefix
        /// </summary>
        /// <param name="field">The field where we search for matching terms</param>
        /// <param name="query">The search term</param>
        /// <param name="nestedField">The nested field in case we want to search on a nested field in the field given</param>
        /// <param name="boost">Boost used on results</param>
        /// <returns></returns>
        public static SearchTerm<T> Prefix(Expression<Func<T, object>> field, object query, Expression<Func<T, object>> nestedField = null, double? boost = null)
        {
            return new SearchTerm<T>()
            {
                Field = new EsField<T>(field, new EsValue(query), EnQueryOperator.And, EnQueryType.Prefix, nestedField, boost)
            };
        }
        /// <summary>
        /// Search for documents  where the term is matched with wildcards *term*
        /// </summary>
        /// <param name="field">The field where we search for matching terms</param>
        /// <param name="query">The search term</param>
        /// <param name="nestedField">The nested field in case we want to search on a nested field in the field given</param>
        /// <param name="boost">Boost used on results</param>
        /// <returns></returns>
        public static SearchTerm<T> Wildcard(Expression<Func<T, object>> field, object query, Expression<Func<T, object>> nestedField = null, double? boost = null)
        {
            return new SearchTerm<T>()
            {
                Field = new EsField<T>(field, new EsValue(query), EnQueryOperator.And, EnQueryType.InWildCard, nestedField, boost)
            };
        }

        /// <summary>
        /// Search for documents between a numeric range
        /// </summary>
        /// <param name="field">The field where we search for matching terms</param>
        /// <param name="from">From this number</param>
        /// <param name="to">To this number</param>
        /// <param name="nestedField">The nested field in case we want to search on a nested field in the field given</param>
        /// <param name="boost">Boost used on results</param>
        /// <returns></returns>
        public static SearchTerm<T> Range(Expression<Func<T, object>> field, object from, object to, Expression<Func<T, object>> nestedField = null, double? boost = null)
        {
            return new SearchTerm<T>()
            {
                Field = new EsField<T>(field, Helper.ExtractQueryValue(from, to), EnQueryOperator.And, EnQueryType.DateRange, nestedField, boost)
            };
        }
        /// <summary>
        /// Search for documents less than than source number
        /// </summary>
        /// <param name="field">The field where we search for matching terms</param>
        /// <param name="number">Search date</param>
        /// <param name="nestedField">The nested field in case we want to search on a nested field in the field given</param>
        /// <param name="boost">Boost used on results</param>
        /// <returns></returns>
        public static SearchTerm<T> GreaterThan(Expression<Func<T, object>> field, object number, Expression<Func<T, object>> nestedField = null, double? boost = null)
        {
            return new SearchTerm<T>()
            {
                Field = new EsField<T>(field, new EsValue(number), EnQueryOperator.And, EnQueryType.DatePast, nestedField, boost)
            };
        }
        /// <summary>
        /// Search for documents greater than source number
        /// </summary>
        /// <param name="field">The field where we search for matching terms</param>
        /// <param name="number">Search number</param>
        /// <param name="nestedField">The nested field in case we want to search on a nested field in the field given</param>
        /// <param name="boost">Boost used on results</param>
        /// <returns></returns>
        public static SearchTerm<T> LessThan(Expression<Func<T, object>> field, object number, Expression<Func<T, object>> nestedField = null, double? boost = null)
        {
            return new SearchTerm<T>()
            {
                Field = new EsField<T>(field, new EsValue(number), EnQueryOperator.And, EnQueryType.DateFuture, nestedField, boost)
            };
        }

        /// <summary>
        /// Search for documents between a date range
        /// </summary>
        /// <param name="field">The field where we search for matching terms</param>
        /// <param name="from">From this date</param>
        /// <param name="to">To this date</param>
        /// <param name="nestedField">The nested field in case we want to search on a nested field in the field given</param>
        /// <param name="boost">Boost used on results</param>
        /// <returns></returns>
        public static SearchTerm<T> DateRange(Expression<Func<T, object>> field, DateTime from, DateTime to, Expression<Func<T, object>> nestedField = null, double? boost = null)
        {
            return new SearchTerm<T>()
            {
                Field = new EsField<T>(field, Helper.ExtractQueryValue(from, to), EnQueryOperator.And, EnQueryType.DateRange, nestedField, boost)
            };
        }
        /// <summary>
        /// Search for documents earlier than this date
        /// </summary>
        /// <param name="field">The field where we search for matching terms</param>
        /// <param name="date">Search date</param>
        /// <param name="nestedField">The nested field in case we want to search on a nested field in the field given</param>
        /// <param name="boost">Boost used on results</param>
        /// <returns></returns>
        public static SearchTerm<T> DatePast(Expression<Func<T, object>> field, DateTime date, Expression<Func<T, object>> nestedField = null, double? boost = null)
        {
            return new SearchTerm<T>()
            {
                Field = new EsField<T>(field, new EsValue(date), EnQueryOperator.And, EnQueryType.DatePast, nestedField, boost)
            };
        }
        /// <summary>
        /// Search for documents later than this date
        /// </summary>
        /// <param name="field">The field where we search for matching terms</param>
        /// <param name="date">Search date</param>
        /// <param name="nestedField">The nested field in case we want to search on a nested field in the field given</param>
        /// <param name="boost">Boost used on results</param>
        /// <returns></returns>
        public static SearchTerm<T> DateFuture(Expression<Func<T, object>> field, DateTime date, Expression<Func<T, object>> nestedField = null, double? boost = null)
        {
            return new SearchTerm<T>()
            {
                Field = new EsField<T>(field, new EsValue(date), EnQueryOperator.And, EnQueryType.DateFuture, nestedField, boost)
            };
        }
        /// <summary>
        /// Geo plugin Search term to search for documents that are within the circle area given
        /// </summary>
        /// <param name="field">Field with geo data</param>
        /// <param name="latitude">Latitude of center</param>
        /// <param name="longitude">Latitude of center</param>
        /// <param name="radius">Radius of search circle</param>
        /// <param name="nestedField">nested field</param>
        /// <param name="boost">Boost on results</param>
        public static SearchTerm<T> Distance(Expression<Func<T, object>> field, double latitude, double longitude, int radius, Expression<Func<T, object>> nestedField = null, double? boost = null)
        {
            return new SearchTerm<T>()
            {
                Field = new EsField<T>(field, new EsValue(new LocationArea(latitude, longitude, radius)), EnQueryOperator.And, EnQueryType.Distance, nestedField, boost)
            };
        }
    }
}
