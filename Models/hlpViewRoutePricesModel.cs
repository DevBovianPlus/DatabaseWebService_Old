using DatabaseWebService.ModelsOTP.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models
{
    public class hlpViewRoutePricesModel
    {        
        public int iViewType { get; set; }
        public int iWeightType { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public List<RouteTransporterPricesModel> lRouteTransporterPriceModel { get; set; }
        public List<RouteModel> lRouteList { get; set; }
    }
}