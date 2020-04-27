using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Tender
{
    public class TransportCountModel
    {
        public int PrevoznikID { get; set; }
        public string NazivPrevoznik { get; set; }
        public int RelacijaID { get; set; }
        public string RelacijaNaziv { get; set; }
        public int StPotrjenihOdpoklicev { get; set; }
        public int StVsehOdpoklicevZaRelacijo { get; set; }
    }
}