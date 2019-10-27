using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using vb.Elastic.Fluent.Core;
using vb.Elastic.Fluent.Enums;
using vb.Elastic.Fluent.Metadata;
using vb.Elastic.Fluent.Objects;
using vb.Elastic.Fluent.Objects.Document;
using vb.Elastic.Fluent.Search.Objects;

namespace vb.Elastic.Fluent.Search
{
    public class FindRequest<T> where T : EsDocument, new()
    {
        #region Attributes
        private QueryInfo<T> searchInfo;
        public int MaxResults { get; set; }
        public int Start { get; set; }
        #endregion

        #region Constractor
        /// <summary>
        /// Query Information Model Constractor
        /// </summary>
        /// <param name="from">Paging Results Start from pFrom</param>
        /// <param name="max">Max Items to return</param>
        public FindRequest(int from = 0, int max = 20)
        {
            Start = from;
            MaxResults = max;
            searchInfo = new QueryInfo<T>();
        }
        #endregion

        #region Search Information
        /// <summary>
        /// Query term with should exists occurence
        /// </summary>
        /// <param name="term">Term Search operation</param>
        public FindRequest<T> Or(SearchTerm<T> term)
        {
            term.Field.Operator = EnQueryOperator.Or;
            searchInfo.Fields.Add(term.Field);
            return this;
        }
        /// <summary>
        /// Query term with MUST exists occurence
        /// </summary>
        /// <param name="term">Term Search operation</param>
        public FindRequest<T> And(SearchTerm<T> term)
        {
            term.Field.Operator = EnQueryOperator.And;
            searchInfo.Fields.Add(term.Field);
            return this;
        }
        /// <summary>
        /// Query term with NOT exists occurence
        /// </summary>
        /// <param name="term">Term Search operation</param>
        public FindRequest<T> Not(SearchTerm<T> term)
        {
            term.Field.Operator = EnQueryOperator.Not;
            searchInfo.Fields.Add(term.Field);
            return this;
        }
        /// <summary>
        /// Add a field into the sorting of result
        /// </summary>
        /// <returns>FindRequest with new sorting option</returns>
        /// <param name="field">The sort field</param>
        /// <param name="ascending">True if direction of sort is Asceding use false for Descending(Default is True)</param>
        public FindRequest<T> Sort(Expression<Func<T, object>> field, bool ascending = true)
        {
            searchInfo.Sort.Add(new EsSort<T>(field, ascending));
            return this;
        }

        public FindRequest<T> GeoSort(Expression<Func<T, object>> field, double lat, double lon, bool ascending = true)
        {
            searchInfo.Sort.Add(new EsGeoSort<T>(field, lat, lon, ascending));
            return this;
        }
        /// <summary>
        /// Used to trim result fields
        /// </summary>
        /// <param name="field">The field you want to return data</param>
        public FindRequest<T> Source(Expression<Func<T, object>> field)
        {
            searchInfo.Source.Add(field);
            return this;
        }
        /// <summary>
        /// Aggregate resuts
        /// </summary>
        /// <param name="key">Key identifier of results</param>
        /// <param name="field">Id field uppon the aggregation will be used</param>
        /// <param name="aggType">Type of Aggregation</param>
        public FindRequest<T> Aggregation(string key, Expression<Func<T, object>> field, EnAggregationType aggType)
        {
            AggregationContainer aggr = null;
            switch (aggType)
            {
                case EnAggregationType.Sum:
                    aggr = new SumAggregation(key, field);
                    break;
                case EnAggregationType.Average:
                    aggr = new AverageAggregation(key, field);
                    break;
                case EnAggregationType.Min:
                    aggr = new MinAggregation(key, field);
                    break;
                case EnAggregationType.Max:
                    aggr = new MaxAggregation(key, field);
                    break;
            }
            if (aggr != null)
            {
                searchInfo.Aggregations.Add(key, aggr);
            }
            return this;
        }
        /// <summary>
        /// Aggregate resuts over a term
        /// </summary>
        /// <param name="groupField">Field that will be used as term to group aggregation results</param>
        /// <param name="field">Id field uppon the aggregation will be used</param>
        /// <param name="aggType">Type of Aggregation</param>
        public FindRequest<T> TermAggregation(Expression<Func<T, object>> groupField, Expression<Func<T, object>> field, EnAggregationType aggType)
        {
            //Get term
            var termName = Utils.ExpressionAttributeName(groupField);
            TermsAggregation term = null;
            if (!searchInfo.termDictionary.ContainsKey(termName))
            {
                term = new TermsAggregation(termName)
                {
                    Field = groupField
                };
                searchInfo.termDictionary.Add(termName, term);
            }
            else
            {
                term = searchInfo.termDictionary[termName];
            }

            if (term != null)
            {
                //if(agg)
                AggregationContainer aggr = null;
                string pKey = "";
                switch (aggType)
                {
                    case EnAggregationType.Sum:
                        pKey = string.Format("{0}_sum", termName);
                        aggr = new SumAggregation(pKey, field);
                        break;
                    case EnAggregationType.Average:
                        pKey = string.Format("{0}_avg", termName);
                        aggr = new AverageAggregation(pKey, field);
                        break;
                    case EnAggregationType.Min:
                        pKey = string.Format("{0}_min", termName);
                        aggr = new MinAggregation(pKey, field);
                        break;
                    case EnAggregationType.Max:
                        pKey = string.Format("{0}_max", termName);
                        aggr = new MaxAggregation(pKey, field);
                        break;
                }
                if (aggr != null)
                {
                    if (term.Aggregations == null)
                    {
                        term.Aggregations = new AggregationDictionary();
                    }
                    term.Aggregations.Add(pKey, aggr);
                }
            }
            return this;
        }

        #endregion

        #region Search
        /// <summary>
        /// Search on index
        /// </summary>
        public QueryResult<T> Execute()
        {
            string indexName = (new T()).IndexAlias;
            QueryContainer criteriaQuery = null;
            var sortingQuery = new List<ISort>();
            if (searchInfo.Fields.Count > 0)
            {
                criteriaQuery = Helper.BuildQuery(searchInfo);
                //create sorting
                foreach (var sort in searchInfo.Sort)
                {
                    if (sort is EsGeoSort<T>)
                    {
                        var gsort = (EsGeoSort<T>)sort;
                        sortingQuery.Add(new GeoDistanceSort { Field = gsort.Field, Unit = DistanceUnit.Meters, DistanceType = GeoDistanceType.Plane, Points = gsort.Points, Order = gsort.Ascending ? SortOrder.Ascending : SortOrder.Descending });
                    }
                    else
                    {
                        sortingQuery.Add(new FieldSort { Field = sort.Field, Order = sort.Ascending ? SortOrder.Ascending : SortOrder.Descending });
                    }
                }
            }
            else
            {
                criteriaQuery = new MatchAllQuery();
            }
            var searchRequest = new SearchRequest<T>(indexName)
            {
                Query = criteriaQuery,
                From = Start,
                Size = MaxResults
            };

            //Set source columns
            if (searchInfo.Source.Count > 0)
            {
                var sourceFilter = new SourceFilter();
                var bfields = searchInfo.Source[0];
                Fields fields = bfields;
                for (int i = 1; i < searchInfo.Source.Count; i++)
                {
                    fields = fields.And(searchInfo.Source[i]);
                }

                sourceFilter.Includes = fields;

                searchRequest.Source = sourceFilter;
            }

            //Sorting
            if (sortingQuery.Count > 0)
            {
                searchRequest.Sort = sortingQuery;
            }

            //Aggregations
            if (searchInfo.Aggregations.Count() > 0)
            {
                searchRequest.Aggregations = searchInfo.Aggregations;
            }
            if (searchInfo.termDictionary.Count > 0)
            {
                if (searchRequest.Aggregations == null)
                {
                    searchRequest.Aggregations = new AggregationDictionary();
                }

                foreach (var term in searchInfo.termDictionary)
                {
                    searchRequest.Aggregations.Add(term.Key, term.Value);
                }
            }
            //Send request
            var searchResponse = Manager.EsClient.Search<T>(searchRequest);
            if (!searchResponse.IsValid)
            {
                throw new Exception("Low Level Fail Call.", searchResponse.OriginalException);
            }
            //handle result
            var result = new QueryResult<T>
            {
                Documents = searchResponse.Documents.AsEnumerable(),
            };
            //get aggregations if any
            if (searchInfo.termDictionary != null)
            {
                foreach (var termInfo in searchInfo.termDictionary)
                {
                    var termKey = termInfo.Key;
                    var termVal = termInfo.Value;
                    var terms = searchResponse.Aggregations.Terms(termKey);
                    if (terms != null)
                    {
                        var bucketDictionary = new Dictionary<string, AggregatorMetrics>();
                        foreach (var bucket in terms.Buckets)
                        {
                            var keyBucket = bucket.Key;
                            var agMetric = new AggregatorMetrics();

                            foreach (var funcKey in bucket.Keys)
                            {
                                if (funcKey == string.Format("{0}_sum", termKey))
                                {
                                    var sum = bucket.Sum(funcKey);
                                    agMetric.Sum = (sum == null) ? null : sum.Value;
                                }
                                if (funcKey == string.Format("{0}_avg", termKey))
                                {
                                    var val = bucket.Average(funcKey);
                                    agMetric.Average = (val == null) ? null : val.Value;
                                }
                                if (funcKey == string.Format("{0}_min", termKey))
                                {
                                    var val = bucket.Min(funcKey);
                                    agMetric.Min = (val == null) ? null : val.Value;
                                }
                                if (funcKey == string.Format("{0}_max", termKey))
                                {
                                    var val = bucket.Max(funcKey);
                                    agMetric.Max = (val == null) ? null : val.Value;
                                }
                            }

                            var cnt = bucket.DocCount;
                            agMetric.Count = cnt;

                            bucketDictionary.Add(bucket.Key, agMetric);
                        }
                        result.Aggregations.Grouped.Add(termKey, bucketDictionary);
                    }

                }
            }
            return result;
        }
        #endregion
    }
}
