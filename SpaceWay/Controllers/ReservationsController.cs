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
        public ActionResult Index(string passengerName)
        {
            // var reservations = db.Reservations.Include(r => r.Passenger);

            if (passengerName == null)
            {
                return View(db.Reservations.ToList());
            }

            var output = from res in db.Reservations.ToList()
                         where string.Equals(res.Passenger.Name, passengerName, StringComparison.OrdinalIgnoreCase)
                         select res;

            
            // no results
            return !output.Any() ? View(new List<Reservation>()) : View(output);


            //return View(reservations.ToList());
        }


        // POST: Reservations
        [HttpPost]
        public ActionResult Search(string name)
        {
            //if the input is empty, redirect to GET action index without the input
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Index");
            }

            //redirect to the GET action index, in order to search in the DB and return results
            return RedirectToAction("Index", new { @passengerName = name });
        }

        // GET: Reservations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return View("Error");
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClientCreate([Bind(Include = "ReservationID,PassengerID,OrderDate,OutboundID,InboundID,NumOfTickets,TotalPrice")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                //save new reservation in the DB
                db.Reservations.Add(reservation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //send the PassengerID to the View in the ViewBag
            ViewBag.PassengerID = new SelectList(db.Passengers, "PassengerID", "Name", reservation.PassengerID);
            return View(reservation);
        }

        //GET: Reservations/NewReservation
        public ActionResult NewReservation(int Outid, int Inid, int NumOfTickets)
        {
            Reservation reservation = new Reservation();

            //outbound flight stations assigning
            Flight outbound = db.Flights.ToList().FirstOrDefault(f => f.FlightID == Outid);

            //inbound flight stations assigning
            Flight inbound = db.Flights.ToList().FirstOrDefault(f => f.FlightID == Inid);

            //assigning date
            reservation.OrderDate = DateTime.Now;

            //assigning tickets
            reservation.NumOfTickets = NumOfTickets;
            
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
        //show the user all the details of the reservation before he pays
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
            //send PassengerID and FlightID through ViewBag to View
            ViewBag.PassengerID = new SelectList(db.Passengers.ToList(), "PassengerID", "Name");
            ViewBag.FlightID = new SelectList(db.Flights.ToList(), "FlightID", "FlightID");
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

            if (reservation.Outbound.NumOfPassengers < reservation.NumOfTickets ||
                reservation.Inbound.NumOfPassengers < reservation.NumOfTickets)
            {
                // ModelState.AddModelError(string.Empty, "No available seats.");
                ModelState.AddModelError("NumOfTickets", "No available seats.");
                ViewBag.PassengerID = new SelectList(db.Passengers.ToList(), "PassengerID", "Name");
                ViewBag.FlightID = new SelectList(db.Flights.ToList(), "FlightID", "FlightID");
                return View();
            }



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
            int starsUpdated = (int)(reservation.Passenger.TotalDistance / 50000) + 1;
            int result = (starsUpdated + reservation.Passenger.Stars) > 5 ? 5 : starsUpdated + reservation.Passenger.Stars;
            reservation.Passenger.Stars = result ;

            if (ModelState.IsValid)
            {
                db.Reservations.Add(reservation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //send PassengerID through ViewBag to View
            ViewBag.PassengerID = new SelectList(db.Passengers, "PassengerID", "Name", reservation.PassengerID);
            return View(reservation);
        }
        // GET
        //show the user all of his reservations
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
                return View("Error");
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return View("Error");
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
                //save the changes in the DB
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
                return View("Error");
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return View("Error");
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
