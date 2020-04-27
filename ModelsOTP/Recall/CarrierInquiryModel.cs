using DatabaseWebService.ModelsOTP.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Recall
{
    public class CarrierInquiryModel
    {

        public int PrijavaPrevoznikaID { get; set; }
        public int OdpoklicID { get; set; }
        public int PrevoznikID { get; set; }
        public decimal PrvotnaCena { get; set; }
        public decimal PrijavljenaCena { get; set; }
        public DateTime DatumNaklada { get; set; }
        public DateTime DatumRazklada { get; set; }
        public DateTime DatumPosiljanjePrijav { get; set; }
        public DateTime DatumPrijave { get; set; }
        public DateTime ts { get; set; }
        public decimal OdstopanjeVEUR { get; set; }

        public CarrierModel Prevoznik { get; set; }
        public RecallModel Odpoklic { get; set; }
    }
}