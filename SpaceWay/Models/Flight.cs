using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpaceWay.Models
{
    public class Flight
    {
        public int FlightID { get; set; }
        public int NumOfPassengers { get; set; } // <= seats
        public int AircraftID { get; set; }
        public Aircraft Aircraft { get; set; }
        public Station Origin { get; set; }  //origin station 
        public Station Destination { get; set; }//destination station
        public double Duration { get; set; }
        public double Distance { get; set; }
        public DateTime Departure { get; set; }
        public DateTime Arrival { get; set; }
        public int Price { get; set; }

    }
}