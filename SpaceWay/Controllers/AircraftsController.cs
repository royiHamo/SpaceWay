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
    public class AircraftsController : Controller
    {
        private SpaceWayDbContext db = new SpaceWayDbContext();

        // GET: Aircrafts
        public ActionResult Index(int? levelToFilter)
        {
            List<Aircraft> airs = new List<Aircraft>();

            //if the user just opened the page
            if (levelToFilter == null || levelToFilter == -1)                        
            {
                return View(db.Aircrafts.ToList());
            }

            //if input is not valid
            if (levelToFilter == 0)                        
            {
                ModelState.AddModelError(string.Empty, "Please enter a valid input");
                return View(new List<Aircraft>());
            }

            //search by user's input 'levelToFilter'
            airs = db.Aircrafts.ToList().Where(a => a.Level == levelToFilter).ToList();

            //no results
            return !airs.Any() ? View(new List<Aircraft>()) : View(airs);
        }

        // POST: Aircrafts
        [HttpPost]
        public ActionResult Search(string lvl)
        {
            int n;
            bool isNumeric = int.TryParse(lvl, out n);

            //if input is empty ε for example
            if (string.IsNullOrEmpty(lvl))
            {
                return RedirectToAction("Index", new { @levelToFilter = -1 });
            }

            //if the input is not valid
            if (!isNumeric)
            { 
                return RedirectToAction("Index", new { @levelToFilter = 0 });
            }

            //redirect to GET action Index with the input which turned to int
            int level = Convert.ToInt16(lvl);
            return RedirectToAction("Index", new { @levelToFilter = level });
        }


        // GET: Aircrafts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            Aircraft aircraft = db.Aircrafts.Find(id);
            if (aircraft == null)
            {
                return View("Error");
            }
            return View(aircraft);
        }

        // GET: Aircrafts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Aircrafts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AircraftID,Level,Seats")] Aircraft aircraft)
        {
            if (ModelState.IsValid)
            {
                //save new aircraft to DB
                db.Aircrafts.Add(aircraft);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aircraft);
        }

        // GET: Aircrafts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            Aircraft aircraft = db.Aircrafts.Find(id);
            if (aircraft == null)
            {
                return View("Error");
            }
            return View(aircraft);
        }

        // POST: Aircrafts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AircraftID,Level,Seats")] Aircraft aircraft)
        {
            if (ModelState.IsValid)
            {
                //save changes in the DB
                db.Entry(aircraft).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aircraft);
        }

        // GET: Aircrafts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            Aircraft aircraft = db.Aircrafts.Find(id);
            if (aircraft == null)
            {
                return View("Error");
            }
            return View(aircraft);
        }

        // POST: Aircrafts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Aircraft aircraft = db.Aircrafts.Find(id);
            var exists = from   f in db.Flights.ToList()
                          where  aircraft.AircraftID == f.AircraftID ||
                                 aircraft.AircraftID == f.AircraftID
                          select f;
            if(exists.Any())
            {
                ModelState.AddModelError(string.Empty, "This aircaft is attached to a flight");
                return View(aircraft);
            }

            //removes the aircraft from the DB
            db.Aircrafts.Remove(aircraft);
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


