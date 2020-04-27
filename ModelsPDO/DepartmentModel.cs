using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsPDO
{
    public class DepartmentModel
    {
        public int OddelekID { get; set; }
        public string Koda { get; set; }
        public string Naziv { get; set; }
        public DateTime ts { get; set; }        
    }
}