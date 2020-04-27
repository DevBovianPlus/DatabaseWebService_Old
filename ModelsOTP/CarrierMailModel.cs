using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP
{
    public class CarrierMailModel
    {
        public string CarrierName { get; set;}
        public string Pozdrav { get; set; }
        public string BodyText { get; set; }
        public string AdditionalText { get; set; }
        public string CustomCarrierURL { get; set; }
        public string Email { get; set; }
        public string SubjectText { get; set; }
        public string ZaVprasanja { get; set; }
        public string Podpis1 { get; set; }
        public string Podpis2 { get; set; }
        public string GumbZaPrijavo { get; set; }
    }
}