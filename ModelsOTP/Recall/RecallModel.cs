using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Recall
{
    public class RecallModel
    {
        public int OdpoklicID { get; set; }
        public int DobaviteljID { get; set; }
        public string DobaviteljNaziv { get; set; }
        public int RelacijaID { get; set; }
        public string RelacijaNaziv { get; set; }
        public int StatusID { get; set; }
        public string StatusNaziv { get; set; }
        public decimal CenaPrevoza { get; set; }
        public decimal KolicinaSkupno { get; set; }
        public string StatusKoda { get; set; }
        public int tsIDOseba { get; set; }
        public int OdpoklicStevilka { get; set; }
        public DateTime ts { get; set; }
        public bool DobaviteljUrediTransport { get; set; }
        public string DobaviteljNaslov { get; set; }
        public string DobaviteljPosta { get; set; }
        public string DobaviteljKraj { get; set; }

        public string P_TransportOrderPDFName { get; set; }
        public int P_UnsuccCountCreatePDFPantheon { get; set; }

    }
}