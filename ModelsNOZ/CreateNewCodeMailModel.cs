using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsNOZ
{
    public class CreateNewCodeMailModel
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string EmployeeName { get; set; }
        public string Email { get; set; }
        public string RootURL { get; set; }
    }
}