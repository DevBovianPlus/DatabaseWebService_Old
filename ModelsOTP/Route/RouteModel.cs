using DatabaseWebService.DomainOTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Route
{
    public class RouteModel
    {
        public int RelacijaID { get; set; }
        public string Koda { get; set; }
        public string Naziv { get; set; }
        public decimal Dolzina { get; set; }
        public DateTime Datum { get; set; }
        public int tsIDOsebe { get; set; }
        public DateTime ts { get; set; }

        public string Opomba { get; set; }

        public int RecallCount { get; set; }
        public int SupplierArrangesTransportRecallCount { get; set; }
        
        //Uporablja samo za izračun število odpoklicev v prejšnjem letu
        public DateTime RouteFirstRecallDate { get; set; }

        //uproablja se samo kjer imamo pregled prevoznikov in cen glede na relacijo (CarrierAndRoutesPricing.aspx)
        public decimal Cena { get; set; }
        public int TempID { get; set; }

        //public virtual ICollection<Razpis> Razpis { get; set; }
    }
}