using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Client
{
    public class ClientTransportType
    {
        public int TipPrevozaID { get; set; }
        public string Koda { get; set; }
        public string Naziv { get; set; }
        public string Opombe { get; set; }
        public decimal DovoljenaTeza { get; set; }
        public bool ShranjevanjePozicij { get; set; }
        public DateTime ts { get; set; }
        public int tsIDPrijave { get; set; }
    }
}