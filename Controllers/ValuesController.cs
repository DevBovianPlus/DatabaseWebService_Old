using DatabaseWebService.Common;
using DatabaseWebService.Domain;
using DatabaseWebService.Domain.Abstract;
using DatabaseWebService.DomainNOZ.Abstract;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.DomainPDO.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsOTP;
using DatabaseWebService.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DatabaseWebService.Controllers
{
    public class ValuesController : ApiController
    {
        private IUserRepository userRepo;
        private IEmployeeRepository employeeRepo;
        private IPostRepository postRepo;
        private IEmployeeRankRepository employeeRankRepo;
        private IClientRepository clientRepo;

        private IUserOTPRepository userOTPRepo;

        private IUserPDORepository userPDORepo;

        private IOptimalStockOrderRepository optimalStockRepo;
        private IUserNOZRepository userNOZRepo;

        public ValuesController(IUserRepository iUserRepo, IEmployeeRepository iemployeeRepo, IPostRepository iPostRepo,
              IEmployeeRankRepository iemployeeRankRepo, IClientRepository iclientRepo, IUserOTPRepository iuserOTPRepo,
              IUserPDORepository iuserPDORepo, IOptimalStockOrderRepository _optimalStockRepo, IUserNOZRepository _userNOZRepo)
        {
            userRepo = iUserRepo;
            employeeRepo = iemployeeRepo;
            postRepo = iPostRepo;
            employeeRankRepo = iemployeeRankRepo;
            clientRepo = iclientRepo;
            userOTPRepo = iuserOTPRepo;
            userPDORepo = iuserPDORepo;
            optimalStockRepo = _optimalStockRepo;
            userNOZRepo = _userNOZRepo;
        }

        [HttpGet]
        public IHttpActionResult SignIn(string username, string password)
        {
            WebResponseContentModel<UserModel> tmpUser = new WebResponseContentModel<UserModel>();

            try
            {
                tmpUser.Content = userRepo.UserLogIn(username, password);

                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_01;
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
        public IHttpActionResult GetAktivnostUporabnikaByDateAndUserID(int UserID, string CurentDate)
        {


            WebResponseContentModel<AktivnostUporabnikaModel> tmpUser = new WebResponseContentModel<AktivnostUporabnikaModel>();

            try
            {
                DateTime cDateTime = DateTime.Now;
                CurentDate = cDateTime.ToShortDateString();

                tmpUser.Content = userOTPRepo.GetAktivnostUporabnikaByDateAndUserID(UserID, CurentDate);

                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_01;
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
        public IHttpActionResult GetAllPosts()
        {
            List<PostModel> list = postRepo.GetAllPostInfo();
            return Json(list);
        }

        [HttpGet]
        public IHttpActionResult GetAllEmployeeRanks()
        {

            /*EmployeeModel model = new EmployeeModel();
            model.PostID = 2;
            model.LastName = "asdas";
            model.ID = null;
            model.FirstName = "dfds";
            model.EmploymentDate = new DateTime(2010, 5, 30).Date;
            model.EmployeeRankID = 3;
            model.Email = "asafsa";
            model.DateOfBirth = new DateTime(1990, 1, 20).Date;
            model.Address = "fasf";*/

            List<EmployeeRankModel> list = employeeRankRepo.GetAllEmployeeRanks();
            return Json(list);
        }

        [HttpPost]
        public IHttpActionResult PostNewEmployeeData([FromBody] object employeeData)
        {
            EmployeeFullModel model = null;
            try
            {
                model = JsonConvert.DeserializeObject<EmployeeFullModel>(employeeData.ToString());

                if (model != null)
                {
                    if (model.idOsebe > 0)//We update existing record in DB
                        employeeRepo.SaveEmployee(model);
                    else // We add and save new recod to DB 
                        employeeRepo.SaveEmployee(model, false);
                }
                else
                {
                    return BadRequest("Cannot convert posted data to Employee entity instance!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + "\r\n" + ex.Source);
            }

            return Ok(model);
        }

        [HttpGet]
        public IHttpActionResult DeleteEmployee(int employeeID)
        {
            bool isDeleted = employeeRepo.DeleteEmployee(employeeID);

            if (isDeleted)
                return Ok(true);

            return BadRequest("Unable to delete employee");
        }

        [HttpGet]
        public IHttpActionResult GetEmployeesByRank(int employeeRankID)
        {
            List<EmployeeSimpleModel> list = employeeRepo.GetEmployeesByRankID(employeeRankID);
            return Json(list);
        }

        [HttpGet]
        public IHttpActionResult GetComplexModel()
        {
            ComplexModel model = new ComplexModel();
            model.Name = "Testni kompleksni model";
            List<PostModel> listPosts = postRepo.GetAllPostInfo();
            List<EmployeeRankModel> listRanks = employeeRankRepo.GetAllEmployeeRanks();
            model.posts = listPosts;
            model.employeeRanks = listRanks;

            WebResponseContentModel<ComplexModel> complexModel = new WebResponseContentModel<ComplexModel>();
            complexModel.Content = model;


            return Json(complexModel);
        }

        [HttpGet]
        public IHttpActionResult GetDataForGraphRendering()
        {
            WebResponseContentModel<DataTable> model = new WebResponseContentModel<DataTable>();
            model.Content = GetDataTable();

            return Json(model);
        }

        private DataTable GetDataTable()
        {
            DataTable dt = new DataTable("ChartTable");
            dt.Columns.Add(new DataColumn("ID", typeof(Int32)));
            dt.Columns.Add(new DataColumn("Tip", typeof(string)));
            dt.Columns.Add(new DataColumn("Cena", typeof(decimal)));

            dt.PrimaryKey = new DataColumn[] { dt.Columns[0] };

            DataRow dr = dt.NewRow();
            dr["ID"] = 1;
            dr["Tip"] = "Marec str";
            dr["Cena"] = 550;

            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["ID"] = 2;
            dr["Tip"] = "Kvartalno str";
            dr["Cena"] = 670;

            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["ID"] = 3;
            dr["Tip"] = "Poletje str";
            dr["Cena"] = 850;

            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["ID"] = 4;
            dr["Tip"] = "sdf str";
            dr["Cena"] = 1100;

            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["ID"] = 5;
            dr["Tip"] = "Poleggfftje str";
            dr["Cena"] = 400;

            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["ID"] = 6;
            dr["Tip"] = "etzhz str";
            dr["Cena"] = 300;

            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["ID"] = 7;
            dr["Tip"] = "kj str";
            dr["Cena"] = 100;

            dt.Rows.Add(dr);



            /*dr = dt.NewRow();
            dr["ID"] = 8;
            dr["Tip"] = "8";
            dr["Cena"] = 600;

            dt.Rows.Add(dr);


            dr = dt.NewRow();
            dr["ID"] = 9;
            dr["Tip"] = "sr";
            dr["Cena"] = 45;

            dt.Rows.Add(dr);


            dr = dt.NewRow();
            dr["ID"] = 10;
            dr["Tip"] = "sdsd";
            dr["Cena"] = 1500;

            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["ID"] = 11;
            dr["Tip"] = "sdsd";
            dr["Cena"] = 1500;

            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["ID"] = 12;
            dr["Tip"] = "12";
            dr["Cena"] = 390;

            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["ID"] = 13;
            dr["Tip"] = "13";
            dr["Cena"] = 756;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["ID"] = 14;
            dr["Tip"] = "14";
            dr["Cena"] = 1000;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["ID"] = 15;
            dr["Tip"] = "15";
            dr["Cena"] = 500;

            dt.Rows.Add(dr);*/

            return dt;
        }


        /*OptimizacijaTransportov*/
        #region Gragolit_OTP
        [HttpGet]
        public IHttpActionResult SignInOTP(string username, string password)
        {
            WebResponseContentModel<UserModel> tmpUser = new WebResponseContentModel<UserModel>();

            try
            {
                tmpUser.Content = userOTPRepo.UserLogIn(username, password);

                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_01;
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

        #region Grafolit_PDO
        [HttpGet]
        public IHttpActionResult SignInPDO(string username, string password)
        {
            WebResponseContentModel<UserModel> tmpUser = new WebResponseContentModel<UserModel>();

            try
            {
                tmpUser.Content = userPDORepo.UserLogIn(username, password);

                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_01;
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

        [HttpGet]
        public IHttpActionResult GetWebServiceLogFile()
        {
            WebResponseContentModel<byte[]> tmpUser = new WebResponseContentModel<byte[]>();

            try
            {
                string logFilePath = AppDomain.CurrentDomain.BaseDirectory + "log.txt";
                byte[] bytes = System.IO.File.ReadAllBytes(logFilePath);
                tmpUser.Content = bytes;

                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_01;
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

        public IHttpActionResult GetUtilityServiceLogFile()
        {
            WebResponseContentModel<byte[]> tmpUser = new WebResponseContentModel<byte[]>();

            try
            {
                string sUtilityServicePath = ConfigurationManager.AppSettings["UtilityServicePath"].ToString();

                string logFilePath = sUtilityServicePath + "log.txt";
                byte[] bytes = System.IO.File.ReadAllBytes(logFilePath);
                tmpUser.Content = bytes;

                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_01;
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




        #region Grafolit_NOZ
        [HttpGet]
        public IHttpActionResult SignInNOZ(string username, string password)
        {
            WebResponseContentModel<UserModel> tmpUser = new WebResponseContentModel<UserModel>();

            try
            {
                tmpUser.Content = userNOZRepo.UserLogIn(username, password);

                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_01;
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
        public IHttpActionResult GetOptimalStockTree()
        {
            optimalStockRepo.GetOptimalStockTree("KARTON", "");

            return Ok();
        }
        #endregion
    }
}
