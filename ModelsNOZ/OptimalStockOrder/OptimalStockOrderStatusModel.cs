using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsNOZ.OptimalStockOrder
{
    public class OptimalStockOrderStatusModel
    {
        public int StatusNarocilaOptimalnihZalogID { get; set; }
        public string Koda { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public int tsIDOseba { get; set; }
        public DateTime ts { get; set; }
        public DateTime tsUpdate { get; set; }
        public int tsUpdateUserID { get; set; }
    }
}