using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using System.Xml.Linq;
using Microsoft.AspNet.Identity;
using Plant_Paradise.Models;
using static System.Data.Entity.Infrastructure.Design.Executor;
using System.Reflection.Emit;
using System.EnterpriseServices;
using Newtonsoft.Json.Serialization;

namespace Plant_Paradise.Controllers
{
    [HandleError]
    public class ProductsController : Controller
    {
        private PlantsEntities db = new PlantsEntities();
        // GET: Products
        public ActionResult Index(int id)
        {
            try
            {
               
                var products = db.Products.Include(p => p.Sub_Categories).Where(p => p.Subcategory_id == id).OrderByDescending(p => p.Product_id);
                ViewBag.PageName = products.FirstOrDefault().Sub_Categories.SubCategoryName;
                ViewBag.id = id;
                return View(products.ToList());

            }
            catch {
                return RedirectToAction("Error", "Home", new { message = "Not Found the specific Products" });

            }
        }
        [Authorize]
        public ActionResult DeleteComment(int productid,int commentid)
        {
            try
            {
                var comment = db.Comments.Where(c => c.Comment_id == commentid).FirstOrDefault();
                if (comment != null)
                {
                    db.Comments.Remove(comment);
                    db.SaveChanges();
                }
                return RedirectToAction("SingleProduct", "Products", new { id = productid });
            }
            catch
            {
                return RedirectToAction("Error", "Home", new { message = "you can't Delete The Comment" });

            }

        }
        public PartialViewResult Comments(int id)
        {

            try
            {
                var comm = db.Comments.Where(p => p.Product_id == id).Include(p=>p.AspNetUser).ToList();
                ViewBag.userid = User.Identity.GetUserId();
                if (comm != null)
                {
                    return PartialView("Comments", comm);
                }
                return PartialView("Comments", null);
            }
            catch {
                return null;

           }
        }
        [Authorize]
        public ActionResult addcomment(int product_id, string Comment_text)
        {
            try
            {
                Comment comm = new Comment();
                if (ModelState.IsValid && comm != null)
                {
                    comm.Product_id = product_id;
                    comm.Comment_text = Comment_text;
                    comm.userId = User.Identity.GetUserId();
                    db.Comments.Add(comm);
                    db.SaveChanges();
                }
                return RedirectToAction("SingleProduct", "Products", new { id = product_id });
            }
            catch
            {
                return RedirectToAction("Error", "Home", new { message = "you are Unauthorized To Add Comment" });

            }
        }
        public ActionResult SingleProduct(int? id)
        {
            try
            {
                Session["Subcat"] = id;              
                var product = db.Products.Include(p => p.Sub_Categories).Where(p => p.Product_id == id);
                ViewBag.Productid = id;
                return View(product.FirstOrDefault());
               
            }
            catch
            {
                return RedirectToAction("Error", "Home", new { message = "Not Found the specific Product" });

            }

        }
        public ActionResult Index2()
        {
            try
            {
                var products = db.Products.Include(p => p.Sub_Categories);
                   
                return View(products.ToList());
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult gitProducts(int id)
        {
            try
            {
                    var products = db.Products.Include(p => p.Sub_Categories).Where(p => p.Subcategory_id == id);
                    return View("Index2", products.ToList());
            }
            catch
            {
                return RedirectToAction("Index2", "Products");
            }
        }
        public ActionResult search(String Search)
        {
            try
            {
                    var products = db.Products.Include(p => p.Sub_Categories).Where(p => p.Product_Name.Contains(Search));
                    return View("Index2", products.ToList());

            }
            catch
            {
                return RedirectToAction("Error", "Home", new { message = "Not Found the specific Product" });
            }

        }
       
        public ActionResult Add_ToCart(int id,string ReturnUrl)
        {
            try
            {
                HttpCookie cart = new HttpCookie("cart");
                // read the existing cart items from the cookie
                List<string> items = new List<string>();
                if (Request.Cookies["cart"] != null)
                {
                    items = new List<string>(Request.Cookies["cart"].Value.Split('|'));

                }

                // check if the item is already in the cart
                bool itemFound = false;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].StartsWith(id.ToString() + "_"))
                    {
                        // update the existing item's quantity
                        int quantity = Convert.ToInt32(items[i].Split('_')[1]) + 1;
                        items[i] = id.ToString() + "_" + quantity.ToString();
                        itemFound = true;
                        break;
                    }
                }
                if (!itemFound)
                {
                  
                        // add the new item to the cart with a quantity of 1
                        items.Add(id.ToString() + "_1");
                    
                }
                // update the cart cookie with the new items
                cart.Value = string.Join("|", items);
                cart.Expires = DateTime.Now.AddDays(7);
                Response.Cookies.Add(cart);
                // redirect back to the shopping cart view
                return RedirectToAction(ReturnUrl);
            }
            catch
            {
                return RedirectToAction("Error", "Home", new { message = "Error happened when add To cart"});
            }
        }

        public ActionResult RemoveItem(int id)
        {

            try
            {
                HttpCookie cart = new HttpCookie("cart");

                // read the existing cart items from the cookie
                List<string> items = new List<string>();
                if (Request.Cookies["cart"] != null)
                {
                    items = new List<string>(Request.Cookies["cart"].Value.Split('|'));
                }
                try
                {
                    // remove the item from the cart
                    items.RemoveAll(x => x.Split('_')[0] == id.ToString());
                }
                catch
                {
                    items = null;
                }
                // update the cart cookie with the new items or delete it if no items left
                if (items.Count > 0)
                {
                    cart.Value = string.Join("|", items);
                    cart.Expires = DateTime.Now.AddDays(7);
                    Response.Cookies.Add(cart);
                }
                else
                {
                    cart.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(cart);
                }


                return RedirectToAction("ViewCart", "Products");
            }
            catch
            {
                return RedirectToAction("Error", "Home", new { message = "some Error happened when Remove The Item" });
            }
        }
        [HttpPost]
        public ActionResult UpdateCart(FormCollection form)
        {
            var userid= User.Identity.GetUserId();  
            var Usercart = db.carts.Where(user => user.userId == userid);
            int Numberitem = 0;
            foreach (var item in Usercart)
            {
                Numberitem++;
            }
            Session["NumberofItem"] = Numberitem;
            foreach (var key in form.AllKeys)
            {
                if (key.StartsWith("quantity-"))
                {
                    int cartItemId = int.Parse(key.Replace("quantity-", ""));
                    int quantity = int.Parse(form[key]);
                    var cartItem = db.carts.Find(cartItemId);
                    if (cartItem != null)
                    {
                        cartItem.Quantity = quantity;
                    }
                }
            }

            db.SaveChanges();
           
            return RedirectToAction("ViewCart");
        }
        public ActionResult ViewCart()
        {
            try
            {
                HttpCookie cart = Request.Cookies["cart"];
                List<Product> allProducts = new List<Product>();
                dynamic UserProducts = new Product();

                if (cart != null)
                {
                    // read the cart items from the cookie
                    List<string> items = new List<string>(cart.Value.Split('|'));

                    foreach (var item in items)
                    {
                        int id = Convert.ToInt32(item.Split('_')[0]);
                        int quantity = Convert.ToInt32(item.Split('_')[1]);
                        List<Product> itemProducts = db.Products.Where(x => x.Product_id == id).ToList();

                        // set the quantity for each product
                        allProducts.AddRange(itemProducts);
                    }

                    UserProducts = allProducts;
                }
                else
                {
                    IEnumerable<Product> model = new List<Product>();
                    // render the empty cart view
                    return View(model);
                }
                return View(UserProducts);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }




        public ActionResult AddToCart_withQuntity(int id,int q)
        {
            try
            {
                HttpCookie cart = new HttpCookie("cart");
                // read the existing cart items from the cookie
                List<string> items = new List<string>();
                if (Request.Cookies["cart"] != null)
                {
                    items = new List<string>(Request.Cookies["cart"].Value.Split('|'));

                }

                // check if the item is already in the cart
                bool itemFound = false;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].StartsWith(id.ToString() + "_"))
                    {
                        // update the existing item's quantity
                        int quantity = Convert.ToInt32(items[i].Split('_')[1]) + 1;
                        items[i] = id.ToString() + "_" + quantity.ToString();
                        itemFound = true;
                        break;
                    }
                }
                if (!itemFound)
                {
                        items.Add(id.ToString() + "_1");
                 
                }
                // update the cart cookie with the new items
                cart.Value = string.Join("|", items);
                cart.Expires = DateTime.Now.AddDays(7);
                Response.Cookies.Add(cart);
                // redirect back to the shopping cart view
                return RedirectToAction("SingleProduct", "Products", new {id=id});
            }
            catch
            {
                return RedirectToAction("Error", "Home", new { message = "Error happened when add To cart" });
            }
        }











        public ActionResult Home()
        {
            return RedirectToAction("Index", "Home");
        }
        // GET: Products/Details/5
        public ActionResult Details(int? id)
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

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.Category_id = new SelectList(db.Categories, "Category_id", "Category_Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Product_id,Product_Name,Product_Image,Product_Price,Product_Size,Product_Color,Quantity,Product_Description,Category_id")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Category_id = new SelectList(db.Categories, "Category_id", "Category_Name", product.Subcategory_id);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.Category_id = new SelectList(db.Categories, "Category_id", "Category_Name", product.Subcategory_id);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Product_id,Product_Name,Product_Image,Product_Price,Product_Size,Product_Color,Quantity,Product_Description,Category_id")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Category_id = new SelectList(db.Categories, "Category_id", "Category_Name", product.Subcategory_id);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
