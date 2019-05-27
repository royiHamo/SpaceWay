using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpaceWay.Models
{
    public class Aircraft
    {
        public int AircraftID { get; set; }
        public int Level { get; set; } //level of luxury 1-3
        public int Seats { get; set; }

    }
}