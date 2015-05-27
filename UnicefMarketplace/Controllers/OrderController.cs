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
    public class OrderController : Controller
    {
        private UnicefMarketplaceEntities db = new UnicefMarketplaceEntities();

        //
        // GET: /Order/

        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Product).Include(o => o.Region);
            return View(orders.ToList());
        }

        //
        // GET: /Order/Details/5

        public ActionResult Details(int id = 0)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        //
        // GET: /Order/Create

        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "ProductName");
            ViewBag.RegionId = new SelectList(db.Regions, "RegionId", "Name");
            return View();
        }

        //
        // POST: /Order/Create

        [HttpPost]
        public ActionResult Create(Order order)
        {
            order.DateEntered = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "ProductName", order.ProductId);
            ViewBag.RegionId = new SelectList(db.Regions, "RegionId", "Name", order.RegionId);
            return View(order);
        }

        //
        // GET: /Order/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "ProductName", order.ProductId);
            ViewBag.RegionId = new SelectList(db.Regions, "RegionId", "Name", order.RegionId);
            return View(order);
        }

        //
        // POST: /Order/Edit/5

        [HttpPost]
        public ActionResult Edit(Order order)
        {
            order.DateEntered = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "ProductName", order.ProductId);
            ViewBag.RegionId = new SelectList(db.Regions, "RegionId", "Name", order.RegionId);
            return View(order);
        }

        //
        // GET: /Order/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        //
        // POST: /Order/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        /*Populate Orders table with test data*/
        public ActionResult PopulateTestData()
        {
            var db2 = new UnicefMarketplaceEntities();
            var productPriceRandomizer = new Random();
            //foreach(var region in db.Regions)
            //{
                foreach (var product in db2.Products)
                {
                
                    //for 0 to 200 days
                        //create random number of orders for this day (5-10)
                        //for each new order
                            //come up with random price +/- 5% of generated price
                            //insert order
                    var baseProductPrice = productPriceRandomizer.Next(20,50);
                    var orderCountRandomizer = new Random();

                    for (int i = 1; i < 201; i++)
                    {
                        var numOrders = orderCountRandomizer.Next(1, 10);
                        var dailyPriceRandomizer = new Random();
                        var currentDate = DateTime.Today.AddDays(-1 * i);
                        for (int q = 0; q < numOrders; q++)
                        {
                            var next = dailyPriceRandomizer.NextDouble();
                            var price = Math.Round( (.95 * baseProductPrice + (next + (1.05 * baseProductPrice - .95 * baseProductPrice))), 2);
                            
                            using (var db3 = new UnicefMarketplaceEntities())
                            {
                                var region = db3.Regions.Find(1);
                                Order order = new Order
                                {
                                    DateEntered = currentDate,
                                    Price = Convert.ToDecimal(price),
                                    Product = db3.Products.Find(product.ProductId),
                                    Region = region
                                };

                                db3.Orders.Add(order);
                                db3.SaveChanges();
                            }
                            Response.Write(product.ProductName + ", " + currentDate + ", Order #" + q + ", " + price + "<br>");
                        }
                    }

                }
            //}
            
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}