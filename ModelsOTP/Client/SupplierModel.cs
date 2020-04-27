using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Client
{
    public class SupplierModel
    {
        public string Dobavitelj { get; set; }
        public string Naslov { get; set; }
        public string Posta { get; set; }
        public string Kraj { get; set; }

        //namenjeno za iskanje pozicij v tabeli LastnaZaloga
        public int StrankaSkladisceID { get; set; }
    }
}