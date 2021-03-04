using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsOTP.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Tender
{
    public class TenderPositionChangeModel
    {
        public int RazpisPozicijaSpremembeID { get; set; }
        public int RazpisID { get; set; }
        public int StrankaID { get; set; }
        public int RelacijaID { get; set; }       
        public int ZbirnikTonID { get; set; }
        public decimal StaraCena { get; set; }
        public decimal NovaCena { get; set; }
        public int IDVnosOseba { get; set; }
        public EmployeeSimpleModel VnosOseba { get; set; }
        public DateTime VnosTS { get; set; }

        public int IDSpremembeOseba { get; set; }
        public EmployeeSimpleModel SpremembeOseba { get; set; }
        public DateTime SpremembeTS { get; set; }

        public ClientFullModel Stranka { get; set; }
        public RouteModel Relacija { get; set; }

        public TonsModel ZbirnikTon { get; set; }

        public ClientSimpleModel OsebaVnos { get; set; }
        public ClientSimpleModel OsebaSpremembe { get; set; }
    }
}