using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsOTP.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Tender
{
    public class TenderPositionModel
    {
        public int RazpisPozicijaID { get; set; }
        public int RazpisID { get; set; }
        public int StrankaID { get; set; }
        public int RelacijaID { get; set; }       
        public int ZbirnikTonID { get; set; }
        public decimal Cena { get; set; }
        public decimal NajnizjaCena { get; set; }
        public string PotDokumenta { get; set; }
        public int IDOseba { get; set; }
        public DateTime ts { get; set; }

        public ClientFullModel Stranka { get; set; }
        public RouteModel Relacija { get; set; }

        public TonsModel ZbirnikTon { get; set; }
    }
}