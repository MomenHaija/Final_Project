using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using Plant_Paradise.Models;
using System.Net.Mail;
using static System.Collections.Specialized.BitVector32;
namespace Plant_Paradise.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private PlantsEntities db = new PlantsEntities();
        // GET: Transactions
        public ActionResult Index()
        {
            try
            {
                string userid = User.Identity.GetUserId();
                AspNetUser user = db.AspNetUsers.Find(userid);
                HttpCookie cart = Request.Cookies["cart"];
                List<Product> allProducts = new List<Product>();
                if (cart != null)
                {
                    // read the cart items from the cookie
                    List<string> items = new List<string>(cart.Value.Split('|'));

                    double totalprice = 0;

                    foreach (var item in items)
                    {
                        int id = Convert.ToInt32(item.Split('_')[0]);
                        int quantity = Convert.ToInt32(item.Split('_')[1]);
                        Product product = db.Products.Where(x => x.Product_id == id).FirstOrDefault();
                        if (quantity > 0)
                        {
                            totalprice = Convert.ToDouble(totalprice + (product.Product_Price * quantity));
                        }
                        else { return RedirectToAction("Error"); }

                    }
                    ViewBag.TotalPrice = totalprice;
                    ViewBag.TotalPriceinUSD = Math.Round(totalprice / 0.71);
                }
                return View(user);
            }
            catch
            {
                return RedirectToAction("ViewCart", "Products");

            }
        }
        [HttpPost]
        public ActionResult MakeOrder(string phone,string Address,string Email)
        {
            try
            {
                if (Request.Cookies["cart"] != null)
                {
                    double totalprice = 0;
                    var userId = User.Identity.GetUserId();
                    Order order = new Order
                    {
                        Order_id = Convert.ToString(Guid.NewGuid()),
                        Order_date = DateTime.Now,
                        userId = userId,

                    };
                    db.Orders.Add(order);
                    db.SaveChanges();
                    HttpCookie cart = Request.Cookies["cart"];
                    List<Product> allProducts = new List<Product>();
                    if (cart != null)
                    {
                        // read the cart items from the cookie
                        List<string> items = new List<string>(cart.Value.Split('|'));
                        foreach (var item in items)
                        {
                            int id = Convert.ToInt32(item.Split('_')[0]);
                            int quantity = Convert.ToInt32(item.Split('_')[1]);
                            Product product = db.Products.Where(x => x.Product_id == id).FirstOrDefault();
                            Order_Details orderDetail = new Order_Details
                            {

                                Order_id = order.Order_id,
                                Product_id = id,
                                Quantity = quantity,


                            };
                            db.Order_Details.Add(orderDetail);
                            if (quantity > 0)
                            {
                                totalprice = Convert.ToDouble(totalprice + (product.Product_Price * quantity));
                            }
                            else { return RedirectToAction("Error"); }
                            order.Total_price = totalprice;
                            db.SaveChanges();
                        }
                    }
                    Transaction transc = new Transaction();
                    transc.TransactionDate = DateTime.Now;
                    transc.Order_id = order.Order_id;
                    transc.userId = userId;
                    transc.Phone_Contact = phone;
                    transc.User_Location = Address;
                    transc.Amount = totalprice;
                    db.Transactions.Add(transc);
                    db.SaveChanges();

                    cart.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(cart);
                    try
                    {
                        var email = db.AspNetUsers.Where(p => p.Id == userId).FirstOrDefault();
                        var e = email.Email.ToString();
                        MailMessage mail = new MailMessage();
                        mail.To.Add(e);
                        mail.From = new MailAddress("haijamomen4@gmail.com");
                        mail.Subject = "Plant Paradise-Thank You!";
                        mail.Body = $"<p>Dear {email.Full_Name}<br/>We wanted to let you know that we have received your payment for your recent order. Thank you for your purchase!<br/>We appreciate your business and thank you for choosing to shop with us. If you have any questions or concerns, please don't hesitate to contact us.<br/><br/>Best regards<br>Plant Paradise</p>";
                        mail.IsBodyHtml = true;

                        SmtpClient smtp = new SmtpClient();
                        smtp.Port = 587; // 25 465
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Host = "smtp.gmail.com";
                        smtp.Credentials = new System.Net.NetworkCredential("haijamomen4@gmail.com", "gucfmedgpmzruyqb");
                        smtp.Send(mail);
                        
                    }
                    catch
                    {
                        TempData["ErrorinEmail"] = "Incorrect Email";
                        Console.WriteLine("Incorrect Email");

                    }

                    TempData["CorrectProcess"] = "Thank you, payment has been made";

                }
                else
                {
                    TempData["OrderEmpty"] = "The Cart is Empty!!!..So can't Make Order";
                }
                return RedirectToAction("Index", "Transactions");

            }
            catch
            {
                return RedirectToAction("Error", "Home", new { message = "Error happened when Make the Order" });

            }
        }

























        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.Order_id = new SelectList(db.Orders, "Order_id", "userId");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TransactionId,Amount,TransactionDate,Order_id,userId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", transaction.userId);
            ViewBag.Order_id = new SelectList(db.Orders, "Order_id", "userId", transaction.Order_id);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", transaction.userId);
            ViewBag.Order_id = new SelectList(db.Orders, "Order_id", "userId", transaction.Order_id);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TransactionId,Amount,TransactionDate,Order_id,userId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", transaction.userId);
            ViewBag.Order_id = new SelectList(db.Orders, "Order_id", "userId", transaction.Order_id);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
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
