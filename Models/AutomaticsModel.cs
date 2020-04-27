using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models
{
    public class AutomaticsModel
    {
        public int AvtomatikaID { get; set; }
        public int OsebaID { get; set; }
        public int StrankaID { get; set; }
        public int KategorijaID { get; set; }
        public int Status { get; set; }
        public int StopnjaNadrejenega { get; set; }
        public string Opis { get; set; }
        public Nullable<System.DateTime> ts { get; set; }

        public string Kategorija { get; set; }
        public string Osebe { get; set; }
        public string Stranka { get; set; }
    }
}