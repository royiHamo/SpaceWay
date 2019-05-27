using SpaceWay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SpaceWay.Context
{
    public class SpaceWayDbContext : DbContext
    {
        public DbSet<Aircraft> Aircrafts { get; set; }

        public DbSet<Flight> Flights { get; set; }

        public DbSet<Station> Stations { get; set; }

        public DbSet<Passenger> Passengers { get; set; }

        public DbSet<Reservation> Reservations { get; set; }
    }
}