using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models
{
    public class EmployeeRankModel
    {
        public int ID { get; set; }
        public string Rank { get; set; }
        public string Description { get; set; }
    }
}