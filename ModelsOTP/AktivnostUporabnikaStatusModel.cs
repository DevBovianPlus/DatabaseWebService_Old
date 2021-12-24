using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP
{
    public class AktivnostUporabnikaStatusModel
    {
        public int AktivnostUporabnikaStatusID { get; set; }
        public string Koda { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public DateTime ts { get; set; }        
    }
}