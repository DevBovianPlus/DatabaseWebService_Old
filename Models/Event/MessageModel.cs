using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models.Event
{
    public class MessageModel
    {
        public int idSporocila { get; set; }
        public int IDDogodek { get; set; }
        public string OpisDel { get; set; }
        public DateTime ts { get; set; }
        public int tsIDOsebe { get; set; }

        public EventFullModel Dogodek { get; set; }
    }
}