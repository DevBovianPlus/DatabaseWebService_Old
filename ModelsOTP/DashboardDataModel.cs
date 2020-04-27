using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP
{
    public class DashboardDataModel
    {
        public int AllRecalls { get; set; }
        public int ApprovedRecalls { get; set; }
        public int RejectedRecalls { get; set; }
        public int NeedsApproval { get; set; }
        public int Routes { get; set; }
        public int Transporters { get; set; }
        public int OwnWarehouse { get; set; }

        public List<object> CurrentYearRecall { get; set; }
        public List<object> EmployeesRecallCount { get; set; }
        public List<object> TransporterRecallCount { get; set; }
        public List<object> RouteRecallCount { get; set; }
        public List<object> SupplierRecallCount { get; set; }
    }

    public class RecallsInYear
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; }
        public int Count { get; set; }
        public string EmployeeName { get; set; }
        public string Transporter { get; set; }
        public string Route { get; set; }
        public string Supplier { get; set; }
    }
}