using DatabaseWebService.Common;
using DatabaseWebService.Common.EmailTemplates;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.ModelsOTP;
using DatabaseWebService.ModelsOTP.Client;
using DatabaseWebService.ModelsOTP.Recall;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using static DatabaseWebService.Common.Enums.Enums;

namespace DatabaseWebService.DomainOTP.Concrete
{
    public class SystemMessageEventsRepository_OTP : ISystemMessageEventsRepository_OTP
    {
        GrafolitOTPEntities context;

        public SystemMessageEventsRepository_OTP(GrafolitOTPEntities _context)
        {
            context = _context;
        }

        public List<SystemMessageEvents_OTP> GetUnProcessedMesseges()
        {
            return context.SystemMessageEvents_OTP.Where(sme => sme.Status.Value == (int)Enums.SystemMessageEventStatus.UnProcessed).ToList();
        }

        public void CreateEmailForLeaderToApproveRecall(RecallFullModel model)
        {
            StreamReader reader = null;
            try
            {
                //DataTypesHelper.LogThis("*****IN Method ProcessEventMessage*****");

                string emailSubject = SystemEmailMessageResource.res_05;
                string templatePath = (AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["RECALL_APPROVAL_MESSAGE"].ToString()).Replace("\"", "\\");
                DataTypesHelper.LogThis(AppDomain.CurrentDomain.BaseDirectory);

                RecallApprovalEmailModel message = new RecallApprovalEmailModel();
                message.Comments = model.OdobritevKomentar;
                message.OdpoklicID = model.OdpoklicStevilka.ToString();
                Osebe_OTP employee = context.Osebe_OTP.Where(o => o.idOsebe == model.tsIDOseba).FirstOrDefault();
                if (employee != null)
                {
                    message.FirstName = employee.Ime;
                    message.Lastname = employee.Priimek;

                    OsebeNadrejeni_OTP supervisor = context.OsebeNadrejeni_OTP.Where(on => on.idOseba == employee.idOsebe).FirstOrDefault();
                    if (supervisor != null)
                    {
                        message.FirstNameSupervisor = supervisor.Osebe_OTP1.Ime;
                        message.LastnameSupervisor = supervisor.Osebe_OTP1.Priimek;
                        message.Email = supervisor.Osebe_OTP1.Email;

                        message.ServerTagOTP = ConfigurationManager.AppSettings["ServerTagOTP"].ToString();

                        reader = new StreamReader(templatePath);
                        string templateString = reader.ReadToEnd();
                        templateString = ReplaceDefaultValuesInTemplate(message, templateString);

                        SaveToSystemEmailMessage(message.Email, templateString, null, 1, emailSubject);
                    }
                    else
                        throw new Exception("Zaposlen " + employee.Ime + " " + employee.Priimek + "nima dodeljenega nadrejenega!");
                }
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message);
            }
        }

        public void CreateEmailForRecallStatusChanged(RecallFullModel model)
        {
            StreamReader reader = null;
            try
            {
                //DataTypesHelper.LogThis("*****IN Method ProcessEventMessage*****");

                string emailSubject = SystemEmailMessageResource.res_06;
                string templatePath = (AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["RECALL_STATUS_CHANGED_MESSAGE"].ToString()).Replace("\"", "\\");
                DataTypesHelper.LogThis(AppDomain.CurrentDomain.BaseDirectory);

                RecallApprovalEmailModel message = new RecallApprovalEmailModel();

                message.OdpoklicID = model.OdpoklicStevilka.ToString();
                Osebe_OTP employee = context.Osebe_OTP.Where(o => o.idOsebe == model.tsIDOseba).FirstOrDefault();
                if (employee != null)
                {
                    reader = new StreamReader(templatePath);
                    string templateString = reader.ReadToEnd();

                    message.FirstName = employee.Ime;
                    message.Lastname = employee.Priimek;
                    message.Email = employee.Email;
                    message.RecallStatus = context.StatusOdpoklica.Where(so => so.StatusOdpoklicaID == model.StatusID).FirstOrDefault().Naziv;
                    message.ServerTagOTP = ConfigurationManager.AppSettings["ServerTagOTP"].ToString();

                    templateString = ReplaceDefaultValuesInTemplate(message, templateString);

                    SaveToSystemEmailMessage(message.Email, templateString, null, 1, emailSubject);

                    //če obstaja nadrejeni še pošljemo mail njemu.
                    OsebeNadrejeni_OTP supervisor = context.OsebeNadrejeni_OTP.Where(on => on.idOseba == employee.idOsebe).FirstOrDefault();
                    if (supervisor != null)
                    {
                        message.FirstName = supervisor.Osebe_OTP1.Ime;
                        message.Lastname = supervisor.Osebe_OTP1.Priimek;
                        message.Email = supervisor.Osebe_OTP1.Email;

                        templateString = ReplaceDefaultValuesInTemplate(message, templateString);

                        SaveToSystemEmailMessage(message.Email, templateString, null, 1, emailSubject);
                    }
                    else
                        throw new Exception("Zaposlen " + employee.Ime + " " + employee.Priimek + "nima dodeljenega nadrejenega!");
                }
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message);
            }
        }

        public void SaveEmailEventMessag_OTP(Models.EmailMessage.EmailMessageModel model)
        {
            try
            {
                SystemMessageEvents_OTP sem = new SystemMessageEvents_OTP();
                sem.SystemMessageEventID = model.ID;
                sem.MasterID = model.MasterID;
                sem.Code = model.Code;
                sem.Status = model.Status;
                sem.ts = DateTime.Now;
                sem.tsIDOsebe = model.tsIDOsebe;

                context.SystemMessageEvents_OTP.Add(sem);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
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

        private void SaveToSystemEmailMessage(string emailTo, string bodyMessage, int? userID, int hierarchyLevel = 1, string emailSubject = "Novo sporočilo", string attachments = "", EmployeeFullModel inquirySubmittedByEmployee = null)
        {
            try
            {
                SystemEmailMessage_OTP emailConstruct = null;

                for (int i = 0; i < hierarchyLevel; i++)
                {
                    emailConstruct = new SystemEmailMessage_OTP();
                    //DataTypesHelper.LogThis("*****in for loop SaveToSystemEmailMessage*****");
                    emailConstruct.EmailFrom = inquirySubmittedByEmployee != null ? inquirySubmittedByEmployee.Email : ConfigurationManager.AppSettings["EmailFromForDB"].ToString();
                    //emailConstruct.EmailFrom = ConfigurationManager.AppSettings["EmailFromForDB"].ToString();
                    emailConstruct.EmailSubject = emailSubject;
                    emailConstruct.EmailBody = bodyMessage;
                    emailConstruct.Status = (int)Enums.SystemEmailMessageStatus.UnProcessed;
                    emailConstruct.ts = DateTime.Now;
                    emailConstruct.tsIDOsebe = userID.HasValue ? userID.Value : 0;

                    if (i == 0)
                        emailConstruct.EmailTo = emailTo;
                    else if (i == 1)
                    {
                        emailConstruct.EmailTo = context.OsebeNadrejeni_OTP.Where(on => on.idOseba == userID.Value).FirstOrDefault().Osebe_OTP1.Email;//Nadrejeni;
                        emailConstruct.EmailSubject += " - Sporočilo za nadrejenega";
                    }
                    else if (i == 2)
                    {
                        int ceo = Convert.ToInt32(ConfigurationManager.AppSettings["CEOId"].ToString());
                        emailConstruct.EmailTo = context.Osebe_OTP.Where(o => o.idOsebe == ceo).FirstOrDefault().Email;//direktor;
                        emailConstruct.EmailSubject += " - Sporočilo za nadrejenega";
                    }

                    if (attachments.Length > 0)
                    {
                        emailConstruct.Attachments = attachments;
                    }

                    context.SystemEmailMessage_OTP.Add(emailConstruct);
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Method SaveToSystemEmailMessage ERROR: " + ex);
            }
        }

        public void CreateEmailForCarriers(RecallFullModel recall, EmployeeFullModel inquirySubmittedByEmployee)
        {
            StreamReader reader = null;
            try
            {
                //DataTypesHelper.LogThis("*****IN Method ProcessEventMessage*****");


                string templatePath = (AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["CARRIER_MAIL"].ToString()).Replace("\"", "\\");
                string rootURL = ConfigurationManager.AppSettings["ServerTagCarrierPage"].ToString();
                DataTypesHelper.LogThis(AppDomain.CurrentDomain.BaseDirectory);
                string[] split = recall.Prevozniki.Split(';');

                List<Stranka_OTP> carriers = context.Stranka_OTP.Where(s => split.Any(sp => sp.Contains(s.NazivPrvi))).ToList();
                Relacija route = context.Relacija.Where(r => r.RelacijaID == recall.RelacijaID).FirstOrDefault();

                CarrierMailModel message = null;

                if (carriers != null)
                {
                    foreach (var item in carriers)
                    {
                        string langStr = (item.JezikID > 0) ? item.Jeziki.Koda : Language.SLO.ToString();

                        if (!String.IsNullOrEmpty(item.Email))//TODO: kaj pa če ima stranka vpisanih več mail-ov
                        {
                            int idPrijavaPrevoznika = SavePrijavaPrevoznika(item.idStranka, recall);//shranimo zapise za vse prevoznike v tabelo PrijavaPrevoznika

                            if (recall.OpombaZaPovprasevnjePrevoznikom == null) recall.OpombaZaPovprasevnjePrevoznikom = "";

                            string id = idPrijavaPrevoznika.ToString() + ";" +
                                recall.CenaPrevoza.ToString("N2") + ";" +
                                recall.RelacijaID.ToString() + ";" +
                                route.Naziv.Trim() + ";" +
                                recall.DatumNaklada.Value.ToString() + ";" +
                                recall.OpombaZaPovprasevnjePrevoznikom.Trim() + ";" +
                                item.NazivPrvi + ";" +
                                recall.OdpoklicStevilka;

                            Language enMLanguage = (Language)(Enum.Parse(typeof(Language), langStr));

                            message = new CarrierMailModel();


                            message.SubjectText = TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.CARRIRERMAIL_SUBJECT);
                            message.Pozdrav = TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.POZDRAV);
                            message.BodyText = TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.CARRIRERMAIL_BODY) + recall.OdpoklicStevilka.ToString();
                            message.AdditionalText = TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.CARRIRERMAIL_ADDTEXT);
                            message.ZaVprasanja = TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.ZA_VPRASANJA);
                            message.Podpis1 = TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.PODPIS1);
                            message.Podpis2 = TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.PODPIS2);
                            message.GumbZaPrijavo = TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.CARRIRERMAIL_REPORTPRICE);

                            message.CarrierName = item.NazivPrvi;
                            message.Email = item.Email;
                            message.CustomCarrierURL = rootURL + "?id=" + DataTypesHelper.Base64Encode(id) + "&lang=" + (item.Jeziki != null ? item.Jeziki.KodaJezik : "sl-SI");//TODO:Generiraj hash z naslednjimi podatki: OdpoklicID, Cena, RelacijaID, RelacijaNaziv, OpombaZaPovprasevnjePrevoznikom, NazivPrevoznika


                            reader = new StreamReader(templatePath);
                            string templateString = reader.ReadToEnd();
                            templateString = ReplaceDefaultValuesInTemplate(message, templateString);

                            SaveToSystemEmailMessage(message.Email, templateString, null, 1, message.SubjectText,"", inquirySubmittedByEmployee);
                        }
                        else
                        {
                            throw new Exception("Stranka " + item.NazivPrvi + "nima vpisanega elektrnoskega naslova!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message);
            }
        }

        public void CreateEmailForAdmin_NoPDFForOrderOTP(string sOdobritevKomentar, string sStevilkaDokumenta, string sStevilkaNarocilnice)
        {
            StreamReader reader = null;
            try
            {
                //DataTypesHelper.LogThis("*****IN Method ProcessEventMessage*****");

                string emailSubject = SystemEmailMessageResource.res_21;
                string templatePathRez = (AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["ADMIN_EMAIL_RECALL"].ToString()).Replace("\"", "\\");
                string templatePath = (ConfigurationManager.AppSettings["ADMIN_EMAIL_RECALL"].ToString()).Replace("\"", "\\");

                //DataTypesHelper.LogThis(AppDomain.CurrentDomain.BaseDirectory);
                RecallApprovalEmailModel message = new RecallApprovalEmailModel();

                templatePath = File.Exists(templatePath) ? templatePath : templatePathRez;

                reader = new StreamReader(templatePath);
                string templateString = reader.ReadToEnd();

                message.Comments = sOdobritevKomentar;
                message.OdpoklicID = sStevilkaDokumenta;
                // return all user with admin role
                List<Osebe_OTP> employeesList = context.Osebe_OTP.Where(o => o.idVloga == 2).ToList();
                if (employeesList != null)
                {
                    foreach (var employee in employeesList)
                    {
                        message.FirstName = employee.Ime;
                        message.Lastname = employee.Priimek;
                        message.Email = employee.Email;

                        message.ServerTagOTP = ConfigurationManager.AppSettings["ServerTagOTP"].ToString();
                        message.Comments = "Kriranje PDF dokumenta v PANTHEONU že 5x ni vrnilo PDF Številka naročilnice: " + sStevilkaNarocilnice;
                        message.Comments += "<br> Preverite zakaj je tako!";

                        message.Comments += "<br> <b>ODPOKLIC</b> ";


                        templateString = ReplaceDefaultValuesInTemplate(message, templateString);




                        SaveToSystemEmailMessage(message.Email, templateString, null, 1, emailSubject);
                    }
                }
                if (employeesList.Count == 0)
                {
                    DataTypesHelper.LogThis("V podatkovni bazi ni osebe z vlogo ADMIN (ID=2)");
                }
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message);
            }
        }

        public void CreateEmailForCarrierOrder(RecallFullModel recall)
        {
            StreamReader reader = null;
            try
            {
                DataTypesHelper.LogThis("*****IN Method CreateEmailForCarrierOrder*****");

                string emailSubject = SystemEmailMessageResource.res_18;
                string templatePath = (AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["CARRIER_MAIL_ORDER"].ToString()).Replace("\"", "\\");
                DataTypesHelper.LogThis("1");
                string rootURL = ConfigurationManager.AppSettings["ServerTagCarrierPage"].ToString();
                DataTypesHelper.LogThis(AppDomain.CurrentDomain.BaseDirectory);
                DataTypesHelper.LogThis("2");
                var Prevoznik = context.Stranka_OTP.Where(s => s.idStranka == recall.DobaviteljID).FirstOrDefault();

                CarrierMailModel message = null;

                if (Prevoznik != null)
                {
                    string langStr = (Prevoznik.JezikID > 0) ? Prevoznik.Jeziki.Koda : Language.SLO.ToString();

                    if (!String.IsNullOrEmpty(Prevoznik.Email))//TODO: kaj pa če ima stranka vpisanih več mail-ov
                    {
                        DataTypesHelper.LogThis("Send order to email: " + Prevoznik.Email);
                        Language enMLanguage = (Language)(Enum.Parse(typeof(Language), langStr));

                        message = new CarrierMailModel();

                        message.CarrierName = Prevoznik.NazivPrvi;
                        message.Email = Prevoznik.Email;

                        message.SubjectText = TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.CARRIRERMAILORDER_SUBJECT);
                        message.Pozdrav = TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.POZDRAV);
                        message.BodyText = TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.CARRIRERMAILORDER_BODY);
                        message.AdditionalText = TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.CARRIRERMAILORDER_ADDTEXT) + "<b>" + recall.Relacija.Naziv + "</b>";
                        message.ZaVprasanja = TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.ZA_VPRASANJA);
                        message.Podpis1 = TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.PODPIS1);
                        message.Podpis2 = TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.PODPIS2);



                        reader = new StreamReader(templatePath);
                        string templateString = reader.ReadToEnd();
                        templateString = ReplaceDefaultValuesInTemplate(message, templateString);

                        SaveToSystemEmailMessage(message.Email, templateString, null, 1, message.SubjectText, recall.P_TransportOrderPDFDocPath);
                    }
                    else
                    {
                        throw new Exception("Stranka " + Prevoznik.NazivPrvi + "nima vpisanega elektrnoskega naslova!");
                    }

                }
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message);
            }
        }

        public void CreateEmailForCarriers(List<CarrierInquiryModel> carriers)
        {
            StreamReader reader = null;
            try
            {
                //DataTypesHelper.LogThis("*****IN Method ProcessEventMessage*****");

                string emailSubject = SystemEmailMessageResource.res_07;
                string templatePath = (AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["CARRIER_MAIL"].ToString()).Replace("\"", "\\");
                string rootURL = ConfigurationManager.AppSettings["ServerTagCarrierPage"].ToString();
                DataTypesHelper.LogThis(AppDomain.CurrentDomain.BaseDirectory);

                CarrierMailModel message = null;

                if (carriers != null)
                {
                    foreach (var item in carriers)
                    {
                        if (!String.IsNullOrEmpty(item.Prevoznik.Email))//TODO: kaj pa če ima stranka vpisanih več mail-ov
                        {
                            var idPrijavaPrevoznika = context.PrijavaPrevoznika.Where(pp => pp.OdpoklicID == item.OdpoklicID && pp.PrevoznikID == item.PrevoznikID).FirstOrDefault();//shranimo zapise za vse prevoznike v tabelo PrijavaPrevoznika
                            if (idPrijavaPrevoznika != null)
                            {
                                string id = idPrijavaPrevoznika.PrijavaPrevoznikaID.ToString() + ";" +
                                    idPrijavaPrevoznika.PrvotnaCena.ToString("N2") + ";" +
                                    idPrijavaPrevoznika.Odpoklic.RelacijaID.ToString() + ";" +
                                    idPrijavaPrevoznika.Odpoklic.Relacija.Naziv.Trim() + ";" +
                                    idPrijavaPrevoznika.DatumNaklada.ToString();

                                message = new CarrierMailModel();
                                message.BodyText = SystemEmailMessageResource.res_16;
                                message.AdditionalText = SystemEmailMessageResource.res_17;
                                message.CarrierName = item.Prevoznik.NazivPrvi;
                                message.Email = item.Prevoznik.Email;
                                message.CustomCarrierURL = rootURL + "?id=" + DataTypesHelper.Base64Encode(id);//TODO:Generiraj hash z naslednjimi podatki: OdpoklicID, Cena, RelacijaID, RelacijaNaziv,


                                reader = new StreamReader(templatePath);
                                string templateString = reader.ReadToEnd();
                                templateString = ReplaceDefaultValuesInTemplate(message, templateString);

                                SaveToSystemEmailMessage(message.Email, templateString, null, 1, emailSubject);
                            }
                        }
                        else
                        {
                            throw new Exception("Stranka " + item.Prevoznik.NazivPrvi + "nima vpisanega elektrnoskega naslova!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message);
            }
        }

        public void CreateEmailCarrierSelectedOrNot(Odpoklic recall, PrijavaPrevoznika carrierSelected, bool selectedCarrier = true)
        {
            StreamReader reader = null;

            try
            {
                string emailSubject = selectedCarrier ? SystemEmailMessageResource.res_08 : SystemEmailMessageResource.res_11;
                string templatePath = (AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["CARRIER_CONGRATS_OR_BETTER_LUCK_NEXT_TIME_MAIL"].ToString()).Replace("\"", "\\");
                DataTypesHelper.LogThis(AppDomain.CurrentDomain.BaseDirectory);

                CarrierMailModel message = new CarrierMailModel();

                string langStr = (carrierSelected.Stranka_OTP.JezikID > 0) ? carrierSelected.Stranka_OTP.Jeziki.Koda : Language.SLO.ToString();

                Language enMLanguage = (Language)(Enum.Parse(typeof(Language), langStr));

                message.Email = carrierSelected.Stranka_OTP.Email;
                message.CarrierName = carrierSelected.Stranka_OTP.NazivPrvi;
                message.SubjectText = selectedCarrier ? (TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.CARRIRERCONGRATS_OR_BETTERLUCK_SUBJECT_SELECT)) : TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.CARRIRERCONGRATS_OR_BETTERLUCK_SUBJECT_REJECT);



                message.Pozdrav = TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.POZDRAV);
                message.ZaVprasanja = TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.ZA_VPRASANJA);
                message.Podpis1 = TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.PODPIS1);
                message.Podpis2 = TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.PODPIS2);

                message.BodyText = (selectedCarrier ? TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.CARRIRERCONGRATS_OR_BETTERLUCK_BODY_SELECT) : TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.CARRIRERCONGRATS_OR_BETTERLUCK_SUBJECT_REJECT));

                if (selectedCarrier)
                {
                    message.BodyText += "<strong>" + (carrierSelected.DatumPrijave.HasValue ? carrierSelected.DatumPrijave.Value.ToShortDateString() : "") + "</strong>" +
                        " za relacijo <strong>" +
                        recall.Relacija.Naziv +
                        "</strong>. " + TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.CARRIRERCONGRATS_DATUMNAKLADA) + "  <strong>" +
                        recall.DatumNaklada.Value.ToShortDateString() + "</strong>";
                }

                message.AdditionalText = (selectedCarrier) ? TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.CARRIRERCONGRATS_OR_BETTERLUCK_ADDTEXT_SELECT) : TranslationHelper.GetTranslateValueByContentAndLanguage(enMLanguage, EmailContentType.CARRIRERCONGRATS_OR_BETTERLUCK_ADDTEXT_REJECT);




                reader = new StreamReader(templatePath);
                string templateString = reader.ReadToEnd();
                templateString = ReplaceDefaultValuesInTemplate(message, templateString);

                SaveToSystemEmailMessage(message.Email, templateString, null, 1, message.SubjectText);
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message);
            }
        }

        //public void CreateEmailForAdmin_NoPDFForOrderOTP(string sOdobritevKomentar, string sStevilkaDokumenta, string sStevilkaNarocilnice)
        //{
        //    StreamReader reader = null;
        //    try
        //    {
        //        //DataTypesHelper.LogThis("*****IN Method ProcessEventMessage*****");

        //        string emailSubject = SystemEmailMessageResource.res_21;
        //        string templatePath = (AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["ADMIN_EMAIL_RECALL"].ToString()).Replace("\"", "\\");

        //        DataTypesHelper.LogThis(AppDomain.CurrentDomain.BaseDirectory);
        //        RecallApprovalEmailModel message = new RecallApprovalEmailModel();

        //        reader = new StreamReader(templatePath);
        //        string templateString = reader.ReadToEnd();

        //        message.Comments = sOdobritevKomentar;
        //        message.OdpoklicID = sStevilkaDokumenta;
        //        Osebe_OTP employee = context.Osebe_OTP.Where(o => o.idOsebe == 1).FirstOrDefault();
        //        if (employee != null)
        //        {
        //            message.FirstName = employee.Ime;
        //            message.Lastname = employee.Priimek;
        //            message.Email = employee.Email;

        //            message.ServerTagOTP = ConfigurationManager.AppSettings["ServerTagOTP"].ToString();
        //            message.Comments = "Kriranje PDF dokumenta v PANTHEONU že 5x ni vrnilo PDF Številka naročilnice: " + sStevilkaNarocilnice;
        //            message.Comments += "<br> Preverite zakaj je tako!";

        //            message.Comments += "<br> <b>ODPOKLIC</b> ";


        //            templateString = ReplaceDefaultValuesInTemplate(message, templateString);




        //            SaveToSystemEmailMessage(message.Email, templateString, null, 1, emailSubject);


        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DataTypesHelper.LogThis(ex.Message);
        //    }
        //}

        public void CreateEmailLogisticsCarrierSelected(Odpoklic recall, PrijavaPrevoznika carrierSelected)
        {
            StreamReader reader = null;

            try
            {
                string emailSubject = SystemEmailMessageResource.res_12;
                string templatePath = (AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["LOGISTICS_MAIL"].ToString()).Replace("\"", "\\");
                string rootURL = ConfigurationManager.AppSettings["ServerTagOTP"].ToString();
                DataTypesHelper.LogThis(AppDomain.CurrentDomain.BaseDirectory);

                string logistics = Enums.UserRole.Logistics.ToString();
                List<Osebe_OTP> logsiticsEmployee = context.Osebe_OTP.Where(o => o.Vloga_OTP.Koda == logistics).ToList();

                reader = new StreamReader(templatePath);
                string templateString = reader.ReadToEnd();

                foreach (var item in logsiticsEmployee)
                {
                    if (!String.IsNullOrEmpty(item.Email))
                    {
                        LogisticsMailModel message = new LogisticsMailModel();
                        message.CustomCarrierURL = rootURL;
                        message.Email = item.Email;
                        message.EmployeeName = item.Ime + " " + item.Priimek;
                        message.BodyText = SystemEmailMessageResource.res_14 +
                            "<strong>" + carrierSelected.Stranka_OTP.NazivPrvi + "</strong>" +
                            " za relacijo <strong>" +
                            recall.Relacija.Naziv + "</strong>, na odpoklicu št.: <strong>" +
                            recall.OdpoklicStevilka.ToString() + "</strong>" +
                            ". Datum naklada: <strong>" +
                            recall.DatumNaklada.Value.ToShortDateString() + "</strong>";
                        message.LogisticsPartialMail = "";

                        templateString = ReplaceDefaultValuesInTemplate(message, templateString);

                        SaveToSystemEmailMessage(message.Email, templateString, null, 1, emailSubject);
                    }
                }
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message);
            }
        }

        public void CreateEmailLogisticsCarrierNotSelected(List<IGrouping<Odpoklic, PrijavaPrevoznika>> listOdpoklicPrijavaPrevoznikov)
        {
            StreamReader reader = null;
            if (listOdpoklicPrijavaPrevoznikov == null || listOdpoklicPrijavaPrevoznikov.Count <= 0) return;

            try
            {
                string emailSubject = SystemEmailMessageResource.res_13;
                string templatePath = (AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["LOGISTICS_MAIL"].ToString()).Replace("\"", "\\");
                string templatePathPartial = (AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["LOGISTICS_PARTIAL_MAIL"].ToString()).Replace("\"", "\\");
                string rootURL = ConfigurationManager.AppSettings["ServerTagOTP"].ToString();
                DataTypesHelper.LogThis(AppDomain.CurrentDomain.BaseDirectory);

                string logistics = Enums.UserRole.Logistics.ToString();
                List<Osebe_OTP> logsiticsEmployee = context.Osebe_OTP.Where(o => o.Vloga_OTP.Koda == logistics).ToList();

                reader = new StreamReader(templatePath);
                string templateString = reader.ReadToEnd();

                reader = new StreamReader(templatePathPartial);
                string templatePartialString = reader.ReadToEnd();
                string logisticsPartialMail = "";
                bool messageGenerated = false;

                LogisticsMailModel message = new LogisticsMailModel();

                //seznam zaposlenih v oddelku logistika
                foreach (var item in logsiticsEmployee)
                {
                    if (!String.IsNullOrEmpty(item.Email))//če obstaja mail pri zaposlenemu
                    {
                        message.Email = item.Email;
                        message.EmployeeName = item.Ime + " " + item.Priimek;
                        message.CustomCarrierURL = rootURL;
                        //za vse zaposlene v logistiki, ki se jim bo poslal mail bo vsebina enaka samo naslovnik drugi, 
                        //zato imamo ta if, da ne rabimo za vsakega zaposlenega na novo generirat šablono
                        if (!messageGenerated)
                        {
                            message.BodyText = SystemEmailMessageResource.res_15;

                            //sestavimo html za odpoklic in vseh prijavljenih prevoznikov
                            foreach (var obj in listOdpoklicPrijavaPrevoznikov)
                            {
                                LogisticsMailPartialModel partialMessage = new LogisticsMailPartialModel();
                                partialMessage.RecallNum = obj.Key.OdpoklicStevilka.ToString();
                                partialMessage.RouteName = obj.Key.Relacija.Naziv;
                                partialMessage.OriginalPrice = obj.Key.CenaPrevoza.Value.ToString("N2");
                                partialMessage.RecallURL = rootURL + "/Pages/Recall/RecallForm.aspx?action=2&recordId=" + obj.Key.OdpoklicID;

                                //sestavimo html iz vseh prijavljenih prevoznikov, sortirano padajoče (od najvišje cene do najnižje)
                                var ppList = obj.OrderByDescending(pp => pp.PrijavljenaCena).ToList();
                                foreach (var pp in ppList)
                                {
                                    partialMessage.PrijavaPrevoznikaHTML += "<tr>" +
                                        "<td><strong>" + pp.Stranka_OTP.NazivPrvi + "</strong></td>" +
                                        "<td><strong>" + (pp.PrijavljenaCena.HasValue ? pp.PrijavljenaCena.Value.ToString("N2") : "0,00") + "</strong></td>" +
                                        "<td><strong>" + (pp.PrijavljenaCena.HasValue ? (pp.PrvotnaCena - pp.PrijavljenaCena.Value).ToString("N2") : "0,00") + "</strong></td>" +
                                        "</tr>";
                                }

                                logisticsPartialMail += ReplaceDefaultValuesInTemplate(partialMessage, templatePartialString);
                            }

                            message.LogisticsPartialMail = logisticsPartialMail;

                            templateString = ReplaceDefaultValuesInTemplate(message, templateString);
                        }

                        SaveToSystemEmailMessage(message.Email, templateString, null, 1, emailSubject);
                    }
                }
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message);
            }
        }

        private int SavePrijavaPrevoznika(int carrierID, RecallFullModel model)
        {
            PrijavaPrevoznika item = new PrijavaPrevoznika();
            item.PrijavaPrevoznikaID = 0;
            item.DatumNaklada = model.DatumNaklada.Value;
            item.DatumPosiljanjePrijav = DateTime.Now;
            item.DatumPrijave = (DateTime?)null;
            item.OdpoklicID = model.OdpoklicID;
            item.PrevoznikID = carrierID;
            item.PrijavljenaCena = (decimal?)null;
            item.PrvotnaCena = model.CenaPrevoza;
            item.ts = DateTime.Now;

            context.PrijavaPrevoznika.Add(item);
            context.SaveChanges();

            return item.PrijavaPrevoznikaID;
        }
    }
}