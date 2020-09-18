using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DatabaseWebService.Common;
using DatabaseWebService.Domain;
using DatabaseWebService.Domain.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Resources;
using DatabaseWebService.Models.Client;
using Newtonsoft.Json;
using System.IO;
using System.Data.Entity.Validation;
using DatabaseWebService.Common.Enums;

namespace DatabaseWebService.Controllers
{
    public class ClientController : ApiController
    {
        IClientRepository clientRepo;
        ISystemMessageEventsRepository messageEventsRepo;
        IChartsRepository chartsRepo;
        public ClientController(IClientRepository iClientRepo, ISystemMessageEventsRepository isystemMessageRepo, IChartsRepository chartsRepo)
        {
            clientRepo = iClientRepo;
            messageEventsRepo = isystemMessageRepo;
            this.chartsRepo = chartsRepo;
        }

        [HttpGet]
        public IHttpActionResult GetAllClients(int employeeID = 0)
        {
            WebResponseContentModel<List<ClientSimpleModel>> tmpUser = new WebResponseContentModel<List<ClientSimpleModel>>();

            try
            {
                if(employeeID == 0)
                    tmpUser.Content = clientRepo.GetAllClients();
                else
                    tmpUser.Content = clientRepo.GetAllClients(employeeID);

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
        public IHttpActionResult GetClientByID(int clientID, int employeeID = 0)
        {
            WebResponseContentModel<ClientFullModel> tmpUser = new WebResponseContentModel<ClientFullModel>();

            try
            {
                if (employeeID == 0)
                    tmpUser.Content = clientRepo.GetClientByID(clientID);
                else
                    tmpUser.Content = clientRepo.GetClientByID(clientID, employeeID);

                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_03;
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
                        
                        clientRepo.SaveClient(model.Content); 
                    }
                    else // We add and save new recod to DB 
                    {
                        //model.Content.KodaStranke = model.Content.KodaStranke + "0" + (clientRepo.GetClientsCountByCode(model.Content.KodaStranke) + 1).ToString();
                        model.Content.idStranka = clientRepo.SaveClient(model.Content, false);
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
                //model.ValidationError = ExceptionValidationHelper.InnerExceptionExist(ex.InnerException);
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
                deleteClient.Content = clientRepo.DeleteClient(clientID);

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
        #region Plan
        [HttpPost]
        public IHttpActionResult SavePlanToClient([FromBody] object planData)
        {
            WebResponseContentModel<PlanModel> returnModel = new WebResponseContentModel<PlanModel>();
            
            try
            {
                returnModel = JsonConvert.DeserializeObject<WebResponseContentModel<PlanModel>>(planData.ToString());

                if (returnModel.Content != null)
                {
                    if (returnModel.Content.idPlan > 0)//We update existing record in DB
                        clientRepo.SaveClientPlan(returnModel.Content);
                    else // We add and save new recod to DB 
                        returnModel.Content.idPlan = clientRepo.SaveClientPlan(returnModel.Content, false);

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
        public IHttpActionResult DeletePlan(int planID, int clientID)
        {
            WebResponseContentModel<bool> isDeleteSuccess = new WebResponseContentModel<bool>();
            try
            {
                isDeleteSuccess.Content = clientRepo.DeletePlan(planID, clientID);

                if (isDeleteSuccess.Content)
                    isDeleteSuccess.IsRequestSuccesful = true;
                else
                {
                    isDeleteSuccess.IsRequestSuccesful = false;
                    isDeleteSuccess.ValidationError = ValidationExceptionError.res_05;
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
        #region  ContactPerson
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
                        clientRepo.SaveContactPerson(returnModel.Content);
                    else // We add and save new recod to DB 
                        returnModel.Content.idKontaktneOsebe = clientRepo.SaveContactPerson(returnModel.Content, false);

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
                isDeleteSuccess.Content = clientRepo.DeleteContactPerson(contactPersonID, clientID);

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

        [HttpGet]
        public IHttpActionResult GetAllCategories()
        {
            WebResponseContentModel<List<CategorieModel>> categories = new WebResponseContentModel<List<CategorieModel>>();
            try
            {
                categories.Content = clientRepo.GetCategories();

                if (categories.Content != null)
                    categories.IsRequestSuccesful = true;
                else
                {
                    categories.IsRequestSuccesful = false;
                    categories.ValidationError = ValidationExceptionError.res_11;
                }
            }
            catch (Exception ex)
            {
                categories.IsRequestSuccesful = false;
                categories.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(categories);
            }

            return Json(categories);
        }

        [HttpGet]
        public IHttpActionResult GetAllFreeClientCategories(int clientID, int catToSkip = 0)
        {
            WebResponseContentModel<List<CategorieModel>> categories = new WebResponseContentModel<List<CategorieModel>>();
            try
            {
                categories.Content = clientRepo.GetClientFreeCategories(clientID, catToSkip);

                if (categories.Content != null)
                    categories.IsRequestSuccesful = true;
                else
                {
                    categories.IsRequestSuccesful = false;
                    categories.ValidationError = ValidationExceptionError.res_11;
                }
            }
            catch (Exception ex)
            {
                categories.IsRequestSuccesful = false;
                categories.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(categories);
            }

            return Json(categories);
        }

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
                        clientRepo.SaveClientEmployee(returnModel.Content);
                    else // We add and save new recod to DB 
                        returnModel.Content.idStrankaOsebe = clientRepo.SaveClientEmployee(returnModel.Content, false);

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
                isDeleteSuccess.Content = clientRepo.DeleteClientEmployee(clientID, employeeID);

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
                isInDatabase.Content = clientRepo.ClientEmployeeExist(clientID, employeeID);

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

        #region Devices
        [HttpPost]
        public IHttpActionResult SaveDeviceData([FromBody] object deviceData)
        {
            WebResponseContentModel<DevicesModel> returnModel = new WebResponseContentModel<DevicesModel>();

            try
            {
                returnModel = JsonConvert.DeserializeObject<WebResponseContentModel<DevicesModel>>(deviceData.ToString());

                if (returnModel.Content != null)
                {
                    if (returnModel.Content.idNaprava > 0)//We update existing record in DB
                        clientRepo.SaveDevice(returnModel.Content);
                    else // We add and save new recod to DB 
                    {
                        returnModel.Content.Koda = returnModel.Content.Koda + "0" + (clientRepo.GetDevicesCountByCode(returnModel.Content.Koda) + 1).ToString();
                        returnModel.Content.idNaprava = clientRepo.SaveDevice(returnModel.Content, false);
                    }

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
        public IHttpActionResult DeleteDevice(int deviceID, int clientID)
        {
            WebResponseContentModel<bool> isDeleteSuccess = new WebResponseContentModel<bool>();
            try
            {
                isDeleteSuccess.Content = clientRepo.DeleteDevice(deviceID, clientID);

                if (isDeleteSuccess.Content)
                    isDeleteSuccess.IsRequestSuccesful = true;
                else
                {
                    isDeleteSuccess.IsRequestSuccesful = false;
                    isDeleteSuccess.ValidationError = ValidationExceptionError.res_20;
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
        public IHttpActionResult GetClientsCodeCout(string code)
        {
            WebResponseContentModel<int> isDeleteSuccess = new WebResponseContentModel<int>();
            try
            {
                isDeleteSuccess.Content = clientRepo.GetClientsCountByCode(code);

                if (isDeleteSuccess.Content > 0)
                    isDeleteSuccess.IsRequestSuccesful = true;
                else
                {
                    isDeleteSuccess.IsRequestSuccesful = false;
                    isDeleteSuccess.ValidationError = ValidationExceptionError.res_20;
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

        #region Notes
        [HttpPost]
        public IHttpActionResult SaveNotesData([FromBody] object notesData)
        {
            WebResponseContentModel<NotesModel> returnModel = new WebResponseContentModel<NotesModel>();

            try
            {
                returnModel = JsonConvert.DeserializeObject<WebResponseContentModel<NotesModel>>(notesData.ToString());

                if (returnModel.Content != null)
                {
                    if (returnModel.Content.idOpombaStranka > 0)//We update existing record in DB
                        clientRepo.SaveNotes(returnModel.Content);
                    else // We add and save new recod to DB 
                    {                        
                        returnModel.Content.idOpombaStranka = clientRepo.SaveNotes(returnModel.Content, false);
                    }

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
        public IHttpActionResult DeleteNotes(int NotesID, int clientID)
        {
            WebResponseContentModel<bool> isDeleteSuccess = new WebResponseContentModel<bool>();
            try
            {
                isDeleteSuccess.Content = clientRepo.DeleteDevice(NotesID, clientID);

                if (isDeleteSuccess.Content)
                    isDeleteSuccess.IsRequestSuccesful = true;
                else
                {
                    isDeleteSuccess.IsRequestSuccesful = false;
                    isDeleteSuccess.ValidationError = ValidationExceptionError.res_20;
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
        public IHttpActionResult GetNotesCodeCout(string code)
        {
            WebResponseContentModel<int> isDeleteSuccess = new WebResponseContentModel<int>();
            try
            {
                isDeleteSuccess.Content = clientRepo.GetClientsCountByCode(code);

                if (isDeleteSuccess.Content > 0)
                    isDeleteSuccess.IsRequestSuccesful = true;
                else
                {
                    isDeleteSuccess.IsRequestSuccesful = false;
                    isDeleteSuccess.ValidationError = ValidationExceptionError.res_20;
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

        #region ClientCategorie
        [HttpPost]
        public IHttpActionResult SaveClientCategorie([FromBody] object clientCategorieData)
        {
            WebResponseContentModel<ClientCategorieModel> returnModel = new WebResponseContentModel<ClientCategorieModel>();

            try
            {
                returnModel = JsonConvert.DeserializeObject<WebResponseContentModel<ClientCategorieModel>>(clientCategorieData.ToString());

                if (returnModel.Content != null)
                {
                    if (returnModel.Content.idStrankaKategorija > 0)//We update existing record in DB
                    { 
                        clientRepo.SaveClientCategorie(returnModel.Content);
                        returnModel.Content.Kategorija = clientRepo.GetCategorieByID(returnModel.Content.idKategorija);
                    }
                    else// We add and save new recod to DB 
                    {
                        returnModel.Content.idStrankaKategorija = clientRepo.SaveClientCategorie(returnModel.Content, false);
                        returnModel.Content.Kategorija = clientRepo.GetCategorieByID(returnModel.Content.idKategorija);
                        ChartRenderModel chartData = chartsRepo.GetDataForChart(returnModel.Content.idStranka, returnModel.Content.idKategorija, (int)Enums.ChartRenderPeriod.MESECNO, (int)Enums.ChartRenderType.KOLICINA);

                        if (chartData != null)
                        {
                            if (chartData.chartRenderData.Count > 0) returnModel.Content.HasChartDataForCategorie = true;
                        }
                    }

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
        public IHttpActionResult DeleteClientCategorie(int clientID, int clientCategorieID)
        {
            WebResponseContentModel<bool> isDeleteSuccess = new WebResponseContentModel<bool>();
            try
            {
                isDeleteSuccess.Content = clientRepo.DeleteClientCategorie(clientID, clientCategorieID);

                if (isDeleteSuccess.Content)
                    isDeleteSuccess.IsRequestSuccesful = true;
                else
                {
                    isDeleteSuccess.IsRequestSuccesful = false;
                    isDeleteSuccess.ValidationError = ValidationExceptionError.res_24;
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

        #region Filtering GridView
        [HttpGet]
        public IHttpActionResult GetClientsByFilter(string propertyName, string containsValue)
        {
            WebResponseContentModel<List<ClientSimpleModel>> clients = new WebResponseContentModel<List<ClientSimpleModel>>();
            try
            {
                clients.Content = clientRepo.FilterClientsByPropertyNames(propertyName, containsValue);

                if (clients.Content != null)
                    clients.IsRequestSuccesful = true;
                else
                {
                    clients.IsRequestSuccesful = false;
                    clients.ValidationError = ValidationExceptionError.res_02;
                }
            }
            catch (Exception ex)
            {
                clients.IsRequestSuccesful = false;
                clients.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(clients);
            }

            return Json(clients);
        }
        #endregion
    }

}
