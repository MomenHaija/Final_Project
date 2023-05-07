using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Plant_Paradise.Models;

namespace Plant_Paradise.Controllers
{
    [HandleError]
    [Authorize]
     
    public class UserProfileController : Controller
    {
        private PlantsEntities db = new PlantsEntities();

        // GET: UserProfile
    
        public ActionResult Index()
        {
            try
            {
                var User_id = User.Identity.GetUserId();
                return View(db.AspNetUsers.Where(p => p.Id == User_id).FirstOrDefault());   
            }
            catch { 
                return RedirectToAction("Index","Home");
            }
        }
        [HttpPost]
        public ActionResult EditProfile([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,Phone_Number,User_Address,Full_Name,User_Image")] AspNetUser aspNetUser ,string name,string email,string address,string phone, HttpPostedFileBase user_image)
        {
            try
            {
                var User_id = User.Identity.GetUserId();

                var user = db.AspNetUsers.Where(p => p.Id == User_id).FirstOrDefault();
                string path2 = user.User_Image;
                try
                {
                    if (user_image != null)
                    {
                        string fileName = Path.GetFileName(user_image.FileName);
                        string path = "../Images/" + fileName;
                        path2 = Path.GetFileName(user_image.FileName);
                        string fullPath = Server.MapPath(path);
                        user_image.SaveAs(fullPath);
                        user.User_Image = path2.ToString();
                    }
                }
                catch
                {
                    TempData["Error"] = "some Error happened when upload the photo";
                }
                if (name != null)
                    user.Full_Name = name;
                if (phone != null)
                    user.Phone_Number = phone;
                if (address != null)
                    user.User_Address = address;

                db.SaveChanges();
                return RedirectToAction("Index", "UserProfile");

            }
            catch
            {
                return RedirectToAction("Error", "Home", new { message = "some Error happened when update the profile" });

            }
        }
        public PartialViewResult Oreder_User()
        {

            try { 
                var User_id = User.Identity.GetUserId();
                var order = db.Orders.Where(p => p.userId == User_id);
                if (order.Count() == 0)
                {
                    ViewBag.Empty = "He hasn't bought anything yet";
                }
                return PartialView("UserOrder", order.ToList());
            }
            catch {
                return PartialView("UserOrder", null);

            }
        }

        public PartialViewResult ProductsinOrders(string id)
        {
            try
            {
                var order_detailes = db.Order_Details.Where(p => p.Order_id == id);

                return PartialView("ProductsinOrders", order_detailes.ToList());
            }
            catch
            {
                return PartialView("ProductsinOrders", null);

            }
        }

        // GET: UserProfile/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // GET: UserProfile/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserProfile/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,Phone_Number,User_Address,Full_Name")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.AspNetUsers.Add(aspNetUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aspNetUser);
        }

        // GET: UserProfile/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }
        public ActionResult AdminProfile()
        {
            return View();
        }
        // POST: UserProfile/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,Phone_Number,User_Address,Full_Name")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspNetUser);
        }

        // GET: UserProfile/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: UserProfile/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUser);
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
