using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsNOZ.OptimalStockOrder
{
    public class OptimalStockCalculationFields
    {
        public decimal optimalStock { get; set; }
        public decimal onStock { get; set; }
        public decimal orderInProgress { get; set; }
    }
}