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
    public class PassengersController : Controller
    {
        private SpaceWayDbContext db = new SpaceWayDbContext();

        // GET: Passengers
        public ActionResult Index(string Username)
        {
            //if user is logged in , send the username to the view
            if (Session["Username"] != null)
            {
                ViewBag.Username = Username;

            }
            //calls the function that returns all the passengers in ajax
            IndexGetAjax();
            return View();
        }

        //returns ajax to get-ajax function, to show all passengers in the beginning in the table
        public ActionResult IndexGetAjax()
        {
            //all passengers list
            List<Passenger> allPassengers = db.Passengers.ToList();
            var jsonData = Json(new { passengers = allPassengers.Select(x => new { x.Name, x.Username, x.Stars, x.IsAdmin, x.TotalDistance, x.PassengerID }) }, JsonRequestBehavior.AllowGet);
            return jsonData;
        }

        [HttpPost]
        public ActionResult Index(String type, String input)
        {
            List<Passenger> ps = new List<Passenger>();

            //all passengers list
            List<Passenger> allPassengers = db.Passengers.ToList();
            var jsonData = Json(new { passengers = allPassengers.Select(x => new { x.Name, x.Username, x.Stars, x.IsAdmin, x.TotalDistance, x.PassengerID }) }, JsonRequestBehavior.AllowGet);

            //check if input is a number, if true, intInput = input
            int intInput;
            var isNumeric = int.TryParse(input, out intInput);


            //if no input or no radio button chosed
            if (type == null || input == null)
                return jsonData;
            //radio button min stars is checked
            if (type.Equals("stars") && isNumeric)
            {
                ps = allPassengers.Where(p => p.Stars >= intInput).ToList();
            }
            //radio button min reservations is checked
            else if (type.Equals("reservations") && isNumeric)
            {
                ps = allPassengers.Where(p => p.Reservations != null).ToList();

                // checks if the user has reservations
                if (ps != null)
                {
                    //all the passengers which have more reservations than the input
                    ps = ps.Where(p => p.Reservations.Count() >= intInput).ToList();
                }
            }
            //radio button name is checked
            else if (type.Equals("name"))
            {
                ps = allPassengers.Where(p => p.Name.Contains(input)).ToList();
            }

            //create json serialization in order to send to client
            jsonData = Json(new { passengers = ps.Select(x => new { x.Name, x.Username, x.Stars, x.IsAdmin, x.TotalDistance, x.PassengerID }) }, JsonRequestBehavior.AllowGet);
            return jsonData;
        }

        public ActionResult Login()
        {
            //checks the session ,if it is not empty the user is already logged in 
            if(Session["Username"] != null)
            {
                return RedirectToAction("Index", "Home", new { Username = Session["Username"].ToString() });
            }
            else
            {
            return View();
            } 
        }

        //the Login Process
        [HttpPost]
        public ActionResult Login(string UN, string PW)
        {
            //check if the username and password are  both correct
            var passengerLoggedIn = db.Passengers.Where(x => string.Equals(x.Username,UN) 
                                                                    && string.Equals(x.Password,PW)).AsEnumerable()
                                                                    .FirstOrDefault(x => string.Equals(x.Username, UN)
                                                                    && string.Equals(x.Password, PW));

            if (ModelState.IsValid)
            {
                //if the passenger is not logged in yet
                if (passengerLoggedIn == null)
                {
                    var passengerNotLoggedIn = db.Passengers.Where(x => string.Equals(x.Username,UN)).AsEnumerable().FirstOrDefault(x => string.Equals(x.Username, UN));
                    //username does not exist
                    if (passengerNotLoggedIn==null)
                    {
                        ModelState.AddModelError("Password", "Wrong Username and/or Password");
                    }
                    //password incorrect
                    else if (!string.Equals(passengerNotLoggedIn.Password,PW))
                    {
                        ModelState.AddModelError("Password", "Wrong Username and/or Password");
                    }
                    return View();
                }
                //username and password are both correct
                else
                {
                    //create the session
                    Session["Username"] = UN;
                    Session["PassengerID"] = passengerLoggedIn.PassengerID;

                    //create the session which indicates if the user is admin
                    Session["isAdmin"] = "0";
                    if (passengerLoggedIn.IsAdmin)
                        Session["isAdmin"] = "1";

                    return RedirectToAction("Index", "Home");
                }
            }
           else
            return View();
            
        }

        public  ActionResult Logout()
        {
            //remove the user's session
            Session["Username"] = null;
            return RedirectToAction("Index", "Home");
        }

        // GET: Passengers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Passenger passenger = db.Passengers.Find(id);
            if (passenger == null)
            {
                return HttpNotFound();
            }
            return View(passenger);
        }

        public ActionResult PassengerProfile()
        {
            //checks id the user is logged in, if he is ,show he's properties
            if (Session["Username"]!= null)
            {
            //get username from session  
            var usernameSession = Session["Username"].ToString();
            var passengerLoggedIn = db.Passengers.SingleOrDefault(x => x.Username == usernameSession);

            //send passenger to view
            return View(passengerLoggedIn);
            }
            return View();
        }

        // GET: Passengers/Create
        public ActionResult Create()
        { 
            return View();
        }

        // POST: Passengers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PassengerID,Name,Username,Password,Stars,IsAdmin,TotalDistance")] Passenger passenger)
        {
            if (ModelState.IsValid)
            {
                    //checks if the username already exists in the DB
                    if (db.Passengers.Any(x => x.Username == passenger.Username))
                    {
                        ModelState.AddModelError("Username", "Username already exists");
                        return View();
                    }
                //save passenger in the DB
                db.Passengers.Add(passenger);
                db.SaveChanges();

                //after the signing up, the user is logged in, update session
                Session["Username"] = passenger.Username;
                Session["isAdmin"] = "0";
                
                //checks if the use has admin permissions
                if (passenger.IsAdmin)
                    Session["isAdmin"] = "1";
                return RedirectToAction("Index","Home");
            }

            return View(passenger);
        }

        //joins passenger properties with the destination of the flights he ordered
        public ActionResult JoinPassengersAndDestinations()
        {
            //the passenger's destinations
            var passengersDests = (
                from p in db.Passengers
                join r in db.Reservations
                on p.PassengerID equals r.PassengerID
                select new
                {
                    PassengerName = p.Name,
                    Destination = r.Outbound.Destination.Name,
                }).AsEnumerable().Select(x => Tuple.Create(x.PassengerName, x.Destination)).ToList();

            return View(passengersDests);
        }

        // GET: Passengers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Passenger passenger = db.Passengers.Find(id);
            if (passenger == null)
            {
                return HttpNotFound();
            }
            return View(passenger);
        }

        // POST: Passengers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PassengerID,Name,Username,Password,Stars,IsAdmin,TotalDistance")] Passenger passenger)
        {
            if (ModelState.IsValid)
            {
                //save passenger changes in the DB
                db.Entry(passenger).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(passenger);
        }

        // GET: Passengers/Edit/5
        public ActionResult PassengerEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Passenger passenger = db.Passengers.Find(id);
            if (passenger == null)
            {
                return HttpNotFound();
            }
            return View(passenger);
        }

        // POST: Passengers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PassengerEdit([Bind(Include = "PassengerID,Name,Username,Password,Stars,IsAdmin,TotalDistance")] Passenger passenger)
        {
            if (ModelState.IsValid)         
            {
                //boolean which checks if the passenger exist
                var exists = db.Passengers.FirstOrDefault(x => x.Username == passenger.Username && x.PassengerID != passenger.PassengerID);
                
                //checks if the user exists in the DB
                if (exists != null)
                {
                        ModelState.AddModelError("Username", "Username already exists");
                        return View(passenger);
                }
                //save changes in DB
                db.Entry(passenger).State = EntityState.Modified;
                db.SaveChanges();

                //update session the the username has changed
                Session["Username"] = passenger.Username;
                return RedirectToAction("Index", "Home");
            }
            return View(passenger);
        }

        // GET: Passengers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return View();
            }
            Passenger passenger = db.Passengers.Find(id);
            if (passenger == null)
            {
                return HttpNotFound();
            }
            return View(passenger);
        }

        // POST: Passengers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Passenger passenger = db.Passengers.Find(id);
            db.Passengers.Remove(passenger);
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
