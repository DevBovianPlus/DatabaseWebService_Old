using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsPDO
{
    public class SupplierMailModel
    {
        public string SupplierName { get; set; }
        public string BodyText { get; set; }
        public string Email { get; set; }
        public string CCEmails { get; set; }        
        public string Signature { get; set; }
        public string SubjectText { get; set; }

        public string EmployeeName { get; set; }
        public string ListOfSuppliers { get; set; }
        public string Reports { get; set; }
        public DateTime InquiryDate { get; set; }
        public string InquiryNumber { get; set; }
        public string ThanksAndGreeting { get; set; }

        public string ListOfPositions { get; set; }
    }
}