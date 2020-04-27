using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsNOZ
{
    public class DashboardNOZModel
    {
        public int OrderCount { get; set; }
        public int SubmitedOrders { get; set; }
        public int CreatedOrders { get; set; }
       
        public List<object> CurrentYearOrder { get; set; }
        public List<object> EmployeesOrderCount { get; set; }
    }

    public class OptimalStockOrdersInYear
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; }
        public int Count { get; set; }
        public string EmployeeName { get; set; }
        public string Supplier { get; set; }
    }
}