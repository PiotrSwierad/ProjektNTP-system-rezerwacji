using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjektNTP.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public int Mileage { get; set; }
        public string Plates { get; set; }

        [NotMapped]
        public string FullBrand { get { return this.CarBrand + ' ' + this.CarModel; } }

        public virtual ICollection<Rental> Rentals { get; set; }
    }
}