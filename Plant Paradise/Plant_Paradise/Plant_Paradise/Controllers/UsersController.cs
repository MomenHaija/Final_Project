using Microsoft.AspNet.Identity;
using Plant_Paradise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Plant_Paradise.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        // GET: Users
        PlantsEntities db = new PlantsEntities();

        public ActionResult Index()
        {
            var users = db.AspNetUsers;
            return View(users.ToList());
        }
        [HttpPost]
        public ActionResult search(string Search)
        {
            try
            {
                var users = db.AspNetUsers.Where(p => p.Full_Name.Contains(Search) || p.Email.Contains(Search) || p.User_Address.Contains(Search));
                return View("Index", users.ToList());
            }
            catch
            {
                TempData["ErrorinUser"] = "Cannot Found the Specific user";
                return Index();
            }
        }
        // GET: Users/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection,string Email,string User_Address,string Phone_Number,string Full_Name,string User_Image)
        {
            try
            {

                // TODO: Add insert logic here
                var user = new AspNetUser();
                user.Full_Name= Full_Name;
                user.Email= Email;
                user.User_Address= User_Address;
                user.Phone_Number= Phone_Number;
                user.User_Image= User_Image;
                db.AspNetUsers.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                return View();
            }
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Users/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {
            try
            {
                var user = db.AspNetUsers.Where(p => p.Id == id).FirstOrDefault();
                db.AspNetUsers.Remove(user);
                db.SaveChanges();
            }
            catch
            {
                TempData["ErrorinUser"] = "Cannot Delete the User because he has Orders";
            }
            return RedirectToAction("Index");
        }

        // POST: Users/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
