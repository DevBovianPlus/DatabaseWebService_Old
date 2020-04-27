using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsNOZ.OptimalStockOrder
{
    public class GetProductsByOptimalStockValuesModel
    {
        public string IDENT { get; set; }
        public string NAZIV { get; set; }
        public string DOBAVITELJ { get; set; }
        public decimal LetnaProdaja { get; set; }
        public decimal TrenutnaZaloga { get; set; }
        public string NazivPodkategorije { get; set; }

        public List<GetProductsByOptimalStockValuesModel> ChildProducts { get; set; }
        public List<SumSubCategoryModel> AllSubCategories { get; set; }
        public List<GetProductsByOptimalStockValuesModel> SelectedChildProducts { get; set; }//uporabljamo ko dodajamo v artikle v NarociloOptimalnihZalogPozicije. Nov lastnost zaradi tega ker moramo še vedeti vrednosti od starša in ne moremo spreminjati ChildProducts lastnosti, ker to je dataSource
    }
}