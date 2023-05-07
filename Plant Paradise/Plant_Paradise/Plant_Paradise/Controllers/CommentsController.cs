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
    [Authorize(Roles = "Admin")]

    public class CommentsController : Controller
    {
        private PlantsEntities db = new PlantsEntities();

        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.AspNetUser).Include(c => c.Product);
            return View(comments.ToList());
        }
        [HttpPost]
        public ActionResult Search(string text)
        {
            try
            {
                var comm = db.Comments.Where(p => p.Comment_text.Contains(text) || p.AspNetUser.Full_Name.Contains(text) || p.Product.Product_Name.Contains(text)).ToList();
                return View("Index", comm);
            }
            catch
            {
                TempData["ErrorinComm"] = "Error Search On the Comment";
                return View("Index");
            }
        }
        [Authorize]
        public ActionResult AddComment()
        {
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.Product_id = new SelectList(db.Products, "Product_id", "Product_Name");
            return View();
        }
        [HttpPost]
        public ActionResult AddComment([Bind(Include = "Comment_id,Comment_text,userId,Product_id")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                var userid = User.Identity.GetUserId();
                comment.userId = userid;
                var user=db.AspNetUsers.Where(p=>p.Id==userid).FirstOrDefault();
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", comment.userId);
            ViewBag.Product_id = new SelectList(db.Products, "Product_id", "Product_Name", comment.Product_id);
            return View(comment);
        }
        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Comment comment = db.Comments.Find(id);
                if (comment == null)
                {
                    return HttpNotFound();
                }
                return View(comment);
            }
            catch
            {
                          
                TempData["ErrorinComm"] = "Error happens when Show the Detailes for the Comment";
                return RedirectToAction("Index");
            }
        }

        // GET: Comments/Create
        public ActionResult Create()
        {
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.Product_id = new SelectList(db.Products, "Product_id", "Product_Name");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Comment_id,Comment_text,userId,Product_id")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", comment.userId);
            ViewBag.Product_id = new SelectList(db.Products, "Product_id", "Product_Name", comment.Product_id);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", comment.userId);
            ViewBag.Product_id = new SelectList(db.Products, "Product_id", "Product_Name", comment.Product_id);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Comment_id,Comment_text,userId,Product_id")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", comment.userId);
            ViewBag.Product_id = new SelectList(db.Products, "Product_id", "Product_Name", comment.Product_id);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                var comm = db.Comments.Find(id);
                db.Comments.Remove(comm);
                db.SaveChanges();
            }
            catch
            {
                TempData["ErrorinComm"] = "Error happens when Delete  the Comment";
            }
            return RedirectToAction("Index");

        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Comment comment = db.Comments.Find(id);
                db.Comments.Remove(comment);
                db.SaveChanges();
            }
            catch
            {
                TempData["ErrorinComm"] = "Error happens when Delete  the Comment";
            }
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
