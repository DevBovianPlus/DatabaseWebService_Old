using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsPDO.Inquiry
{
    public class InquiryStatus
    {
        public int StatusPovprasevanjaID { get; set; }
        public string Koda { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public int tsIDOseba { get; set; }
        public DateTime ts { get; set; }
        public DateTime tsUpdate { get; set; }
        public int tsUpdateuserID { get; set; }
    }
}