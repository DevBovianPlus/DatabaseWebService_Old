using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models.Client
{
    public class ClientCategorieModel
    {
        public int idStrankaKategorija { get; set; }
        public int idStranka { get; set; }
        public int idKategorija { get; set; }
        public DateTime ts { get; set; }
        public int tsIDOseba { get; set; }
        public bool HasChartDataForCategorie { get; set; }

        public string Stranka { get; set; }
        public CategorieModel Kategorija { get; set; }
    }
}