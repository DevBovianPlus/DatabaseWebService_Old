using DatabaseWebService.Common;
using DatabaseWebService.Common.EmailTemplates;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.DomainPDO.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsOTP.Recall;
using DatabaseWebService.ModelsPDO;
using DatabaseWebService.ModelsPDO.Inquiry;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using static DatabaseWebService.Common.Enums.Enums;

namespace DatabaseWebService.DomainPDO.Concrete
{
    public class SystemMessageEventsRepository_PDO : ISystemMessageEventsRepository_PDO
    {
        GrafolitPDOEntities context;

        public SystemMessageEventsRepository_PDO(GrafolitPDOEntities _context)
        {
            context = _context;
        }

        public List<SystemMessageEvents_PDO> GetUnProcessedMesseges()
        {
            return context.SystemMessageEvents_PDO.Where(sme => sme.Status.Value == (int)Enums.SystemMessageEventStatus.UnProcessed).ToList();
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

        private void SaveToSystemEmailMessage(string emailTo, string bodyMessage, int? userID, int hierarchyLevel = 1, string emailSubject = "Novo sporočilo", string attachments = "", EmployeeFullModel submitedInquiryByEmployeeID = null, string CCEmails = "")
        {
            try
            {
                SystemEmailMessage_PDO emailConstruct = null;

                for (int i = 0; i < hierarchyLevel; i++)
                {
                    emailConstruct = new SystemEmailMessage_PDO();
                    //DataTypesHelper.LogThis("*****in for loop SaveToSystemEmailMessage*****");
                    emailConstruct.EmailFrom = submitedInquiryByEmployeeID != null ? submitedInquiryByEmployeeID.Email : ConfigurationManager.AppSettings["EmailFromForDB"].ToString();
                    emailConstruct.EmailSubject = emailSubject;
                    emailConstruct.EmailBody = bodyMessage;
                    emailConstruct.Status = (int)Enums.SystemEmailMessageStatus.UnProcessed;
                    emailConstruct.ts = DateTime.Now;
                    emailConstruct.tsIDOsebe = userID.HasValue ? userID.Value : 0;
                    emailConstruct.CCEmails = CCEmails;

                    emailConstruct.OsebaEmailFromID = submitedInquiryByEmployeeID != null ? submitedInquiryByEmployeeID.idOsebe : (int?)null;

                    if (i == 0)
                        emailConstruct.EmailTo = emailTo;
                    else if (i == 1)
                    {
                        emailConstruct.EmailTo = context.OsebeNadrejeni_PDO.Where(on => on.OsebaID == userID.Value).FirstOrDefault().Osebe_PDO1.Email;//Nadrejeni;
                        emailConstruct.EmailSubject += " - Sporočilo za nadrejenega";
                    }
                    else if (i == 2)
                    {
                        int ceo = Convert.ToInt32(ConfigurationManager.AppSettings["CEOId"].ToString());
                        emailConstruct.EmailTo = context.Osebe_PDO.Where(o => o.OsebaID == ceo).FirstOrDefault().Email;//direktor;
                        emailConstruct.EmailSubject += " - Sporočilo za nadrejenega";
                    }

                    if (attachments != null)
                    {
                        if (attachments.Length > 0)
                        {
                            emailConstruct.Attachments = attachments;
                        }
                    }
                    context.SystemEmailMessage_PDO.Add(emailConstruct);
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Method SaveToSystemEmailMessage ERROR: " + ex);
            }
        }

        private void SetLanguageContentEmail(SupplierMailModel message, string sLangugeKoda)
        {
            switch (sLangugeKoda)
            {
                case "ANG":
                    message.SubjectText = "Grafo Lit - Enquiry for material";
                    break;
                case "SLO":
                    message.SubjectText = "Grafo Lit - Povpraševanje za material";
                    break;
                case "HRV":
                    message.SubjectText = "Grafo Lit - Upit za material";
                    break;
                default:
                    break;
            }
        }


        public void CreateEmailForSuppliers(List<GroupedInquiryPositionsBySupplier> suppliers, EmployeeFullModel inquirySubmittedByEmployee, string StevilkaPovprasevanja)
        {
            StreamReader reader = null;
            try
            {
                //DataTypesHelper.LogThis("*****IN Method ProcessEventMessage*****");

                string emailSubject = SystemEmailMessageResource.res_19;

                DataTypesHelper.LogThis(AppDomain.CurrentDomain.BaseDirectory);

                SupplierMailModel message = null;
                SupplierMailModel modelForEmployees = new SupplierMailModel();

                int inquiryPositionID = 0;

                if (suppliers != null)
                {
                    foreach (var item in suppliers)
                    {
                        string templatePath = (AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["SUPPLIER_MAIL"].ToString()).Replace("\"", "\\");


                        if (!String.IsNullOrEmpty(item.SelectedContactPersonsEmails))//TODO: kaj pa če ima stranka vpisanih več mail-ov
                        {

                            string langStr = (item.Supplier.JezikID > 0) ? item.Supplier.Jezik.Koda : Language.SLO.ToString();
                            Language enLanguage = (Language)(Enum.Parse(typeof(Language), langStr));

                            templatePath = templatePath.Replace("XXX", langStr);

                            message = new SupplierMailModel();


                            message.BodyText = item.EmailBody;
                            message.SupplierName = item.Supplier.NazivPrvi;
                            message.Email = item.Supplier.Email;
                            message.CCEmails = item.SelectedGrafolitPersonsEmails;
                            message.Signature = inquirySubmittedByEmployee.Podpis;
                            message.InquiryNumber = StevilkaPovprasevanja;

                            message.ThanksAndGreeting = TranslationHelper.GetTranslateValueByContentAndLanguage(enLanguage, EmailContentType.EMAILTOSUPPLIER_THANKANDGREETING);

                            if (Convert.ToBoolean(item.KupecViden))
                            {
                                message.ForCustomer = TranslationHelper.GetTranslateValueByContentAndLanguage(enLanguage, EmailContentType.EMAILTOSUPPLIER_FORCUSTOMER) + item.Buyer.NazivPrvi.ToString();
                            }


                            //Dodamo pozicije povpraševanja v html šablono
                            if (item.InquiryPositionsArtikel != null && item.InquiryPositionsArtikel.Count() > 0)
                            {
                                message.ListOfPositions = "<thead><tr><th style=\"text-align:left; border:1px solid;  padding: 10px;\">" + TranslationHelper.GetTranslateValueByContentAndLanguage(enLanguage, EmailContentType.EMAILTOSUPPLIER_MATERIAL) + "</th><th style=\"text-align:center; border:1px solid;  padding: 10px;\">" + TranslationHelper.GetTranslateValueByContentAndLanguage(enLanguage, EmailContentType.EMAILTOSUPPLIER_KOLICINA) + "</th><th style=\"text-align:center; border:1px solid;  padding: 10px;\">" + TranslationHelper.GetTranslateValueByContentAndLanguage(enLanguage, EmailContentType.EMAILTOSUPPLIER_OPOMBE) + "</th></tr></thead>";
                                foreach (var productPos in item.InquiryPositionsArtikel)
                                {
                                    string sKolicina = productPos.Kolicina1.ToString("N2") + " " + productPos.EnotaMere1 + ((productPos.EnotaMere2 != null && productPos.EnotaMere2.Length > 0) ? " (" + productPos.Kolicina2.ToString("N2") + " " + productPos.EnotaMere2 + ")" : "");
                                    string row = "<tr>";
                                    row += "<td style=\"text-align:left; border:1px solid;  padding: 10px;\">" + productPos.Naziv + "</td>";
                                    row += "<td style=\"text-align:center; border:1px solid;  padding: 10px;\">" + sKolicina + "</td>";
                                    row += "<td style=\"text-align:center; border:1px solid;  padding: 10px;\">" + productPos.OpombaNarocilnica + "</td>";
                                    row += "</tr>";

                                    message.ListOfPositions += row;
                                }
                            }

                            SetLanguageContentEmail(message, langStr);

                            message.SubjectText = "(" + StevilkaPovprasevanja + ") " + message.SubjectText;

                            modelForEmployees.ListOfSuppliers += "<li>" + message.SupplierName + "</li>";
                            modelForEmployees.Reports += item.ReportFilePath + ";";

                            reader = new StreamReader(templatePath);
                            string templateString = reader.ReadToEnd();
                            templateString = ReplaceDefaultValuesInTemplate(message, templateString);

                            SaveToSystemEmailMessage(item.SelectedContactPersonsEmails, templateString, null, 1, message.SubjectText, item.ReportFilePath, inquirySubmittedByEmployee, message.CCEmails);

                            if (item.InquiryPositionsArtikel.Count() > 0)
                                inquiryPositionID = item.InquiryPositionsArtikel.FirstOrDefault().PovprasevanjePozicijaID;
                        }



                        //else
                        //{
                        //    throw new Exception("Stranka " + item.Supplier.NazivPrvi + "nima vpisanega elektrnoskega naslova!");
                        //}
                    }

                    //Pošljemo še mail tistemu zaposlenemu ki je in poslal povpraševanje
                    modelForEmployees.InquiryDate = GetInquiryDate(inquiryPositionID);
                    modelForEmployees.Reports = modelForEmployees.Reports.Remove(modelForEmployees.Reports.LastIndexOf(";"));
                    CreateEmailForEmployeeForSendingInquiry(modelForEmployees, inquirySubmittedByEmployee, StevilkaPovprasevanja);
                }
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message);
            }
        }
        private void CreateEmailForEmployeeForSendingInquiry( SupplierMailModel model, EmployeeFullModel inquirySubmittedByEmployee, string StevilkaPovprasevanja)
        {
            StreamReader reader = null;
            try
            {


                string emailSubject = SystemEmailMessageResource.res_20;
                string templatePath = (AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["SUPPLIERS_INQUIRY_FOR_EMPLOYEES_MAIL"].ToString()).Replace("\"", "\\");
                DataTypesHelper.LogThis(AppDomain.CurrentDomain.BaseDirectory);


                if (!String.IsNullOrEmpty(inquirySubmittedByEmployee.Email))
                {
                    model.EmployeeName = inquirySubmittedByEmployee.Ime + " " + inquirySubmittedByEmployee.Priimek;
                    model.Email = inquirySubmittedByEmployee.Email;
                    model.InquiryNumber = StevilkaPovprasevanja;

                    reader = new StreamReader(templatePath);
                    string templateString = reader.ReadToEnd();
                    templateString = ReplaceDefaultValuesInTemplate(model, templateString);
                    emailSubject = "(" + StevilkaPovprasevanja + ") " + emailSubject;
                    SaveToSystemEmailMessage(model.Email, templateString, null, 1, emailSubject, model.Reports, inquirySubmittedByEmployee);
                }
                else
                {
                    throw new Exception("Zaposlen " + inquirySubmittedByEmployee.Ime + " " + inquirySubmittedByEmployee.Priimek + "nima vpisanega elektrnoskega naslova!");
                }
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message);
            }
        }

        private string GetAllPurchaseEmployees(ClientFullModel cfmGrafolit)
        {
            var listOfPurchaseGrafolitDeptEmployees = cfmGrafolit.KontaktneOsebe.Where(ko => ko.IsNabava == true).ToList();

            string sCC = "";

            foreach (var itm in listOfPurchaseGrafolitDeptEmployees)
            {
                if (itm.Email != null && itm.Email.Length > 0)
                {
                    if (listOfPurchaseGrafolitDeptEmployees.Count > 1)
                        sCC += itm.Email + ";";
                    else
                        sCC = itm.Email;

                }
            }
            if (listOfPurchaseGrafolitDeptEmployees.Count > 1)
            {
                sCC = sCC.Substring(0, sCC.Length - 1);
            }


            return sCC;
        }

        public void CreateEmailForGrafolitPurcaheDept(ClientFullModel cfmGrafolit, EmployeeFullModel InqueryEmployee, InquiryFullModel InqModel)
        {
            StreamReader reader = null;
            try
            {
                SupplierMailModel model = new SupplierMailModel();

                string emailSubject = SystemEmailMessageResource.res_24;
                string templatePath = (AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["GRAFOLITPURCHASEDEPT_NOTIFY_MAIL"].ToString()).Replace("\"", "\\");
                DataTypesHelper.LogThis(AppDomain.CurrentDomain.BaseDirectory);

                // get all cc email Purchasing Department
                string sCC = GetAllPurchaseEmployees(cfmGrafolit);

                if (!String.IsNullOrEmpty(InqueryEmployee.Email))
                {
                    model.EmployeeName = InqueryEmployee.Ime + " " + InqueryEmployee.Priimek;
                    model.Email = InqueryEmployee.Email;
                    model.NarociloSt = InqModel.PovprasevanjeStevilka;

                    reader = new StreamReader(templatePath);
                    string templateString = reader.ReadToEnd();
                    templateString = ReplaceDefaultValuesInTemplate(model, templateString);
                    emailSubject = "(" + InqModel.PovprasevanjeStevilka + ") " + emailSubject;
                    SaveToSystemEmailMessage(model.Email, templateString, null, 1, emailSubject, "", InqueryEmployee, sCC);
                }
                else
                {
                    throw new Exception("Zaposlen " + InqueryEmployee.Ime + " " + InqueryEmployee.Priimek + "nima vpisanega elektrnoskega naslova!");
                }
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message);
            }
        }

        private DateTime GetInquiryDate(int ipos)
        {
            //return context.PovprasevanjePozicija.Where(ip => ip.PovprasevanjePozicijaID == ipos).FirstOrDefault().Povprasevanje.DatumOddajePovprasevanja.Value;
            return DateTime.Now;
        }
    }
}