using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SpaceWay.Models
{
    public class Passenger
    {
        public int PassengerID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public int Stars { get; set; } = 0;      //rank 0-5
        public bool IsAdmin { get; set; } = false;
        public double TotalDistance { get; set; } = 0;
        public ICollection<Reservation> Reservations { get; set; }

    }
}