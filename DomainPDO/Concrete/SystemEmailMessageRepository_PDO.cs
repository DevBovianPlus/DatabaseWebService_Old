using DatabaseWebService.Common;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.Domain.Abstract;
using DatabaseWebService.DomainPDO.Abstract;
using DatabaseWebService.ModelsOTP;
using DatabaseWebService.ModelsPDO.Order;
using DatabaseWebService.ModelsPDO.Settings;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace DatabaseWebService.DomainPDO.Concrete
{
    public class SystemEmailMessageRepository_PDO : ISystemEmailMessageRepository_PDO
    {
        GrafolitPDOEntities context;

        public SystemEmailMessageRepository_PDO(GrafolitPDOEntities _context)
        {
            context = _context;
        }

        public List<SystemEmailMessage_PDO> GetUnprocessedEmails()
        {
            var settings = context.Nastavitve.OrderByDescending(s => s.ts).FirstOrDefault();
            if (settings != null)
            {
                if (settings.PosiljanjePoste.HasValue && settings.PosiljanjePoste.Value)
                    return context.SystemEmailMessage_PDO.Where(sem => sem.Status.Value == (int)Enums.SystemEmailMessageStatus.UnProcessed).ToList();
                else
                    throw new Exception("Aplikacija nima odobrene pravice za pošiljanje elektronske pošte!");

            }
            else
                throw new Exception("Aplikacija PDO nima nastavljenih osnovnih nastavitev (tabela Nastavitve!)");
        }

        public List<PDOEmailModel> GetAllEmails()
        {
            try
            {
                var query = from email in context.SystemEmailMessage_PDO
                            select new PDOEmailModel
                            {
                                SystemEmailMessageID = email.SystemEmailMessageID,
                                EmailFrom = email.EmailFrom,
                                EmailTo = email.EmailTo,
                                EmailBody = email.EmailBody,
                                EmailSubject = email.EmailSubject,
                                EmailStatus = email.Status.HasValue ? email.Status.Value : 0,
                                ts = email.ts.HasValue ? email.ts.Value : DateTime.MinValue,
                                tsIDOsebe = email.tsIDOsebe.HasValue ? email.tsIDOsebe.Value : 0,
                            };
                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }




        public void SaveEmail(SystemEmailMessage_PDO model)
        {
            if (model.SystemEmailMessageID != 0)
            {
                model.Status = (int)Enums.SystemEmailMessageStatus.Processed;
                var original = context.SystemEmailMessage_PDO.Where(sem => sem.SystemEmailMessageID == model.SystemEmailMessageID).FirstOrDefault();
                context.Entry(original).CurrentValues.SetValues(model);
                context.SaveChanges();
            }

        }

        public void UpdateFailedMessges()
        {
            List<SystemEmailMessage_PDO> errorList = context.SystemEmailMessage_PDO.Where(sem => sem.Status == (int)Enums.SystemEmailMessageStatus.Error).ToList();
            foreach (var item in errorList)
            {
                item.Status = (int)Enums.SystemEmailMessageStatus.UnProcessed;
            }
            context.SaveChanges();
        }


        private void SetLanguageContentEmail(CarrierMailModel message, string sLangugeKoda)
        {
            switch (sLangugeKoda)
            {
                case "ANG":
                    message.BodyText = "We are sending you a purchase order";
                    message.AdditionalText = "Attached to this email is a purchase order form for Enquiry No: ";
                    message.SubjectText = "Grafo Lit - We are sending you a purchase order";
                    break;
                case "SLO":
                    message.BodyText = "Pošiljamo vam naročilnico.";
                    message.AdditionalText = "V prilogi tega maila je naročilnica za naročilo povpraševanja št: ";
                    message.SubjectText = "Grafo Lit - Pošiljamo vam naročilnico";
                    break;
                case "HRV":
                    message.BodyText = "Šaljemo vam narudžbu za kupnju";
                    message.AdditionalText = "Uz ovu e-poštu priložen je obrazac za narudžbu za br. upita: ";
                    message.SubjectText = "Grafo Lit - Šaljemo vam narudzbu";
                    break;
                default:
                    break;
            }
        }

        public void CreateEmailForSupplierOrder(OrderPDOFullModel order)
        {
            StreamReader reader = null;
            try
            {
                //DataTypesHelper.LogThis("*****IN Method ProcessEventMessage*****");

                string emailSubject = SystemEmailMessageResource.res_22;
                string templatePathRez = (AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["SUPPLIER_MAIL_ORDER"].ToString()).Replace("\"", "\\");
                string templatePath = (ConfigurationManager.AppSettings["SUPPLIER_MAIL_ORDER"].ToString()).Replace("\"", "\\");
                string rootURL = ConfigurationManager.AppSettings["ServerTagCarrierPage"].ToString();
                DataTypesHelper.LogThis(AppDomain.CurrentDomain.BaseDirectory);

                var Dobavitelj = context.Stranka_PDO.Where(s => s.StrankaID == order.StrankaDobaviteljID).FirstOrDefault();

                CarrierMailModel message = null;

                if (Dobavitelj != null)
                {
                    templatePath = templatePath.Replace("XXX", Dobavitelj.Jeziki.Koda.ToString());
                    templatePathRez = templatePathRez.Replace("XXX", Dobavitelj.Jeziki.Koda.ToString());

                    if (!String.IsNullOrEmpty(Dobavitelj.Email))//TODO: kaj pa če ima stranka vpisanih več mail-ov
                    {
                        DataTypesHelper.LogThis("Send order to email: " + Dobavitelj.Email);
                        message = new CarrierMailModel();

                        SetLanguageContentEmail(message, Dobavitelj.Jeziki.Koda);

                        message.AdditionalText = message.AdditionalText + order.NarociloID.ToString();
                        message.CarrierName = Dobavitelj.NazivPrvi;
                        message.Email = Dobavitelj.Email;


                        templatePath = File.Exists(templatePath) ? templatePath : templatePathRez;
                        reader = new StreamReader(templatePath);
                        string templateString = reader.ReadToEnd();
                        templateString = ReplaceDefaultValuesInTemplate(message, templateString);

                        SaveToSystemEmailMessage(message.Email, templateString, null, 1, message.SubjectText, order.P_TransportOrderPDFDocPath);
                    }
                    else
                    {
                        throw new Exception("Stranka " + Dobavitelj.NazivPrvi + "nima vpisanega elektrnoskega naslova!");
                    }

                }
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message);
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

        public void CreateEmailForAdmin_NoPDFForOrderPDO(string sOdobritevKomentar, string sStevilkaDokumenta, string sStevilkaNarocilnice, bool bOdpoklic)
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
                List<Osebe_PDO> employeesList = context.Osebe_PDO.Where(o => o.VlogaID == 2).ToList();
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

                        message.Comments += bOdpoklic ? "<br> <b>ODPOKLIC</b> " : "<br> <b>PDO Naročilo</b> ";


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

        private void SaveToSystemEmailMessage(string emailTo, string bodyMessage, int? userID, int hierarchyLevel = 1, string emailSubject = "Novo sporočilo", string attachments = "")
        {
            try
            {
                SystemEmailMessage_PDO emailConstruct = null;

                for (int i = 0; i < hierarchyLevel; i++)
                {
                    emailConstruct = new SystemEmailMessage_PDO();
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
                        emailConstruct.EmailTo = context.OsebeNadrejeni_PDO.Where(on => on.OsebaID == userID.Value).FirstOrDefault().Osebe_PDO1.Email;//Nadrejeni;
                        emailConstruct.EmailSubject += " - Sporočilo za nadrejenega";
                    }
                    else if (i == 2)
                    {
                        int ceo = Convert.ToInt32(ConfigurationManager.AppSettings["CEOId"].ToString());
                        emailConstruct.EmailTo = context.Osebe_PDO.Where(o => o.OsebaID == ceo).FirstOrDefault().Email;//direktor;
                        emailConstruct.EmailSubject += " - Sporočilo za nadrejenega";
                    }

                    if (attachments.Length > 0)
                    {
                        emailConstruct.Attachments = attachments;
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

        public void SaveSystemEmailMessage(PDOEmailModel model, bool updateRecord = true, bool createCopy = false)
        {
            try
            {
                SystemEmailMessage_PDO m = new SystemEmailMessage_PDO();
                m.EmailBody = model.EmailBody;
                m.EmailFrom = model.EmailFrom;
                m.EmailSubject = model.EmailSubject;
                m.EmailTo = model.EmailTo;
                m.Status = createCopy ?  0 : model.EmailStatus;
                m.tsUpdate = DateTime.Now;
                m.tsUpdateUserID = model.tsUpdateUserID;
                m.ts = model.ts.Equals(DateTime.MinValue) ? (DateTime?)null : model.ts;
                m.tsIDOsebe = model.tsIDOsebe;


                var existingItem = context.SystemEmailMessage_PDO.Where(mes => mes.SystemEmailMessageID == model.SystemEmailMessageID).FirstOrDefault();
                if (existingItem != null)
                {
                    m.Attachments = existingItem.Attachments;
                    m.CCEmails = existingItem.CCEmails;
                    m.OsebaEmailFromID = existingItem.OsebaEmailFromID;
                    m.SystemEmailMessageID = createCopy ? 0 : existingItem.SystemEmailMessageID;
                    m.MasterMailID = existingItem.SystemEmailMessageID;
                }

                if (m.SystemEmailMessageID == 0)
                {
                    m.ts = DateTime.Now;
                    m.tsIDOsebe = model.tsIDOsebe;

                    context.SystemEmailMessage_PDO.Add(m);
                }
                else
                {
                    if (updateRecord)
                    {
                        SystemEmailMessage_PDO original = context.SystemEmailMessage_PDO.Where(mes => mes.SystemEmailMessageID == m.SystemEmailMessageID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(m);
                    }
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public PDOEmailModel GetMailByID(int mailID)
        {
            try
            {
                var query = from mail in context.SystemEmailMessage_PDO
                            where mail.SystemEmailMessageID == mailID
                            select new PDOEmailModel
                            {
                                SystemEmailMessageID = mail.SystemEmailMessageID,
                                EmailBody = mail.EmailBody,
                                EmailFrom = mail.EmailFrom,
                                EmailStatus = mail.Status.HasValue ? mail.Status.Value : -1,
                                EmailSubject = mail.EmailSubject,
                                EmailTo = mail.EmailTo,
                                ts = mail.ts.HasValue ? mail.ts.Value : DateTime.MinValue,
                                tsIDOsebe = mail.tsIDOsebe.HasValue ? mail.tsIDOsebe.Value : 0,
                                tsUpdate = mail.tsUpdate.HasValue ? mail.tsUpdate.Value : DateTime.MinValue,
                                tsUpdateUserID = mail.tsUpdateUserID.HasValue ? mail.tsUpdateUserID.Value : 0,
                            };

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }
    }
}