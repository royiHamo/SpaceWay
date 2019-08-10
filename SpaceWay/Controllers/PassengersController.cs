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

            }
            IndexGetAjax();
            return View();
        }

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
            //radio button is stars
            if (type.Equals("stars") && isNumeric)
            {
                ps = allPassengers.Where(p => p.Stars >= intInput).ToList();
            }
            //radio button is reservations
            else if (type.Equals("reservations") && isNumeric)
            {
                ps = allPassengers.Where(p => p.Reservations != null).ToList();
                if (ps != null)
                {
                    ps = ps.Where(p => p.Reservations.Count() >= intInput).ToList();
                }
            }
            //radio button is name
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
                        ModelState.AddModelError("Password", "Wrong Username and/or Password");
                    }
                    else if (!(passengerNotLoggedIn.Password == PW))
                    {
                        ModelState.AddModelError("Password", "Wrong Username and/or Password");
                    }
                    return View();
                }
                else
                {
                    Session["Username"] = UN;
                    Session["PassengerID"] = passengerLoggedIn.PassengerID;
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
            public ActionResult Search()
        {
            
            return View(db.Passengers.ToList());
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
                return View(db.Passengers.ToList().Where(p => p.Stars >= int.Parse(input)).ToList());
            }
            if (result.Equals("reservations"))
            {
                List<Passenger> passengers = db.Passengers.ToList().Where(p => p.Reservations != null).ToList();
                if(passengers != null)
                {
                return View(passengers.Where(p => p.Reservations.Count() >= int.Parse(input)).ToList());
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

        public ActionResult JoinPassengersAndDestinations()
        {
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
            if (ModelState.IsValid)         //need to enable option for editing the password only
            {
                ////assigning reservations
                //for (int i =0;i< passenger.Reservations.Count;i++)
                //{
                //    int id = passenger.Reservations[i].ReservationID;
                //    passenger.Reservations[i] = db.Reservations.FirstOrDefault(x => x.ReservationID == id);
                //}

                var exists = db.Passengers.FirstOrDefault(x => x.Username == passenger.Username && x.PassengerID != passenger.PassengerID);
                if (exists != null)
                {
                        ModelState.AddModelError("Username", "Username already exists");
                        return View(passenger);
                }
                db.Entry(passenger).State = EntityState.Modified;
                db.SaveChanges();
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
