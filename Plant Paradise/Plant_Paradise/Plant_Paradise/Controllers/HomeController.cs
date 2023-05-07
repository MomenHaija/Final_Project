using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Plant_Paradise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Plant_Paradise.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        PlantsEntities db=new PlantsEntities();
        public ActionResult Index()
        {
            

            var plants =db.Categories;
            if (User.Identity.GetUserId() != null)
            {
                Session["id"] = User.Identity.GetUserId();
            }
            return View(plants.ToList());

        }
         public PartialViewResult feedbacks()
          {
            try
            {

                var feedbacks = db.Feedbacks.Where(p => p.IsAccepted == 1);
                return PartialView("feedbacks", feedbacks.ToList());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
                
            }
          

         }
        public PartialViewResult bestSalles() {

            var products = (from p in db.Products
                            join o in db.Order_Details on p.Product_id equals o.Product_id
                            group p by new { p.Product_id, p.Product_Name, p.Product_Price } into g
                            orderby g.Count() descending
                            select new
                            {
                                Product_id = g.Key.Product_id,
                                Product_Name = g.Key.Product_Name,
                                Product_Price = g.Key.Product_Price,

                                Count = g.Count()
                            }).Take(3).ToList();

            return PartialView("bestSalles", products);
        }

        public ActionResult SubCategory(int? id)
        {
            try
            {
                if (id != null)
                {
                    var TypeCategory = db.Sub_Categories.Where(p => p.Category_id == id);
                    ViewBag.BageName = TypeCategory.FirstOrDefault().Category.Category_Name;
                    return View(TypeCategory.ToList());
                }
                return RedirectToAction("Index");

               
            }
            catch
            {
                return RedirectToAction("Error", "Home", new { message = "Not Found The Specific Sub Category" });


            }
        }
        public ActionResult About()
        {

            return View();
        }
       
        
        public ActionResult Contact()
        {
            if (User.Identity.GetUserId() != null)
            {
                ViewBag.userid = User.Identity.GetUserId();
                return View();
            }
            return View();
            
        }

        public ActionResult Error(string message) {
            ViewBag.message = message;
            return View();
        }
    }
}