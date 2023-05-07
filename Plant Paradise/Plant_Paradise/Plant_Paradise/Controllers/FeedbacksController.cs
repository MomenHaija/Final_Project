using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Plant_Paradise.Models;

namespace Plant_Paradise.Controllers
{
    public class FeedbacksController : Controller
    {
        private PlantsEntities db = new PlantsEntities();
        [Authorize]
        // GET: Feedbacks
        public ActionResult Index()
        {
            try
            {
                var User_id = User.Identity.GetUserId();
                var feedbacks = db.Feedbacks.Include(f => f.AspNetUser).Where(p => p.userId == User_id);
                return View(feedbacks.ToList());
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
       


        [HttpPost]
        [Authorize]
        public ActionResult Addfeeback([Bind(Include = "Feedback_id,Feedback_text,Feedback_Date,userId,IsAccepted")] Feedback feedback)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var USer_Id = User.Identity.GetUserId();
                    feedback.userId = USer_Id;
                    feedback.Feedback_Date = DateTime.Now;
                    feedback.IsAccepted = 0;
                    db.Feedbacks.Add(feedback);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", feedback.userId);
                return View(feedback);
            }
            catch
            {
                return RedirectToAction("Error", "Home", new { message = "you are Unauthorized To Add Feedback" });
            }
        }


        // GET: Feedbacks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedback = db.Feedbacks.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            return View(feedback);
        }

        // GET: Feedbacks/Create
     

        // GET: Feedbacks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedback = db.Feedbacks.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", feedback.userId);
            return View(feedback);
        }

        // POST: Feedbacks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Feedback_id,Feedback_text,Feedback_Date,userId,IsAccepted")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                db.Entry(feedback).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", feedback.userId);
            return View(feedback);
        }

        // GET: Feedbacks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feedback feedback = db.Feedbacks.Find(id);
            if (feedback == null)
            {
                return HttpNotFound();
            }
            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Feedback feedback = db.Feedbacks.Find(id);
            db.Feedbacks.Remove(feedback);
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
