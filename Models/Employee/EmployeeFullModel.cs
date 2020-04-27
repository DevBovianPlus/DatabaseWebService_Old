using DatabaseWebService.Models.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models
{
    public class EmployeeFullModel
    {
        public int idOsebe { get; set; }
        public int idVloga { get; set; }
        public string Ime { get; set; }
        public string Priimek { get; set; }
        public string Naslov { get; set; }
        public DateTime DatumRojstva { get; set; }
        public string Email { get; set; }
        public DateTime DatumZaposlitve { get; set; }
        public string UporabniskoIme { get; set; }
        public string Geslo { get; set; }
        public string TelefonGSM { get; set; }
        public int Zunanji { get; set; }
        public string DelovnoMesto { get; set; }
        public DateTime ts { get; set; }
        public int tsIDOsebe { get; set; }
        public string ProfileImage { get; set; }
        public RoleModel Vloga { get; set; }
        public int NadrejeniID { get; set; }
        public EmployeeFullModel Nadrejeni { get; set; }
        public int idNadrejeni { get; set; }

        public bool PDODostop { get; set; }
        public bool NOZDostop { get; set; }
        public string EmailGeslo { get; set; }
        public string EmailStreznik { get; set; }
        public int EmailVrata { get; set; }
        public bool EmailSifriranjeSSL { get; set; }
        public string Podpis { get; set; }
        public string PantheonUsrID { get; set; }
    }
}
