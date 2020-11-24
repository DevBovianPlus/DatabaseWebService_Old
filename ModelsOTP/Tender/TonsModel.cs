using DatabaseWebService.DomainOTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Tender
{
    public class TonsModel
    {
        public int ZbirnikTonID { get; set; }
        public string Koda { get; set; }
        public string Naziv { get; set; }

        public decimal NajnizjaCena { get; set; }
        
        public DateTime ts { get; set; }

     
    }
}