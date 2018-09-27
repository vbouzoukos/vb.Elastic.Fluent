using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vb.Elastic.Fluent.Search.Objects
{
    /// <summary>
    /// Geo plugin Location data
    /// </summary>
    class LocationArea
    {
        internal GeoCoordinate Center { get; set; }
        internal string Distance { get; set; }
        public LocationArea(double pLat,double pLon, int pDistance)
        {
            Center = new GeoCoordinate(pLat, pLon);
            Distance = string.Format("{0}m", pDistance);
        }
    }
}
