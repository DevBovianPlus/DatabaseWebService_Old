using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models.Event
{
    public class EventMeetingModel
    {
        public int DogodekSestanekID { get; set; }
        public int DogodekID { get; set; }
        public string Tip { get; set; }
        public string Opis { get; set; }
        public DateTime Datum { get; set; }
        public string ImeInPriimekOsebe { get; set; }
        public int tsIDOsebe { get; set; }
        public DateTime ts { get; set; }
        public EventFullModel Dogodek { get; set; }
    }
}