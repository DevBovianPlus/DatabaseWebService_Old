using DatabaseWebService.Domain;
using DatabaseWebService.Models.Client;
using DatabaseWebService.Models.EmailMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Models.Event
{
    public class EventFullModel
    {
        public int idDogodek { get; set; }
        public Nullable<int> idStranka { get; set; }
        public Nullable<int> idKategorija { get; set; }
        public Nullable<int> Skrbnik { get; set; }
        public Nullable<int> Izvajalec { get; set; }
        public Nullable<int> idStatus { get; set; }
        public string Opis { get; set; }
        public DateTime DatumOtvoritve { get; set; }
        public DateTime Rok { get; set; }
        public string DatumZadZaprtja { get; set; }
        public DateTime ts { get; set; }
        public int tsIDOsebe { get; set; }
        public string VneselOseba { get; set; }
        public string Tip { get; set; }
        public Nullable<DateTime> RokIzvedbe { get; set; }

        public CategorieModel Kategorija { get; set; }
        public EmployeeFullModel OsebeIzvajalec { get; set; }
        public EmployeeFullModel OsebeSkrbnik { get; set; }
        public EventStatusModel StatusDogodek { get; set; }
        public ClientSimpleModel Stranka { get; set; }
        public List<MessageModel> Sporocila { get; set; }

        public List<EventMeetingModel> SestanekDokumenti { get; set; }

        public EmailMessageModel emailModel { get; set; }

        public string Priloge { get; set; }
    }
}