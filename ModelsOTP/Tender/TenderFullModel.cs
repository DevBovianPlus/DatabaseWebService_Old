using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsOTP.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Tender
{
    public class TenderFullModel
    {
        public int RazpisID { get; set; }
        public decimal CenaSkupaj { get; set; }
        public DateTime DatumRazpisa { get; set; }
        public int tsIDOseba { get; set; }
        public DateTime ts { get; set; }
        public string Naziv { get; set; }
        public bool RazpisKreiran { get; set; }

        public List<TenderPositionModel> RazpisPozicija { get; set; }
    }
}