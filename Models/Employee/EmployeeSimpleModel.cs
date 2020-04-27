using DatabaseWebService.Models.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models
{
    public class EmployeeSimpleModel
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
        public String Vloga { get; set; }
        
        //public RoleModel Vloga { get; set; }
    }
}