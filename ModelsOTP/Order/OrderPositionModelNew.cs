using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Order
{
    public class OrderPositionModelNew
    {
        public int tempID { get; set; }
        public string UniqueID { get; set; }
        public string Narocilnica { get; set; }
        public int St_Pozicija { get; set; }
        public string Order_Confirm { get; set; }
        public string Tovarna { get; set; }
        public string Artikel { get; set; }
        public DateTime Datum_narocila { get; set; }
        public DateTime Datum_Dobave { get; set; }
        public string Dobavitelj { get; set; }
        public string Kupec { get; set; }
        public string Kupec_Naslov { get; set; }
        public string Kupec_Posta { get; set; }
        public string Kupec_Kraj { get; set; }
        public decimal Naroceno { get; set; }
        public decimal Proizvedeno { get; set; }
        public decimal Prevzeto { get; set; }
        public decimal Razlika { get; set; }
        public string Tip { get; set; }
        public decimal Zaloga { get; set; }
        public int Dovoljeno_Odpoklicati { get; set; }
        public string Interno { get; set; }
        public string Kategorija { get; set; }
        public string Ident { get; set; }
        public decimal OdpoklicKolicinaOTP { get; set; }
        public decimal VsotaOdpoklicKolicinaOTP { get; set; }
        public string EnotaMere { get; set; }

        public DateTime ZeljeniRokDobave { get; set; }
        public DateTime PotrjeniRokDobave { get; set; }
        public string Status { get; set; }        

        //Uporabljeno samo za skladišča z Lastno zalogo
        public int OdpoklicID { get; set; }
        public int OdpoklicPozicijeID { get; set; }
        public string TipAplikacije { get; set; } // NOZ, PDO 
        public string StatusPozicije { get; set; } // Status že Odpoklicano-Potrjeno - OP
        public int SortGledeNaTipApp { get; set; } // 0 - PDO + OC, 1 - PDO, 2 - samo tisti ki imajo OC, 3 - ostali
    }
}