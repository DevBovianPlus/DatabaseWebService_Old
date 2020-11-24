using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsOTP.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Tender
{
    public class TransporterSimpleModel
    {
        public int ClientID { get; set; }
        public string Naziv { get; set; }
        public string ExcellFilePath { get; set; }

        public List<RouteSimpleModel> RouteList { get; set; }

    }
}