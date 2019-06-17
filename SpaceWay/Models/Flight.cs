using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        //public int StationID { get; set; }
        //public Station Origin { get; set; }  //origin station 
        //public int StationID { get; set; }
        //public Station Destination { get; set; }//destination station

        public ICollection<Station> Stations { get; set; } //origin & destination
        public double Duration { get; set; }
        public double Distance { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Departure { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Arrival { get; set; }
        public double Price { get; set; } 

        public Flight()
        {
            Stations = new List<Station>(2);
            Stations.Add(new Station());
            Stations.Add(new Station());

        }
    }
}