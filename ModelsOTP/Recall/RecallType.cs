using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Recall
{
    public class RecallType
    {
        public int TipOdpoklicaID { get; set; }
        public string Koda { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public int tsIDOseba { get; set; }
        public DateTime ts { get; set; }

    }
}