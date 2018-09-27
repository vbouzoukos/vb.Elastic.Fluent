namespace vb.Elastic.Fluent.Enums
{
    /// <summary>
    /// Type of Aggregation to use with a find request
    /// </summary>
    public enum EnAggregationType
    {
        /// <summary>
        /// Used for a sum aggregation
        /// </summary>
        Sum,
        /// <summary>
        /// Used for average aggregation
        /// </summary>
        Average,
        /// <summary>
        /// Used for max aggregation
        /// </summary>
        Max,
        /// <summary>
        /// Used for min aggregation
        /// </summary>
        Min,
        /// <summary>
        /// Used for count aggregation
        /// </summary>
        Count
    }
}
