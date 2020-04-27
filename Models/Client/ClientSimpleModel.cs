using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models.Client
{
    public class ClientSimpleModel
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
        public string FAX { get; set; }
        public string InternetniNalov { get; set; }
        public string KontaktnaOseba { get; set; }
        public string RokPlacila { get; set; }
        public string TRR { get; set; }
        public string DavcnaStev { get; set; }
        public string MaticnaStev { get; set; }
        public string RangStranke { get; set; }
        public string StatusDomacTuji { get; set; }
        public string Zavezanec_DA_NE { get; set; }
        public string IdentifikacijskaStev { get; set; }
        public string Clan_EU { get; set; }
        public string BIC { get; set; }
        public string KodaPlacila { get; set; }
        public string StatusKupecDobavitelj { get; set; }
        public string DrzavaStranke { get; set; }
        public string Neaktivna { get; set; }
        public int GenerirajERacun { get; set; }
        public int JavniZavod { get; set; }
        public int SecondID { get; set; }
        public DateTime ts { get; set; }
        public int tsIDOsebe { get; set; }

        //Employees
        public int idOsebe { get; set; }
        public string ImeInPriimekZaposlen { get; set; }

        //use only fot OTP project
        public string TipStranka { get; set; }
        public int RecallCount { get; set; }

        //use only for PDO and NOZ project
        public bool KupecViden { get; set; }
        public int TempID { get; set; }
        public string Jezik { get; set; }
        public string PrivzetaEM { get; set; }
        public string Drzava { get; set; }

        //use only for NOZ project
        public int HasStock { get; set; }
    }
}