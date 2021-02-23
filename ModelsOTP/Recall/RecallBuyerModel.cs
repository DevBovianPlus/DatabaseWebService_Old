using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Recall
{
    public class RecallBuyerModel
    {
        public int OdpoklicKupecID { get; set; }
        public int PrevoznikID { get; set; }
        public int RazpisPozicijaID { get; set; }
        public string PrevoznikNaziv { get; set; }
        public string StevilkaNarocilnica { get; set; }
        public int RelacijaID { get; set; }
        public string RelacijaNaziv { get; set; }
        public int StatusID { get; set; }
        public string StatusNaziv { get; set; }
        public decimal CenaPrevoza { get; set; }
        public decimal KolicinaSkupno { get; set; }
        public string StatusKoda { get; set; }
        public int tsIDOseba { get; set; }
        public int OdpoklicKupecStevilka { get; set; }
        public DateTime ts { get; set; }      



    }
}