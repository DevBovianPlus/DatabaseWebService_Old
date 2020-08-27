//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DatabaseWebService.DomainPDO
{
    using System;
    using System.Collections.Generic;
    
    public partial class Stranka_PDO
    {
        public Stranka_PDO()
        {
            this.KontaktnaOseba_PDO = new HashSet<KontaktnaOseba_PDO>();
            this.StrankaZaposleni_PDO = new HashSet<StrankaZaposleni_PDO>();
            this.PovprasevanjePozicija = new HashSet<PovprasevanjePozicija>();
            this.NarociloPozicija_PDO = new HashSet<NarociloPozicija_PDO>();
            this.Narocilo_PDO = new HashSet<Narocilo_PDO>();
            this.Povprasevanje = new HashSet<Povprasevanje>();
            this.PovprasevanjePozicijaArtikel = new HashSet<PovprasevanjePozicijaArtikel>();
        }
    
        public int StrankaID { get; set; }
        public string KodaStranke { get; set; }
        public int TipStrankaID { get; set; }
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
        public Nullable<bool> StatusDomacTuji { get; set; }
        public Nullable<bool> Zavezanec_DA_NE { get; set; }
        public string IdentifikacijskaStev { get; set; }
        public Nullable<bool> Clan_EU { get; set; }
        public string BIC { get; set; }
        public string KodaPlacila { get; set; }
        public string DrzavaStranke { get; set; }
        public string Neaktivna { get; set; }
        public Nullable<int> GenerirajERacun { get; set; }
        public Nullable<int> JavniZavod { get; set; }
        public Nullable<System.DateTime> ts { get; set; }
        public Nullable<int> tsIDOsebe { get; set; }
        public Nullable<System.DateTime> tsUpdate { get; set; }
        public Nullable<int> tsUpdateUserID { get; set; }
        public Nullable<int> JezikID { get; set; }
        public string PrivzetaEM { get; set; }
        public string ZadnjaIzbranaKategorija { get; set; }
    
        public virtual Jeziki Jeziki { get; set; }
        public virtual ICollection<KontaktnaOseba_PDO> KontaktnaOseba_PDO { get; set; }
        public virtual ICollection<StrankaZaposleni_PDO> StrankaZaposleni_PDO { get; set; }
        public virtual TipStranka_PDO TipStranka_PDO { get; set; }
        public virtual ICollection<PovprasevanjePozicija> PovprasevanjePozicija { get; set; }
        public virtual ICollection<NarociloPozicija_PDO> NarociloPozicija_PDO { get; set; }
        public virtual ICollection<Narocilo_PDO> Narocilo_PDO { get; set; }
        public virtual ICollection<Povprasevanje> Povprasevanje { get; set; }
        public virtual ICollection<PovprasevanjePozicijaArtikel> PovprasevanjePozicijaArtikel { get; set; }
    }
}
