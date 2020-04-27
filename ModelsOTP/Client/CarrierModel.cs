using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Client
{
    public class CarrierModel
    {
        public int idStranka { get; set; }
        public string KodaStranke { get; set; }
        public string NazivPrvi { get; set; }
        public string NazivDrugi { get; set; }
        public string Naslov { get; set; }
        public string StevPoste { get; set; }
        public string NazivPoste { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
    }
}