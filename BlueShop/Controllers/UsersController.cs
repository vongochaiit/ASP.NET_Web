using System;
using System.Collections;
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
    public class UsersController : Controller
    {
        private ShopOnlineEntities db = new ShopOnlineEntities();

        // GET: Users
        public JsonResult IsExist(string UserName, Guid? Id)
        {
            var validateName = db.Users.FirstOrDefault
                                (x => x.UserName == UserName && x.UserID != Id);
            if (validateName != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Index(String SearchName, DateTime? SearchNgaysinh, String SDT, String Cmnd, String taikhoan, Guid? CityId)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else {
                var users = from p in db.Users
                              where p.IsDeleted == false
                              select p;
                ViewBag.CountryId = new SelectList(db.Countries, "CountryID", "NameCountry");
                ViewBag.CityID = new SelectList(db.Cities, "CityID", "NameCity");
                if (!string.IsNullOrEmpty(SearchName))
                {
                    users = users.Where(e => e.FirstName.Contains(SearchName) || e.LastName.Contains(SearchName) || SearchName.Contains(e.FirstName) && SearchName.Contains(e.LastName));

                }
                if (CityId != null)
                {
                    users = users.Where(e => e.CityID== CityId);
                }
                if (!string.IsNullOrEmpty(taikhoan))
                {
                    users = users.Where(e => e.UserName.Contains(taikhoan));

                }
                if (!string.IsNullOrEmpty(SDT))
                {
                    users = users.Where(e => e.Phone.Contains(SDT));

                }
                if (!string.IsNullOrEmpty(Cmnd))
                {
                    users = users.Where(e => e.CMNN.Contains(Cmnd));

                }
                if (SearchNgaysinh != null)
                {
                    users = users.Where(e => e.BirthDay == SearchNgaysinh);

                }
                return View(users.ToList());
            }
           
        }
        ///GET: Users/Details/5
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
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()

        {
            ViewBag.CountryId = new SelectList(db.Countries, "CountryID", "NameCountry");
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "NameCity");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,LastName,FirstName,BirthDay,Phone,Email,CMNN,CityID,UserName,PassWord,Role,IsDeleted")] User user)
        {
            int a = 1;
            if (ModelState.IsValid)
            {
                user.UserID = Guid.NewGuid();
                user.Role = a; //a
                user.IsDeleted = false;
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CountryId = new SelectList(db.Countries, "CountryID", "NameCountry");
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "NameCity", user.CityID);
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "NameCity", user.CityID);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,LastName,FirstName,BirthDay,Phone,Email,CMNN,CityID,UserName,PassWord,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "NameCity", user.CityID);
            return View(user);
        }

        // GET: Users/Delete/5

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return View("Shared", "Error");
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            //if (user.Role == null)
            //{
            //    return View("Error_Delete");
            //}
            return View(user);
        }
        public ActionResult Error_Delete()
        {
            return View();
        }
        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            User user = db.Users.Find(id);
            user.IsDeleted = true;
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
