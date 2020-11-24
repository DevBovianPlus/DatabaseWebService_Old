using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsOTP.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Tender
{
    public class RouteSimpleModel
    {
        public int RouteID { get; set; }
        public string Naziv { get; set; }
        public int SteviloPrevozVLetuNaRelacijoPrevoznik { get; set; }
        public int SteviloPrevozVLetuNaRelacijoVsiPrevozniki { get; set; }
        public List<TonsModel> TonsList { get; set; }
    }
}