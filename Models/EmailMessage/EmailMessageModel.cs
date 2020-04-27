using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models.EmailMessage
{
    public class EmailMessageModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public int Status { get; set; }
        public DateTime ts { get; set; }
        public int MasterID { get; set; }
        public int tsIDOsebe { get; set; }
    }
}