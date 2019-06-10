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
            if (Session["Username"] != null)
            {
                ViewBag.Username = Username;
                return View();
                //return RedirectToAction("Index", "Home", new { Username = Session["Username"].ToString() });
            }
            return View();
        }

        public ActionResult Login()
        {
            if(Session["Username"] != null)
            {
                return RedirectToAction("Index", "Home", new { Username = Session["Username"].ToString() });
            }
            else
            {
            return View();
            } 
        }

        [HttpPost]
        public ActionResult Login(string UN, string PW)
        {
            var passengerLoggedIn = db.Passengers.SingleOrDefault(x => x.Username == UN && x.Password == PW);

            if (ModelState.IsValid)
            {
                if (passengerLoggedIn == null)
                {
                    var passengerNotLoggedIn = db.Passengers.SingleOrDefault(x => x.Username == UN);
                    if (passengerNotLoggedIn==null)
                    {
                        ModelState.AddModelError("Username", "Username does not exists");
                    }
                    else if (!(passengerNotLoggedIn.Password == PW))
                    {
                        ModelState.AddModelError("Password", "Password does not correct");
                    }
                    return View();
                }
                else
                {
                    Session["Username"] = UN;
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
            Session["Username"] = null;
            return RedirectToAction("Index", "Home");
        }

            // GET: Search
            public ActionResult Search(List<Passenger> l)
        {
            if(l!=null)
            return View(l);
            return View(new List<Passenger>());
        }

        [HttpPost]
        public ActionResult Search(string input, FormCollection fc)
        {
            var result = fc["search"];
            if(result == null)
            {
                return View(new List<Passenger>());
            }
            if (result.Equals("stars"))
            {
                return View(db.Passengers.ToList().Where(p => p.Stars >= Int32.Parse(input)).ToList());
            }
            if (result.Equals("reservations"))
            {
                List<Passenger> passengers = db.Passengers.ToList().Where(p => p.Reservations != null).ToList();
                if(passengers != null)
                {
                return View(passengers.Where(p => p.Reservations.Count() >= Int32.Parse(input)).ToList());
                }
                return View(new List<Passenger>());
            }
            if (result.Equals("name"))
            {
                return View(db.Passengers.ToList().Where(p => p.Name.Contains(input)).ToList());
            }
            return View(new List<Passenger>());
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
            var usernameSession = Session["Username"].ToString();
            var passengerLoggedIn = db.Passengers.SingleOrDefault(x => x.Username == usernameSession);

            return View(passengerLoggedIn);
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
                
                    if (db.Passengers.Any(x => x.Username == passenger.Username))
                    {
                        ModelState.AddModelError("Username", "Username already exists");
                        return View();
                    }
                db.Passengers.Add(passenger);
                db.SaveChanges();
                Session["Username"] = passenger.Username;
                Session["isAdmin"] = "0";
                if (passenger.IsAdmin)
                    Session["isAdmin"] = "1";
                return RedirectToAction("Index","Home");
            }

            return View(passenger);
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
                db.Entry(passenger).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(passenger);
        }

        // GET: Passengers/Delete/5
        public ActionResult Delete(int? id)
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
