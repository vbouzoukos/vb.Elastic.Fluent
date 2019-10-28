using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vb.Elastic.Fluent.Core;
using vb.Elastic.Fluent.Enums;
using vb.Elastic.Fluent.Search.Objects;

namespace vb.Elastic.Fluent.Search
{
    internal class Helper
    {
        /// <summary>
        /// Create the Query Value instance that we will use for the Search
        /// </summary>
        /// <param name="from">The query value or the lower value of a range(From) query</param>
        /// <param name="to">The Upper value (To) of the range query</param>
        /// <returns>The query value instance that will be use for the query</returns>
        internal static EsValue ExtractQueryValue(object from, object to = null)
        {
            var queryTypeCode = Type.GetTypeCode(from.GetType());
            if (to != null)
            {
                var queryTypeCodeTo = Type.GetTypeCode(to.GetType());
                if (queryTypeCodeTo != queryTypeCode)
                    throw new Exception("Range Query Cannot be on different types");
            }
            EsValue queryValue;
            if (to != null)
            {
                queryValue = new EsValue(from, to);
            }
            else
            {
                queryValue = new EsValue(from);
            }
            return queryValue;
        }
        internal static QueryContainer BuildQuery<T>(QueryInfo<T> info) where T : class
        {
            QueryContainer notQuery = null;
            QueryContainer andQuery = null;
            QueryContainer orQuery = null;
            foreach (var field in info.Fields)
            {
                QueryContainer mquery;

                if (field.MultiFields != null)
                {
                    mquery = new MultiMatchQuery
                    {
                        Fields = field.MultiFields,
                        Query = field.Value.ToString()
                    };
                }
                else
                {
                    mquery = QueryItem(field.Field, field.Value, field.QueryType, field.Nested, field.Boost);
                }
                switch (field.Operator)
                {
                    case EnQueryOperator.Must:
                        andQuery &= mquery;
                        break;
                    case EnQueryOperator.Not:
                        notQuery &= !mquery;
                        break;
                    default:
                        orQuery |= mquery;
                        break;
                }
            }
            var returnQuery = new QueryContainer(new BoolQuery
            {
                Must = new QueryContainer[] { andQuery },
                Should = new QueryContainer[] { orQuery },
                MustNot = new QueryContainer[] { notQuery }
            });
            return returnQuery;
        }
        internal static QueryContainer QueryItem(Field field, EsValue queryValue, EnQueryType queryType, Field nestedField = null, double? boost = null)
        {
            //extract string parts between "" and use PhraseMatch
            bool exactMatch = false;
            if (queryValue.Value is string && queryType == EnQueryType.Match)
            {
                exactMatch = ((string)queryValue.Value).Contains("\"");
            }
            if (exactMatch)
            {
                var tokens = Parser.SplitQuotes(((string)queryValue.Value), new char[] { ' ' });
                QueryContainer container = null;
                foreach (var token in tokens)
                {
                    QueryContainer query;
                    if (token.HasQuotes)
                    {
                        query = new MatchPhraseQuery
                        {
                            Field = field,
                            Query = token.Token,
                            Boost = boost
                        };
                    }
                    else
                    {
                        if (token.Token.Contains("*"))
                        {
                            return new WildcardQuery
                            {
                                Field = field,
                                Value = token.Token,
                                Boost = boost
                            };
                        }
                        else
                        {
                            query = new MatchQuery
                            {
                                Field = field,
                                Query = token.Token,
                                Fuzziness = Fuzziness.Auto,
                                Boost = boost
                            };
                        }
                    }
                    if (container is null)
                    {
                        container = query;
                    }
                    else
                    {
                        container |= query;
                    }
                }
                return container;
            }
            if (nestedField != null)
            {
                return new NestedQuery
                {
                    Path = field,
                    Query = SimpleQueryItem(nestedField, queryValue, queryType, boost)
                };
            }
            else
            {
                return SimpleQueryItem(field, queryValue, queryType, boost);
            }
        }
        internal static QueryContainer SimpleQueryItem(Field field, EsValue query, EnQueryType queryType, double? boost)
        {
            switch (queryType)
            {
                case EnQueryType.Term:
                    return new TermQuery
                    {
                        Field = field,
                        IsVerbatim = true,
                        Value = query.Value.ToString(),
                        Boost = boost
                    };
                case EnQueryType.Prefix:
                    return new PrefixQuery
                    {
                        Field = field,
                        Value = query.Value.ToString(),
                        Boost = boost
                    };
                case EnQueryType.InWildCard:
                    return new WildcardQuery
                    {
                        Field = field,
                        Value = string.Format("*{0}*", query.Value.ToString()),
                        Boost = boost
                    };
                case EnQueryType.DateRange:
                    {
                        var q = new DateRangeQuery
                        {
                            Field = field,
                            GreaterThan = (DateTime)query.From,
                            Boost = boost
                        };
                        if (query.To != null)
                        {
                            q.LessThan = (DateTime)query.To;
                        }
                        return q;
                    }
                case EnQueryType.GreaterThan:
                    {
                        var q = new NumericRangeQuery
                        {
                            Field = field,
                            GreaterThan = Convert.ToDouble(query.From),
                            Boost = boost
                        };
                        return q;
                    }
                case EnQueryType.LessThan:
                    {
                        var q = new NumericRangeQuery
                        {
                            Field = field,
                            LessThan = Convert.ToDouble(query.From),
                            Boost = boost
                        };
                        return q;
                    }
                case EnQueryType.Range:
                    {
                        var q = new NumericRangeQuery
                        {
                            Field = field,
                            GreaterThan = Convert.ToDouble(query.From),
                            Boost = boost
                        };
                        if (query.To != null)
                        {
                            q.LessThan = Convert.ToDouble(query.To);
                        }
                        return q;
                    }
                case EnQueryType.DatePast:
                    {
                        var q = new DateRangeQuery
                        {
                            Field = field,
                            LessThan = ((DateTime)query.Value),
                            Boost = boost
                        };
                        return q;
                    }
                case EnQueryType.DateFuture:
                    {
                        var q = new DateRangeQuery
                        {
                            Field = field,
                            GreaterThan = (DateTime)query.Value,
                            Boost = boost
                        };
                        return q;
                    }
                case EnQueryType.Distance:
                    {
                        var areaQuery = (LocationArea)query.Value;
                        var q = new GeoDistanceQuery
                        {
                            Field = field,
                            Distance = new Distance(areaQuery.Distance),
                            Location = areaQuery.Center,
                            DistanceType = GeoDistanceType.Plane,
                            Boost = boost
                        };
                        return q;
                    }
                default:
                    return new MatchQuery
                    {
                        Field = field,
                        Query = query.Value.ToString(),
                        Fuzziness = Fuzziness.Auto,
                        Boost = boost
                    };
            }
        }
    }
}
