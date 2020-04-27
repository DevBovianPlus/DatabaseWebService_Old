using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsPDO.Inquiry
{
    public class ProductModel
    {
        public int TempID { get; set; }
        public string StevilkaArtikel { get; set; }
        public string Dobavitelj { get; set; }
        public string Naziv { get; set; }
        public string Kategorija { get; set; }
        public string Gloss { get; set; }
        public string Gramatura { get; set; }
        public string Velikost { get; set; }
        public string Tek { get; set; }
    }
}