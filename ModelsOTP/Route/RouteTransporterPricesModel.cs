using DatabaseWebService.DomainOTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Route
{
    public class RouteTransporterPricesModel
    {
        public int TempID { get; set; }
        public int RecallCount { get; set; }
        public string Relacija { get; set; }
        public string PrevoznikID_1 { get; set; }
        public string Prevoznik_1 { get; set; }
        public decimal Prevoznik_1_Cena { get; set; }
        public string Prevoznik_2 { get; set; }
        public decimal Prevoznik_2_Cena { get; set; }
        public string Prevoznik_3 { get; set; }
        public decimal Prevoznik_3_Cena { get; set; }
        public string Prevoznik_4 { get; set; }
        public decimal Prevoznik_4_Cena { get; set; }

        public List<int> DodaneStrankeID { get; set; }

        public bool LastniPrevoz { get; set; }
        public bool IsRoute { get; set; }
        public int SortIndx { get; set; }

    }
}