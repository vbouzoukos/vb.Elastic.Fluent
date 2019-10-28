namespace vb.Elastic.Fluent.Enums
{
    internal enum EnQueryOperator
    {
        /// <summary>
        /// The clause must occur in the results
        /// </summary>
        Must,
        /// <summary>
        /// The clause should occur in the results
        /// </summary>
        Should,
        /// <summary>
        /// The clause must not be in the results
        /// </summary>
        Not
    }
}
