using DatabaseWebService.ModelsOTP.Route;
using DatabaseWebService.ModelsOTP.Tender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models
{
    public class hlpTenderTransporterSelection
    {        
        public bool CheapestTransporterTender { get; set; }

        public string ZipFilePath { get; set; }

        public List<object> SelectedRowsRoutes { get; set; }
        public List<object> SelectedRowsCarriers { get; set; }
        public List<object> SelectedRowsTons { get; set; }
        
        public hlpTenderCreateExcellData tTenderCreateExcellData { get; set; }

        public List<TenderPositionModel> RazpisPozicija { get; set; }
    }
}