using DatabaseWebService.Common;
using DatabaseWebService.Domain.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Models.EmailMessage;
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
    public class EmailMessageController : ApiController
    {
        private ISystemMessageEventsRepository systemEmailMessageEventsRepo;

        public EmailMessageController(ISystemMessageEventsRepository emailMessageEventsRepo)
        {
            systemEmailMessageEventsRepo = emailMessageEventsRepo;
        }

        [HttpPost]
        public IHttpActionResult SaveSystemMessageData([FromBody]object eventMessageData)
        {
            WebResponseContentModel<EmailMessageModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<EmailMessageModel>>(eventMessageData.ToString());

                if (model.Content != null)
                {
                    systemEmailMessageEventsRepo.SaveEmailEventMessage(model.Content);
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
        public IHttpActionResult Get()
        {
            return Ok("Controller here");
        }
    }
}
