using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Recall
{
    public class RecallPositionModel
    {
        public int OdpoklicPozicijaID { get; set; }
        public int OdpoklicID { get; set; }
        public string NarociloID { get; set; }
        public int NarociloPozicijaID { get; set; }
        public int TipID { get; set; }
        public decimal Kolicina { get; set; }
        public decimal StPalet { get; set; }
        public decimal KolicinaIzNarocila { get; set; }
        public int StatusKolicine { get; set; }
        public string Material { get; set; }
        public int tsIDOseba { get; set; }
        public DateTime ts { get; set; }

        public string OC { get; set; }
        public decimal KolicinaPrevzeta { get; set; }
        public decimal KolicinaRazlika { get; set; }
        public decimal Palete { get; set; }
        public string KupecNaziv { get; set; }
        public int KupecViden { get; set; }
        public decimal TrenutnaZaloga { get; set; }
        public decimal OptimalnaZaloga { get; set; }
        public string TipNaziv { get; set; }
        public string Interno { get; set; }
        public decimal Proizvedeno { get; set; }
        public string MaterialIdent { get; set; }
        public DateTime DatumVnosa { get; set; }
        public decimal KolicinaOTP { get; set; }
        public bool StatusPrevzeto { get; set; }
        public int ZaporednaStevilka { get; set; }
        public decimal KolicinaOTPPozicijaNarocilnice { get; set; }

        public string KupecNaslov { get; set; }
        public string KupecPosta { get; set; }
        public string KupecKraj { get; set; }

        public bool OdpoklicIzLastneZaloge { get; set; }
        public int PrvotniOdpoklicPozicijaID { get; set; }
        public bool Split { get; set; }
        public decimal TransportnaKolicina { get; set; }
        public string EnotaMere { get; set; }

        public RecallModel Odpoklic { get; set; }
        public RecallType TipOdpoklica { get; set; }

        //Uporaba pri nastavljanju nohi id-jev na razdeljenih pozicija RecallForm.aspx - line: 1170
        public bool childSplit { get; set; }
        public bool addedFromPopUp { get; set; }

    }
}