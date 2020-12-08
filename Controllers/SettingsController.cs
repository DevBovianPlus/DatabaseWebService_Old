using DatabaseWebService.Common;
using DatabaseWebService.DomainNOZ.Abstract;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.DomainPDO.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.ModelsNOZ.Settings;
using DatabaseWebService.ModelsOTP;
using DatabaseWebService.ModelsPDO.Settings;
using DatabaseWebService.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DatabaseWebService.Controllers
{
    public class SettingsController : ApiController
    {
        private ISettingsRepository settingsRepo;
        private ISystemEmailMessageRepository_PDO mailsRepo;
        private ISystemEmailMessageRepository_OTP OTPmailsRepo;
        private ISystemEmailMessageRepository_NOZ NOZmailsRepo;

        private ISettingsNOZRepository settingsNOZRepo;

        public delegate WebResponseContentModel<T> Del<T>(WebResponseContentModel<T> model, Exception ex = null);

        public SettingsController(ISettingsRepository isettingsRepo, ISystemEmailMessageRepository_PDO ImailsRepo, ISettingsNOZRepository IsettingsNOZRepo, ISystemEmailMessageRepository_NOZ ImailsNOZRepo, ISystemEmailMessageRepository_OTP iOTPmailsRepo)
        {
            settingsRepo = isettingsRepo;
            mailsRepo = ImailsRepo;
            NOZmailsRepo = ImailsNOZRepo;
            settingsNOZRepo = IsettingsNOZRepo;
            OTPmailsRepo = iOTPmailsRepo;
        }

        public WebResponseContentModel<T> ProcessContentModel<T>(WebResponseContentModel<T> model, Exception ex = null)
        {
            if (ex == null)
            {
                if (model.Content != null)
                {
                    model.IsRequestSuccesful = true;
                    model.ValidationError = "";
                }
                else
                {
                    model.IsRequestSuccesful = false;
                    model.ValidationError = ValidationExceptionError.res_03;
                }
            }
            else
            {
                model.IsRequestSuccesful = false;
                model.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
            }
            return model;
        }

        //PDO
        [HttpGet]
        public IHttpActionResult GetAppSettings()
        {
            WebResponseContentModel<SettingsModel> tmpUser = new WebResponseContentModel<SettingsModel>();
            Del<SettingsModel> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = settingsRepo.GetLatestSettings();
                responseStatusHandler(tmpUser);
            }
            catch (Exception ex)
            {
                responseStatusHandler(tmpUser, ex);
                return Json(tmpUser);
            }

            return Json(tmpUser);
        }

        [HttpGet]
        public IHttpActionResult GetAllEmails()
        {
            WebResponseContentModel<List<PDOEmailModel>> tmpUser = new WebResponseContentModel<List<PDOEmailModel>>();
            Del<List<PDOEmailModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = mailsRepo.GetAllEmails();

                responseStatusHandler(tmpUser);
            }
            catch (Exception ex)
            {
                responseStatusHandler(tmpUser, ex);
                return Json(tmpUser);
            }

            return Json(tmpUser);
        }

        [HttpGet]
        public IHttpActionResult GetAllEmailsOTP()
        {
            WebResponseContentModel<List<OTPEmailModel>> tmpUser = new WebResponseContentModel<List<OTPEmailModel>>();
            Del<List<OTPEmailModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = OTPmailsRepo.GetAllEmailsOTP();

                responseStatusHandler(tmpUser);
            }
            catch (Exception ex)
            {
                responseStatusHandler(tmpUser, ex);
                return Json(tmpUser);
            }

            return Json(tmpUser);
        }

        [HttpGet]
        public IHttpActionResult RunSQLString(string sSQL)
        {
            WebResponseContentModel<bool> tmpUser = new WebResponseContentModel<bool>();
            Del<bool> responseStatusHandler = ProcessContentModel;
            try
            {
                settingsRepo.RunSQLString(sSQL);
                tmpUser.Content = true;
                responseStatusHandler(tmpUser);
            }
            catch (Exception ex)
            {
                tmpUser.Content = false;
                responseStatusHandler(tmpUser, ex);
                return Json(tmpUser);
            }

            return Json(tmpUser);
        }

        [HttpPost]
        public IHttpActionResult SaveSettings([FromBody]object settingsData)
        {
            WebResponseContentModel<SettingsModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<SettingsModel>>(settingsData.ToString());

                if (model.Content != null)
                {
                    if (model.Content.NastavitveID > 0)//We update existing record in DB
                    {
                        settingsRepo.SaveSettings(model.Content);
                    }
                    else // We add and save new recod to DB 
                    {
                        model.Content.NastavitveID = settingsRepo.SaveSettings(model.Content, false);
                    }

                    model.IsRequestSuccesful = true;
                }
                else
                {
                    model.IsRequestSuccesful = false;
                    model.ValidationError = ValidationExceptionError.res_09;
                }
            }
            catch (Exception ex)
            {
                model.IsRequestSuccesful = false;
                model.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(model);
            }

            return Json(model);
        }

        [HttpGet]
        public IHttpActionResult CreateMailCopy(int mailID)
        {
            WebResponseContentModel<bool> tmpUser = new WebResponseContentModel<bool>();
            Del<bool> responseStatusHandler = ProcessContentModel;
            try
            {
                var mail = mailsRepo.GetMailByID(mailID);
                if (mail != null) mail.EmailStatus = 0;
                mailsRepo.SaveSystemEmailMessage(mail, false, true);
                tmpUser.Content = true;
                responseStatusHandler(tmpUser);
            }
            catch (Exception ex)
            {
                responseStatusHandler(tmpUser, ex);
                return Json(tmpUser);
            }

            return Json(tmpUser);
        }

        [HttpPost]
        public IHttpActionResult CreateMailCopy([FromBody]object mailData)
        {
            WebResponseContentModel<PDOEmailModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<PDOEmailModel>>(mailData.ToString());

                if (model.Content != null)
                {
                    mailsRepo.SaveSystemEmailMessage(model.Content, false, true);

                    model.IsRequestSuccesful = true;
                }
                else
                {
                    model.IsRequestSuccesful = false;
                    model.ValidationError = ValidationExceptionError.res_09;
                }
            }
            catch (Exception ex)
            {
                model.IsRequestSuccesful = false;
                model.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(model);
            }

            return Json(model);
        }

        [HttpGet]
        public IHttpActionResult GetMailByID(int mailID)
        {
            WebResponseContentModel<PDOEmailModel> tmpUser = new WebResponseContentModel<PDOEmailModel>();
            Del<PDOEmailModel> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = mailsRepo.GetMailByID(mailID);
                responseStatusHandler(tmpUser);
            }
            catch (Exception ex)
            {
                responseStatusHandler(tmpUser, ex);
                return Json(tmpUser);
            }

            return Json(tmpUser);
        }

        #region Grafolit_NOZ

        [HttpGet]
        public IHttpActionResult GetAllEmailsNOZ()
        {
            WebResponseContentModel<List<NOZEmailModel>> tmpUser = new WebResponseContentModel<List<NOZEmailModel>>();
            Del<List<NOZEmailModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = NOZmailsRepo.GetAllEmailsNOZ();

                responseStatusHandler(tmpUser);
            }
            catch (Exception ex)
            {
                responseStatusHandler(tmpUser, ex);
                return Json(tmpUser);
            }

            return Json(tmpUser);
        }

        [HttpGet]
        public IHttpActionResult GetAppNOZSettings()
        {
            WebResponseContentModel<SettingsNOZModel> tmpUser = new WebResponseContentModel<SettingsNOZModel>();
            Del<SettingsNOZModel> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = settingsNOZRepo.GetLatestSettings();
                responseStatusHandler(tmpUser);
            }
            catch (Exception ex)
            {
                responseStatusHandler(tmpUser, ex);
                return Json(tmpUser);
            }

            return Json(tmpUser);
        }

        [HttpPost]
        public IHttpActionResult SaveNOZSettings([FromBody]object settingsData)
        {
            WebResponseContentModel<SettingsNOZModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<SettingsNOZModel>>(settingsData.ToString());

                if (model.Content != null)
                {
                    if (model.Content.NastavitveID > 0)//We update existing record in DB
                    {
                        settingsNOZRepo.SaveSettings(model.Content);
                    }
                    else // We add and save new recod to DB 
                    {
                        model.Content.NastavitveID = settingsNOZRepo.SaveSettings(model.Content, false);
                    }

                    model.IsRequestSuccesful = true;
                }
                else
                {
                    model.IsRequestSuccesful = false;
                    model.ValidationError = ValidationExceptionError.res_09;
                }
            }
            catch (Exception ex)
            {
                model.IsRequestSuccesful = false;
                model.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(model);
            }

            return Json(model);
        }
        #endregion
    }
}

