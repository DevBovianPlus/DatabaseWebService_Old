using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP
{
    public class RecallApprovalEmailModel
    {
        public string FirstNameSupervisor { get; set; }
        public string LastnameSupervisor { get; set; }
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string OdpoklicID { get; set; }
        public string Comments { get; set; }
        public string Email { get; set; }
        public string ServerTagOTP { get; set; }
        public string RecallStatus { get; set; }
    }
}