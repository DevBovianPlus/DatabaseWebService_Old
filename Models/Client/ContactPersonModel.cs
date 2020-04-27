using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models.Client
{
    public class ContactPersonModel
    {
        public int idKontaktneOsebe { get; set; }
        public int idStranka { get; set; }
        public string NazivKontaktneOsebe { get; set; }
        public string Telefon { get; set; }
        public string GSM { get; set; }
        public string Email { get; set; }
        public string DelovnoMesto { get; set; }
        public int ZaporednaStevika { get; set; }
        public string Fax { get; set; }
        public string Opombe { get; set; }
        public DateTime ts { get; set; }
        public int tsIDOsebe { get; set; }

        public string Stranka { get; set; }

        public string NazivPodpis { get; set; }
    }
}