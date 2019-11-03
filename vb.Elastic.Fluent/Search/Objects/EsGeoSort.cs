using Nest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace vb.Elastic.Fluent.Search.Objects
{
    /// <summary>
    /// Sorting Option for a Field
    /// </summary>
    /// <typeparam name="T">The Class of the entity we going to search</typeparam>
    class EsGeoSort<T> : EsSort<T> where T : class
    {
        public EsGeoSort(Expression<Func<T, object>> field, double lat, double lon, bool ascending = true) : base(field, ascending)
        {
            Points = new List<GeoLocation>() { new GeoLocation(lat, lon) };
        }
        /// <summary>
        /// Sorting center
        /// </summary>
        public IEnumerable<GeoLocation> Points { get; set; }


    }
}
