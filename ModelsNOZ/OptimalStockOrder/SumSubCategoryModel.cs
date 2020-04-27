using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsNOZ.OptimalStockOrder
{
    public class SumSubCategoryModel
    {
        public string NazivPodKategorije { get; set; }
        public string Kategorija { get; set; }
        public string Gloss { get; set; }
        public string Gramatura { get; set; }
        public string Velikost { get; set; }
        public string Tek { get; set; }
        public string Barva { get; set; }
        public string Paket { get; set; }
        public string Pefc { get; set; }
        public string Fsc { get; set; }
        public decimal VsotaZaloge { get; set; }

        public List<GetProductsByOptimalStockValuesModel> ChildProducts { get; set; }
    }
}