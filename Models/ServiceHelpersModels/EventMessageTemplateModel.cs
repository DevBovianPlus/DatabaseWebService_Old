using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models.ServiceHelpersModels
{
    public class EventMessageTemplateModel
    {
        public int idDogodek { get; set; }
        public string ImeSkrbnik { get; set; }
        public string PriimekSkrbnik { get; set; }
        public string ImeIzvajalec { get; set; }
        public string PriimekIzvajalec { get; set; }
        public string NazivPrvi { get; set; }
        public string Status { get; set; }
        public string Kategorija { get; set; }
        public string KategorijaKoda { get; set; }
        public DateTime Rok { get; set; }

        public string Tip { get; set; }
        public int Zamuda { get; set; }
        public DateTime Otvoritev { get; set; }
        public string Opis { get; set; }

        public string EmailTo { get; set; }

        public string ServerTag { get; set; }
    }
}