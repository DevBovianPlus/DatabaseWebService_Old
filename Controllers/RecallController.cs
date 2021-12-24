using DatabaseWebService.Common;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.ModelsOTP;
using DatabaseWebService.ModelsOTP.Recall;
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
    public class RecallController : ApiController
    {
        private IRecallRepository recallRepo;
        private ISystemMessageEventsRepository_OTP messageEventsRepo;
        private IUtilityServiceRepository utilServiceRepo;
        private IEmployeeOTPRepository employeeRepo;
        private IMSSQLFunctionsRepository sqlFunctionRepo;

        public delegate WebResponseContentModel<T> Del<T>(WebResponseContentModel<T> model, Exception ex = null);
        public RecallController(IRecallRepository irecallRepo, ISystemMessageEventsRepository_OTP imessageEventsRepo, IUtilityServiceRepository utilRepo, IEmployeeOTPRepository empRepo, IMSSQLFunctionsRepository isqlFunctionRepo)
        {
            recallRepo = irecallRepo;
            messageEventsRepo = imessageEventsRepo;
            utilServiceRepo = utilRepo;
            employeeRepo = empRepo;
            sqlFunctionRepo = isqlFunctionRepo;
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
        public IHttpActionResult GetAllRecalls()
        {
            WebResponseContentModel<List<RecallModel>> tmpUser = new WebResponseContentModel<List<RecallModel>>();
            Del<List<RecallModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = recallRepo.GetAllRecalls();
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
        public IHttpActionResult GetAllTakeOverRecalls()
        {
            WebResponseContentModel<List<RecallModel>> tmpUser = new WebResponseContentModel<List<RecallModel>>();
            Del<List<RecallModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = recallRepo.GetAllTakeOverRecalls();
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
        public IHttpActionResult GetAllNoneTakeOverRecalls()
        {
            WebResponseContentModel<List<RecallModel>> tmpUser = new WebResponseContentModel<List<RecallModel>>();
            Del<List<RecallModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = recallRepo.GetAllNonTakeOverRecalls();
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
        public IHttpActionResult GetAllBuyersRecalls()
        {
            WebResponseContentModel<List<RecallBuyerModel>> tmpUser = new WebResponseContentModel<List<RecallBuyerModel>>();
            Del<List<RecallBuyerModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = recallRepo.GetAllBuyersRecalls();
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
        public IHttpActionResult GetRecallByID(int recallID)
        {
            WebResponseContentModel<RecallFullModel> tmpUser = new WebResponseContentModel<RecallFullModel>();
            Del<RecallFullModel> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = recallRepo.GetRecallFullModelByID(recallID);
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
        public IHttpActionResult GetRecallBuyerByID(int recallID)
        {
            WebResponseContentModel<RecallBuyerFullModel> tmpUser = new WebResponseContentModel<RecallBuyerFullModel>();
            Del<RecallBuyerFullModel> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = recallRepo.GetRecallBuyerFullModelByID(recallID);
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
        public IHttpActionResult SaveRecall([FromBody]object recallData)
        {
            WebResponseContentModel<RecallFullModel> model = null;
            try
            {
                

                model = JsonConvert.DeserializeObject<WebResponseContentModel<RecallFullModel>>(recallData.ToString());

                if (model.Content != null)
                {
                    DataTypesHelper.LogThis("SaveRecall : (model.Content.OdpoklicID) - Začetek! - Status : " + model.Content.StatusID.ToString());
                    DataTypesHelper.LogThis("SaveRecall-0 : (model.Content.Dobavitelj) :" + DataTypesHelper.Parse(model.Content.DobaviteljNaziv));
                    DataTypesHelper.LogThis("SaveRecall-0 : (model.Content.DobaviteljID) :" + DataTypesHelper.ParseInt(model.Content.DobaviteljID).ToString());

                    DataTypesHelper.LogThis("SaveRecall-0 : (model.Content.Dobavitelj) : :" + model.Content.DobaviteljID == null ? "" : model.Content.DobaviteljID.ToString());
                    DataTypesHelper.LogThis("SaveRecall-0 :" + model.Content.DobaviteljNaziv == null ? "" : model.Content.DobaviteljNaziv);

                    var employee = employeeRepo.GetEmployeeByID(model.Content.UserID);

                    if (model.Content.OdpoklicID > 0)//We update existing record in DB
                    {
                        DataTypesHelper.LogThis("SaveRecall1 : (model.Content.OdpoklicID) :" + model.Content.OdpoklicID.ToString() + "- Status : " + model.Content.StatusID.ToString());
                        DataTypesHelper.LogThis("SaveRecall1 : (model.Content.OdpoklicStevilka) :" + model.Content.OdpoklicStevilka.ToString() + "- Status : " + model.Content.StatusID.ToString());
                        DataTypesHelper.LogThis("SaveRecall-1 : (model.Content.Dobavitelj) : :" + model.Content.DobaviteljID == null ? "" : model.Content.DobaviteljID.ToString());
                        DataTypesHelper.LogThis("SaveRecall-1 :" + model.Content.DobaviteljNaziv == null ? "" : model.Content.DobaviteljNaziv);

                        recallRepo.SaveRecall(model.Content);
                        
                        RecallStatus stat = recallRepo.GetRecallStatusByCode(Enums.StatusOfRecall.V_ODOBRITEV.ToString());
                        
                        if (model.Content.RecallStatusChanged && (stat != null && model.Content.StatusID == stat.StatusOdpoklicaID))
                        {
                            //messageEventsRepo.CreateEmailForRecallStatusChanged(model.Content);
                            DataTypesHelper.LogThis("SaveRecall : " + stat.Koda + "- Status : " + model.Content.StatusID.ToString());
                            messageEventsRepo.CreateEmailForLeaderToApproveRecall(model.Content);
                        }

                        stat = recallRepo.GetRecallStatusByCode(Enums.StatusOfRecall.RAZPIS_PREVOZNIK.ToString());
                        if (model.Content.RecallStatusChanged && (stat != null && model.Content.StatusID == stat.StatusOdpoklicaID))
                        {
                            DataTypesHelper.LogThis("SaveRecall : " + stat.Koda + "- Status : " + model.Content.StatusID.ToString());
                            messageEventsRepo.CreateEmailForCarriers(model.Content, employee);
                        }
                    }
                    else // We add and save new recod to DB 
                    {
                        DataTypesHelper.LogThis("SaveRecall-2 : (model.Content.Dobavitelj) : :" + model.Content.DobaviteljID == null ? "" : model.Content.DobaviteljID.ToString());
                        DataTypesHelper.LogThis("SaveRecall-2 :" + model.Content.DobaviteljNaziv == null ? "" : model.Content.DobaviteljNaziv);

                        DataTypesHelper.LogThis("SaveRecall2 : (model.Content.OdpoklicID) :" + model.Content.OdpoklicID.ToString());
                        string sRazlog = model.Content.RazlogOdobritveSistem == null ? "" : model.Content.RazlogOdobritveSistem.ToString();
                        DataTypesHelper.LogThis("SaveRecall2 : (RazlogOdobritveSistem) :" + sRazlog);

                        model.Content.OdpoklicID = recallRepo.SaveRecall(model.Content, false);
                        DataTypesHelper.LogThis("SaveRecall-3 : (model.Content.Dobavitelj) : :" + model.Content.DobaviteljID == null ? "" : model.Content.DobaviteljID.ToString());
                        DataTypesHelper.LogThis("SaveRecall-3 :" + model.Content.DobaviteljNaziv == null ? "" : model.Content.DobaviteljNaziv);

                        RecallStatus stat = recallRepo.GetRecallStatusByCode(Enums.StatusOfRecall.V_ODOBRITEV.ToString());
                        if (stat != null && model.Content.StatusID == stat.StatusOdpoklicaID)
                        {
                            DataTypesHelper.LogThis("SaveRecall2 : " + stat.Koda);
                            messageEventsRepo.CreateEmailForLeaderToApproveRecall(model.Content);
                        }

                        stat = recallRepo.GetRecallStatusByCode(Enums.StatusOfRecall.RAZPIS_PREVOZNIK.ToString());
                        if (model.Content.RecallStatusChanged && (stat != null && model.Content.StatusID == stat.StatusOdpoklicaID))
                        {
                            DataTypesHelper.LogThis("SaveRecall3 : " + stat.Koda);
                            messageEventsRepo.CreateEmailForCarriers(model.Content, employee);
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
        public IHttpActionResult SaveBuyerRecall([FromBody] object recallData)
        {
            WebResponseContentModel<RecallBuyerFullModel> model = null;
            try
            {
                DataTypesHelper.LogThis("SaveBuyerRecall : (model.Content.OdpoklicID) - Začetek!");

                model = JsonConvert.DeserializeObject<WebResponseContentModel<RecallBuyerFullModel>>(recallData.ToString());

                if (model.Content != null)
                {

                    var employee = employeeRepo.GetEmployeeByID(model.Content.UserID);

                    if (model.Content.OdpoklicKupecID > 0)//We update existing record in DB
                    {
                        DataTypesHelper.LogThis("SaveBuyerRecall : (model.Content.OdpoklicKupecID) :" + model.Content.OdpoklicKupecID.ToString());

                         recallRepo.SaveBuyerRecall(model.Content);                   
                    }
                    else // We add and save new recod to DB 
                    {
                        DataTypesHelper.LogThis("SaveRecall2 : (model.Content.OdpoklicID) :" + model.Content.OdpoklicKupecID.ToString());
                        model.Content.OdpoklicKupecID = recallRepo.SaveBuyerRecall(model.Content, false);                        
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
        public IHttpActionResult DeleteRecall(int recallID)
        {
            WebResponseContentModel<bool> deleteRecall = new WebResponseContentModel<bool>();
            try
            {
                deleteRecall.Content = recallRepo.DeleteRecall(recallID);

                if (deleteRecall.Content)
                    deleteRecall.IsRequestSuccesful = true;
                else
                {
                    deleteRecall.IsRequestSuccesful = false;
                    deleteRecall.ValidationError = ValidationExceptionError.res_04;
                }
            }
            catch (Exception ex)
            {
                deleteRecall.IsRequestSuccesful = false;
                deleteRecall.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(deleteRecall);
            }

            return Json(deleteRecall);
        }

        [HttpGet]
        public IHttpActionResult DeleteBuyerRecall(int recallBuyerID)
        {
            WebResponseContentModel<bool> deleteRecall = new WebResponseContentModel<bool>();
            try
            {
                deleteRecall.Content = recallRepo.DeleteBuyerRecall(recallBuyerID);

                if (deleteRecall.Content)
                    deleteRecall.IsRequestSuccesful = true;
                else
                {
                    deleteRecall.IsRequestSuccesful = false;
                    deleteRecall.ValidationError = ValidationExceptionError.res_04;
                }
            }
            catch (Exception ex)
            {
                deleteRecall.IsRequestSuccesful = false;
                deleteRecall.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(deleteRecall);
            }

            return Json(deleteRecall);
        }
        #endregion

        #region RecallPositions

        [HttpGet]
        public IHttpActionResult GetRecallPositionByID(int recallPosID)
        {
            WebResponseContentModel<RecallPositionModel> tmpUser = new WebResponseContentModel<RecallPositionModel>();
            Del<RecallPositionModel> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = recallRepo.GetRecallPositionByID(recallPosID);
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
        public IHttpActionResult SaveRecallPosition([FromBody]object recallPosData)
        {
            WebResponseContentModel<RecallPositionModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<RecallPositionModel>>(recallPosData.ToString());

                if (model.Content != null)
                {
                    if (model.Content.OdpoklicPozicijaID > 0)//We update existing record in DB
                        recallRepo.SaveRecallPosition(model.Content);
                    else // We add and save new recod to DB 
                        model.Content.OdpoklicPozicijaID = recallRepo.SaveRecallPosition(model.Content, false);

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
        public IHttpActionResult SaveRecallPositions([FromBody]object recallPosData)
        {
            WebResponseContentModel<List<RecallPositionModel>> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<List<RecallPositionModel>>>(recallPosData.ToString());

                if (model.Content != null)
                {
                    recallRepo.SaveRecallPosition(model.Content);

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
        public IHttpActionResult DeleteRecallPosition(int recallPosID)
        {
            WebResponseContentModel<bool> deleteRecallPos = new WebResponseContentModel<bool>();
            try
            {
                deleteRecallPos.Content = recallRepo.DeleteRecallPosition(recallPosID);

                if (deleteRecallPos.Content)
                    deleteRecallPos.IsRequestSuccesful = true;
                else
                {
                    deleteRecallPos.IsRequestSuccesful = false;
                    deleteRecallPos.ValidationError = ValidationExceptionError.res_04;
                }
            }
            catch (Exception ex)
            {
                deleteRecallPos.IsRequestSuccesful = false;
                deleteRecallPos.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(deleteRecallPos);
            }

            return Json(deleteRecallPos);
        }

        [HttpPost]
        public IHttpActionResult GetLatestKolicinaOTPForProduct([FromBody]object recallPosData)
        {
            WebResponseContentModel<List<MaterialModel>> model = null;

            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<List<MaterialModel>>>(recallPosData.ToString());
                if (model.Content != null)
                {
                    model.Content = recallRepo.GetLatestKolicinaOTPForProduct(model.Content);

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
        public IHttpActionResult GetRecallPosFromPartialOverTakeRecalls([FromBody]object recallData)
        {
            WebResponseContentModel<List<RecallPositionModel>> model = new WebResponseContentModel<List<RecallPositionModel>>();

            try
            {
                WebResponseContentModel<List<int>> recallIDs = JsonConvert.DeserializeObject<WebResponseContentModel<List<int>>>(recallData.ToString());
                if (recallIDs.Content != null)
                {
                    model.Content = recallRepo.GetRecallPosFromPartialOverTakeRecalls(recallIDs.Content);

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

        #region RecallType

        [HttpGet]
        public IHttpActionResult GetRecallTypeByID(int typeID)
        {
            WebResponseContentModel<RecallType> tmpUser = new WebResponseContentModel<RecallType>();
            Del<RecallType> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = recallRepo.GetRecallTypeByID(typeID);
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
        public IHttpActionResult GetRecallTypeByCode(string typeCode)
        {
            WebResponseContentModel<RecallType> tmpUser = new WebResponseContentModel<RecallType>();
            Del<RecallType> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = recallRepo.GetRecallTypeByCode(typeCode);
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
        public IHttpActionResult GetRecallTypes()
        {
            WebResponseContentModel<List<RecallType>> tmpUser = new WebResponseContentModel<List<RecallType>>();
            Del<List<RecallType>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = recallRepo.GetRecallTypes();
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

        #region RecallStatus

        [HttpGet]
        public IHttpActionResult GetRecallStatusByID(int statusID)
        {
            WebResponseContentModel<RecallStatus> tmpUser = new WebResponseContentModel<RecallStatus>();
            Del<RecallStatus> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = recallRepo.GetRecallStatusByID(statusID);
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
        public IHttpActionResult GetRecallStatusByCode(string statusCode)
        {
            WebResponseContentModel<RecallStatus> tmpUser = new WebResponseContentModel<RecallStatus>();
            Del<RecallStatus> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = recallRepo.GetRecallStatusByCode(statusCode);
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
            WebResponseContentModel<List<RecallStatus>> tmpUser = new WebResponseContentModel<List<RecallStatus>>();
            Del<List<RecallStatus>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = recallRepo.GetRecallStatuses();
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

        #region Admin
        [HttpGet]
        public IHttpActionResult ResetSequentialNumInRecallPos()
        {
            WebResponseContentModel<bool> tmpUser = new WebResponseContentModel<bool>();
            Del<bool> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = recallRepo.ResetSequentialNumInRecallPos();
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
        public IHttpActionResult TakeOverConfirmedRecalls()
        {
            WebResponseContentModel<bool> tmpUser = new WebResponseContentModel<bool>();
            Del<bool> responseStatusHandler = ProcessContentModel;
            try
            {
                utilServiceRepo.CheckForOrderTakeOver2();
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
        public IHttpActionResult CreateAndSendOrdersMultiple()
        {
            WebResponseContentModel<bool> tmpUser = new WebResponseContentModel<bool>();
            Del<bool> responseStatusHandler = ProcessContentModel;
            try
            {
                utilServiceRepo.CreateAndSendOrdersMultiple();
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
        public IHttpActionResult CreatePDFAndSendPDOOrdersMultiple()
        {
            WebResponseContentModel<bool> tmpUser = new WebResponseContentModel<bool>();
            Del<bool> responseStatusHandler = ProcessContentModel;
            try
            {
                //utilServiceRepo.CreatePDFAndSendPDOOrdersMultiple();
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
        public IHttpActionResult CheckRecallsForCarriersSubmittingPrices()
        {
            WebResponseContentModel<bool> tmpUser = new WebResponseContentModel<bool>();
            Del<bool> responseStatusHandler = ProcessContentModel;
            try
            {
                utilServiceRepo.CheckRecallsForCarriersSubmittingPrices();
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
        public IHttpActionResult CheckForRecallsWithNoSubmitedPrices()
        {
            WebResponseContentModel<bool> tmpUser = new WebResponseContentModel<bool>();
            Del<bool> responseStatusHandler = ProcessContentModel;
            try
            {
                utilServiceRepo.CheckForRecallsWithNoSubmitedPrices();
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
        public IHttpActionResult CreateOrderTransport([FromBody]object CreateOrderData)
        {
            WebResponseContentModel<CreateOrderModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<CreateOrderModel>>(CreateOrderData.ToString());
                
                utilServiceRepo.CreateOrderTransport(model.Content);

                model.IsRequestSuccesful = true;
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
        public IHttpActionResult LaunchPantheonCreatePDF()
        {
            WebResponseContentModel<bool> tmpUser = new WebResponseContentModel<bool>();
            Del<bool> responseStatusHandler = ProcessContentModel;
            try
            {
                utilServiceRepo.LaunchPantheonCreatePDF();
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
        public IHttpActionResult CheckForOrderTakeOver2()
        {
            WebResponseContentModel<bool> tmpUser = new WebResponseContentModel<bool>();
            Del<bool> responseStatusHandler = ProcessContentModel;
            try
            {
                utilServiceRepo.CheckForOrderTakeOver2();
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
        public IHttpActionResult GetOrderPDFFile(int iRecallID)
        {
            WebResponseContentModel<string> tmpUser = new WebResponseContentModel<string>();
            Del<string> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content =  utilServiceRepo.GetOrderPDFFile(iRecallID);
                
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
        public IHttpActionResult ResetRecallStatusByID(int RecallID)
        {
            WebResponseContentModel<bool> resetOrder = new WebResponseContentModel<bool>();
            try
            {
                recallRepo.ResetRecallStatusByID(RecallID);
                resetOrder.IsRequestSuccesful = true;
                resetOrder.Content = true;

            }
            catch (Exception ex)
            {
                resetOrder.IsRequestSuccesful = false;
                resetOrder.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(resetOrder);
            }

            return Json(resetOrder);
        }

        [HttpGet]
        public IHttpActionResult ChangeConfigValue(string sConfigName, string sConfigValue)
        {
            WebResponseContentModel<bool> resetOrder = new WebResponseContentModel<bool>();
            try
            {
                var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                if (config.AppSettings.Settings[sConfigName] != null)
                {
                    config.AppSettings.Settings[sConfigName].Value = sConfigValue;
                }
                else { config.AppSettings.Settings.Add(sConfigName, sConfigValue); }
                config.Save();

                resetOrder.IsRequestSuccesful = true;
                resetOrder.Content = true;

            }
            catch (Exception ex)
            {
                resetOrder.IsRequestSuccesful = false;
                resetOrder.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(resetOrder);
            }

            return Json(resetOrder);
        }

        [HttpGet]
        public IHttpActionResult GetConfigValue(string sConfigName)
        {
            WebResponseContentModel<string> resetOrder = new WebResponseContentModel<string>();
            Del<string> responseStatusHandler = ProcessContentModel;
            try
            {
                string sReturn = "";

                var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                if (config.AppSettings.Settings[sConfigName] != null)
                {
                    sReturn = config.AppSettings.Settings[sConfigName].Value;
                }
                else { sReturn = ""; }

                resetOrder.Content = sReturn;
                responseStatusHandler(resetOrder);


            }
            catch (Exception ex)
            {
                resetOrder.IsRequestSuccesful = false;
                resetOrder.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(resetOrder);
            }

            return Json(resetOrder);
        }

        #endregion

        #region Recalls for carriers to submit their prices based on our tender values

        [HttpGet]
        public IHttpActionResult IsSubmittingPriceForCarrierStillValid(int prijavaPevoznikaID)
        {
            WebResponseContentModel<string> tmpUser = new WebResponseContentModel<string>();
            Del<string> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = recallRepo.IsPriceSubmittingStillValid(prijavaPevoznikaID);
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
        public IHttpActionResult SubmitPriceForCarrierTransport(int prijavaPevoznikaID, decimal newPrice)
        {
            WebResponseContentModel<bool> tmpUser = new WebResponseContentModel<bool>();
            Del<bool> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = recallRepo.SubmitPriceToPrijavaPrevoznika(prijavaPevoznikaID, newPrice);

                responseStatusHandler(tmpUser);
            }
            catch (Exception ex)
            {
                responseStatusHandler(tmpUser, ex);
                tmpUser.Content = false;
                return Json(tmpUser);
            }

            return Json(tmpUser);
        }

        [HttpGet]
        public IHttpActionResult GetCarriersInquiry(int recallID)
        {
            WebResponseContentModel<List<CarrierInquiryModel>> tmpUser = new WebResponseContentModel<List<CarrierInquiryModel>>();
            Del<List<CarrierInquiryModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = recallRepo.GetCarriersInquiry(recallID);

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
        public IHttpActionResult SavePrijavaPrevoznika([FromBody]object recallFullModel)
        {
            WebResponseContentModel<RecallFullModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<RecallFullModel>>(recallFullModel.ToString());

                if (model.Content != null)
                {
                    var employee = employeeRepo.GetEmployeeByID(model.Content.UserID);

                    messageEventsRepo.CreateEmailForCarriers(model.Content, employee);
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
        public IHttpActionResult ReSendEmailToCarriers([FromBody]object carrierInquiry)
        {
            WebResponseContentModel<List<CarrierInquiryModel>> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<List<CarrierInquiryModel>>>(carrierInquiry.ToString());

                if (model.Content != null)
                {
                    messageEventsRepo.CreateEmailForCarriers(model.Content);
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
        public IHttpActionResult ManualSelectCarrierForTransport(int prijavaPrevoznikaID)
        {
            WebResponseContentModel<bool> tmpUser = new WebResponseContentModel<bool>();
            Del<bool> responseStatusHandler = ProcessContentModel;
            try
            {
                recallRepo.ManualSelectCarrierForTransport(prijavaPrevoznikaID);

                tmpUser.Content = true;

                responseStatusHandler(tmpUser);
            }
            catch (Exception ex)
            {
                responseStatusHandler(tmpUser, ex);

                tmpUser.Content = false;

                return Json(tmpUser);
            }

            return Json(tmpUser);
        }

        [HttpGet]
        public IHttpActionResult DeleteCarrierInquiry(int prijavaPrevoznikaID)
        {
            WebResponseContentModel<bool> tmpUser = new WebResponseContentModel<bool>();
            Del<bool> responseStatusHandler = ProcessContentModel;
            try
            {
                recallRepo.DeleteCarrierInquiry(prijavaPrevoznikaID);

                tmpUser.Content = true;

                responseStatusHandler(tmpUser);
            }
            catch (Exception ex)
            {
                responseStatusHandler(tmpUser, ex);

                tmpUser.Content = false;

                return Json(tmpUser);
            }

            return Json(tmpUser);
        }

        #endregion

        #region Odpoklic kupec
        
        [HttpGet]
        public IHttpActionResult GetDisconnectedInvoices()
        {
            WebResponseContentModel<List<DisconnectedInvoicesModel>> tmpUser = new WebResponseContentModel<List<DisconnectedInvoicesModel>>();
            Del<List<DisconnectedInvoicesModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = sqlFunctionRepo.GetDisconnectedInvoices();
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
    }
}
