using System;
using System.Collections.Generic;
using System.Text;

namespace AirportsTracking
{
    public class Airport : IComparable<Airport>
    {
        public string ID { get; set; }
        public string AirportName { get; set; }
        public string CityName { get; set; }
        public string CountryName { get; set; }

        public string IATA { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public LinkedList<Route> routes { get; set; }
        public Airport(string[] temp, LinkedList<Route> routes)
        {
            ID = temp[0];
            AirportName = temp[1];
            CityName = temp[2];
            CountryName = temp[3];
            IATA = temp[4];
            Latitude = double.Parse(temp[6], System.Globalization.CultureInfo.InvariantCulture);
            Longitude = double.Parse(temp[7], System.Globalization.CultureInfo.InvariantCulture);
            this.routes = routes;
        }
        public Airport()
        {

        }

        public int CompareTo(Airport other)
        {
            return AirportName.CompareTo(other.AirportName);
        }
    }
}
