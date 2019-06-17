using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpaceWay.Models
{
    public class Reservation
    {
        public int ReservationID { get; set; }
        public Passenger Passenger { get; set; }
        public int PassengerID { get; set; }
        public DateTime OrderDate { get; set; }
        public ICollection<Flight> Flights { get; set; } //outbound and inbound

        //public virtual Flight Outbound { get; set; }
        //public int OutFlightID { get; set; }
        //public virtual Flight Inbound { get; set; }
        //public int InFlightID { get; set; }

        public int NumOfTickets { get; set; }
        public double TotalPrice { get; set; }

    }
}