using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models.FinancialControl
{
    public class FinancialControlModel
    {
        public DateTime Timestmp { get; set; }
        public int FinancniDashboardID { get; set; }
        public decimal Dobavitelji { get; set; }
        public decimal Kupci { get; set; }
        public decimal Zaloga { get; set; }
        public decimal Investicije { get; set; }
        public decimal Investicijsko_vzdrzevanje { get; set; }
        public decimal Krediti { get; set; }
        public decimal Dobicek { get; set; }
        public decimal Skupaj { get; set; }

        public int StDniKupci { get; set; }
        public int StDniDobavitelji { get; set; }
    }
}