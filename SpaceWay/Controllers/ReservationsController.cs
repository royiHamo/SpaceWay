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
            Reservation reservation = new Reservation();

            //outbound flight stations assigning
            Flight outbound = db.Flights.ToList().FirstOrDefault(f => f.FlightID == Outid);

            //inbound flight stations assigning
            Flight inbound = db.Flights.ToList().FirstOrDefault(f => f.FlightID == Inid);

            reservation.OrderDate = DateTime.Now;
            
            //assigning passenger
            reservation.PassengerID = Convert.ToInt16(Session["PassengerID"]);
            reservation.Passenger = db.Passengers.First(p => p.PassengerID.Equals(reservation.PassengerID));

            //assigning flights
            reservation.OutboundID = outbound.FlightID;
            reservation.Outbound = outbound;
            reservation.InboundID = inbound.FlightID;
            reservation.Inbound = inbound;

            return View(reservation);
        }

        ////POST: Reservations/NewReservation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewReservation([Bind(Include = "ReservationID,PassengerID,OrderDate,OutboundID,Outbound,InboundID,Inbound,NumOfTickets")] Reservation reservation)
        {
            //assigning flights
            reservation.Outbound = db.Flights.ToList().FirstOrDefault(f => f.FlightID == reservation.OutboundID);
            reservation.Inbound = db.Flights.ToList().FirstOrDefault(f => f.FlightID == reservation.InboundID);

            //assigning passenger
            reservation.Passenger = db.Passengers.First(p => p.PassengerID.Equals(reservation.PassengerID));

            //calculate total price 
            reservation.TotalPrice = reservation.NumOfTickets * (reservation.Outbound.Price + reservation.Inbound.Price);
            return RedirectToAction("Payment", reservation);
            
        }

        //GET: Reservations/Payment
        public ActionResult Payment(Reservation reservation)
        {
            return View(reservation);
        }

        //POST: Reservations/Payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinalPayment([Bind(Include = "ReservationID,PassengerID,OutboundID,InboundID,NumOfTickets,TotalPrice")]Reservation reservation)
        {
            //assigning flights
            reservation.Outbound = db.Flights.ToList().FirstOrDefault(f => f.FlightID == reservation.OutboundID);
            reservation.Inbound = db.Flights.ToList().FirstOrDefault(f => f.FlightID == reservation.InboundID);

            //assigning passenger
            reservation.Passenger = db.Passengers.First(p => p.PassengerID.Equals(reservation.PassengerID));

            reservation.OrderDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                if(reservation.Outbound.NumOfPassengers > reservation.NumOfTickets)//if there are seats available
                {
                //occupy seats on flights
                reservation.Outbound.NumOfPassengers -= reservation.NumOfTickets;
                reservation.Inbound.NumOfPassengers -= reservation.NumOfTickets;

                //updating passenger's total distance
                reservation.Passenger.TotalDistance += reservation.Outbound.Distance + reservation.Inbound.Distance;

                //updating stars
                reservation.Passenger.Stars = (int)(reservation.Passenger.TotalDistance / 50000) + 1; 

                //updating db
                db.Reservations.Add(reservation);
                db.SaveChanges(); 
                }
            }
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

            //assigning flights
            reservation.Outbound = db.Flights.ToList().FirstOrDefault(f => f.FlightID == reservation.OutboundID);
            reservation.Inbound = db.Flights.ToList().FirstOrDefault(f => f.FlightID == reservation.InboundID);

            //assigning date of order
            reservation.OrderDate = DateTime.Now;

            //assigning passenger
            reservation.Passenger = db.Passengers.First(p => p.PassengerID.Equals(reservation.PassengerID));

            //occupy seats on flights
            reservation.Outbound.NumOfPassengers -= reservation.NumOfTickets;
            reservation.Inbound.NumOfPassengers -= reservation.NumOfTickets;

            //updating passenger's total distance
            reservation.Passenger.TotalDistance += reservation.Outbound.Distance + reservation.Inbound.Distance;

            //updating stars
            reservation.Passenger.Stars = (int)(reservation.Passenger.TotalDistance / 50000) + 1;


            if (ModelState.IsValid)
            {
                db.Reservations.Add(reservation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PassengerID = new SelectList(db.Passengers, "PassengerID", "Name", reservation.PassengerID);
            return View(reservation);
        }
        // GET:
        public ActionResult MyReservations()
        {
            int id = Convert.ToInt16(Session["PassengerID"]);
            List<Reservation> toShow = db.Reservations.Where(p => p.PassengerID == id).ToList();
            ViewBag.isEmpty = toShow.Count == 0;
            return View(toShow);
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
