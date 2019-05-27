using System;
using System.Collections.Generic;
using System.Text;

namespace AirportsTracking
{
    public struct Route
    {
        public string DestinationId { get; set; }
        public string NumberOfStops { get; set; }

        public Route(string destId, string stops)
        {
            DestinationId = destId;
            NumberOfStops = stops;
        }
    }
}
