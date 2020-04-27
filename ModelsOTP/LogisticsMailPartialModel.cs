using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP
{
    public class LogisticsMailPartialModel
    {
        public string RouteName { get; set;}
        public string RecallNum { get; set; }
        public string OriginalPrice { get; set; }
        public string PrijavaPrevoznikaHTML { get; set; }
        public string RecallURL { get; set; }
    }
}