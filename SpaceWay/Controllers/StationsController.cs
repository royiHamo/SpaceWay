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
    public class StationsController : Controller
    {
        private SpaceWayDbContext db = new SpaceWayDbContext();

        // GET: Stations
        public ActionResult Index(string planetToFilter)
        {
            if(planetToFilter == null)
                return View(db.Stations.ToList());

            //checks if the input in the filter equlas to the planet
            var filteredStations = from s in db.Stations.ToList()
                                   where string.Equals(s.Planet,planetToFilter,StringComparison.OrdinalIgnoreCase)
                                   select s;

            //checks if the filter didn't return any result ,then return all the stations

           return !filteredStations.Any() ? View(new List<Station>()) : View(filteredStations);

        }

        // POST: Stations
        [HttpPost]
        public ActionResult Search(string planet)
        {
            //if the input is empty, redirect to GET action index without the input
            if (string.IsNullOrEmpty(planet))
                return RedirectToAction("Index");

            //redirect to the GET action index, in order to search in the DB and return results
            return RedirectToAction("Index", new { @planetToFilter = planet });
        }

        // GET: Stations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Station station = db.Stations.Find(id);
            if (station == null)
            {
                return HttpNotFound();
            }
            return View(station);
        }

        // GET: Stations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Stations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StationID,Planet,Name,Address")] Station station)
        {
            if (ModelState.IsValid)
            {
                //add station to the DB
                db.Stations.Add(station);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(station);
        }

        // GET: Stations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Station station = db.Stations.Find(id);
            if (station == null)
            {
                return HttpNotFound();
            }
            return View(station);
        }

        // POST: Stations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StationID,Planet,Name,Address")] Station station)
        {
            if (ModelState.IsValid)
            {
                //save changes in the DB
                db.Entry(station).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(station);
        }

        // GET: Stations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Station station = db.Stations.Find(id);
            if (station == null)
            {
                return HttpNotFound();
            }
            return View(station);
        }

        // POST: Stations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Station station = db.Stations.Find(id);

            var exists = from   f in db.Flights.ToList()
                         where  station.StationID == f.Origin.StationID ||
                                station.StationID == f.Destination.StationID
                         select f;

            if (exists.Any())
            {
                ModelState.AddModelError(string.Empty, "This station is attached to a flight");
                return View(station);
            }

            //removes station from the DB
            db.Stations.Remove(station);
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
