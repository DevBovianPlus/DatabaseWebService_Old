using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Recall
{
    public class RecallBuyerPositionModel
    {
        public int Akcija { get; set; } // ADD=1, UPDATE, DELETE=2
        public int ZaporednaStevilka { get; set; }
        public int OdpoklicKupecPozicijaID { get; set; }
        public int OdpoklicKupecID { get; set; }
        public string NarociloID { get; set; }
        public string Kljuc { get; set; }
        public string acKey { get; set; }
        public DateTime Datum { get; set; }
        public string Valuta { get; set; }
        public string Kupec { get; set; }
        public string Prevzemnik { get; set; }
        public decimal Kolicina { get; set; }
        public decimal Vrednost { get; set; }
        public decimal VrednostPrevoza { get; set; }
        public decimal ProcentPrevoza { get; set; }
        public bool addedFromPopUp { get; set; }        

        public int tsIDOseba { get; set; }        
        public DateTime ts { get; set; }
    }
}