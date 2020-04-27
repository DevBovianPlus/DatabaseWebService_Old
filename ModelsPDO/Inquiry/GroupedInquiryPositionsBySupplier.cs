using DatabaseWebService.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsPDO.Inquiry
{
    public class GroupedInquiryPositionsBySupplier
    {
        public IEnumerable<InquiryPositionArtikelModel> InquiryPositionsArtikel { get; set; }
        public ClientFullModel Supplier { get; set; }
        public string ReportFilePath { get; set; }
        public ClientFullModel Buyer { get; set; }
        public string SelectedContactPersons { get; set; } // we fill contact persons emails with ; (miki@miki.si;janez@janez.si)
        public string SelectedContactPersonsEmails { get; set; } // we fill contact persons emails with ; (miki@miki.si;janez@janez.si)
        public string SelectedGrafolitPersons { get; set; } // we fill contact persons emails with ; (miki@miki.si;janez@janez.si)
        public string SelectedGrafolitPersonsEmails { get; set; } // we fill contact persons emails with ; (miki@miki.si;janez@janez.si)
        public Nullable<bool> KupecViden { get; set; }
        public string EmailBody { get; set; } 
        public int PovprasevanjePozicijaID { get; set; }
    }
}