namespace vb.Elastic.Fluent.Enums
{
    internal enum EnQueryType
    {
        /// <summary>
        /// Common Term Query of Elastic Search
        /// </summary>
        CommonTerm,
        /// <summary>
        /// Matches the value with full text search
        /// </summary>
        Match,
        /// <summary>
        /// Exact occurence search
        /// </summary>
        Term,
        /// <summary>
        /// Starts with the query value
        /// </summary>
        Prefix,
        /// <summary>
        /// Search term with wildcard eg *term*
        /// </summary>
        InWildCard,
        /// <summary>
        /// Date Range Query
        /// </summary>
        DateRange,
        /// <summary>
        /// Search around a center coordinate for a given radius
        /// </summary>
        Distance,
        /// <summary>
        /// Get dates older than source
        /// </summary>
        DatePast,
        /// <summary>
        /// Get dates younger than source
        /// </summary>
        DateFuture
    }
}
