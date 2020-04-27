using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsDW.CashFlow_Skupno
{
    public class CashFlow_SkupnoModel
    {
        public int CashFlowSkupnoID { get; set; }
        public DateTime Datum { get; set; }
        public DateTime DatumPlana { get; set; }
        public string Vrsta { get; set; }
        public decimal PlacilaKupcev { get; set; }
        public decimal PlacilaAvansov { get; set; }
        public decimal PlacilaDobaviteljskiFaktoring { get; set; }
        public decimal PlacilaDobaviteljem { get; set; }
        public decimal PlacilaCassaSconto { get; set; }
        public decimal PlacilaOdkupov { get; set; }
        public decimal PlacilaOdkupovHR { get; set; }
        public decimal PlacilaDDV { get; set; }
        public decimal PlacilaPlace { get; set; }
        public decimal PlacilaKredit { get; set; }
        public decimal PlacilaLeasing { get; set; }
        public decimal PlacilaCassaScontoRocni { get; set; }
    }
}