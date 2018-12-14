using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjektNTP.Models;

namespace ProjektNTP.Controllers
{
    public class CarsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Cars
        
        public ActionResult Index(string carBrand, string carModel, DateTime? startDate, DateTime? endDate)
        {
            var BrandList = new List<string>();
            var BrandQry = from b in db.Cars orderby b.CarBrand select b.CarBrand;

            var ModelList = new List<string>();
            var ModelQry = from m in db.Cars where m.CarBrand == carBrand orderby m.CarModel select m.CarModel;

            BrandList.AddRange(BrandQry.Distinct());
            ModelList.AddRange(ModelQry.Distinct());

            ViewBag.carBrand = new SelectList(BrandList);
            ViewBag.carModel = new SelectList(ModelList);

            var cars = from c in db.Cars select c;

            if (!string.IsNullOrEmpty(carBrand))
            {
                cars = cars.Where(x => x.CarBrand == carBrand);
            }

            if (!string.IsNullOrEmpty(carModel) && !string.IsNullOrEmpty(carBrand))
            {
                cars = cars.Where(x => x.CarBrand == carBrand && x.CarModel == carModel);
            }

            if (startDate.HasValue && endDate.HasValue)
            {   
                cars = cars.Where(x => x.Rentals.All(y => y.ReturnDate < startDate && y.BorrowDate < startDate || y.BorrowDate > endDate && y.ReturnDate > endDate));
            }



            return View(cars);
        }

        // GET: Cars/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // GET: Cars/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Id,CarBrand,CarModel,Mileage,Plates")] Car car)
        {
            if (ModelState.IsValid)
            {
                db.Cars.Add(car);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(car);
        }

        // GET: Cars/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // POST: Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,CarBrand,CarModel,Mileage,Plates")] Car car)
        {
            if (ModelState.IsValid)
            {
                db.Entry(car).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(car);
        }

        // GET: Cars/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Car car = db.Cars.Find(id);
            db.Cars.Remove(car);
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
