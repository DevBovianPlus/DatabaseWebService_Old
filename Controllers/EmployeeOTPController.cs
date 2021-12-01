using DatabaseWebService.Common;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Models.Employee;
using DatabaseWebService.Resources;
using DatabaseWebService.DomainNOZ.Abstract;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Web.Http;

namespace DatabaseWebService.Controllers
{
    public class EmployeeOTPController : ApiController
    {
        private IEmployeeOTPRepository employeeOtpRepo;
        private IMSSQLNOZFunctionRepository msSqlFunctionRepo;
        

        public delegate WebResponseContentModel<T> Del<T>(WebResponseContentModel<T> model, Exception ex = null);
        public EmployeeOTPController(IEmployeeOTPRepository iemployeeOtpRepo, IMSSQLNOZFunctionRepository imsSqlFunctionRepo)
        {
            employeeOtpRepo = iemployeeOtpRepo;
            msSqlFunctionRepo = imsSqlFunctionRepo;
            
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

        [HttpGet]
        public IHttpActionResult GetPantheonUsers()
        {
            WebResponseContentModel<List<PantheonUsers>> tmpUser = new WebResponseContentModel<List<PantheonUsers>>();
            Del<List<PantheonUsers>> responseStatusHandler = ProcessContentModel;

            try
            {
                tmpUser.Content = msSqlFunctionRepo.GetPantheonUsers();
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
        public IHttpActionResult SaveEmployeeOTP([FromBody] object emplyoeeData)
        {
            WebResponseContentModel<EmployeeFullModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<EmployeeFullModel>>(emplyoeeData.ToString());

                if (model.Content != null)
                {
                    if (model.Content.idOsebe > 0)//We update existing record in DB
                        employeeOtpRepo.SaveEmployeeOTP(model.Content);
                    else // We add and save new recod to DB 
                        model.Content.idOsebe = employeeOtpRepo.SaveEmployeeOTP(model.Content, false);

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

        #region Role

        [HttpGet]
        public IHttpActionResult GetRolesOTP()
        {
            WebResponseContentModel<List<RoleModel>> tmpUser = new WebResponseContentModel<List<RoleModel>>();
            Del<List<RoleModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = employeeOtpRepo.GetRolesOTP();

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
