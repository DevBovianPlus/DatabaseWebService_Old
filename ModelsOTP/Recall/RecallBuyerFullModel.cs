using DatabaseWebService.ModelsOTP.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Recall
{
    public class RecallBuyerFullModel
    {
        public int OdpoklicKupecID { get; set; }
        public int RazpisPozicijaID { get; set; }
        public string PrevoznikNaziv { get; set; }
        public string KupecNaziv { get; set; }
        public int RelacijaID { get; set; }
        public string RelacijaNaziv { get; set; }
        public int StatusID { get; set; }
        public string StatusNaziv { get; set; }
        public decimal CenaPrevozaSkupno { get; set; }
        public decimal KolicinaSkupno { get; set; }
        public decimal ProcentPrevozaSkupno { get; set; }
        public string StatusKoda { get; set; }
        public int tsIDOseba { get; set; }
        public int OdpoklicKupecStevilka { get; set; }
        public DateTime ts { get; set; }

        public int UserID { get; set; }

        public int ZbirnikTonID { get; set; }
        public string ZbirnikTonKoda { get; set; }

        public string StevilkaNarocilnica { get; set; }
        public string XMLOrder { get; set; }
        public string XMLInvoice { get; set; }

        public int IzdelajNarocilnico { get; set; }

        public List<RecallBuyerPositionModel> OdpoklicKupecPozicija { get; set; }
        public RouteModel Relacija { get; set; }
        public RecallStatus StatusOdpoklica { get; set; }
        public bool bBrezFakture { get; set; }
    }
}