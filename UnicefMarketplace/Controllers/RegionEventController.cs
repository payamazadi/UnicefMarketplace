using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UnicefMarketplace.Models;

namespace UnicefMarketplace.Controllers
{
    public class RegionEventController : Controller
    {
        private UnicefMarketplaceEntities db = new UnicefMarketplaceEntities();

        //
        // GET: /RegionEvent/

        public ActionResult Index()
        {
            var regionevents = db.RegionEvents.Include(r => r.Region);
            return View(regionevents.ToList());
        }

        //
        // GET: /RegionEvent/Details/5

        public ActionResult Details(int id = 0)
        {
            RegionEvent regionevent = db.RegionEvents.Find(id);
            if (regionevent == null)
            {
                return HttpNotFound();
            }
            return View(regionevent);
        }

        //
        // GET: /RegionEvent/Create

        public ActionResult Create()
        {
            ViewBag.RegionId = new SelectList(db.Regions, "RegionId", "Name");
            return View();
        }

        //
        // POST: /RegionEvent/Create

        [HttpPost]
        public ActionResult Create(RegionEvent regionevent)
        {
            if (ModelState.IsValid)
            {
                db.RegionEvents.Add(regionevent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RegionId = new SelectList(db.Regions, "RegionId", "Name", regionevent.RegionId);
            return View(regionevent);
        }

        //
        // GET: /RegionEvent/Edit/5

        public ActionResult Edit(int id = 0)
        {
            RegionEvent regionevent = db.RegionEvents.Find(id);
            if (regionevent == null)
            {
                return HttpNotFound();
            }
            ViewBag.RegionId = new SelectList(db.Regions, "RegionId", "Name", regionevent.RegionId);
            return View(regionevent);
        }

        //
        // POST: /RegionEvent/Edit/5

        [HttpPost]
        public ActionResult Edit(RegionEvent regionevent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(regionevent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RegionId = new SelectList(db.Regions, "RegionId", "Name", regionevent.RegionId);
            return View(regionevent);
        }

        //
        // GET: /RegionEvent/Delete/5

        public ActionResult Delete(int id = 0)
        {
            RegionEvent regionevent = db.RegionEvents.Find(id);
            if (regionevent == null)
            {
                return HttpNotFound();
            }
            return View(regionevent);
        }

        //
        // POST: /RegionEvent/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            RegionEvent regionevent = db.RegionEvents.Find(id);
            db.RegionEvents.Remove(regionevent);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}