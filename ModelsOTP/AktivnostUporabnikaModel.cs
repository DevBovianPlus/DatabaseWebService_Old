using DatabaseWebService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP
{
    public class AktivnostUporabnikaModel
    {
        public int AktivnostUporabnikaID { get; set; }
        public int OsebaID { get; set; }
        public UserModel Oseba { get; set; }
        public int AktivnostUporabnikaStatusID { get; set; }
        public string IP { get; set; }
        public string Opis { get; set; }
        public DateTime ts { get; set; }        
    }
}