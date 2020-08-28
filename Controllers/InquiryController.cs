using DatabaseWebService.Common;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.DomainPDO;
using DatabaseWebService.DomainPDO.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DatabaseWebService.Models.Employee;
using DatabaseWebService.ModelsPDO.Inquiry;
using DatabaseWebService.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DatabaseWebService.Controllers
{
    public class InquiryController : ApiController
    {
        private IInquiryRepository inquiryRepo;
        private IMSSQLPDOFunctionRepository mssqlRepo;
        private ISystemMessageEventsRepository_PDO messageEventsRepo;
        private IEmployeePDORepository employeeRepo;
        private IClientPDORepository clientPdoRepo;


        public delegate WebResponseContentModel<T> Del<T>(WebResponseContentModel<T> model, Exception ex = null);
        public InquiryController(IInquiryRepository iinquiryRepo, IMSSQLPDOFunctionRepository imssqlRepo, ISystemMessageEventsRepository_PDO imessageEventsRepo,
            IEmployeePDORepository iemployeeRepo, IClientPDORepository iclientPdoRepo)
        {
            inquiryRepo = iinquiryRepo;
            mssqlRepo = imssqlRepo;
            messageEventsRepo = imessageEventsRepo;
            employeeRepo = iemployeeRepo;
            clientPdoRepo = iclientPdoRepo;
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

        #region Recall


        [HttpGet]
        public IHttpActionResult GetAllInquiries()
        {
            WebResponseContentModel<List<InquiryModel>> tmpUser = new WebResponseContentModel<List<InquiryModel>>();
            Del<List<InquiryModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = inquiryRepo.GetInquiryList();
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
        public IHttpActionResult GetInquiryByID(int inquiryID, bool bOnlySelected, int iSelDobaviteljID)
        {
            WebResponseContentModel<InquiryFullModel> tmpUser = new WebResponseContentModel<InquiryFullModel>();
            Del<InquiryFullModel> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = inquiryRepo.GetInquiryByID(inquiryID, bOnlySelected, iSelDobaviteljID);
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
        public IHttpActionResult SaveInquiry([FromBody] object inquiryData)
        {
            WebResponseContentModel<InquiryFullModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<InquiryFullModel>>(inquiryData.ToString());

                if (model.Content != null)
                {
                    if (model.Content.PovprasevanjeID > 0)//We update existing record in DB
                    {
                        inquiryRepo.SaveInquiry(model.Content);

                        if (model.Content.EmployeeSubmitInquiry)
                        {
                            //TODO: Send mail to suppliers!
                            var employee = employeeRepo.GetEmployeeByID(model.Content.PovprasevanjeOddalID);
                            var group = inquiryRepo.GetInquiryPositionsGroupedBySupplier(model.Content.PovprasevanjeID);
                            messageEventsRepo.CreateEmailForSuppliers(group, employee, model.Content.PovprasevanjeStevilka);
                        }
                    }
                    else // We add and save new recod to DB 
                    {
                        model.Content.PovprasevanjeID = inquiryRepo.SaveInquiry(model.Content, false);

                        if (model.Content.EmployeeSubmitInquiry)
                        {
                            //TODO: Send mail to suppliers!
                            var employee = employeeRepo.GetEmployeeByID(model.Content.PovprasevanjeOddalID);
                            var group = inquiryRepo.GetInquiryPositionsGroupedBySupplier(model.Content.PovprasevanjeID);
                            messageEventsRepo.CreateEmailForSuppliers(group, employee, model.Content.PovprasevanjeStevilka);
                        }
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

        [HttpPost]
        public IHttpActionResult SaveInquiryPurchase([FromBody] object inquiryData)
        {
            WebResponseContentModel<InquiryFullModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<InquiryFullModel>>(inquiryData.ToString());



                if (model.Content != null)
                {
                    if (model.Content.PovprasevanjeID > 0)//We update existing record in DB
                    {
                        inquiryRepo.SaveInquiry(model.Content);
                        // send email to all grafolit contacts = Nabava
                        if (model.Content.StatusPovprasevanja.Koda == Enums.StatusOfInquiry.POSLANO_V_NABAVO.ToString())
                        {
                            var employee = employeeRepo.GetEmployeeByID(model.Content.PovprasevanjeOddalID);
                            string sGrafolitDept = ConfigurationManager.AppSettings["PantheonCreateOrderDefBuyer"].ToString();
                            ClientFullModel cfmGrafolit = clientPdoRepo.GetClientByName(sGrafolitDept);

                            messageEventsRepo.CreateEmailForGrafolitPurcaheDept(cfmGrafolit, employee, model.Content);
                        }
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
        public IHttpActionResult DeleteInquiry(int InquiryID)
        {
            WebResponseContentModel<bool> deleteInquiry = new WebResponseContentModel<bool>();
            try
            {
                deleteInquiry.Content = inquiryRepo.DeleteInquiry(InquiryID);

                if (deleteInquiry.Content)
                    deleteInquiry.IsRequestSuccesful = true;
                else
                {
                    deleteInquiry.IsRequestSuccesful = false;
                    deleteInquiry.ValidationError = ValidationExceptionError.res_04;
                }
            }
            catch (Exception ex)
            {
                deleteInquiry.IsRequestSuccesful = false;
                deleteInquiry.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(deleteInquiry);
            }

            return Json(deleteInquiry);
        }

        [HttpGet]
        public IHttpActionResult CopyInquiryByID(int InquiryID)
        {
            WebResponseContentModel<bool> deleteInquiry = new WebResponseContentModel<bool>();
            try
            {
                deleteInquiry.Content = inquiryRepo.CopyInquiryByID(InquiryID);

                if (deleteInquiry.Content)
                    deleteInquiry.IsRequestSuccesful = true;
                else
                {
                    deleteInquiry.IsRequestSuccesful = false;
                    deleteInquiry.ValidationError = ValidationExceptionError.res_04;
                }
            }
            catch (Exception ex)
            {
                deleteInquiry.IsRequestSuccesful = false;
                deleteInquiry.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(deleteInquiry);
            }

            return Json(deleteInquiry);
        }
        #endregion

        #region InquiryPositions

        [HttpGet]
        public IHttpActionResult GetInquiryPositionByID(int inquiryPosID)
        {
            WebResponseContentModel<InquiryPositionModel> tmpUser = new WebResponseContentModel<InquiryPositionModel>();
            Del<InquiryPositionModel> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = inquiryRepo.GetInquiryPositionByID(inquiryPosID);
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
        public IHttpActionResult CopyInquiryPositionByID(int inquiryPosID)
        {
            WebResponseContentModel<InquiryPositionModel> tmpUser = new WebResponseContentModel<InquiryPositionModel>();
            Del<InquiryPositionModel> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = inquiryRepo.CopyInquiryPositionByID(inquiryPosID);
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
        public IHttpActionResult SaveInquiryPosition([FromBody] object inquiryPosData)
        {
            WebResponseContentModel<InquiryPositionModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<InquiryPositionModel>>(inquiryPosData.ToString());

                if (model.Content != null)
                {
                    if (model.Content.PovprasevanjePozicijaID > 0)//We update existing record in DB
                        inquiryRepo.SaveInquiryPositionModel(model.Content);
                    else // We add and save new recod to DB 
                        model.Content.PovprasevanjePozicijaID = inquiryRepo.SaveInquiryPositionModel(model.Content, false);

                    model.Content.PovprasevanjePozicijaArtikel = inquiryRepo.GetInquiryPositionArtikelByInquiryPositionID(model.Content.PovprasevanjePozicijaID);
                    //model = inquiryRepo.GetInquiryPositionByID(model.Content.PovprasevanjeID);
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

        [HttpPost]
        public IHttpActionResult SaveInquiryPositions([FromBody] object inquiryPosData)
        {
            WebResponseContentModel<List<InquiryPositionModel>> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<List<InquiryPositionModel>>>(inquiryPosData.ToString());

                if (model.Content != null)
                {
                    inquiryRepo.SaveInquiryPositionsModel(model.Content);

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
        public IHttpActionResult DeleteInquiryPosition(int inquiryPosID)
        {
            WebResponseContentModel<bool> deleteInquiryPos = new WebResponseContentModel<bool>();
            try
            {
                deleteInquiryPos.Content = inquiryRepo.DeleteInquiryPosition(inquiryPosID);

                if (deleteInquiryPos.Content)
                    deleteInquiryPos.IsRequestSuccesful = true;
                else
                {
                    deleteInquiryPos.IsRequestSuccesful = false;
                    deleteInquiryPos.ValidationError = ValidationExceptionError.res_04;
                }
            }
            catch (Exception ex)
            {
                deleteInquiryPos.IsRequestSuccesful = false;
                deleteInquiryPos.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(deleteInquiryPos);
            }

            return Json(deleteInquiryPos);
        }

        [HttpPost]
        public IHttpActionResult DeleteInquiryPositionArtikles([FromBody] object inquirySupplierPosData)
        {
            WebResponseContentModel<List<InquiryPositionArtikelModel>> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<List<InquiryPositionArtikelModel>>>(inquirySupplierPosData.ToString());

                if (model.Content != null)
                {
                    inquiryRepo.DeleteInquiryPositionArtikles(model.Content);

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

        #region RecallStatus

        [HttpGet]
        public IHttpActionResult GetInquiryStatusByID(int statusID)
        {
            WebResponseContentModel<InquiryStatusModel> tmpUser = new WebResponseContentModel<InquiryStatusModel>();
            Del<InquiryStatusModel> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = inquiryRepo.GetInquiryStatusByID(statusID);
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
        public IHttpActionResult GetInquiryStatusByCode(string statusCode)
        {
            WebResponseContentModel<InquiryStatusModel> tmpUser = new WebResponseContentModel<InquiryStatusModel>();
            Del<InquiryStatusModel> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = inquiryRepo.GetInquiryStatusByCode(statusCode);
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
        public IHttpActionResult GetRecallStatuses()
        {
            WebResponseContentModel<List<InquiryStatusModel>> tmpUser = new WebResponseContentModel<List<InquiryStatusModel>>();
            Del<List<InquiryStatusModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = inquiryRepo.GetInquiryStatuses();
                responseStatusHandler(tmpUser);
            }
            catch (Exception ex)
            {
                responseStatusHandler(tmpUser, ex);
                return Json(tmpUser);
            }

            return Json(tmpUser);
        }

        #endregion

        #region MS SQL FUNCTIONS

        [HttpGet]
        public IHttpActionResult GetBuyerList()
        {
            WebResponseContentModel<List<ClientSimpleModel>> tmpUser = new WebResponseContentModel<List<ClientSimpleModel>>();
            Del<List<ClientSimpleModel>> responseStatusHandler = ProcessContentModel;

            try
            {
                tmpUser.Content = mssqlRepo.GetBuyerList();
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
        public IHttpActionResult GetPantheonUsers()
        {
            WebResponseContentModel<List<PantheonUsers>> tmpUser = new WebResponseContentModel<List<PantheonUsers>>();
            Del<List<PantheonUsers>> responseStatusHandler = ProcessContentModel;

            try
            {
                tmpUser.Content = mssqlRepo.GetPantheonUsers();
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
        public IHttpActionResult GetCategoryList()
        {
            WebResponseContentModel<List<ProductCategory>> tmpUser = new WebResponseContentModel<List<ProductCategory>>();
            Del<List<ProductCategory>> responseStatusHandler = ProcessContentModel;

            try
            {
                tmpUser.Content = mssqlRepo.GetCategoryList();
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
        public IHttpActionResult GetSupplierByName(string name)
        {
            WebResponseContentModel<List<ClientSimpleModel>> tmpUser = new WebResponseContentModel<List<ClientSimpleModel>>();
            Del<List<ClientSimpleModel>> responseStatusHandler = ProcessContentModel;

            try
            {
                tmpUser.Content = mssqlRepo.GetSupplierByName(name);
                responseStatusHandler(tmpUser);
            }
            catch (Exception ex)
            {
                responseStatusHandler(tmpUser, ex);
                return Json(tmpUser);
            }

            return Json(tmpUser);
        }

        #endregion

        [HttpGet]
        public IHttpActionResult LockInquiry(int inquiryID, int userID)
        {
            WebResponseContentModel<bool> tmpUser = new WebResponseContentModel<bool>();
            Del<bool> responseStatusHandler = ProcessContentModel;
            try
            {
                inquiryRepo.LockInquiry(inquiryID, userID);
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

        [HttpGet]
        public IHttpActionResult UnLockInquiry(int inquiryID, int userID)
        {
            WebResponseContentModel<bool> tmpUser = new WebResponseContentModel<bool>();
            Del<bool> responseStatusHandler = ProcessContentModel;
            try
            {
                inquiryRepo.UnLockInquiry(inquiryID, userID);
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

        [HttpGet]
        public IHttpActionResult IsInquiryLocked(int inquiryID)
        {
            WebResponseContentModel<bool> tmpUser = new WebResponseContentModel<bool>();
            Del<bool> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = inquiryRepo.IsInquiryLocked(inquiryID);
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

        [HttpGet]
        public IHttpActionResult UnLockInquiriesByUserID(int userID)
        {
            WebResponseContentModel<bool> tmpUser = new WebResponseContentModel<bool>();
            Del<bool> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = inquiryRepo.UnLockInquiriesByUserID(userID);
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

        [HttpGet]
        public IHttpActionResult GetInquiryPositionsGroupedBySupplier(int inquiryID)
        {
            WebResponseContentModel<List<GroupedInquiryPositionsBySupplier>> tmpUser = new WebResponseContentModel<List<GroupedInquiryPositionsBySupplier>>();
            Del<List<GroupedInquiryPositionsBySupplier>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = inquiryRepo.GetInquiryPositionsGroupedBySupplier(inquiryID);

                responseStatusHandler(tmpUser);
            }
            catch (Exception ex)
            {
                tmpUser.Content = null;
                responseStatusHandler(tmpUser, ex);
                return Json(tmpUser);
            }

            return Json(tmpUser);
        }

        [HttpPost]
        public IHttpActionResult SaveInquiryPositionSupplierPdfReport([FromBody] object data)
        {
            WebResponseContentModel<GroupedInquiryPositionsBySupplier> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<GroupedInquiryPositionsBySupplier>>(data.ToString());

                if (model.Content != null)
                {
                    inquiryRepo.SaveInquiryPositionPdfReport(model.Content);

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
    }
}
