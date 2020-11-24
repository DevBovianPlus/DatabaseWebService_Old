using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsOTP.Route;
using DatabaseWebService.ModelsOTP.Tender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models
{
    public class hlpTenderCreateExcellData
    {

        public TenderFullModel _TenderModel { get; set; }
        public List<TransporterSimpleModel> TransporterList { get; set; }

    }
}