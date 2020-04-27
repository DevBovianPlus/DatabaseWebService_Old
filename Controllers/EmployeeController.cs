using DatabaseWebService.Common;
using DatabaseWebService.Domain.Abstract;
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
    public class EmployeeController : ApiController
    {
        private IEmployeeRepository employeeRepo;
        private IClientRepository clientRepo;

        public EmployeeController(IEmployeeRepository iemployeeRepo, IClientRepository iclientRepo)
        {
            employeeRepo = iemployeeRepo;
            clientRepo = iclientRepo;
        }

        [HttpGet]
        public IHttpActionResult GetAllEmployees()
        {
            WebResponseContentModel<List<EmployeeSimpleModel>> tmpUser = new WebResponseContentModel<List<EmployeeSimpleModel>>();

            try
            {
                tmpUser.Content = employeeRepo.GetAllEmployees();

                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_12;
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
        public IHttpActionResult GetEmployeeByID(int employeeID)
        {
            WebResponseContentModel<EmployeeFullModel> tmpUser = new WebResponseContentModel<EmployeeFullModel>();

            try
            {
                tmpUser.Content = employeeRepo.GetEmployeeByID(employeeID);

                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_21;
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
        public IHttpActionResult SaveEmployeeData([FromBody]object employeeData)
        {
            WebResponseContentModel<EmployeeFullModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<EmployeeFullModel>>(employeeData.ToString());

                if (model.Content != null)
                {
                    if (model.Content.idOsebe > 0)//We update existing record in DB
                        employeeRepo.SaveEmployee(model.Content);
                    else // We add and save new recod to DB 
                        model.Content.idOsebe = employeeRepo.SaveEmployee(model.Content, false);

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
            WebResponseContentModel<bool> deleteClient = new WebResponseContentModel<bool>();
            try
            {
                deleteClient.Content = employeeRepo.DeleteEmployee(employeeID);

                if (deleteClient.Content)
                    deleteClient.IsRequestSuccesful = true;
                else
                {
                    deleteClient.IsRequestSuccesful = false;
                    deleteClient.ValidationError = ValidationExceptionError.res_22;
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
        public IHttpActionResult GetAllRoles()
        {
            WebResponseContentModel<List<RoleModel>> tmpUser = new WebResponseContentModel<List<RoleModel>>();

            try
            {
                tmpUser.Content = employeeRepo.GetAllRoles();

                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_23;
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
        public IHttpActionResult GetAllEmployeeSupervisors()
        {
            WebResponseContentModel<List<EmployeeSimpleModel>> tmpUser = new WebResponseContentModel<List<EmployeeSimpleModel>>();

            try
            {
                tmpUser.Content = employeeRepo.GetAllEmployeeSupervisors();

                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_23;
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
