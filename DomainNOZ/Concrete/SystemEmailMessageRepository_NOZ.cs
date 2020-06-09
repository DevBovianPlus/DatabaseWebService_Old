using DatabaseWebService.Common;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.DomainNOZ.Abstract;
using DatabaseWebService.ModelsNOZ;
using DatabaseWebService.ModelsNOZ.Settings;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace DatabaseWebService.DomainNOZ.Concrete
{
    public class SystemEmailMessageRepository_NOZ : ISystemEmailMessageRepository_NOZ
    {
        GrafolitNOZEntities context;

        public SystemEmailMessageRepository_NOZ(GrafolitNOZEntities _context)
        {
            context = _context;
        }

        public void CreateEmailForUserCreateNewCodeForProduct(CreateNewCodeMailModel model)
        {
            StreamReader reader = null;
            try
            {
                //DataTypesHelper.LogThis("*****IN Method ProcessEventMessage*****");

                string emailSubject = SystemEmailMessageResource.res_23;
                string templatePath = (AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["USER_MAIL_NOZ"].ToString()).Replace("\"", "\\");
                string rootURL = ConfigurationManager.AppSettings["ServerURL_NOZ"].ToString();
                DataTypesHelper.LogThis(AppDomain.CurrentDomain.BaseDirectory);

                var user = context.Osebe_NOZ.Where(o => o.OsebaID == model.UserId).FirstOrDefault();

                if (user != null)
                {
                    if (!String.IsNullOrEmpty(user.Email))//TODO: kaj pa če ima stranka vpisanih več mail-ov
                    {
                        model.Email = user.Email;
                        model.EmployeeName = user.Ime + " " + user.Priimek;
                        model.RootURL = rootURL;
                        
                        reader = new StreamReader(templatePath);
                        string templateString = reader.ReadToEnd();
                        templateString = ReplaceDefaultValuesInTemplate(model, templateString);

                        SaveToSystemEmailMessage(user.Email, templateString, null, 1, emailSubject);
                    }
                    else
                    {
                        throw new Exception("Oseba " + user.Ime + " " + user.Priimek + " nima vpisanega elektrnoskega naslova!");
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

        private void SaveToSystemEmailMessage(string emailTo, string bodyMessage, int? userID, int hierarchyLevel = 1, string emailSubject = "Novo sporočilo", string attachments = "")
        {
            try
            {
                SystemEmailMessage_NOZ emailConstruct = null;

                for (int i = 0; i < hierarchyLevel; i++)
                {
                    emailConstruct = new SystemEmailMessage_NOZ();
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
                        emailConstruct.EmailTo = context.OsebeNadrejeni_NOZ.Where(on => on.OsebaID == userID.Value).FirstOrDefault().Osebe_NOZ1.Email;//Nadrejeni;
                        emailConstruct.EmailSubject += " - Sporočilo za nadrejenega";
                    }
                    else if (i == 2)
                    {
                        int ceo = Convert.ToInt32(ConfigurationManager.AppSettings["CEOId"].ToString());
                        emailConstruct.EmailTo = context.Osebe_NOZ.Where(o => o.OsebaID == ceo).FirstOrDefault().Email;//direktor;
                        emailConstruct.EmailSubject += " - Sporočilo za nadrejenega";
                    }

                    if (attachments.Length > 0)
                    {
                        emailConstruct.Attachments = attachments;
                    }

                    context.SystemEmailMessage_NOZ.Add(emailConstruct);
                }

                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Method SaveToSystemEmailMessage ERROR: " + ex);
            }
        }

        public void SaveEmail(SystemEmailMessage_NOZ model)
        {
            if (model.SystemEmailMessageID != 0)
            {
                model.Status = (int)Enums.SystemEmailMessageStatus.Processed;
                var original = context.SystemEmailMessage_NOZ.Where(sem => sem.SystemEmailMessageID == model.SystemEmailMessageID).FirstOrDefault();
                context.Entry(original).CurrentValues.SetValues(model);
                context.SaveChanges();
            }

        }

        public void UpdateFailedMessges()
        {
            List<SystemEmailMessage_NOZ> errorList = context.SystemEmailMessage_NOZ.Where(sem => sem.Status == (int)Enums.SystemEmailMessageStatus.Error).ToList();
            foreach (var item in errorList)
            {
                item.Status = (int)Enums.SystemEmailMessageStatus.UnProcessed;
            }
            context.SaveChanges();
        }

        public List<SystemEmailMessage_NOZ> GetUnprocessedEmails()
        {

            return context.SystemEmailMessage_NOZ.Where(sem => sem.Status.Value == (int)Enums.SystemEmailMessageStatus.UnProcessed).ToList();
            /*var settings = context.Nastavitve.OrderByDescending(s => s.ts).FirstOrDefault();
            if (settings != null)
            {
                if (settings.PosiljanjePoste.HasValue && settings.PosiljanjePoste.Value)
                    return context.SystemEmailMessage_PDO.Where(sem => sem.Status.Value == (int)Enums.SystemEmailMessageStatus.UnProcessed).ToList();
                else
                    throw new Exception("Aplikacija nima odobrene pravice za pošiljanje elektronske pošte!");

            }
            else
                throw new Exception("Aplikacija PDO nima nastavljenih osnovnih nastavitev (tabela Nastavitve!)");*/
        }

        public List<NOZEmailModel> GetAllEmailsNOZ()
        {
            try
            {
                var query = from email in context.SystemEmailMessage_NOZ
                            select new NOZEmailModel
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
    }
}