using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsOTP.Client;
using DatabaseWebService.ModelsOTP.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.ModelsOTP.Recall
{
    public class RecallFullModel
    {
        public int OdpoklicID { get; set; }
        public int DobaviteljID { get; set; }
        public int RelacijaID { get; set; }
        public int StatusID { get; set; }
        public decimal CenaPrevoza { get; set; }
        public decimal KolicinaSkupno { get; set; }
        public string Opis { get; set; }
        public decimal PaleteSkupaj { get; set; }
        public string SoferNaziv { get; set; }
        public string Registracija { get; set; }
        public string OdobritevKomentar { get; set; }
        public int tsIDOseba { get; set; }
        public DateTime ts { get; set; }
        public int RazpisPozicijaID { get; set; }
        public int OdpoklicStevilka { get; set; }

        public string DobaviteljNaziv { get; set; }
        public string DobaviteljNaslov { get; set; }
        public string DobaviteljPosta { get; set; }
        public string DobaviteljKraj { get; set; }
        public Nullable<System.DateTime> DatumNaklada { get; set; }
        public Nullable<System.DateTime> DatumRazklada { get; set; }
        public bool DobaviteljUrediTransport { get; set; }
        public bool RecallStatusChanged { get; set; }
        public string RazlogOdobritveSistem { get; set; }
        public bool LastenPrevoz { get; set; }
        public ClientFullModel Dobavitelj { get; set; }
        public RouteModel Relacija { get; set; }
        public RecallStatus StatusOdpoklica { get; set; }
        public List<RecallPositionModel> OdpoklicPozicija { get; set; }

        public int UserID { get; set; }
        public EmployeeSimpleModel User { get; set; }

        public int TipPrevozaID { get; set; }
        public ClientTransportType TipPrevoza { get; set; }
        public int LastnoSkladisceID { get; set; }

        public string Prevozniki { get; set; }
        public bool KupecUrediTransport { get; set; }

        public bool PovprasevanjePoslanoPrevoznikom { get; set; }
        public bool PrevoznikOddalNajnizjoCeno { get; set; }
        public string OpombaZaPovprasevnjePrevoznikom { get; set; }

        public DateTime P_CreateOrder { get; set; }
        public int P_UnsuccCountCreatePDFPantheon { get; set; }
        public DateTime P_LastTSCreatePDFPantheon { get; set; }
        public string P_TransportOrderPDFName { get; set; }
        public string P_TransportOrderPDFDocPath { get; set; }
        public DateTime P_GetPDFOrderFile { get; set; }
        public int P_SendWarningToAdmin { get; set; }
    }
}