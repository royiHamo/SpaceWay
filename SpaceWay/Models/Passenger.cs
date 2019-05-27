using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpaceWay.Models
{
    public class Passenger
    {
        public int PassengerID { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Stars { get; set; }      //rank 0-5
        public bool IsAdmin { get; set; }
        public double TotalDistance { get; set; }
        public ICollection<Reservation> Reservations;

    }
}