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
        [Range(1,5)]
        public int Stars { get; set; } = 0;      //between 1-5
        public bool IsAdmin { get; set; } = false;
        public double TotalDistance { get; set; } = 0;
        public virtual ICollection<Reservation> Reservations { get; set; }

    }
}