using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models.Client
{
    public class DevicesModel
    {
        public int idNaprava { get; set; }
        public Nullable<int> idStranka { get; set; }
        public string Koda { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public DateTime ts { get; set; }
        public int tsIDOsebe { get; set; }

        public string Stranka { get; set; }
    }
}