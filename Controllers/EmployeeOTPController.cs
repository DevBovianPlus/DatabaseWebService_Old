using DatabaseWebService.Common;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DatabaseWebService.Controllers
{
    public class EmployeeOTPController : ApiController
    {
        private IEmployeeOTPRepository employeeOtpRepo;

        public delegate WebResponseContentModel<T> Del<T>(WebResponseContentModel<T> model, Exception ex = null);
        public EmployeeOTPController(IEmployeeOTPRepository iemployeeOtpRepo)
        {
            employeeOtpRepo = iemployeeOtpRepo;
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

        #region Employee

        [HttpGet]
        public IHttpActionResult GetAllEmployees()
        {
            WebResponseContentModel<List<EmployeeFullModel>> tmpUser = new WebResponseContentModel<List<EmployeeFullModel>>();
            Del<List<EmployeeFullModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = employeeOtpRepo.GetAllEmployees();

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
        public IHttpActionResult GetAllEmployeesByRoleID(int roleID)
        {
            WebResponseContentModel<List<EmployeeFullModel>> tmpUser = new WebResponseContentModel<List<EmployeeFullModel>>();
            Del<List<EmployeeFullModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = employeeOtpRepo.GetAllEmployees(roleID);

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
        public IHttpActionResult GetEmployeeByID(int employeeId)
        {
            WebResponseContentModel<EmployeeFullModel> tmpUser = new WebResponseContentModel<EmployeeFullModel>();
            Del<EmployeeFullModel> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = employeeOtpRepo.GetEmployeeByID(employeeId);

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
