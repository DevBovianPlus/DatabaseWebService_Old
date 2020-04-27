using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models.Employee
{
    public class RoleModel
    {
        public int idVloga { get; set; }
        public string Koda { get; set; }
        public string Naziv { get; set; }
        public DateTime ts { get; set; }
        public int tsIDOsebe { get; set; }
    }
}