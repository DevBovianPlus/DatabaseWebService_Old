using DatabaseWebService.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsPDO.Inquiry
{
    public class InquiryPositionModel
    {
        public int PovprasevanjePozicijaID { get; set; }
        public int PovprasevanjeID { get; set; }
        public int DobaviteljID { get; set; }
        public string DobaviteljNaziv_P { get; set; }
        public string DobaviteljKontaktOsebe { get; set; }
        public string ObvesceneOsebe { get; set; }
        public int GrafolitID { get; set; }
        public string DobaviteljID_P { get; set; }
        public string Dobavitelj_PrivzetaEM { get; set; }
        public string Artikli { get; set; }
        public bool KupecViden { get; set; }
        public DateTime ts { get; set; }
        public int tsIDOsebe { get; set; }
        public DateTime tsUpdate { get; set; }
        public int tsUpdateUserID { get; set; }
        public string PotDokumenta { get; set; }
        public string Opomba { get; set; }
        public string EmailBody { get; set; }
        public ClientFullModel Dobavitelj { get; set; }
        public DateTime DatumPredvideneDobave { get; set; }

        public List<InquiryPositionArtikelModel> PovprasevanjePozicijaArtikel { get; set; }

    }
}