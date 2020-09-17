using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BlueShop.Models;

namespace BlueShop.Controllers
{
    public class OrderDetailsController : Controller
    {
        private ShopOnlineEntities db = new ShopOnlineEntities();

        // GET: OrderDetails
        public ActionResult Index(String SearchName, Decimal? min, Decimal? max, String code, String trangthai)
        {
            if (Session["UserID"] != null && Session["Role"].ToString() == "")
            {
                var orderDetails = from p in db.OrderDetails

                                       select p;
                if (!string.IsNullOrEmpty(SearchName))
                {

                    orderDetails = orderDetails.Where(e => e.Product.Name.Contains(SearchName));

                }
                if (!string.IsNullOrEmpty(code))
                {

                    orderDetails = orderDetails.Where(e => e.OrderCode.Contains(code));

                }
                if (!string.IsNullOrEmpty(trangthai))
                {

                    orderDetails = orderDetails.Where(e => e.Status.Contains(trangthai));

                }
                if (min != null)
                {
                    orderDetails = orderDetails.Where(p => p.Price >= min);

                }
                if (max != null)
                {
                    orderDetails = orderDetails.Where(p => p.Price <= max);

                }
                return View(orderDetails.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        // GET: OrderDetails/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return View("Error_Delete");
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }

        // GET: OrderDetails/Create
        public ActionResult Create()
        {
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderID");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "Name");
            return View();
        }

        // POST: OrderDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderDetailsID,ProductID,QuantityOrdered,Price,OrderCode,OrderID,Status")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                orderDetail.OrderDetailsID = Guid.NewGuid();
                db.OrderDetails.Add(orderDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderID", orderDetail.OrderID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "Name", orderDetail.ProductID);
            return View(orderDetail);
        }

        // GET: OrderDetails/Edit/5
        public ActionResult Edit(Guid? id)
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
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderID", orderDetail.OrderID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "Name", orderDetail.ProductID);
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderDetailsID,ProductID,QuantityOrdered,Price,OrderCode,OrderID,Status")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderID", orderDetail.OrderID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "Name", orderDetail.ProductID);
            return View(orderDetail);
        }

        // GET: OrderDetails/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return View("Error_Delete");
            }
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            OrderDetail orderDetail = db.OrderDetails.Find(id);
            db.OrderDetails.Remove(orderDetail);
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
        public ActionResult Error_Delete()
        {
            return View();
        }
    }
}
