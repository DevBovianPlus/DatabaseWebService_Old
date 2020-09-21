using DatabaseWebService.Common;
using DatabaseWebService.DomainPDO.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsOTP.Client;
using DatabaseWebService.ModelsPDO;
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
    public class ClientPDOController : ApiController
    {
        private IClientPDORepository clientPdoRepo;

        public delegate WebResponseContentModel<T> Del<T>(WebResponseContentModel<T> model, Exception ex = null);
        public ClientPDOController(IClientPDORepository iclientPdoRepo)
        {
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
                        tmpUser.Content = clientPdoRepo.GetAllClientsByType(typeCode);
                    else
                        tmpUser.Content = clientPdoRepo.GetAllClientsByType(typeCode, employeeID);
                }
                else
                {
                    if (employeeID == 0)
                        tmpUser.Content = clientPdoRepo.GetAllClients();
                    else
                        tmpUser.Content = clientPdoRepo.GetAllClients(employeeID);
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
                    tmpUser.Content = clientPdoRepo.GetClientByID(clientID);
                else
                    tmpUser.Content = clientPdoRepo.GetClientByID(clientID, employeeID);

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
        public IHttpActionResult GetClientByCode(string sKoda)
        {
            WebResponseContentModel<ClientFullModel> tmpUser = new WebResponseContentModel<ClientFullModel>();
            Del<ClientFullModel> responseStatusHandler = ProcessContentModel;
            try
            {

                tmpUser.Content = clientPdoRepo.GetClientByCode(sKoda);

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

                        clientPdoRepo.SaveClient(model.Content);
                    }
                    else // We add and save new recod to DB 
                    {
                        //model.Content.KodaStranke = model.Content.KodaStranke + "0" + (clientRepo.GetClientsCountByCode(model.Content.KodaStranke) + 1).ToString();
                        model.Content.idStranka = clientPdoRepo.SaveClient(model.Content, false);
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
        public IHttpActionResult GetClientByNameOrInsert(string clientName)
        {
            WebResponseContentModel<ClientFullModel> Client = new WebResponseContentModel<ClientFullModel>();
            Del<ClientFullModel> responseStatusHandler = ProcessContentModel;

            try
            {
                clientName = clientName.IndexOf("{*}") > 0 ? clientName.Replace("{*}", "&") : clientName;

                Client.Content = clientPdoRepo.GetClientByNameOrInsert(clientName);
                responseStatusHandler(Client);
            }
            catch (Exception ex)
            {
                responseStatusHandler(Client, ex);
                return Json(Client);
            }

            return Json(Client);
        }



        [HttpGet]
        public IHttpActionResult DeleteClient(int clientID)
        {
            WebResponseContentModel<bool> deleteClient = new WebResponseContentModel<bool>();
            try
            {
                deleteClient.Content = clientPdoRepo.DeleteClient(clientID);

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
                tmpUser.Content = clientPdoRepo.GetClientByName(clientName);
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
                tmpUser.Content = clientPdoRepo.GetClientTypeByCode(typeCode);

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
                tmpUser.Content = clientPdoRepo.GetClientTypeByID(id);

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
                tmpUser.Content = clientPdoRepo.GetClientTypes();

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
            WebResponseContentModel<List<LanguageModel>> tmpUser = new WebResponseContentModel<List<LanguageModel>>();
            Del<List<LanguageModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = clientPdoRepo.GetLanguages();

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
        public IHttpActionResult GetDepartments()
        {
            WebResponseContentModel<List<DepartmentModel>> tmpUser = new WebResponseContentModel<List<DepartmentModel>>();
            Del<List<DepartmentModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = clientPdoRepo.GetDepartments();

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
                        clientPdoRepo.SaveClientEmployee(returnModel.Content);
                    else // We add and save new recod to DB 
                        returnModel.Content.idStrankaOsebe = clientPdoRepo.SaveClientEmployee(returnModel.Content, false);

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
                isDeleteSuccess.Content = clientPdoRepo.DeleteClientEmployee(clientID, employeeID);

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
                isInDatabase.Content = clientPdoRepo.ClientEmployeeExist(clientID, employeeID);

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
                        clientPdoRepo.SaveContactPerson(returnModel.Content);
                    else // We add and save new recod to DB 
                        returnModel.Content.idKontaktneOsebe = clientPdoRepo.SaveContactPerson(returnModel.Content, false);

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
                isDeleteSuccess.Content = clientPdoRepo.DeleteContactPerson(contactPersonID, clientID);

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

        [HttpGet]
        public IHttpActionResult GetContactPersonModelListByName(string SupplierName)
        {
            WebResponseContentModel<List<ContactPersonModel>> tmpUser = new WebResponseContentModel<List<ContactPersonModel>>();

            try
            {
                tmpUser.Content = clientPdoRepo.GetContactPersonModelListByName(SupplierName);

                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_02;
                }
            }
            catch (Exception ex)
            {
                tmpUser.IsRequestSuccesful = false;
                tmpUser.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(tmpUser);
            }

            return Json(tmpUser);
        }

        [HttpGet]
        public IHttpActionResult GetContactPersonModelListByClientID(int ClientID)
        {
            WebResponseContentModel<List<ContactPersonModel>> tmpUser = new WebResponseContentModel<List<ContactPersonModel>>();

            try
            {
                tmpUser.Content = clientPdoRepo.GetContactPersonModelList(ClientID);

                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_02;
                }
            }
            catch (Exception ex)
            {
                tmpUser.IsRequestSuccesful = false;
                tmpUser.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(tmpUser);
            }

            return Json(tmpUser);
        }
        #endregion

    }
}
