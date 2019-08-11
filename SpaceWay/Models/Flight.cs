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

        [Range(1, Int32.MaxValue, ErrorMessage = "Invalid Number Of Passengers Enterd Please Enter Positive Number")]
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

        [Range(0, Int32.MaxValue,ErrorMessage ="Invalid Duration Enterd Please Enter Positive Number")]
        public double Duration { get; set; }

        [Range(0, Int32.MaxValue, ErrorMessage = "Invalid Distance Enterd Please Enter Positive Number")]
        public double Distance { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime Departure { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Arrival { get; set; }


        [Range(0, Int32.MaxValue, ErrorMessage = "Invalid Price Enterd Please Enter Positive Number")]
        [DataType(DataType.Currency)]
        public double Price { get; set; }

 
        
    }
}