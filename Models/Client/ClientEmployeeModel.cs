using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models.Client
{
    public class ClientEmployeeModel
    {
        public int idStrankaOsebe { get; set; }
        public int idStranka { get; set; }
        public int idOsebe { get; set; }
        public DateTime ts { get; set; }
        public int tsIDOsebe { get; set; }

        public EmployeeSimpleModel oseba { get; set; }
    }
}