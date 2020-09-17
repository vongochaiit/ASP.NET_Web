using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BlueShop.Models;
namespace BlueShop.Controllers
{
    public class HomeController : Controller
    {
        private ShopOnlineEntities db = new ShopOnlineEntities();
      

            public ActionResult Index(String SearchName)
        {
            //if (Session["Role"]==null&& Session["UserID"]!=null) {
            //    return View();

            //  }
            var product = from p in db.Products
                          where p.Shop.IsDeleted == null && p.IsDeleted == null
                          select p;
            if (!string.IsNullOrEmpty(SearchName))
            {
                product = product.Where(e => e.Name.Contains(SearchName));

            }
            if (TempData["cart"] != null)
            {
                float x = 0;
                List<cart> li2 = TempData["cart"] as List<cart>;
                foreach (var item in li2)
                {
                    x += item.bill;

                } 
                TempData["total"] = x;
            }
            TempData.Keep();
            //return View(db.Products.(x => x.Shop.IsDeleted == false).ToList());
            return View(product.ToList());
        }
        public ActionResult IndexCategoryProduct(Guid id) {
            var product = from p in db.Products
                          where p.Shop.IsDeleted == false && p.IsDeleted == false && p.CategoryProductID == id
                          select p;
            return View();
        }
        public ActionResult IndexOredr()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                Guid a = new Guid(Convert.ToString(Session["UserID"].ToString()));
                if (Session["UserId"] == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    var order = from p in db.OrderDetails
                                where p.Order.UserID == a && p.Order.Shop.IsDeleted == false
                                select p;
                    return View(order.ToList());
                }
            }
        }
        // 
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Logoff()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Login()
        {
            if (Session["UserId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult Login(User user)
        {
            using (ShopOnlineEntities db = new ShopOnlineEntities())
            {

                var usr = db.Users.SingleOrDefault(u => u.UserName == user.UserName && u.PassWord == user.PassWord && u.Role == null);
                var usr1 = db.Users.SingleOrDefault(u => u.UserName == user.UserName && u.PassWord == user.PassWord && u.Role == 1 && u.IsDeleted == false);
                if (usr != null)
                {
                    Session["UserID"] = usr.UserID.ToString();
                    Session["Role"] = usr.Role.ToString();
                    Session["UserName"] = usr.UserName.ToString();
                    return RedirectToAction("Index", "Home");
                }
               
                if (usr1 != null )
                {
                    Session["UserID"] = usr1.UserID.ToString();
                    Session["Role"] = usr1.Role.ToString();
                    Session["UserName"] = usr1.UserName.ToString();
                    return RedirectToAction("Index", "Home");
                }
               

            }
            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Details(Guid? id)
        {
              //String u = Convert.ToString(Session["UserID"].ToString())
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Where(x => x.ProductID == id).SingleOrDefault();
            //Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
          
            return View(product);
        }
        List<cart> li = new List<cart>();
        [HttpPost]
        public ActionResult Details(Product product, string qty, Guid id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login","Home");
            }
            else
            {
                Product p = db.Products.Where(x => x.ProductID == id).SingleOrDefault();

                //String u = Convert.ToString(Session["UserID"].ToString());
                cart c = new cart();
                c.ProductID = p.ProductID;
                c.ShopID = p.ShopID;
                c.ShopName = p.Shop.NameShop;
                c.Image = p.Image;
                c.Price = (int)p.Price;
                c.qty = Convert.ToInt32(qty);
                c.bill = c.Price * c.qty;
                c.Name = p.Name;
                
                if (TempData["cart"] == null)
                {
                    li.Add(c);
                    TempData["cart"] = li;

                }
                else
                {
                    List<cart> li2 = TempData["cart"] as List<cart>;
                    int flag = 0;
                    foreach (var item in li2)
                    {
                        if (item.ProductID == c.ProductID)
                        {
                            item.qty += c.qty;
                            item.bill += c.bill;
                            flag = 1;
                        }
                    }
                    if (flag == 0)
                    {
                        li2.Add(c);
                        //item is new...
                    }
                    TempData["Product"] = li2;

                }
                TempData.Keep();

                return RedirectToAction("Index");

            }
           
        }
        public ActionResult checkout()
        {
            TempData.Keep();
            Order order = new Order();
            ViewBag.ShopID = new SelectList(db.Shops, "ShopID", "NameShop", order.ShopID);
            return View();
        }
     
        public ActionResult checkout1( Order order,Guid shopid)
        {

            String user = Convert.ToString(Session["UserID"].ToString());
            

            List<cart> li = TempData["cart"] as List<cart>;
            //String Shop = Convert.ToString(Session["ShopID"].ToString());
            //Order order = new Order();
            order.OrderID = Guid.NewGuid();
            order.UserID = new Guid(user);
            order.ShopID = shopid; 
            
            order.OrderDate = System.DateTime.Now;
            //iv.in_totalbill = (float)TempData["total"];
            ViewBag.ShopID = new SelectList(db.Shops, "ShopID", "NameShop", order.ShopID);
            db.Orders.Add(order);
            db.SaveChanges();
            OrderDetail od = new OrderDetail();
            foreach (var item in li)
            {
                od.OrderDetailsID = Guid.NewGuid();
                od.ProductID = item.ProductID;
                od.OrderID = order.OrderID;
                od.Status = "Đặt mua";
                od.OrderCode = DateTime.Now.ToString("yy-MM-dd-mm")+"-ASP";
                //od.o_date = System.DateTime.Now;
                od.QuantityOrdered = item.qty;
                od.Price = (int)item.Price * item.qty;
                //od.Price = item.bill;
                db.OrderDetails.Add(od);
                db.SaveChanges();
                //UpdateAuditFields(item.ProductID, item.qty);

                var productId = db.Products.SingleOrDefault(m => m.ProductID == od.ProductID);
                if (productId != null)
                {
                    productId.Count = productId.Count - item.qty;
                    db.Update(productId);
                    db.SaveChanges();
                }

            }

            TempData.Remove("total");
            TempData.Remove("cart");

            TempData["msg"] = "Transaction Completed.........";
            TempData.Keep();
            return RedirectToAction("Index");
        }
        private void UpdateAuditFields(Guid? id , int a )
        {
           
            Product product = db.Products.Find(id);
            db.Entry(product).State = EntityState.Modified;
            product.Count = a;
            db.SaveChanges();
        }
        public ActionResult remove(Guid? id)
        {
            List<cart> li2 = TempData["cart"] as List<cart>;
            cart c = li2.Where(x => x.ProductID == id).SingleOrDefault();
            li2.Remove(c);
            float h = 0;
            foreach (var item in li2)
            {
                h += item.bill;
            }
            TempData["total"] = h;
            return RedirectToAction("checkout");

        }
        public ActionResult DeleteOrder(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }
        [HttpPost, ActionName("DeleteOrder")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteOrderConfirmed(Guid id)
        {
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail.Status == "Đã duyệt") {
                return RedirectToAction("Error");
               
            }
            else {
                db.OrderDetails.Remove(orderDetail);
                db.SaveChanges();
                return RedirectToAction("IndexOredr");
            }
            
        }
        public ActionResult Error()
        {
            return View();
        }
        public ActionResult EditOrder(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error");
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "Name", orderDetail.ProductID);
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderID", orderDetail.OrderID);
            return View(orderDetail);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrder([Bind(Include = "OrderDetailsID,ProductID,QuantityOrdered,Price,OrderCode,OrderID,Status")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {

                //string fileName = Path.GetFileNameWithoutExtension(product.ImageFile.FileName);

                db.Entry(orderDetail).State = EntityState.Modified;
                if (orderDetail.Status == "Đặt mua") {
                    return RedirectToAction("Error");
                }
                else {
                    orderDetail.Status = "Đã Nhận";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
               
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "Name", orderDetail.ProductID);
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderID", orderDetail.OrderID);
            return View(orderDetail);
        }

    }
}