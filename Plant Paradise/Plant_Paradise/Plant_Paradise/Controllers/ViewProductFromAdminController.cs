using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Plant_Paradise.Models;

namespace Plant_Paradise.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ViewProductFromAdminController : Controller
    {
        private PlantsEntities db = new PlantsEntities();

        // GET: ViewProductFromAdmin
        public ActionResult Index(string id)
        {
            Session["Orderid"] = id; 
            var order_Details = db.Order_Details.Include(o => o.Order).Include(o => o.Product).Where(p=>p.Order_id==id);
            return View(order_Details.ToList());
        }
        [HttpPost]
        public ActionResult search(string Search)
        {
            string id = Session["Orderid"].ToString();
            var OrderProducts = db.Order_Details.Include(s => s.Order).Where(p => p.Product.Product_Name.Contains(Search) && p.Order_id == id);
            return View("Index", OrderProducts.ToList());

        }

        // GET: ViewProductFromAdmin/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order_Details order_Details = db.Order_Details.Find(id);
            if (order_Details == null)
            {
                return HttpNotFound();
            }
            return View(order_Details);
        }

        // GET: ViewProductFromAdmin/Create
        public ActionResult Create()
        {
            ViewBag.Order_id = new SelectList(db.Orders, "Order_id", "userId");
            ViewBag.Product_id = new SelectList(db.Products, "Product_id", "Product_Name");
            return View();
        }

        // POST: ViewProductFromAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderDetails,Order_id,Product_id,Quantity")] Order_Details order_Details)
        {
            if (ModelState.IsValid)
            {
                db.Order_Details.Add(order_Details);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Order_id = new SelectList(db.Orders, "Order_id", "userId", order_Details.Order_id);
            ViewBag.Product_id = new SelectList(db.Products, "Product_id", "Product_Name", order_Details.Product_id);
            return View(order_Details);
        }

        // GET: ViewProductFromAdmin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order_Details order_Details = db.Order_Details.Find(id);
            if (order_Details == null)
            {
                return HttpNotFound();
            }
            ViewBag.Order_id = new SelectList(db.Orders, "Order_id", "userId", order_Details.Order_id);
            ViewBag.Product_id = new SelectList(db.Products, "Product_id", "Product_Name", order_Details.Product_id);
            return View(order_Details);
        }

        // POST: ViewProductFromAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderDetails,Order_id,Product_id,Quantity")] Order_Details order_Details)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order_Details).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Order_id = new SelectList(db.Orders, "Order_id", "userId", order_Details.Order_id);
            ViewBag.Product_id = new SelectList(db.Products, "Product_id", "Product_Name", order_Details.Product_id);
            return View(order_Details);
        }

        // GET: ViewProductFromAdmin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order_Details order_Details = db.Order_Details.Find(id);
            if (order_Details == null)
            {
                return HttpNotFound();
            }
            return View(order_Details);
        }

        // POST: ViewProductFromAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order_Details order_Details = db.Order_Details.Find(id);
            db.Order_Details.Remove(order_Details);
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
