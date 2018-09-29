using Nest;

namespace vb.Elastic.Fluent.Objects.Geo
{
    public class Location
    {
        public Location(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }
        public Location()
        {
        }
        [Number(Name="lat")]
        public double Latitude { get; set; }
        [Number(Name = "lon")]
        public double Longitude { get; set; }
    }
}
