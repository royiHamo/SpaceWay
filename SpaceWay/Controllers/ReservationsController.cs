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
    public class ReservationsController : Controller
    {
        private SpaceWayDbContext db = new SpaceWayDbContext();

        // GET: Reservations
        public ActionResult Index()
        {
            var reservations = db.Reservations.Include(r => r.Passenger);
            return View(reservations.ToList());
        }

        // GET: Reservations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }

        // GET: Reservations/ClientCreate
        public ActionResult ClientCreate()
        {
            ViewBag.PassengerID = new SelectList(db.Passengers, "PassengerID", "Name");
            return View();
        }

        // POST: Reservations/ClientCreate
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClientCreate([Bind(Include = "ReservationID,PassengerID,OrderDate,OutboundID,InboundID,NumOfTickets,TotalPrice")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                db.Reservations.Add(reservation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PassengerID = new SelectList(db.Passengers, "PassengerID", "Name", reservation.PassengerID);
            return View(reservation);
        }

        //GET: Reservations/NewReservation
        public ActionResult NewReservation(int Outid, int Inid)
        {
            //assigning stations to flight by ids

            //outbound flight stations assigning
            Flight outbound = db.Flights.ToList().FirstOrDefault(f => f.FlightID == Outid);
            outbound.Origin = db.Stations.ToList().FirstOrDefault(s => s.StationID == outbound.OriginID);             //delete when flight create is fixed
            outbound.Destination = db.Stations.ToList().FirstOrDefault(s => s.StationID == outbound.DestinationID);   //delete when flight create is fixed

            //inbound flight stations assigning
            Flight inbound = db.Flights.ToList().FirstOrDefault(f => f.FlightID == Inid);
            inbound.Origin = db.Stations.ToList().FirstOrDefault(s => s.StationID == inbound.OriginID);
            inbound.Destination = db.Stations.ToList().FirstOrDefault(s => s.StationID == inbound.DestinationID);

            Reservation toDisplay = new Reservation();
            toDisplay.OrderDate = DateTime.Now;
            var usernameSession = Session["Username"].ToString();
            toDisplay.Passenger = db.Passengers.First(p => p.Username.Equals(usernameSession));
            //assigning flights and flightsIDs to reservation
            toDisplay.OutboundID = outbound.FlightID;
            toDisplay.Outbound = outbound;
            Session["outID"] = toDisplay.OutboundID;
            toDisplay.InboundID = inbound.FlightID;
            toDisplay.Inbound = inbound;
            Session["inID"] = toDisplay.InboundID; 
            ViewData["res"] = toDisplay;
            return View(toDisplay);
        }

        ////POST: Reservations/NewReservation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewReservation([Bind(Include = "ReservationID,PassengerID,OrderDate,OutboundID,InboundID,NumOfTickets,TotalPrice")]Reservation finalReservation)
        {


            finalReservation.OutboundID = Convert.ToInt16(Session["outID"]);
            finalReservation.InboundID = Convert.ToInt16(Session["inID"]);

            finalReservation.Outbound = db.Flights.ToList().FirstOrDefault(f => f.FlightID == finalReservation.OutboundID);
            finalReservation.Outbound.Origin = db.Stations.ToList().FirstOrDefault(s => s.StationID == finalReservation.Outbound.OriginID);           //delete when flight create is fixed
            finalReservation.Outbound.Destination = db.Stations.ToList().FirstOrDefault(s => s.StationID == finalReservation.Outbound.DestinationID);//delete when flight create is fixed


            finalReservation.Inbound = db.Flights.ToList().FirstOrDefault(f => f.FlightID == finalReservation.InboundID);
            finalReservation.Inbound.Origin = db.Stations.ToList().FirstOrDefault(s => s.StationID == finalReservation.Inbound.OriginID);
            finalReservation.Inbound.Destination = db.Stations.ToList().FirstOrDefault(s => s.StationID == finalReservation.Inbound.DestinationID);

            return RedirectToAction("Payment", finalReservation);
        }

        //GET: Reservations/Payment
        public ActionResult Payment(Reservation finalReservation)
        {
            finalReservation.OutboundID = Convert.ToInt16(Session["outID"]);
            finalReservation.InboundID = Convert.ToInt16(Session["inID"]);

            finalReservation.Outbound = db.Flights.ToList().FirstOrDefault(f => f.FlightID == finalReservation.OutboundID);
            finalReservation.Outbound.Origin = db.Stations.ToList().FirstOrDefault(s => s.StationID == finalReservation.Outbound.OriginID);           //delete when flight create is fixed
            finalReservation.Outbound.Destination = db.Stations.ToList().FirstOrDefault(s => s.StationID == finalReservation.Outbound.DestinationID);//delete when flight create is fixed


            finalReservation.Inbound = db.Flights.ToList().FirstOrDefault(f => f.FlightID == finalReservation.InboundID);
            finalReservation.Inbound.Origin = db.Stations.ToList().FirstOrDefault(s => s.StationID == finalReservation.Inbound.OriginID);
            finalReservation.Inbound.Destination = db.Stations.ToList().FirstOrDefault(s => s.StationID == finalReservation.Inbound.DestinationID);

            //updating TotalPrice by NumOfTickets chosen
            finalReservation.TotalPrice = finalReservation.NumOfTickets * (finalReservation.Outbound.Price + finalReservation.Inbound.Price);
            return View(finalReservation);
        }

        //POST: Reservations/Payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payment()
        {
            string username = Session["Username"].ToString();
            Passenger PaidPassenger = db.Passengers.First(p => p.Username == username);
            //continue here ... should get the reservation's details...
            //PaidPassenger.Reservations.Add();

            return RedirectToAction("Index", "Home");
        }


        // GET: Reservations/AdminCreate
        public ActionResult AdminCreate()
        {
            ViewBag.PassengerID = new SelectList(db.Passengers, "PassengerID", "Name");
            ViewBag.FlightID = new SelectList(db.Flights, "FlightID", "FlightID");
            return View();
        }

        // POST: Reservations/AdminCreate
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminCreate([Bind(Include = "ReservationID,PassengerID,OrderDate,OutboundID,InboundID,NumOfTickets,TotalPrice")] Reservation reservation)
        {
            ViewBag.FlightID = new SelectList(db.Flights, "FlightID", "FlightID");
            if (ModelState.IsValid)
            {
                db.Reservations.Add(reservation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PassengerID = new SelectList(db.Passengers, "PassengerID", "Name", reservation.PassengerID);
            return View(reservation);
        }

        public ActionResult MyReservations(int? id)
        {
            return View(db.Reservations.Where(p => p.PassengerID == id).ToList());
        }


        // GET: Reservations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            ViewBag.PassengerID = new SelectList(db.Passengers, "PassengerID", "Name", reservation.PassengerID);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReservationID,PassengerID,OrderDate,OutboundID,InboundID,NumOfTickets,TotalPrice")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reservation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PassengerID = new SelectList(db.Passengers, "PassengerID", "Name", reservation.PassengerID);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reservation reservation = db.Reservations.Find(id);
            db.Reservations.Remove(reservation);
            db.SaveChanges();
            return RedirectToAction("Index");
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
