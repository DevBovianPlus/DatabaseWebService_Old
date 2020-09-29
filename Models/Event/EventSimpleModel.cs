using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models.Client
{
    public class EventSimpleModel
    {
        public int idDogodek { get; set; }
        public int idStranka { get; set; }
        public int idKategorija { get; set; }
        public int Skrbnik { get; set; }
        public int Izvajalec { get; set; }
        public int idStatus { get; set; }
        public string Opis { get; set; }
        public DateTime DatumOtvoritve { get; set; }
        public DateTime Rok { get; set; }
        public string DatumZadZaprtja { get; set; }
        public DateTime ts { get; set; }
        public int tsIDOsebe { get; set; }
        public string VneselOseba { get; set; }

        public string Kategorija { get; set; }
        public string OsebeSkrbnik { get; set; }
        public string OsebeIzvajalec { get; set; }
        public string StatusDogodek { get; set; }
        public string Stranka { get; set; }
        /*public virtual ICollection<Sporocila> Sporocila { get; set; }*/
    }
}