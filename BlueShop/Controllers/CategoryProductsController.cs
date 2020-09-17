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
    public class CategoryProductsController : Controller
    {
        private ShopOnlineEntities db = new ShopOnlineEntities();

        // GET: CategoryProducts
        public ActionResult Index(String SearchName)
        {
            if (Session["UserID"]!= null && Session["Role"].ToString()=="") {
                var categoryProducts = from p in db.CategoryProducts

                                       select p;
                if (!string.IsNullOrEmpty(SearchName))
                {
                   
                    categoryProducts = categoryProducts.Where(e => e.Desciption.Contains(SearchName));

                }
                return View(categoryProducts.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }

        // GET: CategoryProducts/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoryProduct categoryProduct = db.CategoryProducts.Find(id);
            if (categoryProduct == null)
            {
                return HttpNotFound();
            }
            return View(categoryProduct);
        }

        // GET: CategoryProducts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoryProducts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryProductID,Desciption")] CategoryProduct categoryProduct)
        {
            if (ModelState.IsValid)
            {
                categoryProduct.CategoryProductID = Guid.NewGuid();
                db.CategoryProducts.Add(categoryProduct);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(categoryProduct);
        }

        // GET: CategoryProducts/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoryProduct categoryProduct = db.CategoryProducts.Find(id);
            if (categoryProduct == null)
            {
                return HttpNotFound();
            }
            return View(categoryProduct);
        }

        // POST: CategoryProducts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryProductID,Desciption")] CategoryProduct categoryProduct)
        {
            if (ModelState.IsValid)
            {
                db.Entry(categoryProduct).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(categoryProduct);
        }

        // GET: CategoryProducts/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CategoryProduct categoryProduct = db.CategoryProducts.Find(id);
            if (categoryProduct == null)
            {
                return HttpNotFound();
            }
            return View(categoryProduct);
        }

        // POST: CategoryProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            CategoryProduct categoryProduct = db.CategoryProducts.Find(id);
            db.CategoryProducts.Remove(categoryProduct);
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
