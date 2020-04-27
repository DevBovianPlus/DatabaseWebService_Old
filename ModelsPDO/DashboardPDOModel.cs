using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsPDO
{
    public class DashboardPDOModel
    {
        public int InquiryCount { get; set; }
        public int ConfirmedInquiries { get; set; }
        public int InquiriesInProgress { get; set; }
        public int InquiriesInPurchase { get; set; }
        public int SubmitedOrders { get; set; }

        public List<object> CurrentYearInquiry { get; set; }
        public List<object> EmployeesInquiryCount { get; set; }
    }

    public class InquiriesInYear
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; }
        public int Count { get; set; }
        public string EmployeeName { get; set; }
        public string Supplier { get; set; }
    }
}