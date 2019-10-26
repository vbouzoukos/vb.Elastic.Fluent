namespace vb.Elastic.Fluent.Enums
{
    internal enum EnQueryOperator
    {
        /// <summary>
        /// The field must occur in the results
        /// </summary>
        And,
        /// <summary>
        /// The field should occur in the results
        /// </summary>
        Or,
        /// <summary>
        /// The field should not be in the results
        /// </summary>
        Not
    }
}
