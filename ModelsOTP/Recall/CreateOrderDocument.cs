using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Recall
{
    public class CreateOrderDocument
    {
        public string PDFFile { get; set; }
        public string ExportPath { get; set; }
        public string ErrorDesc { get; set; }

        public string OrderXML { get; set; }
        public string InvoicesXML { get; set; }
        public string OrderNumber { get; set; }
    }
}