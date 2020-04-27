using DatabaseWebService.Common;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsOTP;
using DatabaseWebService.ModelsOTP.Client;
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
    public class ClientOTPController : ApiController
    {
        private IClientOTPRepository clientOtpRepo;

        public delegate WebResponseContentModel<T> Del<T>(WebResponseContentModel<T> model, Exception ex = null);
        public ClientOTPController(IClientOTPRepository iclientOtpRepo)
        {
            clientOtpRepo = iclientOtpRepo;
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

        #region Client
        [HttpGet]
        public IHttpActionResult GetAllClients(int employeeID = 0, string typeCode = "")
        {
            WebResponseContentModel<List<ClientSimpleModel>> tmpUser = new WebResponseContentModel<List<ClientSimpleModel>>();
            Del<List<ClientSimpleModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                if (!String.IsNullOrEmpty(typeCode))
                {
                    if (employeeID == 0)
                        tmpUser.Content = clientOtpRepo.GetAllClientsByType(typeCode);
                    else
                        tmpUser.Content = clientOtpRepo.GetAllClientsByType(employeeID, typeCode);
                }
                else
                {
                    if (employeeID == 0)
                        tmpUser.Content = clientOtpRepo.GetAllClients();
                    else
                        tmpUser.Content = clientOtpRepo.GetAllClients(employeeID);
                }
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
        public IHttpActionResult GetClientByID(int clientID, int employeeID = 0)
        {
            WebResponseContentModel<ClientFullModel> tmpUser = new WebResponseContentModel<ClientFullModel>();
            Del<ClientFullModel> responseStatusHandler = ProcessContentModel;
            try
            {
                if (employeeID == 0)
                    tmpUser.Content = clientOtpRepo.GetClientByID(clientID);
                else
                    tmpUser.Content = clientOtpRepo.GetClientByID(clientID, employeeID);

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
        public IHttpActionResult SaveClientData([FromBody]object clientData)
        {
            WebResponseContentModel<ClientFullModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<ClientFullModel>>(clientData.ToString());

                if (model.Content != null)
                {
                    if (model.Content.idStranka > 0)//We update existing record in DB
                    {
                        //if (model.Content.KodaStranke.Length <= 3)//when add client was executed by tab change the default code was saved. So od the secod saving we add letters form NazivPrvi and the appropriate number
                        //  model.Content.KodaStranke = model.Content.KodaStranke + "0" + (clientRepo.GetClientsCountByCode(model.Content.KodaStranke) + 1).ToString();

                        clientOtpRepo.SaveClient(model.Content);
                    }
                    else // We add and save new recod to DB 
                    {
                        //model.Content.KodaStranke = model.Content.KodaStranke + "0" + (clientRepo.GetClientsCountByCode(model.Content.KodaStranke) + 1).ToString();
                        model.Content.idStranka = clientOtpRepo.SaveClient(model.Content, false);
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
        public IHttpActionResult DeleteClient(int clientID)
        {
            WebResponseContentModel<bool> deleteClient = new WebResponseContentModel<bool>();
            try
            {
                deleteClient.Content = clientOtpRepo.DeleteClient(clientID);

                if (deleteClient.Content)
                    deleteClient.IsRequestSuccesful = true;
                else
                {
                    deleteClient.IsRequestSuccesful = false;
                    deleteClient.ValidationError = ValidationExceptionError.res_04;
                }
            }
            catch (Exception ex)
            {
                deleteClient.IsRequestSuccesful = false;
                deleteClient.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(deleteClient);
            }

            return Json(deleteClient);
        }

        [HttpGet]
        public IHttpActionResult GetClientByName(string clientName)
        {
            WebResponseContentModel<ClientFullModel> tmpUser = new WebResponseContentModel<ClientFullModel>();
            Del<ClientFullModel> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = clientOtpRepo.GetClientByName(clientName);
                tmpUser.IsRequestSuccesful = true;
                //responseStatusHandler(tmpUser);
            }
            catch (Exception ex)
            {
                responseStatusHandler(tmpUser, ex);
                return Json(tmpUser);
            }

            return Json(tmpUser);
        }
        #endregion

        #region CLientType

        [HttpGet]
        public IHttpActionResult GetClientTypeByCode(string typeCode)
        {
            WebResponseContentModel<ClientType> tmpUser = new WebResponseContentModel<ClientType>();
            Del<ClientType> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = clientOtpRepo.GetClientTypeByCode(typeCode);

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
        public IHttpActionResult GetClientTypeByID(int id)
        {
            WebResponseContentModel<ClientType> tmpUser = new WebResponseContentModel<ClientType>();
            Del<ClientType> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = clientOtpRepo.GetClientTypeByID(id);

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
        public IHttpActionResult GetClientTypes()
        {
            WebResponseContentModel<List<ClientType>> tmpUser = new WebResponseContentModel<List<ClientType>>();
            Del<List<ClientType>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = clientOtpRepo.GetClientTypes();

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
        public IHttpActionResult GetLanguages()
        {
            WebResponseContentModel<List<LanguageModelOTP>> tmpUser = new WebResponseContentModel<List<LanguageModelOTP>>();
            Del<List<LanguageModelOTP>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = clientOtpRepo.GetLanguages();

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

        #region ClientEmployee
        [HttpPost]
        public IHttpActionResult SaveClientEmployee([FromBody] object clientEmployeeData)
        {
            WebResponseContentModel<ClientEmployeeModel> returnModel = new WebResponseContentModel<ClientEmployeeModel>();

            try
            {
                returnModel = JsonConvert.DeserializeObject<WebResponseContentModel<ClientEmployeeModel>>(clientEmployeeData.ToString());

                if (returnModel.Content != null)
                {
                    if (returnModel.Content.idStrankaOsebe > 0)//We update existing record in DB
                        clientOtpRepo.SaveClientEmployee(returnModel.Content);
                    else // We add and save new recod to DB 
                        returnModel.Content.idStrankaOsebe = clientOtpRepo.SaveClientEmployee(returnModel.Content, false);

                    returnModel.IsRequestSuccesful = true;
                }
                else
                {
                    returnModel.IsRequestSuccesful = false;
                    returnModel.ValidationError = ValidationExceptionError.res_09;
                }
            }
            catch (Exception ex)
            {
                returnModel.IsRequestSuccesful = false;
                returnModel.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(returnModel);
            }

            return Json(returnModel);
        }

        [HttpGet]
        public IHttpActionResult DeleteClientEmployee(int clientID, int employeeID)
        {
            WebResponseContentModel<bool> isDeleteSuccess = new WebResponseContentModel<bool>();
            try
            {
                isDeleteSuccess.Content = clientOtpRepo.DeleteClientEmployee(clientID, employeeID);

                if (isDeleteSuccess.Content)
                    isDeleteSuccess.IsRequestSuccesful = true;
                else
                {
                    isDeleteSuccess.IsRequestSuccesful = false;
                    isDeleteSuccess.ValidationError = ValidationExceptionError.res_13;
                }
            }
            catch (Exception ex)
            {
                isDeleteSuccess.IsRequestSuccesful = false;
                isDeleteSuccess.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(isDeleteSuccess);
            }

            return Json(isDeleteSuccess);
        }

        [HttpGet]
        public IHttpActionResult ClientEmployeeExist(int clientID, int employeeID)
        {
            WebResponseContentModel<bool> isInDatabase = new WebResponseContentModel<bool>();
            try
            {
                isInDatabase.Content = clientOtpRepo.ClientEmployeeExist(clientID, employeeID);

                if (isInDatabase.Content)
                    isInDatabase.IsRequestSuccesful = true;
                else
                {
                    isInDatabase.IsRequestSuccesful = false;
                    isInDatabase.ValidationError = ValidationExceptionError.res_14;
                }
            }
            catch (Exception ex)
            {
                isInDatabase.IsRequestSuccesful = false;
                isInDatabase.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(isInDatabase);
            }

            return Json(isInDatabase);
        }
        #endregion

        #region ContactPerson
        [HttpPost]
        public IHttpActionResult SaveContactPersonToClient([FromBody] object contactPersonData)
        {
            WebResponseContentModel<ContactPersonModel> returnModel = new WebResponseContentModel<ContactPersonModel>();

            try
            {
                returnModel = JsonConvert.DeserializeObject<WebResponseContentModel<ContactPersonModel>>(contactPersonData.ToString());

                if (returnModel.Content != null)
                {
                    if (returnModel.Content.idKontaktneOsebe > 0)//We update existing record in DB
                        clientOtpRepo.SaveContactPerson(returnModel.Content);
                    else // We add and save new recod to DB 
                        returnModel.Content.idKontaktneOsebe = clientOtpRepo.SaveContactPerson(returnModel.Content, false);

                    returnModel.IsRequestSuccesful = true;
                }
                else
                {
                    returnModel.IsRequestSuccesful = false;
                    returnModel.ValidationError = ValidationExceptionError.res_09;
                }
            }
            catch (Exception ex)
            {
                returnModel.IsRequestSuccesful = false;
                returnModel.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(returnModel);
            }

            return Json(returnModel);
        }

        [HttpGet]
        public IHttpActionResult DeleteContactPerson(int contactPersonID, int clientID)
        {
            WebResponseContentModel<bool> isDeleteSuccess = new WebResponseContentModel<bool>();
            try
            {
                isDeleteSuccess.Content = clientOtpRepo.DeleteContactPerson(contactPersonID, clientID);

                if (isDeleteSuccess.Content)
                    isDeleteSuccess.IsRequestSuccesful = true;
                else
                {
                    isDeleteSuccess.IsRequestSuccesful = false;
                    isDeleteSuccess.ValidationError = ValidationExceptionError.res_10;
                }
            }
            catch (Exception ex)
            {
                isDeleteSuccess.IsRequestSuccesful = false;
                isDeleteSuccess.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(isDeleteSuccess);
            }

            return Json(isDeleteSuccess);
        }
        #endregion

        #region ClientTransportType
        [HttpGet]
        public IHttpActionResult GetAllTransportTypes()
        {
            WebResponseContentModel<List<ClientTransportType>> tmpUser = new WebResponseContentModel<List<ClientTransportType>>();
            Del<List<ClientTransportType>> responseStatusHandler = ProcessContentModel;
            try
            {

                tmpUser.Content = clientOtpRepo.GetClientTransportTypes();

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
        public IHttpActionResult GetTransportTypeByID(int transportTypeID)
        {
            WebResponseContentModel<ClientTransportType> tmpUser = new WebResponseContentModel<ClientTransportType>();
            Del<ClientTransportType> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = clientOtpRepo.GetClientTransportTypeByID(transportTypeID);

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
        public IHttpActionResult SaveTransportTypeData([FromBody]object clientData)
        {
            WebResponseContentModel<ClientTransportType> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<ClientTransportType>>(clientData.ToString());

                if (model.Content != null)
                {
                    if (model.Content.TipPrevozaID > 0)//We update existing record in DB
                    {
                        //if (model.Content.KodaStranke.Length <= 3)//when add client was executed by tab change the default code was saved. So od the secod saving we add letters form NazivPrvi and the appropriate number
                        //  model.Content.KodaStranke = model.Content.KodaStranke + "0" + (clientRepo.GetClientsCountByCode(model.Content.KodaStranke) + 1).ToString();

                        clientOtpRepo.SaveClientTransportType(model.Content);
                    }
                    else // We add and save new recod to DB 
                    {
                        //model.Content.KodaStranke = model.Content.KodaStranke + "0" + (clientRepo.GetClientsCountByCode(model.Content.KodaStranke) + 1).ToString();
                        model.Content.TipPrevozaID = clientOtpRepo.SaveClientTransportType(model.Content, false);
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
        public IHttpActionResult DeleteTransportType(int transportTypeID)
        {
            WebResponseContentModel<bool> deleteTransportType = new WebResponseContentModel<bool>();
            try
            {
                deleteTransportType.Content = clientOtpRepo.DeleteClientTransportType(transportTypeID);

                if (deleteTransportType.Content)
                    deleteTransportType.IsRequestSuccesful = true;
                else
                {
                    deleteTransportType.IsRequestSuccesful = false;
                    deleteTransportType.ValidationError = ValidationExceptionError.res_04;
                }
            }
            catch (Exception ex)
            {
                deleteTransportType.IsRequestSuccesful = false;
                deleteTransportType.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(deleteTransportType);
            }

            return Json(deleteTransportType);
        }
        #endregion
    }
}
