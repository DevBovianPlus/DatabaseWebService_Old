using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsNOZ.OptimalStockOrder
{
    public class OptimalStockOrderPositionModel
    {
        public int NarociloOptimalnihZalogPozicijaID { get; set; }
        public int NarociloOptimalnihZalogID { get; set; }
        public string KategorijaNaziv { get; set; }
        public string NazivArtikla { get; set; }
        public decimal Kolicina { get; set; }
        public string Opombe { get; set; }
        public DateTime ts { get; set; }
        public int tsIDOsebe { get; set; }
        public DateTime tsUpdate { get; set; }
        public int tsUpdateUserID { get; set; }
        public string IdentArtikla_P { get; set; }

        public decimal VsotaZalNarRazlikaOpt { get; set; }
        //Uporaba samo za barvanje vrstic zapisov v        
        public string NazivPodKategorija { get; set; }
        
        //Barvo določimo samo tistim, katere imajo nazive podkategorij iste (2 ali več)
        public string Barva { get; set; }
    }
}