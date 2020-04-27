using DatabaseWebService.Domain;
using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DatabaseWebService.Models.EmailMessage;
using DatabaseWebService.Models.Event;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DatabaseWebService.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            ContactPersonModel model = new ContactPersonModel();
            model.DelovnoMesto = "sdfsdf";
            model.Email ="dsfsdf@wfd.com";
            model.Fax = "031123456";
            model.GSM = "123456789";
            model.idStranka = 1;
            model.NazivKontaktneOsebe = "test2";
            model.Opombe = "sdčlksjdčglkjsdgds";
            model.Telefon = "987654321";
            model.ts = DateTime.Now;
            model.tsIDOsebe = 7;
            model.ZaporednaStevika = 2;
            /*EmailMessageModel model = new EmailMessageModel();
            model.tsIDOsebe = 1;
            model.ID = 180;
            model.ts = DateTime.Now;
            model.Status = 0;
            model.MasterID = 57;
            model.Code = "EVENT_DOGODEK";
              PlanModel model = new PlanModel();
            model.idPlan = 0;
            model.idKategorija = 2;
            model.IDStranka = 1;
            model.Kategorija = "";
            model.LetniZnesek = 1050;
            model.Leto = 2017;
            model.Stranka = "";
            model.tsIDOsebe = 1;
            model.ts = DateTime.Now;*/

            /*EventFullModel newEvent = new EventFullModel();
            newEvent.idDogodek = 0;
            newEvent.idStranka = 1;
            newEvent.idKategorija = 1;
            newEvent.Skrbnik = 1;
            newEvent.Izvajalec = 1;
            newEvent.idStatus = 1;
            newEvent.Opis = "";
            newEvent.DatumOtvoritve = DateTime.Now;
            newEvent.Rok = DateTime.Now;
            newEvent.DatumZadZaprtja = "";
            newEvent.ts = DateTime.Now;
            newEvent.tsIDOsebe = 1;
            newEvent.emailModel = model;
           /*ClientFullModel model = new ClientFullModel();
            model.idStranka = 0;
            model.KodaStranke = "str";
            model.NazivPrvi = "test1";
            model.NazivDrugi = "test2";
            model.Naslov = "test3";
            model.StevPoste = "test4";
            model.NazivPoste = "test1";
            model.Email = "test1";
            model.Telefon = "test1";
            model.FAX = "test1";
            model.InternetniNalov = "test1";
            model.KontaktnaOseba = "test1";
            model.RokPlacila = "tes";
            model.StrankaZaposleni = new List<ClientEmployeeModel>();
            /*EmployeeFullModel model = new EmployeeFullModel();
            model.idOsebe = 0;
            model.idVloga = 3;
            model.Ime = "Stanko";
            model.Priimek = "Janko";
            model.Naslov = "Stanetova 23";
            model.TelefonGSM = "03124568";
            model.ts = DateTime.Now;
            model.tsIDOsebe = 1;
            model.UporabniskoIme = "StankoJ";
            model.Zunanji = 0;
            model.Geslo = "JankoS";
            model.Email = "janko@gmail.com";
            model.DelovnoMesto = "Komercialist";
            model.DatumZaposlitve = DateTime.Now;
            model.DatumRojstva = DateTime.Now;*/
            /*model.StrankaZaposleni = new List<ClientEmployeeModel>() { new ClientEmployeeModel() { idOsebe = 1, idStranka = 1, ts = DateTime.Now, tsIDOsebe = 1 } };*/
           /* WebResponseContentModel<ContactPersonModel> returnModel = new WebResponseContentModel<ContactPersonModel>();
            returnModel.Content = model;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://192.168.20.12/DatabaseWebService/api/client/SaveContactPersonToClient");
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            using (var sw = new StreamWriter(request.GetRequestStream()))
            {
                string clientData = JsonConvert.SerializeObject(returnModel);
                sw.Write(clientData);
                sw.Flush();
                sw.Close();
            }
            WebResponse respone = null;
            respone = (HttpWebResponse)request.GetResponse();
            Stream stream = respone.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string test = reader.ReadToEnd();*/

            return View();
        }
    }
}
