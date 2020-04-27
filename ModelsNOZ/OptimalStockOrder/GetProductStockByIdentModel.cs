using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsNOZ.OptimalStockOrder
{
    public class GetProductStockByIdentModel
    {
        public string IDENT { get; set; }        
        public decimal Zaloga { get; set; }
        public double ZalogaVrednost { get; set; }        
    }
}