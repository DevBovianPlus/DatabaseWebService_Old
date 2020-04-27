using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models.Event
{
    public class EventStatusModel
    {
        public int idStatusDogodek { get; set; }
        public string Koda { get; set; }
        public string Naziv { get; set; }
        public DateTime ts { get; set; }
        public int tsIDOsebe { get; set; }
    }
}