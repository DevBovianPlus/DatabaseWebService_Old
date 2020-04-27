using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models
{
    public class PlanModel
    {
        public int idPlan { get; set; }
        public int idKategorija { get; set; }
        public int IDStranka { get; set; }
        public decimal LetniZnesek { get; set; }
        public int Leto { get; set; }
        public DateTime ts { get; set; }
        public int tsIDOsebe { get; set; }

        public string Kategorija { get; set; }
        public string Stranka { get; set; }
    }
}