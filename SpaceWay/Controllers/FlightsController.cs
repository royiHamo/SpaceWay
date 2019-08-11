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

        public ActionResult LearnFromStatistics()
        {
            Station fave = null;
            string username = (string)Session["Username"];
            int stars = db.Passengers.FirstOrDefault(p => p.Username == username).Stars;
            var resList = db.Reservations.Where(r => r.Passenger.Stars == stars);

            if (resList.Any())
            {
                fave = resList.Select(x => x.Outbound.Destination).GroupBy(i => i)
                                                    .OrderByDescending(grp => grp.Count())
                                                    .Select(grp => grp.Key).First();
            }
            if (fave != null)
            {
                return RedirectToAction("SearchFlight", new { @faveStation = fave.Name });
            }
            return RedirectToAction("SearchFlight");
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
        public ActionResult Create([Bind(Include = "FlightID,NumOfPassengers,AircraftID,OriginID,DestinationID,Duration,Distance,Departure,Arrival,Price")] Flight flight)
        {
       
            if (ModelState.IsValid)
            {
                flight.Aircraft = db.Aircrafts.FirstOrDefault(a=>a.AircraftID==flight.AircraftID);
                flight.Origin = db.Stations.FirstOrDefault(s => s.StationID == flight.OriginID);
                flight.Destination = db.Stations.FirstOrDefault(s => s.StationID == flight.DestinationID);
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
        public ActionResult SearchFlight(string faveStation)
        {
            //ViewData["Stations"] = db.Stations.ToList();
            ViewBag.FaveStation = faveStation;
            ViewBag.OriginID = new SelectList(db.Stations, "StationID", "Name");
            ViewBag.DestinationID = new SelectList(db.Stations, "StationID", "Name");
            //ViewBag.AircraftID = new SelectList(db.Aircrafts, "AircraftID", "AircraftID");
            return View();
        }

        //POST: Flights/SearchFlight
        [HttpPost]
        public ActionResult SearchFlight(FormCollection form)
        {
            //flight.Origin = db.Stations.FirstOrDefault(s => s.StationID == flight.OriginID);
            //flight.Destination = db.Stations.FirstOrDefault(s => s.StationID == flight.DestinationID);
            TempData["lvl"] = form["lvl"];
            TempData["orig"] = form["OriginID"];
            TempData["dest"] = form["DestinationID"];
            TempData["dept"] = form["departure"];
            TempData["arri"] = form["arrival"];
            if (ModelState.IsValid)
            {
                return RedirectToAction("SearchFlightResultsOut");
            }

            return View();
        }

        public ActionResult JoinFlightsAndAircrafts()
        {
            var aircraftsFlightsAvailableSeats = (
                from a in db.Aircrafts
                join f in db.Flights
                on a.AircraftID equals f.AircraftID
                select new
                {
                    a.AircraftID,
                    AvailableSeats = f.NumOfPassengers,
                }).AsEnumerable().Select(x => Tuple.Create(x.AircraftID, x.AvailableSeats)).ToList();

            return View(aircraftsFlightsAvailableSeats);
        }

        public ActionResult GroupFlightsByDestination()
        {
            var flightByDestsGroups = 
                from f in db.Flights
                group f by f.Destination.Name into flightsByDestinations
                orderby flightsByDestinations.Key ascending
                select flightsByDestinations;
          
            return View(flightByDestsGroups);
        }

        //GET:
        public ActionResult SearchFlightResultsOut()
        {
            int lvl = Convert.ToInt16(TempData.Peek("lvl"));
            int orig = Convert.ToInt16(TempData.Peek("orig"));
            int dest = Convert.ToInt16(TempData.Peek("dest"));
            //DateTime departure; = DateTime.Parse((string)TempData.Peek("dept"));
            DateTime departure;
            if (!DateTime.TryParse((string)TempData.Peek("dept"), out departure))
                return View(new List<Flight>());
            //DateTime arrival = (DateTime)TempData["arri"];


            List<Flight> flightsToDisplay = null;

            flightsToDisplay = db.Flights.ToList().Where(f => f.AircraftID == lvl &&
                                                              DateTime.Compare(f.Departure.Date, departure.Date) == 0 &&
                                                              f.OriginID == orig &&
                                                              f.DestinationID == dest).ToList();

            if (flightsToDisplay == null || flightsToDisplay.Count == 0)
            {
                ViewBag.A = "t";
                return View();
            }

            return View(flightsToDisplay);
        }


        //POST:
        [HttpPost]
        public ActionResult SearchFlightResultsOut(FormCollection form)
        {

            TempData["selectedOutID"] = Convert.ToInt16(form["OutRadio"]);
            //int selectedOutFlightID = Convert.ToInt16(form["OutRadio"]);
            //int selectedInFlightID = Convert.ToInt16(form["InRadio"]);

            //return RedirectToAction("NewReservation", "Reservations", new { @Outid = selectedOutFlightID, @Inid = selectedInFlightID });
            return RedirectToAction("SearchFlightResultsIn");
        }



        //GET:
        public ActionResult SearchFlightResultsIn()
        {
            List<Flight> flightsToDisplay = null;              // Israel (orig)01/07 -> London (dest)
                                                                //

            int lvl = Convert.ToInt16(TempData.Peek("lvl"));
            int orig = Convert.ToInt16(TempData.Peek("dest"));
            int dest = Convert.ToInt16(TempData.Peek("orig"));
            DateTime departure = DateTime.Parse((string)TempData.Peek("arri"));

            flightsToDisplay = db.Flights.ToList().Where(f => f.AircraftID == lvl &&
                                                            DateTime.Compare(f.Departure.Date, departure.Date) == 0 &&
                                                              f.OriginID == orig &&
                                                              f.DestinationID == dest).ToList();

            return View(flightsToDisplay);
        }

        //POST:
        [HttpPost]
        public ActionResult SearchFlightResultsIn(FormCollection form)
        {
            TempData["selectedInID"] = Convert.ToInt16(form["InRadio"]);

            return RedirectToAction("NewReservation", "Reservations", new { @Outid = TempData["selectedOutID"], @Inid = TempData["selectedInID"] });
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
