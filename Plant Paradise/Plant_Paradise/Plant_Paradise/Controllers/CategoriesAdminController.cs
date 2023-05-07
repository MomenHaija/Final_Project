using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Plant_Paradise.Models;

namespace Plant_Paradise.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesAdminController : Controller
    {
        private PlantsEntities db = new PlantsEntities();

        // GET: CategoriesAdmin
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }
        [HttpPost]
        public ActionResult search(string search)
        {
            try
            {
                var Categories = db.Categories.Where(p => p.Category_Name.Contains(search)).ToList();
                return View("Index", Categories.ToList());
            }
            catch
            {
                TempData["Errorincategory"] = "Error happens when you search on specific Category";
                return RedirectToAction("Index");
            }
        }

        public ActionResult Statistics()
        {

            var Countuser = db.AspNetUsers.Count();
            ViewBag.Countuser = Countuser;
            
            var countProduct=db.Products.Count();
            ViewBag.Countproduct = countProduct;

            var countFeedback=db.Feedbacks.Count();
            ViewBag.Countfeedback = countFeedback;

            var TotalRevenue = db.Orders.Sum(p => p.Total_price);
            ViewBag.TotalRevenue = TotalRevenue;

            var totalOrders = db.Orders.Count();
            ViewBag.Totalorders = totalOrders;

            var dayOrders=db.Orders.Where(p=>p.Order_date==DateTime.Today).Count();
            ViewBag.dayorders = dayOrders;

            var dayRevenues= db.Orders.Where(p => p.Order_date == DateTime.Today).Sum(p=>p.Total_price);
            ViewBag.Dayrevenues = dayRevenues;

            var countComments=db.Comments.Count();
            ViewBag.Totalcomments = countComments;

            var orders = db.Orders.ToList();
            return View(orders);  
        }
        // GET: CategoriesAdmin/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return HttpNotFound();
                }
                return View(category);
            }
            catch
            {
                 TempData["Errorincategory"] = "Error happens when you show Detailes specific Category";
                 return RedirectToAction("Index");
            }
        }

        // GET: CategoriesAdmin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoriesAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Category_id,Category_Name,Category_Image")] Category category, HttpPostedFileBase Category_Image)
        {
            try
            {
                string fileName = fileName = Path.GetFileName(Category_Image.FileName);

                if (ModelState.IsValid)
                {
                    try
                    {
                        string path = "/Images/" + Path.GetFileName(Category_Image.FileName);
                        string path2 = Path.GetFileName(Category_Image.FileName);
                        category.Category_Image = path2.ToString();
                        Category_Image.SaveAs(Server.MapPath(path));
                    }
                    catch {
                        ViewBag.Error = "Error happens when you upload the photo";
                    }

                    db.Categories.Add(category);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(category);
            }
            catch
            {
                TempData["Errorincategory"] = "Error happens when Add the Category";
                return RedirectToAction("Index");
            }
        }

        // GET: CategoriesAdmin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: CategoriesAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id,[Bind(Include = "Category_id,Category_Name,Category_Image")] Category category, HttpPostedFileBase Category_Image)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingModel = db.Categories.AsNoTracking().FirstOrDefault(x => x.Category_id == id);

                    if (category != null)
                    {
                        try
                        {
                            string path = "/Images/" + Path.GetFileName(Category_Image.FileName);
                            string path2 = Path.GetFileName(Category_Image.FileName);
                            Category_Image.SaveAs(Server.MapPath(path));
                            category.Category_Image = path2;
                        }
                        catch
                        {
                            ViewBag.Error = "Error happens when you upload the photo";
                            category.Category_Image = existingModel.Category_Image;
                            
                        }
                    }
                    else
                    {
                        category.Category_Image = existingModel.Category_Image;
                    }

                    db.Entry(category).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(category);
            }
            catch
            {
                TempData["Errorincategory"] = "Error happens when Edit on the Category";
                return RedirectToAction("Index");
            }
        }

        // GET: CategoriesAdmin/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Category category = db.Categories.Find(id);
                if (category == null)
                {
                    return HttpNotFound();
                }
                return View(category);
            }
            catch
            {
                TempData["Errorincategory"] = "Error happens when Delete The Category";
                return RedirectToAction("Index");
            }
        }

        // POST: CategoriesAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Category category = db.Categories.Find(id);

                if (db.Sub_Categories.Where(s => s.Category.Category_id == category.Category_id).Count() == 0)
                {
                    db.Categories.Remove(category);
                    db.SaveChanges();
                    return RedirectToAction("Index");

                }
                else
                {
                    TempData["ErrorWhenDeleteCategory"] = "Can't Delete The Category because Contain One Or more Subcategory";
                    return RedirectToAction("Delete");
                }
            }
            catch
            {
                TempData["Errorincategory"] = "Error happens when Delete The Category";
                return RedirectToAction("Index");
            }
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
