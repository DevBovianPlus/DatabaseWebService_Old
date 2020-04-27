using DatabaseWebService.Common;
using DatabaseWebService.Domain;
using DatabaseWebService.Domain.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DatabaseWebService.Models.EmailMessage;
using DatabaseWebService.Models.Event;
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
    public class EventController : ApiController
    {
        IClientRepository clientRepo;
        IEventRepository eventRepo;
        IEmployeeRepository employeeRepo;
        ISystemMessageEventsRepository messageEventsRepo;
        public EventController(IClientRepository iClientRepo, IEventRepository iEventRepo, IEmployeeRepository iEmployeeRepo, ISystemMessageEventsRepository iSysteMessagRepo)
        {
            clientRepo = iClientRepo;
            eventRepo = iEventRepo;
            employeeRepo = iEmployeeRepo;
            messageEventsRepo = iSysteMessagRepo;
        }

        [HttpGet]
        public IHttpActionResult GetAllEvents(int employeeID = 0)
        {
            WebResponseContentModel<List<EventSimpleModel>> tmpUser = new WebResponseContentModel<List<EventSimpleModel>>();

            try
            {
                if (employeeID == 0)
                    tmpUser.Content = eventRepo.GetAllEventModelList();
                else
                    tmpUser.Content = eventRepo.GetAllEventModelList(employeeID);

                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_15;
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
        public IHttpActionResult GetEventByID(int eventID, int employeeID = 0)
        {
            WebResponseContentModel<EventFullModel> tmpUser = new WebResponseContentModel<EventFullModel>();

            try
            {
                if (employeeID == 0)
                    tmpUser.Content = eventRepo.GetEvent(eventID);
                else
                    tmpUser.Content = eventRepo.GetEvent(eventID, employeeID);

                if (tmpUser.Content != null)
                {
                    if (tmpUser.Content.idKategorija > 0)
                        tmpUser.Content.Kategorija = clientRepo.GetCategorieByID(tmpUser.Content.idKategorija.Value);
                    if (tmpUser.Content.Izvajalec > 0)
                        tmpUser.Content.OsebeIzvajalec = employeeRepo.GetEmployeeByID(tmpUser.Content.Izvajalec.Value);
                    if (tmpUser.Content.Skrbnik > 0)
                        tmpUser.Content.OsebeSkrbnik = employeeRepo.GetEmployeeByID(tmpUser.Content.Skrbnik.Value);
                    //if(model.idStatus > 0)
                    //  model.StatusDogodek = //TODO
                    if (tmpUser.Content.idStranka > 0)
                        tmpUser.Content.Stranka = clientRepo.GetClientSimpleModelByCode(tmpUser.Content.idStranka.Value);

                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_15;
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
        public IHttpActionResult GetEmployeesByRoleID(int roleID)
        {
            WebResponseContentModel<List<EmployeeSimpleModel>> tmpUser = new WebResponseContentModel<List<EmployeeSimpleModel>>();

            try
            {
                tmpUser.Content = employeeRepo.GetEmployeesByRoleID(roleID);

                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_16;
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
        public IHttpActionResult GetEventStatuses()
        {
            WebResponseContentModel<List<EventStatusModel>> tmpUser = new WebResponseContentModel<List<EventStatusModel>>();

            try
            {
                tmpUser.Content = eventRepo.GetEventStatuses();

                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_17;
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
        public IHttpActionResult SaveEventData([FromBody]object eventData)
        {
            WebResponseContentModel<EventFullModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<EventFullModel>>(eventData.ToString());

                //messageEventsRepo.ProcessEventMessage(new SystemMessageEvents() { Code = model.Content.Code, MasterID = model.Content.MasterID, Status = model.Content.Status, SystemMessageEventID = model.Content.ID });

                if (model.Content != null)
                {
                    if (model.Content.idDogodek > 0)//We update existing record in DB
                        eventRepo.SaveEvent(model.Content);
                    else // We add and save new recod to DB 
                        model.Content.idDogodek = eventRepo.SaveEvent(model.Content, false);

                    if (model.Content.emailModel != null)
                    {
                        model.Content.emailModel.MasterID = model.Content.idDogodek;
                        messageEventsRepo.SaveEmailEventMessage(model.Content.emailModel);
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
        public IHttpActionResult DeleteClient(int eventID)
        {
            WebResponseContentModel<bool> deleteClient = new WebResponseContentModel<bool>();
            try
            {
                deleteClient.Content = eventRepo.DeleteEvent(eventID);

                if (deleteClient.Content)
                    deleteClient.IsRequestSuccesful = true;
                else
                {
                    deleteClient.IsRequestSuccesful = false;
                    deleteClient.ValidationError = ValidationExceptionError.res_18;
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

        [HttpPost]
        public IHttpActionResult SaveMessageData([FromBody]object messageData)
        {
            WebResponseContentModel<MessageModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<MessageModel>>(messageData.ToString());

                if (model.Content != null)
                {
                    if (model.Content.idSporocila > 0)//We update existing record in DB
                        eventRepo.SaveMessage(model.Content);
                    else // We add and save new recod to DB 
                        model.Content.idSporocila = eventRepo.SaveMessage(model.Content, false);

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
        public IHttpActionResult DeleteMessage(int messageID, int eventID)
        {
            WebResponseContentModel<bool> deleteClient = new WebResponseContentModel<bool>();
            try
            {
                deleteClient.Content = eventRepo.DeleteMessage(messageID, eventID);

                if (deleteClient.Content)
                    deleteClient.IsRequestSuccesful = true;
                else
                {
                    deleteClient.IsRequestSuccesful = false;
                    deleteClient.ValidationError = ValidationExceptionError.res_19;
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
        public IHttpActionResult GetEmployeeSupervisorByEmployeeID(int employeeID)
        {
            WebResponseContentModel<List<EmployeeSimpleModel>> tmpUser = new WebResponseContentModel<List<EmployeeSimpleModel>>();

            try
            {
                tmpUser.Content = employeeRepo.GetEmployeeSupervisorByID(employeeID);

                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_26;
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
        public IHttpActionResult SaveEventMeetingData([FromBody]object eventMeetingData)
        {
            WebResponseContentModel<EventMeetingModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<EventMeetingModel>>(eventMeetingData.ToString());

                if (model.Content != null)
                {
                    if (model.Content.DogodekSestanekID > 0)//We update existing record in DB
                        eventRepo.SaveEventMeeting(model.Content);
                    else // We add and save new recod to DB 
                        model.Content.DogodekSestanekID = eventRepo.SaveEventMeeting(model.Content, false);

                    EmployeeFullModel tmp = employeeRepo.GetEmployeeByID(model.Content.tsIDOsebe);
                    if (tmp != null)
                        model.Content.ImeInPriimekOsebe = tmp.Ime + " " + tmp.Priimek;

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
        public IHttpActionResult DeleteEventMeeting(int eventMeetingID, int eventID)
        {
            WebResponseContentModel<bool> deleteClient = new WebResponseContentModel<bool>();
            try
            {
                deleteClient.Content = eventRepo.DeleteEventMeeting(eventMeetingID, eventID);

                if (deleteClient.Content)
                    deleteClient.IsRequestSuccesful = true;
                else
                {
                    deleteClient.IsRequestSuccesful = false;
                    deleteClient.ValidationError = ValidationExceptionError.res_19;
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
    }
}
