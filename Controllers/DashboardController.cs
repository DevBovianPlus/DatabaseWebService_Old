using DatabaseWebService.Common;
using DatabaseWebService.DomainNOZ.Abstract;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.DomainPDO.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.ModelsNOZ;
using DatabaseWebService.ModelsOTP;
using DatabaseWebService.ModelsPDO;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DatabaseWebService.Controllers
{
    public class DashboardController : ApiController
    {
        private IDashboardRepository dashboardRepo;
        private IDashboardPDORepository dashboardPDORepo;
        private IDashboardNOZRepository dashboardNOZRepo;

        public delegate WebResponseContentModel<T> Del<T>(WebResponseContentModel<T> model, Exception ex = null);
        public DashboardController(IDashboardRepository idashboardRepo, IDashboardPDORepository idashboardPDORepo, IDashboardNOZRepository _dashboardNOZRepo)
        {
            dashboardRepo = idashboardRepo;
            dashboardPDORepo = idashboardPDORepo;
            dashboardNOZRepo = _dashboardNOZRepo;
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

        [HttpGet]
        public IHttpActionResult GetDashboardData()
        {
            WebResponseContentModel<DashboardDataModel> tmpUser = new WebResponseContentModel<DashboardDataModel>();
            Del<DashboardDataModel> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = dashboardRepo.GetDashboardData();
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
        public IHttpActionResult GetDashboardPDOData()
        {
            WebResponseContentModel<DashboardPDOModel> tmpUser = new WebResponseContentModel<DashboardPDOModel>();
            Del<DashboardPDOModel> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = dashboardPDORepo.GetDashboardData();
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
        public IHttpActionResult GetDashboardNOZData()
        {
            WebResponseContentModel<DashboardNOZModel> tmpUser = new WebResponseContentModel<DashboardNOZModel>();
            Del<DashboardNOZModel> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = dashboardNOZRepo.GetDashboardData();
                responseStatusHandler(tmpUser);
            }
            catch (Exception ex)
            {
                responseStatusHandler(tmpUser, ex);
                return Json(tmpUser);
            }

            return Json(tmpUser);
        }
    }
}
