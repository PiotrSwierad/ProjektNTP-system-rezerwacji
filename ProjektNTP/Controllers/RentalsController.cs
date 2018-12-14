using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjektNTP.Models;


namespace ProjektNTP.Controllers
{
    public class RentalsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private bool returnDenied = false;

        // GET: Rentals
        [Authorize]
        public ActionResult Index()
        {
            if (returnDenied == true)
            {
                Response.Write("<script language=javascript>alert('Nie możesz usuwać nie swoich rezerwacji')</script>");
                returnDenied = false;
            }
            var rentals = db.Rentals.Include(r => r.Car).Include(r => r.ApplicationUser);
            return View(rentals.ToList());
        }

        // GET: Rentals/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return HttpNotFound();
            }
            return View(rental);
        }

        // GET: Rentals/Create
        [Authorize]
        public ActionResult Create()
        {
            Rental rental = new Rental();
            rental.BorrowDate = DateTime.Now;
            rental.ReturnDate = DateTime.Now.AddDays(1);

            ViewBag.CarID = new SelectList(db.Cars, "ID", "FullBrand");

            
            return View(rental);
        }
       
        // POST: Rentals/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CarId,UserId,BorrowDate,ReturnDate")] Rental rental)
        {
            string username = User.Identity.Name;
            ApplicationUser user = db.Users.FirstOrDefault(u => u.UserName.Equals(username));

            rental.UserId = user.Id;
            rental.ApplicationUser = user;
            
            if (ModelState.IsValid && IsCarAviable(rental) && rental.BorrowDate >= DateTime.Now)
            {
                db.Rentals.Add(rental);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CarId = new SelectList(db.Cars, "Id", "FullBrand", rental.CarId);
            return View(rental);

        }

        // GET: Rentals/Edit/5
       /* [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return HttpNotFound();
            }
            ViewBag.CarID = new SelectList(db.Cars, "ID", "Marka", rental.CarId);
            return View(rental);
        }

        // POST: Rentals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CarID,UserID,Datawypozyczenia,Datazwrotu")] Rental rental)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rental).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            ViewBag.CarID = new SelectList(db.Cars, "ID", "Marka", rental.CarId);
            return View(rental);
        }

        // GET: Rentals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return HttpNotFound();
            }
            return View(rental);
        }

        // POST: Rentals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rental rental = db.Rentals.Find(id);
            db.Rentals.Remove(rental);
            db.SaveChanges();
            return RedirectToAction("Index");
        }*/

        // GET: Rentals/Returns/5
        public ActionResult Returns(int? id, int? numberOfDistance)
        {
            string username = User.Identity.Name;
            ApplicationUser user = db.Users.FirstOrDefault(u => u.UserName.Equals(username));

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rental rental = db.Rentals.Find(id);
            if (rental == null)
            {
                return HttpNotFound();
            }
            if (rental.UserId != user.Id)
            {
                returnDenied = true;
                return RedirectToAction("Index");           
            }

            return View(rental);
        }

        [HttpPost, ActionName("Returns")]
        [ValidateAntiForgeryToken]
        public ActionResult ReturnConfirmed(int id,int? numberOfDistance)
        {
            Rental rental = db.Rentals.Find(id);
            Car car = db.Cars.Find(rental.CarId);
            if (numberOfDistance.HasValue && numberOfDistance.Value >= car.Mileage)
            {
                car.Mileage = numberOfDistance.Value;
                db.Rentals.Remove(rental);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(rental);
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

        bool IsCarAviable(Rental rental)
        {
            var overlappingRentals = from r in db.Rentals where r.CarId == rental.CarId && r.ReturnDate > rental.BorrowDate select r;

            if (overlappingRentals.Any())
            {
                return false; //są rezerwacje
            }
            else
            {
                return true; //nie ma rezerwacji
            }
         
        }

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        public DateGreaterThanAttribute(string dateToCompareToFieldName)
        {
            DateToCompareToFieldName = dateToCompareToFieldName;
        }

        private string DateToCompareToFieldName { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime dataWypozyczenia = (DateTime)value;
            
            DateTime dataZwrotu = (DateTime)validationContext.ObjectType.GetProperty(DateToCompareToFieldName).GetValue(validationContext.ObjectInstance, null);

            if (dataZwrotu < dataWypozyczenia)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Zła data");
            }
        }

    }

}