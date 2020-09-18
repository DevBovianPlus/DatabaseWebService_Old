using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models.Client
{
    public class NotesModel
    {
        public int idOpombaStranka { get; set; }
        public Nullable<int> idStranka { get; set; }
        public string Opomba { get; set; }
        public DateTime ts { get; set; }
        public int tsIDOsebe { get; set; }

        public string Stranka { get; set; }
    }
}