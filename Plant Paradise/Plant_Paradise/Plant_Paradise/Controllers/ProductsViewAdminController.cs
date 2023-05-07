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
    public class ProductsViewAdminController : Controller
    {
        private PlantsEntities db = new PlantsEntities();

        // GET: Products1
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Sub_Categories);
            return View(products.ToList());
        }
        [HttpPost]
        public ActionResult search(string search, int plants = 0)
        {
            try
            {
                var products = db.Products.Include(p => p.Sub_Categories);

                if (plants != 0)
                {
                    products = db.Products.Include(p => p.Sub_Categories).Where(p => p.Sub_Categories.Category_id == plants && p.Product_Name.Contains(search));
                }
                else
                {
                    products = db.Products.Include(p => p.Sub_Categories).Where(p => p.Product_Name.Contains(search));

                }
                return View("Index", products.ToList());
            }
            catch
            {
                TempData["ErrorinProduct"] = "Error happens when you search on specific Product";
                return View("Index");
            }
        }
        // GET: Products1/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                return View(product);
            }
            catch
            {
                TempData["ErrorinProduct"] = "Error happens when you show Detailes specific Product";
                return View("Index");
            }
        }

        // GET: Products1/Create
        public ActionResult Create()
        {
            ViewBag.Subcategory_id = new SelectList(db.Sub_Categories, "SubCategory_id", "SubCategoryName");
            return View();
        }

        // POST: Products1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Product_id,Product_Name,Product_Image,Product_Size,Product_Color,Product_Price,Quantity,Product_Description,Subcategory_id,Water_needed")] Product product, HttpPostedFileBase  Product_Image)
        {
            try
            {
                string fileName = fileName = Path.GetFileName(Product_Image.FileName);

                if (ModelState.IsValid)
                {
                    try
                    {
                        string path = "/Images/" + Path.GetFileName(Product_Image.FileName);
                        string path2 = Path.GetFileName(Product_Image.FileName);
                        product.Product_Image = path2.ToString();
                        Product_Image.SaveAs(Server.MapPath(path));
                    }
                    catch {
                        ViewBag.Error = "Error happens when you upload the photo";
                        product.Product_Image= product.Product_Image.ToString();
                    }
                    db.Products.Add(product);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.Subcategory_id = new SelectList(db.Sub_Categories, "SubCategory_id", "SubCategory_Description", product.Subcategory_id);
                return View(product);
            }
            catch
            {
                TempData["ErrorinProduct"] = "Error happens when Add the Product";
                return RedirectToAction("Index", "CategoriesAdmin");
            }
        }

        // GET: Products1/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Subcategory_id = new SelectList(db.Sub_Categories, "SubCategory_id", "SubCategoryName", product.Subcategory_id);
                return View(product);
            }
            catch
            {
                TempData["ErrorinProduct"] = "Error happens when Edit on the Product";
                return RedirectToAction("Index");
            }
        }

        // POST: Products1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id,[Bind(Include = "Product_id,Product_Name,Product_Image,Product_Size,Product_Color,Product_Price,Quantity,Product_Description,Subcategory_id,Water_needed")] Product product, HttpPostedFileBase Product_Image)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var existingModel = db.Products.AsNoTracking().FirstOrDefault(x => x.Product_id == id);

                    if (Product_Image != null)
                    {
                        try
                        {
                            string path = "/Images/" + Path.GetFileName(Product_Image.FileName);
                            string path2 = Path.GetFileName(Product_Image.FileName);
                            Product_Image.SaveAs(Server.MapPath(path));
                            product.Product_Image = path2;
                        }
                        catch {
                            ViewBag.Error = "Error happens when you upload the photo";

                        }
                    }
                    else
                    {
                        product.Product_Image = existingModel.Product_Image;
                    }

                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.Subcategory_id = new SelectList(db.Sub_Categories, "SubCategory_id", "SubCategory_Description", product.Subcategory_id);
                return View(product);
            }
            catch
            {
                TempData["ErrorinProduct"] = "Error happens when Edit on the Product";
                return RedirectToAction("Index");
            }
        }

        // GET: Products1/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                return View(product);
            }
            catch
            {
                TempData["ErrorinProduct"] = "Error happens when Delete The Product";
                return RedirectToAction("Index");
            }
        }

        // POST: Products1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {

                Product product = db.Products.Find(id);
                var countOreder = db.Order_Details.Where(o => o.Product_id == product.Product_id).Count();
                if (countOreder == 0)
                {
                    db.Products.Remove(product);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorWhenDelteProduct"] = "Can't Delete The Product because have One Or more Order for this product";
                    return RedirectToAction("Delete");
                }
            }
            catch
            {
                TempData["ErrorinProduct"] = "Error happens when Delete The Product";
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
