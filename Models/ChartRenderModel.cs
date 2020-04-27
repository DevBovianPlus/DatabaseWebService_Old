using DatabaseWebService.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models
{
    public class ChartRenderModel
    {
        public List<ChartRenderSimple> chartRenderData { get; set; }
        public List<EventSimpleModel> EventList { get; set; }
    }

    public class ChartRenderSimple
    {
        public int IzpisGrafaID { get; set; }
        public int StrankaID { get; set; }
        public int KategorijaID { get; set; }
        public int Obdobje { get; set; }
        public int Tip { get; set; }
        public string EnotaMere { get; set; }
        public string Opis { get; set; }
        public decimal Vrednost { get; set; }
        public DateTime Datum { get; set; }
    }
}