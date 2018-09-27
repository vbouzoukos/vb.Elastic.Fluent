using Nest;
using System;
using System.Linq.Expressions;
using vb.Elastic.Fluent.Enums;

namespace vb.Elastic.Fluent.Search.Objects
{
    class EsField<T> where T : class
    {
        public EsField(Expression<Func<T, object>> field, EsValue value, EnQueryOperator op,
            EnQueryType queryType = EnQueryType.Match, Expression<Func<T, object>> nestedField = null, double? boost = null)
        {
            Field = field;
            Nested = nestedField;
            Value = value;
            Operator = op;
            QueryType = queryType;
            Boost = boost;
        }

        public EsField(EsValue value, double? boost, params Expression<Func<T, object>>[] multiFields)
        {
            Value = value;
            Boost = boost;
            MultiFields = multiFields;
        }
        internal Fields MultiFields { get; set; }
        internal Field Field { get; set; }
        internal Field Nested { get; set; }
        internal double? Boost { get; set; }
        public EsValue Value { get; set; }
        public EnQueryOperator Operator { get; set; }
        public EnQueryType QueryType { get; set; }

    }
}
