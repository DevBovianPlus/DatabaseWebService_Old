using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsNOZ.OptimalStockOrder
{
    public class OptimalStockOrderModel
    {

        public int NarociloOptimalnihZalogID { get; set; }
        public int StrankaID { get; set; }
        public int StatusID { get; set; }
        public string Naziv { get; set; }
        public string NarociloOptimalnihZalogStevilka { get; set; }
        public string NarociloID_P { get; set; }
        public DateTime DatumOddaje { get; set; }
        public decimal Kolicina { get; set; }
        public string Opombe { get; set; }
        public DateTime ts { get; set; }
        public int tsIDOsebe { get; set; }
        public DateTime tsUpdate { get; set; }
        public int tsUpdateUserID { get; set; }
        public string PotDokumenta { get; set; }
        public int NarociloOddal { get; set; }

        public EmployeeSimpleModel Zaposlen { get; set; }
        public ClientSimpleModel Stranka { get; set; }
        public IEnumerable<OptimalStockOrderPositionModel> NarociloOptimalnihZalogPozicija { get; set; }
        public virtual OptimalStockOrderStatusModel StatusNarocilaOptimalnihZalog { get; set; }
    }
}