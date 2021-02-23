using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Recall
{
    public class DisconnectedInvoicesModel
    {
        public int TempID { get; set; }
        public string Kljuc { get; set; }
        public string acKey { get; set; }
        public DateTime Datum { get; set; }
        public string Valuta { get; set; }
        public string Kupec { get; set; }
        public string Prevzemnik { get; set; }
        public decimal Kolicina { get; set; }
        public decimal ZnesekFakture { get; set; }
        public decimal Vrednost { get; set; }
        public decimal VrednostPrevoza { get; set; }
        public decimal ProcentPrevoza { get; set; }
        
    }
}