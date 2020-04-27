using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsNOZ.OptimalStockOrder
{
    public class hlpOptimalStockOrderModel
    {

        public List<ClientSimpleModel> Suppliers { get; set; }
        public List<OptimalStockTreeHierarchy> SubCategoryWithProducts { get; set; }

        public List<SumSubCategoryModel> lAllSubCategory { get; set; }
    }
}