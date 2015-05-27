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
    public class RegionController : Controller
    {
        private UnicefMarketplaceEntities db = new UnicefMarketplaceEntities();

        //
        // GET: /Region/

        public ActionResult Index()
        {
            return View(db.Regions.ToList());
        }

        //
        // GET: /Region/Details/5

        public ActionResult Details(int id = 0)
        {
            Region region = db.Regions.Find(id);

            ViewBag.RegionalEvents = GetEvents(id);

            if (region == null)
            {
                return HttpNotFound();
            }

            ViewBag.Products = db.Products.ToList();

            return View(region);
        }

        //
        // GET: /Region/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Region/Create

        [HttpPost]
        public ActionResult Create(Region region)
        {
            if (ModelState.IsValid)
            {
                db.Regions.Add(region);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(region);
        }

        //
        // GET: /Region/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Region region = db.Regions.Find(id);
            if (region == null)
            {
                return HttpNotFound();
            }
            return View(region);
        }

        //
        // POST: /Region/Edit/5

        [HttpPost]
        public ActionResult Edit(Region region)
        {
            if (ModelState.IsValid)
            {
                db.Entry(region).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(region);
        }

        //
        // GET: /Region/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Region region = db.Regions.Find(id);
            if (region == null)
            {
                return HttpNotFound();
            }
            return View(region);
        }

        //
        // POST: /Region/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Region region = db.Regions.Find(id);
            db.Regions.Remove(region);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private List<MovingAverage> GetMovingAverages(int regionId, int productId)
        {
            List<int> days = new List<int> { 1, 2, 5, 10, 50, 200 };
            List<MovingAverage> averages = new List<MovingAverage>();

            foreach (var day in days)
            {
                var dateSince = DateTime.Today.AddDays(-1 * day);
                var averageQuery = from o in db.Orders
                                   where o.DateEntered >= dateSince && o.ProductId == productId && o.RegionId == regionId
                                   select o;
                var averageOrderCount = averageQuery.Count() / day;
                var averagePrice = averageQuery.Average(x => x.Price);

                averages.Add(new MovingAverage { Days = day, Orders = averageOrderCount, Price = averagePrice});
            }

            return averages;
        }

        private decimal GetDemandSurgePrice(List<MovingAverage> averages)
        {
            var twoDayMovingAverage = averages[1].Orders;
            var fiveDayMovingAverage = averages[2].Orders;

            var fiveDayMAPlus15 = fiveDayMovingAverage * 1.15;

            if (twoDayMovingAverage > fiveDayMAPlus15)
            {
                var changeFactor = (twoDayMovingAverage - fiveDayMovingAverage) * 1.0 / fiveDayMovingAverage;
                var priceWithChangedFactor = averages[2].Price * (1 + Convert.ToDecimal(changeFactor));
                priceWithChangedFactor = priceWithChangedFactor * Convert.ToDecimal(1.05);

                return priceWithChangedFactor;
            }

            return averages[2].Price;
        }

        private decimal GetBreakoutPrice(List<MovingAverage> averages)
        {
            var fiveDayPrice = averages[2].Price;
            var twoHundredDayprice = averages[5].Price;

            

            if (fiveDayPrice > twoHundredDayprice)
            {
                var newPrice = fiveDayPrice * Convert.ToDecimal(1.25);
                return newPrice;
            }

            return fiveDayPrice;
        }

        private decimal GetEventSurgePrice(decimal fiveDayPrice, IEnumerable<RegionEvent> events)
        {
            var newPrice = fiveDayPrice;
            foreach(var regionalEvent in events)
            {
                newPrice *= Convert.ToDecimal(1 + (regionalEvent.Severity * .1));
            }
            return newPrice;
        }

        private IEnumerable<RegionEvent> GetEvents(int regionId)
        {
            return from e in db.RegionEvents where e.RegionId == regionId select e;
        }

        public ActionResult RegionProductDetails(int id, int productId)
        {
            if (db.Orders.Where(o => o.RegionId == id && o.ProductId == productId).Count() == 0)
            {
                ViewBag.HasData = false;
                ViewBag.Product = db.Products.Find(productId);
                ViewBag.Region = db.Regions.Find(id);
            }
            else
            {
                ViewBag.HasData = true;
                var movingAverages = GetMovingAverages(id, productId);
                var regionalEvents = GetEvents(id);

                var demandSurgePrice = GetDemandSurgePrice(movingAverages);
                var breakoutPrice = GetBreakoutPrice(movingAverages);
                var eventSurgePrice = GetEventSurgePrice(movingAverages[2].Price, regionalEvents);

                ViewBag.MovingAverages = movingAverages;
                ViewBag.RegionalEvents = GetEvents(id);

                ViewBag.FiveDayPrice = movingAverages[2].Price;
                ViewBag.DemandSurgePrice = demandSurgePrice;
                ViewBag.BreakoutPrice = breakoutPrice;
                ViewBag.EventSurgePrice = eventSurgePrice;
            }
            return View();
        }
    }
}