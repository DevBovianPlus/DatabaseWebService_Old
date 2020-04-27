using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsPDO.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsPDO.Inquiry
{
    public class InquiryFullModel
    {
        public int PovprasevanjeID { get; set; }
        public int KupecID { get; set; }
        public int StatusID { get; set; }
        public string Naziv { get; set; }
        public string PovprasevanjeStevilka { get; set; }
        public int NarociloID { get; set; }
        public DateTime DatumOddajePovprasevanja { get; set; }
        public DateTime ts { get; set; }
        public int tsIDOsebe { get; set; }
        public DateTime tsUpdate { get; set; }
        public int tsUpdateUserID { get; set; }
        public string KupecNaziv_P { get; set; }

        public List<InquiryPositionModel> PovprasevanjePozicija { get; set; }
        public List<InquiryPositionArtikelModel> PovprasevanjePozicijaArtikel { get; set; }

        public ClientFullModel Kupec { get; set; }
        public InquiryStatusModel StatusPovprasevanja { get; set; }

        public bool Zakleni { get; set; }
        public int ZaklenilUporabnik { get; set; }

        public OrderPDOFullModel Narocilo { get; set; }

        public DateTime DatumPredvideneDobave { get; set; }

        public int PovprasevanjeOddalID { get; set;}
        public EmployeeFullModel PovprasevanjeOddal { get; set; }
        public bool NotSendPDFAndEmailsToSupplier { get; set; }

        //samo za takrat ko kliknemo na gumb oddaj povpraševanje
        public bool EmployeeSubmitInquiry { get; set; }

        public string OpombeNarocilnica { get; set; }
        public string Narocila { get; set; }
        public DepartmentModel Oddelek { get; set; }
        public int OddelekID { get; set; }
        public String OddelekNaziv { get; set; }
    }
}