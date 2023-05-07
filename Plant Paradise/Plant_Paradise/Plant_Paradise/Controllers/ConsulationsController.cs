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
    public class ConsulationsController : Controller
    {
        private PlantsEntities db = new PlantsEntities();

        // GET: Consulations
        public ActionResult Index()
        {
            var consulations = db.Consulations.Include(c => c.AspNetUser).Include(c => c.Order);
            return View(consulations.ToList());
        }

        // GET: Consulations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consulation consulation = db.Consulations.Find(id);
            if (consulation == null)
            {
                return HttpNotFound();
            }
            return View(consulation);
        }

        // GET: Consulations/Create
        public ActionResult Create()
        {
            ViewBag.User_Id = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.Order_id = new SelectList(db.Orders, "Order_id", "userId");
            return View();
        }

        // POST: Consulations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Consulation_Id,User_Id,Order_id,Plant_status,Plant_image")] Consulation consulation)
        {
            if (ModelState.IsValid)
            {
                db.Consulations.Add(consulation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.User_Id = new SelectList(db.AspNetUsers, "Id", "Email", consulation.User_Id);
            ViewBag.Order_id = new SelectList(db.Orders, "Order_id", "userId", consulation.Order_id);
            return View(consulation);
        }

        // GET: Consulations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consulation consulation = db.Consulations.Find(id);
            if (consulation == null)
            {
                return HttpNotFound();
            }
            ViewBag.User_Id = new SelectList(db.AspNetUsers, "Id", "Email", consulation.User_Id);
            ViewBag.Order_id = new SelectList(db.Orders, "Order_id", "userId", consulation.Order_id);
            return View(consulation);
        }

        // POST: Consulations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Consulation_Id,User_Id,Order_id,Plant_status,Plant_image")] Consulation consulation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(consulation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.User_Id = new SelectList(db.AspNetUsers, "Id", "Email", consulation.User_Id);
            ViewBag.Order_id = new SelectList(db.Orders, "Order_id", "userId", consulation.Order_id);
            return View(consulation);
        }

        // GET: Consulations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consulation consulation = db.Consulations.Find(id);
            if (consulation == null)
            {
                return HttpNotFound();
            }
            return View(consulation);
        }

        // POST: Consulations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Consulation consulation = db.Consulations.Find(id);
            db.Consulations.Remove(consulation);
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
