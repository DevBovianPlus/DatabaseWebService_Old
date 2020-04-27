using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models
{
    public class ComplexModel
    {
        public string Name { get; set; }
        public List<PostModel> posts { get; set; }
        public List<EmployeeRankModel> employeeRanks { get; set; }
    }
}