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

            ViewBag.OriginID = new SelectList(db.Stations, "StationID", "Name");
            ViewBag.DestinationID = new SelectList(db.Stations, "StationID", "Name");
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
            ViewBag.AircraftID = new SelectList(db.Aircrafts, "AircraftID", "Level");
            ViewBag.OriginID = new SelectList(db.Stations, "StationID", "Name");
            ViewBag.DestinationID = new SelectList(db.Stations, "StationID", "Name");
            return View();
        }

        
        // POST: Flights/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(Flight flight)
        {
            Flight newFlight = new Flight();
            newFlight.Origin = db.Stations.FirstOrDefault(s => s.StationID == flight.OriginID);
            newFlight.Destination = db.Stations.FirstOrDefault(s => s.StationID == flight.DestinationID);
            if (ModelState.IsValid)
            {
                db.Flights.Add(newFlight);
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
        public ActionResult Edit([Bind(Include = "FlightID,NumOfPassengers,AircraftID,OriginID,DestinationID,Duration,Distance,Departure,Arrival,Price")] Flight flight)
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

        //GET
        public ActionResult SearchFlight()
        {
            //ViewData["Stations"] = db.Stations.ToList();
            ViewBag.OriginID = new SelectList(db.Stations, "StationID", "Name");
            ViewBag.DestinationID = new SelectList(db.Stations, "StationID", "Name");
            ViewBag.AircraftID = new SelectList(db.Aircrafts, "AircraftID", "AircraftID");
            return View();
        }

        //POST: Flights/SearchFlight
        [HttpPost]
        public ActionResult SearchFlight(Flight flight)
        {
            flight.Origin = db.Stations.FirstOrDefault(s => s.StationID == flight.OriginID);
            flight.Destination = db.Stations.FirstOrDefault(s => s.StationID == flight.DestinationID);
            if (ModelState.IsValid)
            {
                return RedirectToAction("SearchFlightResults", flight);
            }

            return View();
        }

        //GET:
        public ActionResult SearchFlightResults(Flight result)
        {
            List<Flight> flightsToDisplay = new List<Flight>();

            flightsToDisplay = db.Flights.ToList().Where(f => f.AircraftID == result.AircraftID && 
                                                              DateTime.Compare(f.Departure,result.Departure) == 0 && 
                                                              DateTime.Compare(f.Arrival.Date,result.Arrival.Date) == 0 &&
                                                              f.OriginID==result.OriginID &&
                                                              f.DestinationID == result.DestinationID).ToList();

            return View(flightsToDisplay);
        }


        //POST:
        [HttpPost]
        public ActionResult SearchFlightResults(FormCollection form)
        {
            int selectedOutFlightID = Convert.ToInt16(form["OutRadio"]);
            int selectedInFlightID = Convert.ToInt16(form["InRadio"]);

            return RedirectToAction("NewReservation", "Reservations", new { @Outid = selectedOutFlightID, @Inid = selectedInFlightID });
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
