using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsPDO.Settings
{
    public class SettingsModel
    {
        public int NastavitveID { get; set; }
        public int PovprasevanjeStevilcenjeStev { get; set; }
        public string PovprasevanjeStevilcenjePredpona { get; set; }
        public string Opombe { get; set; }
        public DateTime ts { get; set; }
        public int tsIDOsebe { get; set; }
        public DateTime tsUpdate { get; set; }
        public int tsUpdateUserID { get; set; }
        public bool PosiljanjePoste { get; set; }
        public string EmailStreznik { get; set; }
        public int EmailVrata { get; set; }
        public bool EmailSifriranjeSSL { get; set; }
    }
}