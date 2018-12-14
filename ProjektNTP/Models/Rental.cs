using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjektNTP.Models
{
    public class Rental
    {
        public int RentalId { get; set; }
        public int CarId { get; set; }
        public string UserId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BorrowDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReturnDate { get; set; }

        public virtual Car Car { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

    }
}