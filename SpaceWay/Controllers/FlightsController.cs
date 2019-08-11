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
        public ActionResult Index(int? flightID)
        {
            var flights = db.Flights.Include(f => f.Aircraft);

            if (flightID == null)
            {
                return View(db.Flights.ToList());
            }
            //checks if the input in the filter equlas to the planet
            var filteredFlights = from f in db.Flights.ToList()
                                  where f.FlightID == flightID
                                  select f;

            //checks if the filter didn't return any result ,then return all the stations
            if (!filteredFlights.Any())
            {
                return View(new List<Flight>());
            }

            // send OriginID and DestinationID to the View
            ViewBag.OriginID = new SelectList(db.Stations, "StationID", "Name");
            ViewBag.DestinationID = new SelectList(db.Stations, "StationID", "Name");

            //return the filter results
            return View(filteredFlights);
        }

        // POST: Flights
        [HttpPost]
        public ActionResult Search(string flID)
        {
            //if the input is empty, redirect to GET action index without the input
            if (string.IsNullOrEmpty(flID))
            {
                return RedirectToAction("Index");
            }

            //redirect to the GET action index, in order to search in the DB and return results
            return RedirectToAction("Index", new { @flightID = flID });
        }

        // GET: Flights/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            Flight flight = db.Flights.Find(id);
            if (flight == null)
            if (flight == null)
            {
                return HttpNotFound();
            }
            return View(flight);
        }

        //show the passenger the destination which is the most commom in all reservations 
        //that other passengers with the same star level as the user ordered
        public ActionResult LearnFromStatistics()
        {
            Station fave = null;
            string username = (string)Session["Username"];
            
            //find the user's star level
            int stars = db.Passengers.FirstOrDefault(p => p.Username == username).Stars;

            //find all the reservations where the user's star level is equals to the passengers'
            //star level which made the reservation
            var resList = db.Reservations.Where(r => r.Passenger.Stars == stars);

            //if the list contains any passenger
            if (resList.Any())
            {
                //find the destination which is most common among all the reservations with
                //the passenger with the same star level
                fave = resList.Select(x => x.Outbound.Destination).GroupBy(i => i)
                                                    .OrderByDescending(grp => grp.Count())
                                                    .Select(grp => grp.Key).First();
            }

            //return the favorite station's name to the popup in Search Flight
            if (fave != null)
            {
                return RedirectToAction("SearchFlight", new { @faveStation = fave.Name });
            }
            return RedirectToAction("SearchFlight");
        }

        // GET: Flights/Create
        public ActionResult Create()
        {
            //send AircardtID, OriginID and DestinationID to the View
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
               
                //add flight to DB
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
                //save the changes in the DB
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
                return View("Error");
            }
            Flight flight = db.Flights.Find(id);
            if (flight == null)
            {
                return View("Error");
            }
            return View(flight);
        }

        // POST: Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Flight flight = db.Flights.Find(id);
            var output = from   r in db.Reservations.ToList()
                         where  r.InboundID == flight.FlightID ||
                                r.OutboundID == flight.FlightID
                         select r;

            if(output.Any())
            {
                ModelState.AddModelError(string.Empty, "This flight is attached to a reservation");
                return View(flight);
            }

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
            TempData["tickets"] = form["tickets"];
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
            int tickets = Convert.ToInt16(TempData.Peek("tickets"));
            if (!DateTime.TryParse((string)TempData.Peek("dept"), out DateTime departure))
                return View(new List<Flight>());

            List<Flight> flightsToDisplay = null;

            flightsToDisplay = db.Flights.ToList().Where(f => f.Aircraft.Level == lvl &&
                                                              f.Aircraft.Seats > 0 &&
                                                              f.Aircraft.Seats >= 0 &&
                                                              f.NumOfPassengers >= tickets &&
                                                              DateTime.Compare(f.Departure.Date, departure.Date) == 0 &&
                                                              f.OriginID == orig &&
                                                              f.DestinationID == dest).ToList();

            if (flightsToDisplay == null || flightsToDisplay.Count == 0)
            {
                ViewBag.A = "t";
                return View();
            }
            TempData["ftd"] = flightsToDisplay;
            return View(flightsToDisplay);
        }


        //POST:
        [HttpPost]
        public ActionResult SearchFlightResultsOut(FormCollection form)
        {
            if (form["OutRadio"] == null)
            {
                List<Flight> toDisplay = (List<Flight>)TempData["ftd"];
                ModelState.AddModelError(string.Empty, "Please select a flight");
                return View(toDisplay);
            }
            TempData["selectedOutID"] = Convert.ToInt16(form["OutRadio"]);
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
            int tickets = Convert.ToInt16(TempData.Peek("tickets"));
            DateTime departure = DateTime.Parse((string)TempData.Peek("arri"));

            flightsToDisplay = db.Flights.ToList().Where(f => f.Aircraft.Level == lvl &&
                                                              f.NumOfPassengers >= tickets &&
                                                              DateTime.Compare(f.Departure.Date, departure.Date) == 0 &&
                                                              f.OriginID == orig &&
                                                              f.DestinationID == dest).ToList();
            TempData["ftd"] = flightsToDisplay;
            return View(flightsToDisplay);
        }

        //POST:
        [HttpPost]
        public ActionResult SearchFlightResultsIn(FormCollection form)
        {
            if (form["InRadio"] == null)
            {
                List<Flight> toDisplay = (List<Flight>)TempData["ftd"];
                ModelState.AddModelError(string.Empty, "Please select a flight");
                return View(toDisplay);
            }
            TempData["selectedInID"] = Convert.ToInt16(form["InRadio"]);

            return RedirectToAction("NewReservation", "Reservations", new { @Outid = TempData["selectedOutID"], @Inid = TempData["selectedInID"] , @NumOfTickets = TempData["tickets"] });
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
