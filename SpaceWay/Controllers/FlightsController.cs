using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SpaceWay.Context;
using SpaceWay.Models;

namespace SpaceWay.Controllers
{
    public class FlightsController : Controller
    {
        private SpaceWayDbContext db = new SpaceWayDbContext();

        // GET: Flights
        public ActionResult Index()
        {
            var flights = db.Flights.Include(f => f.Aircraft);
            return View(flights.ToList());
        }

        // GET: Flights/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Flight flight = db.Flights.Find(id);
            if (flight == null)
            {
                return HttpNotFound();
            }
            return View(flight);
        }

        // GET: Flights/Create
        public ActionResult Create()
        {
            ViewBag.AircraftID = new SelectList(db.Aircrafts, "AircraftID", "AircraftID");
            ViewBag.StationID = new SelectList(db.Stations, "StationID", "StationID");
            ViewData["Stations"] = db.Stations.ToList();

            Flight f = new Flight();
            //f.Stations = new List<Station>();
            //Station s1 = new Station();
            //Station s2 = new Station();
            //s1.Name = "bla1";
            //s2.Name = "bla2";
            //db.Stations.Add(s1);
            //db.Stations.Add(s2);
            //db.SaveChanges();
            //f.Stations.Add(s1);
            //f.Stations.Add(s2);
            
            return View(f);
        }

        // POST: Flights/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Flight flight)
        { 
            if (ModelState.IsValid)
            {
                db.Flights.Add(flight);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AircraftID = new SelectList(db.Aircrafts, "AircraftID", "AircraftID", flight.AircraftID);
            return View(flight);
        }

        // GET: Flights/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Flight flight = db.Flights.Find(id);
            if (flight == null)
            {
                return HttpNotFound();
            }
            ViewBag.AircraftID = new SelectList(db.Aircrafts, "AircraftID", "AircraftID", flight.AircraftID);
            return View(flight);
        }

        // POST: Flights/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FlightID,NumOfPassengers,AircraftID,Duration,Distance,Departure,Arrival,Price")] Flight flight)
        {
            if (ModelState.IsValid)
            {
                db.Entry(flight).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AircraftID = new SelectList(db.Aircrafts, "AircraftID", "AircraftID", flight.AircraftID);
            return View(flight);
        }

        // GET: Flights/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Flight flight = db.Flights.Find(id);
            if (flight == null)
            {
                return HttpNotFound();
            }
            return View(flight);
        }

        // POST: Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Flight flight = db.Flights.Find(id);
            db.Flights.Remove(flight);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        // GET: Flights/SearchFlight/5
        public ActionResult SearchFlight()
        {
            ViewData["Stations"] = db.Stations.ToList();
            ViewBag.AircraftID = new SelectList(db.Aircrafts, "AircraftID", "AircraftID");
            return View();
        }


        // POST: Flights/SearchFlight
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchFlight(Flight flight)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("SearchFlightResults", flight);
            }

            return View();
        }

        //GET:
        public ActionResult SearchFlightResults(Flight result)
        {
            List<Flight> flightsToDisplay = new  List<Flight>();

            flightsToDisplay = db.Flights.ToList().Where(f => f.AircraftID == result.AircraftID/*DateTime.Compare(f.Departure,result.Departure) == 0 && DateTime.Compare(f.Arrival,result.Arrival) == 0*/ ).ToList();
                
            return View(flightsToDisplay);
        }


        //POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchFlightResults(FormCollection fcOut, FormCollection fcIn)
        {
            int selectedOutFlight = int.Parse(fcOut["selectedOutbound"]);

            int selectedInFlight = int.Parse(fcIn["selectedInbound"]);

            Flight Outbound = db.Flights.ToList().Where(f => f.FlightID == selectedOutFlight).ToList()[0];
            Flight Inbound = db.Flights.ToList().Where(f => f.FlightID == selectedInFlight).ToList()[0];

            List<Flight> flights = new List<Flight>();

            flights.Add(Outbound);
            flights.Add(Inbound);

                return RedirectToAction("Create", "Reservation",flights);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
