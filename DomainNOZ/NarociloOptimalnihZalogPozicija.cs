//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DatabaseWebService.DomainNOZ
{
    using System;
    using System.Collections.Generic;
    
    public partial class NarociloOptimalnihZalogPozicija
    {
        public int NarociloOptimalnihZalogPozicijaID { get; set; }
        public int NarociloOptimalnihZalogID { get; set; }
        public string KategorijaNaziv { get; set; }
        public string NazivArtikla { get; set; }
        public Nullable<decimal> Kolicina { get; set; }
        public string Opombe { get; set; }
        public Nullable<System.DateTime> ts { get; set; }
        public Nullable<int> tsIDOsebe { get; set; }
        public Nullable<System.DateTime> tsUpdate { get; set; }
        public Nullable<int> tsUpdateUserID { get; set; }
        public string IdentArtikla_P { get; set; }
        public Nullable<decimal> KolicinaPol { get; set; }
    
        public virtual NarociloOptimalnihZalog NarociloOptimalnihZalog { get; set; }
    }
}
