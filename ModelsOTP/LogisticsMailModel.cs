using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP
{
    public class LogisticsMailModel
    {
        public string EmployeeName { get; set;}
        public string BodyText { get; set; }
        public string LogisticsPartialMail { get; set; }
        public string CustomCarrierURL { get; set; }
        public string Email { get; set; }
    }
}