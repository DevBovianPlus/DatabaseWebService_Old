using DatabaseWebService.Common;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.Domain.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DatabaseWebService.Models.EmailMessage;
using DatabaseWebService.Models.Event;
using DatabaseWebService.Models.ServiceHelpersModels;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace DatabaseWebService.Domain.Concrete
{
    public class SystemMessageEventsRepository : ISystemMessageEventsRepository
    {
        AnalizaProdajeEntities context = new AnalizaProdajeEntities();
        IEventRepository eventRepo;

        public SystemMessageEventsRepository(IEventRepository iEventRepo)
        {
            eventRepo = iEventRepo;
        }

        public List<SystemMessageEvents> GetUnProcessedMesseges()
        {
            DataTypesHelper.LogThis("*****IN GetUnProcessedMesseges*****");
            return context.SystemMessageEvents.Where(sme => sme.Status.Value == (int)Enums.SystemMessageEventStatus.UnProcessed).ToList();
        }

        public void ProcessNewMessage(SystemMessageEvents message)
        {
            string body = "";
            string templatePath = (AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["NEW_MESSAGE_TEMPLATE"].ToString()).Replace("\"", "\\");
            StreamReader reader = new StreamReader(templatePath);
            try
            {
                string templateString = reader.ReadToEnd();
                templateString = templateString.Replace("$#", body);

                SystemEmailMessage emailConstruct = new SystemEmailMessage();

                emailConstruct.EmailFrom = ConfigurationManager.AppSettings["EmailFrom"].ToString();
                emailConstruct.EmailTo = "polegekmartin@gmail.com";
                emailConstruct.EmailSubject = SystemEmailMessageResource.res_01;
                emailConstruct.EmailBody = templateString;
                emailConstruct.Status = (int)Enums.SystemEmailMessageStatus.UnProcessed;
                emailConstruct.ts = DateTime.Now;
                emailConstruct.tsIDOsebe = message.tsIDOsebe;
                context.SystemEmailMessage.Add(emailConstruct);
                context.SaveChanges();

                //update status
                message.Status = (int)Enums.SystemMessageEventStatus.Processed;
                var original = context.SystemMessageEvents.Where(s => s.SystemMessageEventID == message.SystemMessageEventID).FirstOrDefault();
                context.Entry(original).CurrentValues.SetValues(message);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                reader.Close();
            }
        }


        public void SaveEmailEventMessage(EmailMessageModel model)
        {
            try
            {
                SystemMessageEvents sem = new SystemMessageEvents();
                sem.SystemMessageEventID = model.ID;
                sem.MasterID = model.MasterID;
                sem.Code = model.Code;
                sem.Status = model.Status;
                sem.ts = DateTime.Now;
                sem.tsIDOsebe = model.tsIDOsebe;

                context.SystemMessageEvents.Add(sem);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public void GetUnProcessedRecordsAvtomatika()
        {
            try
            {
                ProcessEventsToAvtomatika();
                List<Avtomatika> list = context.Avtomatika.Where(a => a.Status == (int)Enums.SystemMessageEventStatus.UnProcessed).ToList();
                var groupedList = list.GroupBy(ga => ga.OsebaID).ToList();
                if (list != null)
                {
                    foreach (var item in groupedList)
                    {
                        SystemMessageEvents newEvent = new SystemMessageEvents();
                        newEvent.SystemMessageEventID = 0;
                        newEvent.Status = (int)Enums.SystemMessageEventStatus.UnProcessed;
                        newEvent.MasterID = item.Key;
                        newEvent.Code = Enums.SystemMessageEventCodes.AUTO.ToString();
                        newEvent.ts = DateTime.Now;
                        //newEvent.tsIDOsebe = 1;
                        context.SystemMessageEvents.Add(newEvent);
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        private void ProcessEventsToAvtomatika()
        {
            try
            {
                //poiščemo takšne dogodke ki so v tabeli Dogodki in jih ne najdemo v SystemMessageEvents tabeli
                //int tip = (int)Enums.SystemMessageEventStatus.UnProcessed;
                var query = from dogodek in context.Dogodek
                            where dogodek.Tip.Equals("Avtomatika") && !context.Avtomatika.Any(a => (a.DogodekID == dogodek.idDogodek))
                            select new EventFullModel
                            {
                                idDogodek = dogodek.idDogodek,
                                tsIDOsebe = dogodek.tsIDOsebe.HasValue ? dogodek.tsIDOsebe.Value : 0,
                                DatumOtvoritve = dogodek.DatumOtvoritve.Value,
                                Izvajalec = dogodek.Osebe.idOsebe,
                                Opis = dogodek.Opis
                            };
                foreach (var item in query.ToList())
                {
                    DataTypesHelper.LogThis("*****In foreach statement*****");
                    DataTypesHelper.LogThis("*****Dogodek: *****" + "ID: " + item.idDogodek.ToString() + " Oseba: " + item.Izvajalec.Value.ToString());

                    Avtomatika newAutoEvent = new Avtomatika();
                    newAutoEvent.AvtomatikaID = 0;
                    newAutoEvent.Status = (int)Enums.SystemMessageEventStatus.UnProcessed;
                    newAutoEvent.OsebaID = item.Izvajalec.HasValue ? item.Izvajalec.Value : 0;
                    newAutoEvent.DogodekID = item.idDogodek;
                    newAutoEvent.StopnjaNadrejenega = 1;
                    newAutoEvent.ts = DateTime.Now;
                    newAutoEvent.Opis = item.Opis;
                    context.Avtomatika.Add(newAutoEvent);
                }
                context.SaveChanges();
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();
                //DataTypesHelper.LogThis("*****ReflectionTypeLoadException!  - ***** " + errorMessage);
                //Display or log the error based on your application.
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis("*****Exception!  - ***** " + ValidationExceptionError.res_08 + " " + ex.Message);
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public void ProcessAutoMessage(SystemMessageEvents message)
        {
            DataTypesHelper.LogThis("ProcessAutoMessage Method - 1 : BEFORE Open templatePath");
            string templatePath = (AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["EMPLOYEE_MESSAGE_TEMPLATE"].ToString()).Replace("\"", "\\");
            StreamReader reader = new StreamReader(templatePath);
            DataTypesHelper.LogThis("ProcessAutoMessage Method - 2 : AFTER Open templatePath");
            try
            {
                DataTypesHelper.LogThis("ProcessAutoMessage Method - 3 : BEFORE get avtomatikaList");
                List<Avtomatika> avtomatikaList = context.Avtomatika.Where(a => a.OsebaID == message.MasterID && a.Status == (int)Enums.SystemMessageEventStatus.UnProcessed).ToList();
                DataTypesHelper.LogThis("ProcessAutoMessage Method - 4 : After get avtomatikaList");
                DataTypesHelper.LogThis("ProcessAutoMessage Method - 5 : BEFORE get Get Oseba by message.MasterID");
                Osebe oseba = context.Osebe.Where(o => o.idOsebe == message.MasterID).FirstOrDefault();
                //DataTypesHelper.LogThis("After getting avtomatika values from DB - " + avtomatikaList.Count.ToString());
                DataTypesHelper.LogThis("BEFORE reading template string");
                string templateString = reader.ReadToEnd();
                DataTypesHelper.LogThis("AFTER reading template string.");
                //DataTypesHelper.LogThis("Before if! AvtomatikaList count =" + avtomatikaList.Count.ToString() + "  - Oseba :" + (oseba != null).ToString() + " - OsebaID: " + message.MasterID.ToString());
                if (avtomatikaList.Count > 0 && oseba != null)
                {
                    //DataTypesHelper.LogThis("*********Before ReplaceDefaultValuesInTemplate**********");
                    templateString = ReplaceDefaultValuesInTemplate(oseba, templateString);
                    DataTypesHelper.LogThis("BEFORE ConstructTemplate for clients.");
                    int index = templateString.IndexOf("##INSERT_CLIENTS_HERE##");
                    //DataTypesHelper.LogThis("*********Before ConstructTemplate**********");
                    templateString = templateString.Insert(index, ConstructTemplate(avtomatikaList, templateString));
                    DataTypesHelper.LogThis("AFTER ConstructTemplate for clients.");
                    templateString = templateString.Replace("##INSERT_CLIENTS_HERE##", "");
                    //DataTypesHelper.LogThis("*****After  ConstructTemplate method IN ProcessAutoMessage***** - user email : " + avtomatikaList[0].Osebe.Email + "Stopnja nadrejenega : " + avtomatikaList[0].StopnjaNadrejenega.ToString());
                    DataTypesHelper.LogThis("BEFORE Saving item to SystemEmailMessage table.");
                    SaveToSystemEmailMessage(oseba.Email, templateString, message.tsIDOsebe, avtomatikaList[0].StopnjaNadrejenega, SystemEmailMessageResource.res_02);
                    DataTypesHelper.LogThis("AFTER Saving item to SystemEmailMessage table.");
                }

                int count = 0;
                DataTypesHelper.LogThis("BEFORE update avtomatikaList.");
                //update Avtomatika table status
                foreach (var item in avtomatikaList)
                {
                    //var original = context.Avtomatika.Where(av => av.AvtomatikaID == item.AvtomatikaID).FirstOrDefault();
                    // if (original != null)
                    // {
                    context.Entry(item).Entity.Status = (int)Enums.SystemMessageEventStatus.Processed;
                    count++;
                    DataTypesHelper.LogThis("Trenutni count : " + count);
                    // }
                }
                DataTypesHelper.LogThis("Avtomatika / Count :" + count.ToString() + "\r\n" + "AvtomatikaList count : " + avtomatikaList.Count.ToString());

                context.SaveChanges();

                DataTypesHelper.LogThis("BEFORE update UpdateSystemMessageEvents status.");
                //update status
                message.Status = (int)Enums.SystemMessageEventStatus.Processed;
                UpdateSystemMessageEvents(message);
                DataTypesHelper.LogThis("AFTER update UpdateSystemMessageEvents status.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
            finally
            {
                reader.Close();
            }
        }

        public void ProcessEventMessage(SystemMessageEvents message, Enums.SystemMessageEventCodes eventCode = Enums.SystemMessageEventCodes.EVENT_DOGODEK)
        {
            StreamReader reader = null;
            try
            {
                //DataTypesHelper.LogThis("*****IN Method ProcessEventMessage*****");
                int employeeHierarchyLevel = GetEmployeeHierarchyLevel(eventCode);
                string emailSubject = SystemEmailMessageResource.res_03;
                string templatePath = (AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["NEW_EVENT_TEMPLATE"].ToString()).Replace("\"", "\\");
                DataTypesHelper.LogThis(AppDomain.CurrentDomain.BaseDirectory);
                Dogodek dogodek = context.Dogodek.Where(dog => dog.idDogodek == message.MasterID).FirstOrDefault();
                EventMessageTemplateModel modelForTemplate = new EventMessageTemplateModel();

                if (EventPreparationOrReport(eventCode))
                {
                    templatePath = (AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["MEETING_WARNING_TEMPLATE"].ToString()).Replace("\"", "\\");
                    modelForTemplate.Tip = EventPreparation(eventCode) ? "Priprava" : "Poročilo";
                    modelForTemplate.Zamuda = DateTime.Now.Subtract(dogodek.DatumOtvoritve.Value).Days;
                    modelForTemplate.Otvoritev = dogodek.DatumOtvoritve.Value;
                    modelForTemplate.ServerTag = ConfigurationManager.AppSettings["ServerTag"].ToString();
                    emailSubject = SystemEmailMessageResource.res_04.Replace("%Tip%", modelForTemplate.Tip);
                }
                //DataTypesHelper.LogThis("*****Afer if statement of checking if event is about priprava or Poročilo***** EVENT ID: " + dogodek.idDogodek.ToString());
                //DataTypesHelper.LogThis("*****TemplatePath: ********" + templatePath);
                reader = new StreamReader(templatePath);
                //DataTypesHelper.LogThis("***1***");

                modelForTemplate.idDogodek = dogodek.idDogodek;
                //DataTypesHelper.LogThis("***2***");
                modelForTemplate.ImeIzvajalec = dogodek.Osebe != null ? dogodek.Osebe.Ime : "";
                //DataTypesHelper.LogThis("***3***");
                modelForTemplate.PriimekIzvajalec = dogodek.Osebe != null ? dogodek.Osebe.Priimek : "";
                //DataTypesHelper.LogThis("***4***");
                modelForTemplate.ImeSkrbnik = dogodek.Osebe1 != null ? dogodek.Osebe1.Ime : "";
                //DataTypesHelper.LogThis("***5***");
                modelForTemplate.PriimekSkrbnik = dogodek.Osebe1 != null ? dogodek.Osebe1.Priimek : "";
                //DataTypesHelper.LogThis("***6***");
                modelForTemplate.Kategorija = dogodek.Kategorija != null ? dogodek.Kategorija.Naziv : "";
                //DataTypesHelper.LogThis("***7***");
                modelForTemplate.KategorijaKoda = dogodek.Kategorija != null ? dogodek.Kategorija.Koda : "";
                //DataTypesHelper.LogThis("***8***");
                modelForTemplate.NazivPrvi = dogodek.Stranka != null ? dogodek.Stranka.NazivPrvi : "";
                //DataTypesHelper.LogThis("***9***");
                modelForTemplate.Opis = dogodek.Opis;
                //DataTypesHelper.LogThis("***10***");
                modelForTemplate.EmailTo = dogodek.Osebe != null ? dogodek.Osebe.Email : "";
                //DataTypesHelper.LogThis("***11***");
                modelForTemplate.Rok = dogodek.Rok.HasValue ? dogodek.Rok.Value : DateTime.MinValue;
                //DataTypesHelper.LogThis("***12***");
                modelForTemplate.Status = dogodek.StatusDogodek != null ? dogodek.StatusDogodek.Naziv : "";
                //DataTypesHelper.LogThis("***13***");
                string templateString = reader.ReadToEnd();
                //DataTypesHelper.LogThis("*****Before ReplaceDefaultValuesInTemplate*****");
                templateString = ReplaceDefaultValuesInTemplate(modelForTemplate, templateString);
                //DataTypesHelper.LogThis("*****After ReplaceDefaultValuesInTemplate*****");

                if (String.IsNullOrEmpty(modelForTemplate.Opis))
                    templateString = templateString.Replace("$%Opis%$ ", "");

                //DataTypesHelper.LogThis("*****Before if statement Sporocila*****");
                if (dogodek.Sporocila != null && eventCode.Equals(Enums.SystemMessageEventCodes.EVENT_DOGODEK))
                {
                    foreach (var item in dogodek.Sporocila)
                    {
                        int index = templateString.IndexOf("##INSERT_MESSAGE_HERE##");
                        templateString = templateString.Insert(index, item.OpisDel + "<br />");
                    }
                }
                //DataTypesHelper.LogThis("*****After if statement Sporocila*****");
                templateString = templateString.Replace("##INSERT_MESSAGE_HERE##", "");
                if (dogodek.Osebe != null)
                    SaveToSystemEmailMessage(dogodek.Osebe.Email, templateString, dogodek.Osebe.idOsebe, employeeHierarchyLevel, emailSubject);
                //DataTypesHelper.LogThis("*****After SaveSystemEmailMessage*****");
                //TODO: Update Avtomatika tabela
                //update status
                message.Status = (int)Enums.SystemMessageEventStatus.Processed;
                //DataTypesHelper.LogThis("*****Before UpdateSysteMessageEvents*****");
                UpdateSystemMessageEvents(message);
                //DataTypesHelper.LogThis("*****After UpdateSysteMessageEvents*****");

            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis("Error:" + ex.Message);
                throw new Exception(ex.Message);
            }
            finally
            {
                reader.Close();
            }
        }

        public List<AutomaticsModel> GetAutomaticsByEmployeeIDAndStatus(int employeeID, int status)
        {
            try
            {
                var query = from automatics in context.Avtomatika
                            where automatics.OsebaID == employeeID && automatics.Status == (int)Enums.SystemMessageEventStatus.UnProcessed
                            select new AutomaticsModel
                            {
                                AvtomatikaID = automatics.AvtomatikaID,
                                //KategorijaID = automatics.KategorijaID,
                                OsebaID = automatics.OsebaID,
                                //Kategorija = automatics.Kategorija.Naziv,
                                Osebe = automatics.Osebe.Ime + " " + automatics.Osebe.Priimek,
                                Opis = automatics.Opis,
                                Status = automatics.Status,
                                StopnjaNadrejenega = automatics.StopnjaNadrejenega,
                                //Stranka = automatics.Stranka.NazivPrvi,
                                //StrankaID = automatics.StrankaID,
                                ts = automatics.ts
                            };
                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void SaveToSystemEmailMessage(string emailTo, string bodyMessage, int? userID, int hierarchyLevel = 1, string emailSubject = "Novo sporočilo")
        {
            try
            {
                SystemEmailMessage emailConstruct = null;

                for (int i = 0; i < hierarchyLevel; i++)
                {
                    emailConstruct = new SystemEmailMessage();
                    //DataTypesHelper.LogThis("*****in for loop SaveToSystemEmailMessage*****");
                    emailConstruct.EmailFrom = ConfigurationManager.AppSettings["EmailFromForDB"].ToString();
                    emailConstruct.EmailSubject = emailSubject;
                    emailConstruct.EmailBody = bodyMessage;
                    emailConstruct.Status = (int)Enums.SystemEmailMessageStatus.UnProcessed;
                    emailConstruct.ts = DateTime.Now;
                    emailConstruct.tsIDOsebe = userID.HasValue ? userID.Value : 0;

                    if (i == 0)
                        emailConstruct.EmailTo = emailTo;
                    else if (i == 1)
                    {
                        emailConstruct.EmailTo = context.OsebeNadrejeni.Where(on => on.idOseba == userID.Value).FirstOrDefault().Osebe1.Email;//Nadrejeni;
                        emailConstruct.EmailSubject += " - Sporočilo za nadrejenega"; 
                    }
                    else if (i == 2)
                    {
                        int ceo = Convert.ToInt32(ConfigurationManager.AppSettings["CEOId"].ToString());
                        emailConstruct.EmailTo = context.Osebe.Where(o => o.idOsebe == ceo).FirstOrDefault().Email;//direktor;
                        emailConstruct.EmailSubject += " - Sporočilo za nadrejenega"; 
                    }

                    context.SystemEmailMessage.Add(emailConstruct);
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Method SaveToSystemEmailMessage ERROR: " + ex);
            }
        }

        public void UpdateSystemMessageEvents(SystemMessageEvents message)
        {
            try
            {
                DataTypesHelper.LogThis("UPDATING SystemMessageEvents status.");
                var original = context.SystemMessageEvents.Where(s => s.SystemMessageEventID == message.SystemMessageEventID).FirstOrDefault();
                context.Entry(original).CurrentValues.SetValues(message);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string ReplaceDefaultValuesInTemplate(Object o, string template)
        {
            string result = "";
            string value = template;
            Type type = o.GetType();
            object[] indexArgs = { 0 };

            PropertyInfo[] myFields = type.GetProperties(BindingFlags.Public
                | BindingFlags.Instance);

            for (int i = 0; i < myFields.Length; i++)
            {
                try
                {
                    value = value.Replace("$%" + myFields[i].Name + "%$", myFields[i].GetValue(o, null) == null ? "" : myFields[i].GetValue(o, null).ToString());
                }
                catch (Exception ex)
                {
                    DataTypesHelper.LogThis(ex.Message);
                }
            }

            result = value;
            return result;
        }

        private string ConstructTemplate(List<Avtomatika> list, string parentTemplate)
        {
            try
            {
                string templatePath = (AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["EMPLOYEE_PARTIAL_TEMPLATE"].ToString()).Replace("\"", "\\");
                StreamReader reader = new StreamReader(templatePath);
                string clientPartialTemplate = reader.ReadToEnd();
                reader.Close();

                clientPartialTemplate = clientPartialTemplate.Replace("$%ServerTag%$", ConfigurationManager.AppSettings["ServerTag"].ToString());

                string concatenateTemplate = "";

                var groupByClient = list.GroupBy(ag => ag.Dogodek.idStranka).ToList();
                //DataTypesHelper.LogThis("*****ConstructTemplate before first foreach*****");
                foreach (var item in groupByClient)//Number of clients
                {
                    //DataTypesHelper.LogThis("*****ConstructTemplate IN first foreach*****" + " clientID : " + item.Key.Value.ToString() + " Dogodek : " + item.Select(o => o.Dogodek).FirstOrDefault().idDogodek);

                    Stranka stranka = item.Select(o => o.Dogodek).FirstOrDefault().Stranka;

                    if (stranka == null) break;//če stranka ne obstaja v bazi zaključimo for zanko in vrenmo template

                    concatenateTemplate += ReplaceDefaultValuesInTemplate(stranka, clientPartialTemplate);
                    int itemCount = 1;
                    //DataTypesHelper.LogThis("*****ConstructTemplate before second foreach - number of clients: *****" + groupByClient.Count.ToString());
                    foreach (var avtomatika in item.ToList())
                    {
                        if (avtomatika.Dogodek.Kategorija != null)//če so določene posamezne kategorije na katerih je bil zaznan padec prodaje
                        {
                            //DataTypesHelper.LogThis("*****ConstructTemplate IN second foreach***** EVENT ID" + avtomatika.DogodekID.ToString() + " : items: " + item.ToList().Count.ToString() + " clientID : " + item.Key.Value.ToString() + " employeeID : " + avtomatika.OsebaID.ToString());
                            int index = concatenateTemplate.IndexOf("##INSERT_CATEGORIES_HERE##");
                            //DataTypesHelper.LogThis("*****1 - indexOF method***** : " + index.ToString());
                            if (avtomatika.Equals(item.ToList().Last()) && (itemCount % 2 != 0 || item.ToList().Count == 1))
                            {
                                //DataTypesHelper.LogThis("*****2 - Dogodek Values: ***** : " + avtomatika.Dogodek.idDogodek.ToString() + " - " + avtomatika.Dogodek.Kategorija.Naziv);

                                concatenateTemplate = concatenateTemplate.Insert(index, "<div class='catFullWidth'>" + avtomatika.Dogodek.Kategorija.Naziv + "<br />" + " <strong>PADEC</strong></div>");
                                break;
                            }
                            //DataTypesHelper.LogThis("*****3 - after if***** : ");

                            if (itemCount % 2 == 0)
                                concatenateTemplate = concatenateTemplate.Insert(index, "<div class='catOneHalfRight'>" + avtomatika.Dogodek.Kategorija.Naziv + "<br />" + " <strong>PADEC</strong></div>");
                            else
                                concatenateTemplate = concatenateTemplate.Insert(index, "<div class='catOneHalfLeft'>" + avtomatika.Dogodek.Kategorija.Naziv + "<br />" + " <strong>PADEC</strong></div>");
                            //DataTypesHelper.LogThis("*****4 - before itemCount++***** : " + itemCount.ToString());
                        }
                        itemCount++;
                    }
                    //DataTypesHelper.LogThis("*****ConstructTemplate AFTER first foreach*****");
                    concatenateTemplate = concatenateTemplate.Replace("##INSERT_CATEGORIES_HERE##", "");
                }

                return concatenateTemplate;
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis("*****Error - ***** : " + ex.Message + " " + ex.InnerException.Message + " " + ex.StackTrace);
                throw new Exception("", ex);
            }
        }

        public void GetEventsWithNoReportForMeeting()
        {
            try
            {

                DataTypesHelper.LogThis("*****IN GetEventsWithNoReportForMeeting*****");
                int reportDeadline = int.Parse(ConfigurationManager.AppSettings["DeadlineForReportInDays"].ToString());
                DataTypesHelper.LogThis("*****AFTER ConfigurationManager.AppSettings 1 *********");
                int notifyBeforeDeadline = reportDeadline - int.Parse(ConfigurationManager.AppSettings["NotificationBeforeReportDeadlineInDays"].ToString());
                DataTypesHelper.LogThis("*****AFTER ConfigurationManager.AppSettings 1 *********");
                string tipDogodka = Enums.EventMeetingType.POROCILO.ToString();
                var query = from dogodek in context.Dogodek
                            where !context.DogodekSestanek.Any(ds => (ds.DogodekID == dogodek.idDogodek) && (ds.Tip.Equals(tipDogodka)))
                            select new EventFullModel
                            {
                                idDogodek = dogodek.idDogodek,
                                tsIDOsebe = dogodek.tsIDOsebe.HasValue ? dogodek.tsIDOsebe.Value : 0,
                                DatumOtvoritve = dogodek.DatumOtvoritve.Value
                            };
                DataTypesHelper.LogThis("*****IN GetEventsWithNoReportForMeeting count: *****" + query.ToList().Count.ToString());
                foreach (var item in query.ToList())
                {
                    DateTime dateNotifyBeforeDeadline = item.DatumOtvoritve.AddDays(notifyBeforeDeadline);// datum ki je 7 dni pred rokom 
                    DateTime dateDeadline = item.DatumOtvoritve.AddDays(reportDeadline);//datum ki je rok poteka
                    if (DateTime.Now.CompareTo(dateNotifyBeforeDeadline) >= 0 && DateTime.Now.CompareTo(dateDeadline) <= 0)//če je trenutni datum med zgraj navedenima datuma
                        CreateSystemMessageEventsForEvent(item, Enums.SystemMessageEventCodes.EVENT_POROCILO_OPOZORILO);
                    else if (DateTime.Now.CompareTo(dateDeadline) > 0 && DateTime.Now.CompareTo(dateDeadline.AddDays(7)) <= 0)// če na dan roka še ni poročila se od datuma roka +7 dni pošilja mail zaposlenemu in nadrejenemu
                        CreateSystemMessageEventsForEvent(item, Enums.SystemMessageEventCodes.EVENT_POROCILO_OPOZORILO_NADREJENI);
                    else if (DateTime.Now.CompareTo(dateDeadline.AddDays(7)) > 0 && DateTime.Now.CompareTo(dateDeadline.AddDays(14)) <= 0)//če po 7 dneh od datuma roka še vedno ni poročila se pošlje mail še direktorju - je potrebno omejit zgornji datum? (v tem primeru je zgornji datum 14 dni od datuma zapadlosti dogodka)
                        CreateSystemMessageEventsForEvent(item, Enums.SystemMessageEventCodes.EVENT_POROCILO_OPOZORILO_NADREJENI_DIREKTOR);
                }
                context.SaveChanges();
                DataTypesHelper.LogThis("*****IN GetEventsWithNoReportForMeeting AFTER SAVING: *****");
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis("*****IN GetEventsWithNoReportForMeeting ERROR: *****" + ex.Message);
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public void GetEventsWithNoPreparationForMeeting()
        {
            try
            {
                int delaysend = int.Parse(ConfigurationManager.AppSettings["DelaySendInDays"].ToString());
                int sendDurationDays = int.Parse(ConfigurationManager.AppSettings["SendDurationInDays"].ToString()) + delaysend;
                int deadline = int.Parse(ConfigurationManager.AppSettings["DeadlineForReportInDays"].ToString());

                var q = from dogodek in context.Dogodek
                        join dSestanek in context.DogodekSestanek
                            on dogodek.idDogodek equals dSestanek.DogodekID into loj
                        from ds in loj.DefaultIfEmpty()
                        where ds == null
                        select new EventFullModel
                        {
                            idDogodek = dogodek.idDogodek,
                            tsIDOsebe = dogodek.tsIDOsebe.HasValue ? dogodek.tsIDOsebe.Value : 0,
                            DatumOtvoritve = dogodek.DatumOtvoritve.Value
                        };

                foreach (var item in q.ToList())
                {
                    if (!item.DatumOtvoritve.Equals(DateTime.MinValue))
                    {
                        if (item.DatumOtvoritve.AddDays(delaysend).CompareTo(DateTime.Now) <= 0 &&//Če je datum otvoritve prej kot današnji datum  
                            item.DatumOtvoritve.AddDays(sendDurationDays).CompareTo(DateTime.Now) >= 0)//Če je trenutni datum znotraj datuma otvoritve - 7 dni(dodali smo 8 dni zaradi tega,ker v sredinskem pogoju prvi dan preskočimo
                            CreateSystemMessageEventsForEvent(item, Enums.SystemMessageEventCodes.EVENT_PRIPRAVA_OPOZORILO);
                        else if (item.DatumOtvoritve.AddDays(sendDurationDays).CompareTo(DateTime.Now) < 0 &&//če je časovni okvir za pošiljanje komercialstu že poteklo je potrebno poslati njemu in nadrajenemu
                            item.DatumOtvoritve.AddDays(((sendDurationDays * 2) - 2)).CompareTo(DateTime.Now) >= 0)//časovni okvir med 7 in 14 dnevom od datuma otvoritve (sendDurationDays pomnožimo da dobimo 14 dan v msesecu)
                            CreateSystemMessageEventsForEvent(item, Enums.SystemMessageEventCodes.EVENT_PRIPRAVA_OPOZORILO_NADREJENI);
                        else if (item.DatumOtvoritve.AddDays(((sendDurationDays * 2) - 2)).CompareTo(DateTime.Now) < 0 && //če je časovni okvir za pošiljanje komercialistu in nadrejenemu že poteklo je potrebno poslati še direktorju
                            item.DatumOtvoritve.AddDays(deadline).CompareTo(DateTime.Now) >= 0)//če je časovni okvir med 14 dnem in rokom zapadlosti - je potrebno omejit zgornji datum? (v tem primeru je zgornji datum datum zapadlosti dogodka)
                            CreateSystemMessageEventsForEvent(item, Enums.SystemMessageEventCodes.EVENT_PRIPRAVA_OPOZORILO_NADREJENI_DIREKTOR);

                    }
                }
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        private void CreateSystemMessageEventsForEvent(EventFullModel dogodek, Enums.SystemMessageEventCodes eventType)
        {
            DataTypesHelper.LogThis("*****CREATING NEW SYSTEMMESSGE EVENT: event type = *****" + eventType.ToString());
            SystemMessageEvents newEvent = new SystemMessageEvents();
            newEvent.SystemMessageEventID = 0;
            newEvent.Status = (int)Enums.SystemMessageEventStatus.UnProcessed;
            newEvent.MasterID = dogodek.idDogodek;
            newEvent.Code = eventType.ToString();
            newEvent.ts = DateTime.Now;

            if (dogodek.tsIDOsebe != 0) newEvent.tsIDOsebe = dogodek.tsIDOsebe;

            context.SystemMessageEvents.Add(newEvent);
        }

        public bool EventPreparationOrReport(Enums.SystemMessageEventCodes eventCode)
        {
            return (eventCode == Enums.SystemMessageEventCodes.EVENT_PRIPRAVA_OPOZORILO ||
                eventCode == Enums.SystemMessageEventCodes.EVENT_PRIPRAVA_OPOZORILO_NADREJENI ||
                eventCode == Enums.SystemMessageEventCodes.EVENT_PRIPRAVA_OPOZORILO_NADREJENI_DIREKTOR ||
                eventCode == Enums.SystemMessageEventCodes.EVENT_POROCILO_OPOZORILO ||
                eventCode == Enums.SystemMessageEventCodes.EVENT_POROCILO_OPOZORILO_NADREJENI ||
                eventCode == Enums.SystemMessageEventCodes.EVENT_POROCILO_OPOZORILO_NADREJENI_DIREKTOR);
        }

        public bool EventPreparationOrReport(string eventCode)
        {
            return (eventCode == Enums.SystemMessageEventCodes.EVENT_PRIPRAVA_OPOZORILO.ToString() ||
                eventCode == Enums.SystemMessageEventCodes.EVENT_PRIPRAVA_OPOZORILO_NADREJENI.ToString() ||
                eventCode == Enums.SystemMessageEventCodes.EVENT_PRIPRAVA_OPOZORILO_NADREJENI_DIREKTOR.ToString() ||
                eventCode == Enums.SystemMessageEventCodes.EVENT_POROCILO_OPOZORILO.ToString() ||
                eventCode == Enums.SystemMessageEventCodes.EVENT_POROCILO_OPOZORILO_NADREJENI.ToString() ||
                eventCode == Enums.SystemMessageEventCodes.EVENT_POROCILO_OPOZORILO_NADREJENI_DIREKTOR.ToString());
        }

        public bool EventPreparation(Enums.SystemMessageEventCodes eventCode)
        {
            return (eventCode == Enums.SystemMessageEventCodes.EVENT_PRIPRAVA_OPOZORILO ||
                eventCode == Enums.SystemMessageEventCodes.EVENT_PRIPRAVA_OPOZORILO_NADREJENI ||
                eventCode == Enums.SystemMessageEventCodes.EVENT_PRIPRAVA_OPOZORILO_NADREJENI_DIREKTOR);
        }

        public int GetEmployeeHierarchyLevel(Enums.SystemMessageEventCodes eventCode)
        {
            int level = 1;
            if (eventCode == Enums.SystemMessageEventCodes.EVENT_PRIPRAVA_OPOZORILO || eventCode == Enums.SystemMessageEventCodes.EVENT_POROCILO_OPOZORILO)
                level = 1;
            else if (eventCode == Enums.SystemMessageEventCodes.EVENT_PRIPRAVA_OPOZORILO_NADREJENI || eventCode == Enums.SystemMessageEventCodes.EVENT_POROCILO_OPOZORILO_NADREJENI)
                level = 2;
            else if (eventCode == Enums.SystemMessageEventCodes.EVENT_PRIPRAVA_OPOZORILO_NADREJENI_DIREKTOR || eventCode == Enums.SystemMessageEventCodes.EVENT_POROCILO_OPOZORILO_NADREJENI_DIREKTOR)
                level = 3;

            return level;
        }
    }
}