using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Order
{
    public class OrderModel
    {
        public int NarociloID { get; set; }
        public int NarociloStevilka { get; set; }
        public decimal Cena { get; set; }
        public string Relacija { get; set; }
        public Nullable<decimal> KolicinaSkupaj { get; set; }
        public int tsIDOseba { get; set; }
        public DateTime ts { get; set; }
        
        public List<OrderPositionModel> NarociloPozicija { get; set; }

     
    }
}