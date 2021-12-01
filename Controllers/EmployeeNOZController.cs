using DatabaseWebService.Common;
using DatabaseWebService.DomainNOZ.Abstract;
using DatabaseWebService.DomainPDO.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Models.Employee;
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
    public class EmployeeNOZController : ApiController
    {
        private IEmployeeNOZRepository employeeNOZRepo;
        private IMSSQLNOZFunctionRepository msSqlFunctionRepo;
        

        public delegate WebResponseContentModel<T> Del<T>(WebResponseContentModel<T> model, Exception ex = null);
        public EmployeeNOZController(IEmployeeNOZRepository _employeeNOZRepo, IMSSQLNOZFunctionRepository _msSqlFunctionRepo)
        {
            employeeNOZRepo = _employeeNOZRepo;
            msSqlFunctionRepo = _msSqlFunctionRepo;
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
                tmpUser.Content = employeeNOZRepo.GetAllEmployees();

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
                tmpUser.Content = employeeNOZRepo.GetAllEmployees(roleID);

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
                tmpUser.Content = employeeNOZRepo.GetEmployeeByID(employeeId);

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
        public IHttpActionResult SaveEmployee([FromBody]object emplyoeeData)
        {
            WebResponseContentModel<EmployeeFullModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<EmployeeFullModel>>(emplyoeeData.ToString());

                if (model.Content != null)
                {
                    if (model.Content.idOsebe > 0)//We update existing record in DB
                        employeeNOZRepo.SaveEmployee(model.Content);
                    else // We add and save new recod to DB 
                        model.Content.idOsebe = employeeNOZRepo.SaveEmployee(model.Content, false);

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
        public IHttpActionResult DeleteEmployee(int employeeID)
        {
            WebResponseContentModel<bool> deleteEmployee = new WebResponseContentModel<bool>();
            try
            {
                deleteEmployee.Content = employeeNOZRepo.DeleteEmployee(employeeID);

                if (deleteEmployee.Content)
                    deleteEmployee.IsRequestSuccesful = true;
                else
                {
                    deleteEmployee.IsRequestSuccesful = false;
                    deleteEmployee.ValidationError = ValidationExceptionError.res_04;
                }
            }
            catch (Exception ex)
            {
                deleteEmployee.IsRequestSuccesful = false;
                deleteEmployee.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(deleteEmployee);
            }

            return Json(deleteEmployee);
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
        #endregion

        #region Role

        [HttpGet]
        public IHttpActionResult GetRoles()
        {
            WebResponseContentModel<List<RoleModel>> tmpUser = new WebResponseContentModel<List<RoleModel>>();
            Del<List<RoleModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = employeeNOZRepo.GetRoles();

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
