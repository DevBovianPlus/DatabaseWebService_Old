using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Recall
{
    public class ZbirnikTonModel
    {
        public int ZbirnikTonID { get; set; }
        public string Koda { get; set; }
        public string Naziv { get; set; }
        public decimal TezaOd { get; set; }
        public decimal TezaDo { get; set; }
        public DateTime ts { get; set; }

    }
}