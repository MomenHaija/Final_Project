using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Plant_Paradise.Models;

namespace Plant_Paradise.Controllers
{
    public class OrdersAdminController : Controller
    {
        private PlantsEntities db = new PlantsEntities();

        // GET: OrdersAdmin
        public ActionResult Index()
        {
            var orders = db.Transactions.Include(p=>p.Order).Include(o => o.AspNetUser);
            return View(orders.ToList());
        }
        [HttpPost]
        public ActionResult search(string Search,int order=0)
        {
            try
            {
                var orders = db.Transactions.Include(p => p.Order).Include(o => o.AspNetUser);
                if (order == 0 || order == 2)
                {
                    orders = orders.Where(p => p.AspNetUser.Full_Name.Contains(Search) || p.Order_id.Contains(Search));
                }
                else if (order == 1)
                {
                    orders = orders.Where(p => (p.AspNetUser.Full_Name.Contains(Search) || p.Order_id.Contains(Search)) && p.Order.Order_date == DateTime.Today);

                }
                return View("Index", orders.ToList());

            }
            catch {
                TempData["ErrorinOrder"] = "Oops!....Something went wrong when searching on demand";
                return View("Index");
            }
        }
        // GET: OrdersAdmin/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: OrdersAdmin/Create
        public ActionResult Create()
        {
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email");
            return View();
        }

        // POST: OrdersAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Order_id,Order_date,userId,Total_price")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", order.userId);
            return View(order);
        }

        // GET: OrdersAdmin/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", order.userId);
            return View(order);
        }

        // POST: OrdersAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Order_id,Order_date,userId,Total_price")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", order.userId);
            return View(order);
        }

        // GET: OrdersAdmin/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: OrdersAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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
