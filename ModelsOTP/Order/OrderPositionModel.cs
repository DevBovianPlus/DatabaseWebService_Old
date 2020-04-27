using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Order
{
    public class OrderPositionModel
    {
        public int NarociloPozicijaID { get; set; }
        public int NarociloID { get; set; }
        public decimal Cena { get; set; }
        public decimal Kolicina { get; set; }
        public int tsIDOseba { get; set; }
        public DateTime ts { get; set; }

        public virtual OrderModel Narocilo { get; set; }

    }
}