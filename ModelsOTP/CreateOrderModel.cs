using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP
{
    public class CreateOrderModel
    {
        public int RecallID { get; set; }
        public string TypeCode { get; set; }
        public string Note { get; set; }
        public List<ServiceListModel> services { get; set; }
    }

    public class ServiceListModel
    {
        public int ServiceID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal Price { get; set; }

    }
}