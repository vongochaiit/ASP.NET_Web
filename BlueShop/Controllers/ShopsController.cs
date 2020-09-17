using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BlueShop.Models;

namespace BlueShop.Controllers
{
    public class ShopsController : Controller
    {
        private ShopOnlineEntities db = new ShopOnlineEntities();

        // GET: Shops
        public JsonResult IsExist(string NameShop, Guid? Id)
        {
            var validateName = db.Shops.FirstOrDefault
                                (x => x.NameShop == NameShop && x.ShopID != Id);
            if (validateName != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Index(String SearchName, String SearchDiachi, Guid? CityId)
        {
            ViewBag.CountryId = new SelectList(db.Countries, "CountryID", "NameCountry");
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "NameCity");
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            if (Session["UserID"] != null && Session["Role"].ToString() == "")
            {
                var shops = from p in db.Shops
                            where p.IsDeleted == false 
                            select p;
                if (!string.IsNullOrEmpty(SearchName))
                {
                    shops = shops.Where(e => e.NameShop.Contains(SearchName)); 
                     
                }
                if (!string.IsNullOrEmpty(SearchDiachi))
                {
                    shops = shops.Where(e => e.Address.Contains(SearchDiachi));

                }
                if (CityId != null)
                {
                    shops = from p in db.Shops
                            where p.CityID == CityId
                                select p;
                }
                return View(shops.ToList());
            }
            else {
                Guid a = new Guid(Convert.ToString(Session["UserID"].ToString()));
                var shops = from p in db.Shops
                            where p.UserID == a && p.IsDeleted == false
                            select p;
                //var shops = db.Shops.Include(s => s.UserID == a).Include(s => s.City);
                return View(shops.ToList());
            }
            
        }
        public JsonResult BindCity(Guid CountryId)
        {
            IEnumerable cityList = null;
            try
            {
                ShopOnlineEntities db = new ShopOnlineEntities();

                cityList = from p in db.Cities where p.CountryID == CountryId select p;
            }
            catch (Exception e)
            {
                throw e;
            }
            return Json(new SelectList(cityList, "CityID", "NameCity"), JsonRequestBehavior.AllowGet);

        }
        public ActionResult IndexOredr(Guid id)
        {
            
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                var order = from p in db.OrderDetails
                            where p.Order.ShopID == id
                            select p;
                return View(order.ToList());
            }

        }
        public ActionResult IndexProduct(Guid id)
        {
            /* Guid a = new Guid(Convert.ToString(Session["UserID"].ToString()))*/
            var Products = from p in db.Products
                           where p.ShopID == id && p.IsDeleted==false
                        select p;
            //var shops = db.Shops.Include(s => s.UserID == a).Include(s => s.City);
            return View(Products.ToList());
        }
        // GET: Shops/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shop shop = db.Shops.Find(id);
            if (shop == null)
            {
                return HttpNotFound();
            }
            return View(shop);
        }
       
        // GET: Shops/Create
        public ActionResult Create()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Home");
            } else{
                ViewBag.CountryId = new SelectList(db.Countries, "CountryID", "NameCountry");
                ViewBag.UserID = new SelectList(db.Users, "UserID", "LastName");
                ViewBag.CityID = new SelectList(db.Cities, "CityID", "NameCity");
                return View();
            }
            
        }
        [HttpGet]
        public ActionResult CreateProduct(Guid? id)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                ViewBag.CountryId = new SelectList(db.Countries, "CountryID", "NameCountry");
                ViewBag.CategoryProductID = new SelectList(db.CategoryProducts, "CategoryProductID", "Desciption");
                ViewBag.ShopID = new SelectList(db.Shops, "ShopID", "NameShop");
                ViewBag.CityID = new SelectList(db.Cities, "CityID", "NameCity");
                return View();
            }
        }
        [HttpPost]

        public ActionResult CreateProduct(Product imageModel, Guid id)
        {
            if (ModelState.IsValid)
            {
                Shop shop = db.Shops.Find(id);
                imageModel.ProductID = Guid.NewGuid();
                imageModel.ShopID = id;
                imageModel.IsDeleted = false;
                string fileName = Path.GetFileNameWithoutExtension(imageModel.ImageFile.FileName);
                string extension = Path.GetExtension(imageModel.ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                imageModel.Image = "~/Image/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                imageModel.ImageFile.SaveAs(fileName);
                db.Products.Add(imageModel);
                db.SaveChanges();
            }
            //ModelState.Clear();
            ViewBag.CountryId = new SelectList(db.Countries, "CountryID", "NameCountry");
            ViewBag.CategoryProductID = new SelectList(db.CategoryProducts, "CategoryProductID", "Desciption");
            ViewBag.ShopID = new SelectList(db.Shops, "ShopID", "NameShop");
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "NameCity");
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ShopID,UserID,NameShop,Address,CityID,IsDeleted")] Shop shop)
        {
            String user_id;
            user_id = Convert.ToString(Session["UserID"].ToString());
            if (ModelState.IsValid)
            {
                shop.ShopID = Guid.NewGuid();
                shop.UserID = new Guid(user_id);
                shop.IsDeleted = false;
                db.Shops.Add(shop);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CountryId = new SelectList(db.Countries, "CountryID", "NameCountry");
            ViewBag.UserID = new SelectList(db.Users, "UserID", "LastName", shop.UserID);
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "NameCity", shop.CityID);
            return View(shop);

        }

        // GET: Shops/Edit/5
        public ActionResult EditProduct(Guid? id)
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
            ViewBag.ShopID = new SelectList(db.Shops, "SHopID", "NameShop", product.ShopID);
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "NameCity", product.CityID);
            ViewBag.CategoryProductID = new SelectList(db.CategoryProducts, "CategoryProductID", "Desciption", product.CategoryProductID);
            return View(product);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditProduct(FormCollection image)
        {
            
            ShopOnlineEntities blCarousel = new ShopOnlineEntities();

            Product product = new Product();

            product.ProductID = Guid.Parse(image.Get("ProductID"));
            product.CategoryProductID = Guid.Parse(image.Get("CategoryProductID"));
            product.Name = image.Get("Name").ToString();
            product.Desciption = image.Get("Desciption").ToString();
            product.Price = Convert.ToDecimal(image.Get("Price"));
            product.CityID = Guid.Parse(image.Get("CityID"));
            HttpPostedFileBase filebase = Request.Files["Image"];
            //Guid ShopID = Guid.Parse(image.Get("ShopID"));
            product.Count = Convert.ToInt32(image.Get("Count"));
            Product prot = db.Products.SingleOrDefault(n => n.ProductID == product.ProductID);
            if (ModelState.IsValid)
            {
                prot.ProductID = product.ProductID;
                product.IsDeleted = prot.IsDeleted;
                // so sánh trên view và db theo producID
                if (prot.CategoryProductID != product.CategoryProductID)
                {
                    prot.CategoryProductID = product.CategoryProductID;
                }
                 if (prot.Name != product.Name)
                {
                    prot.Name = product.Name;
                }
                 if (prot.Desciption != product.Desciption)
                {
                    prot.Desciption = product.Desciption;
                }
                 if (prot.Price != product.Price)
                {
                    prot.Price = product.Price;
                }
                 if (prot.CityID != product.CityID)
                {
                    prot.CityID = product.CityID;
                }
                //else if (prot.ShopID != ShopID)
                //{
                //    prot.ShopID = ShopID;
                //}
                 if (prot.Count != product.Count)
                {
                    prot.Count = product.Count;
                }

                //if (string.IsNullOrEmpty(filebase.FileName) != false)
                //{
                //    ViewBag.CityID = new SelectList(db.Cities, "CityID", "NameCity");
                //    ViewBag.CategoryProductID = new SelectList(db.CategoryProducts, "CategoryProductID", "Desciption");
                //    //return View(db.Products.SingleOrDefault(n => n.ProductID == product.ProductID));
                //}
               if (prot.Image != filebase.FileName && string.IsNullOrEmpty(filebase.FileName) == false)
                {
                    int fileSize = filebase.ContentLength;
                    byte[] readbytefile = new byte[fileSize];
                    filebase.InputStream.Read(readbytefile, 0, fileSize);
                    string file = Path.GetFileNameWithoutExtension(filebase.FileName);
                    string extension = Path.GetExtension(filebase.FileName);
                    string fileName = file + DateTime.Now.ToString("yymmssfff") + extension;
                    prot.Image = "~/Image/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                    filebase.SaveAs(fileName);
                }

                db.Update(prot);
                db.SaveChanges();
                ViewBag.CityID = new SelectList(db.Cities, "CityID", "NameCity");
                ViewBag.CategoryProductID = new SelectList(db.CategoryProducts, "CategoryProductID", "Desciption");
            }

            return View(db.Products.SingleOrDefault(n=>n.ProductID == product.ProductID));
           
        }
        // GET: Shops/Edit/5
        public ActionResult EditOrder(Guid? id)
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
                orderDetail.Status = "Đã duyệt";
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "Name", orderDetail.ProductID);
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderID", orderDetail.OrderID);
            return View(orderDetail);
        }
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shop shop = db.Shops.Find(id);
            if (shop == null)
            {
                return HttpNotFound();
            }
            ViewBag.CountryId = new SelectList(db.Countries, "CountryID", "NameCountry");
            ViewBag.UserID = new SelectList(db.Users, "UserID", "LastName", shop.UserID);
            ViewBag.CityId = new SelectList(db.Cities, "CityID", "NameCity", shop.CityID);
            return View(shop);
        }

        // POST: Shops/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ShopID,UserID,NameShop,Address,CityID,IsDeleted")] Shop shop)
        {
            String user_id;
            user_id = Convert.ToString(Session["UserID"].ToString());
            if (ModelState.IsValid)
            {
                shop.UserID = new Guid(user_id);
                shop.IsDeleted = false;

                db.Entry(shop).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CountryId = new SelectList(db.Countries, "CountryID", "NameCountry");
            ViewBag.UserID = new SelectList(db.Users, "UserID", "LastName", shop.UserID);
            ViewBag.CityId = new SelectList(db.Cities, "CityID", "NameCity", shop.CityID);
            return View(shop);
        }

        // GET: Shops/Delete/5
        public ActionResult DeleteProduct(Guid? id)
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
        // POST: Shops/Delete/5
        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProductConfirmed(Guid id)
        {
            Product product = db.Products.Find(id);
            product.IsDeleted = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        public ActionResult Error_Delete()
        {
            return View();
        }
        // POST: Shops/Delete/5

        public ActionResult Delete(Guid? id)
        {
            //var entityExit = (from p in db.Products
            //                  where p.ShopID == id
            //                  select p).FirstOrDefault();
            //if (entityExit != null)
            //{
            //    return RedirectToAction("Error_Delete");
            //}
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shop shop = db.Shops.Find(id);
            if (shop == null)
            {
                return HttpNotFound();
            }
            return View(shop);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public ActionResult DeleteConfirmed(Guid id)
        {
            Shop shop = db.Shops.Find(id);
            shop.IsDeleted = true;
            //db.Shops.Remove(shop);
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
