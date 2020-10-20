using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsPDO.Inquiry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsPDO.Order
{
    public class OrderPDOFullModel
    {
        public int NarociloID { get; set; }
        public string NarociloStevilka_P { get; set; }
        public DateTime DatumDobave { get; set; }
         public string Opombe { get; set; }
        public DateTime ts { get; set; }
        public int tsIDOsebe { get; set; }
        public DateTime tsUpdate { get; set; }
        public int tsUpdateUserID { get; set; }
        public int? StatusID { get; set; }
        public string TypeCode { get; set; }
        public List<OrderPDOPositionModel> NarociloPozicija_PDO { get; set; }
        public ClientSimpleModel DobaviteljID { get; set; }
        public int StrankaDobaviteljID { get; set; }
        public InquiryStatus StatusModel { get; set; }

        public EmployeeFullModel NarociloIzdelal { get; set; }

        //uporabljamo samo ko se prvič kreira naročilo da naročilo povežemo z povpraševanjem
        public int PovprasevanjeID { get; set; }
        public int PovprasevanjeStatusID { get; set; }
        

        public DateTime P_CreateOrder { get; set; }
        public int P_UnsuccCountCreatePDFPantheon { get; set; }
        public DateTime P_LastTSCreatePDFPantheon { get; set; }
        public string P_TransportOrderPDFName { get; set; }
        public string P_TransportOrderPDFDocPath { get; set; }
        public DateTime P_GetPDFOrderFile { get; set; }
        public int P_SendWarningToAdmin { get; set; }

        public DepartmentModel Oddelek { get; set; }
        public int OddelekID { get; set; }
        public String OddelekNaziv { get; set; }
        public string PovprasevanjeStevilka { get; set; }
    }
}