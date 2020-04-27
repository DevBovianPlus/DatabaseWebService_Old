using DatabaseWebService.Common;
using DatabaseWebService.Domain.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Models.FinancialControl;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DatabaseWebService.Controllers
{
    public class FinancialControlController : ApiController
    {
        IFinancialControlRepository financialRepo;

        public FinancialControlController(IFinancialControlRepository _financialRepo)
        {
            financialRepo = _financialRepo;
        }

        [HttpGet]
        public IHttpActionResult GetFinancialControlData()
        {
            WebResponseContentModel<FinancialControlModel> tmpUser = new WebResponseContentModel<FinancialControlModel>();

            try
            {

                tmpUser.Content = financialRepo.GetDataForFinancialDashboard();

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
    }
}
