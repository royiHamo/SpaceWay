using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace SpaceWay.Models
{
    public class Flight
    {
        public int FlightID { get; set; }
        public int NumOfPassengers { get; set; } // <= seats
        
        //ForeignKey
        public int AircraftID { get; set; }
        public virtual Aircraft Aircraft { get; set; }

        //Origin & Destination
        //[ForeignKey("Station")]
        public int OriginID { get; set; }
        public virtual Station Origin { get; set; }

        //[ForeignKey("Station")]
        public int DestinationID { get; set; }
        public virtual Station Destination { get; set; }
        
        public double Duration { get; set; }
        public double Distance { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Departure { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Arrival { get; set; }
        public double Price { get; set; }

 
        
    }
}