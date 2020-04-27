using DatabaseWebService.Common;
using DatabaseWebService.DomainPDO.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.ModelsPDO.Inquiry;
using DatabaseWebService.ModelsPDO.Order;
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
    public class OrderPDOController : ApiController
    {
        private IOrderPDORepository orderRepo;
        private IMSSQLPDOFunctionRepository mssqlRepo;

        public delegate WebResponseContentModel<T> Del<T>(WebResponseContentModel<T> model, Exception ex = null);
        public OrderPDOController(IOrderPDORepository iorderRepo, IMSSQLPDOFunctionRepository imssqlRepo)
        {
            orderRepo = iorderRepo;
            mssqlRepo = imssqlRepo;
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
        public IHttpActionResult GetOrderList()
        {
            WebResponseContentModel<List<OrderPDOFullModel>> tmpUser = new WebResponseContentModel<List<OrderPDOFullModel>>();
            Del<List<OrderPDOFullModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = orderRepo.GetOrderList();
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
        public IHttpActionResult GetOrderByID(int oID)
        {
            WebResponseContentModel<OrderPDOFullModel> tmpUser = new WebResponseContentModel<OrderPDOFullModel>();
            Del<OrderPDOFullModel> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = orderRepo.GetOrderByID(oID);
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
        public IHttpActionResult GetOrderPositionsByOrderID(int oID)
        {
            WebResponseContentModel<List<OrderPDOPositionModel>> tmpUser = new WebResponseContentModel<List<OrderPDOPositionModel>>();
            Del<List<OrderPDOPositionModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = orderRepo.GetOrderPositionsByOrderID(oID);
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
        public IHttpActionResult GetOrderByInquiryIDForNewOrder(int iID)
        {
            WebResponseContentModel<InquiryFullModel> tmpUser = new WebResponseContentModel<InquiryFullModel>();
            Del<InquiryFullModel> responseStatusHandler = ProcessContentModel;
            try
            {                
                tmpUser.Content = orderRepo.GetOrderPositionsByInquiryIDForNewOrder(iID);
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
        public IHttpActionResult SaveOrderModel([FromBody]object orderData)
        {
            WebResponseContentModel<OrderPDOFullModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<OrderPDOFullModel>>(orderData.ToString());

                if (model.Content != null)
                {
                    var groupedBySupplier = orderRepo.GroupOrderPositionsBySupplier(model.Content);
                    foreach (var item in groupedBySupplier)
                    {
                        var positions = item.Select(s => s).ToList();
                        model.Content.NarociloPozicija_PDO = positions;
                        model.Content.StrankaDobaviteljID = item.Key;
                        orderRepo.SaveOrder(model.Content, false, true);
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

        [HttpPost]
        public IHttpActionResult CheckPantheonArtikles([FromBody]object orderData)
        {
            WebResponseContentModel<InquiryFullModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<InquiryFullModel>>(orderData.ToString());

                if (model.Content != null)
                {

                    model.Content = orderRepo.CheckPantheonArtikles(model.Content, false);

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
        public IHttpActionResult GetAllPurchases()
        {
            WebResponseContentModel<List<InquiryModel>> tmpUser = new WebResponseContentModel<List<InquiryModel>>();
            Del<List<InquiryModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = orderRepo.GetAllPurchases();
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
        public IHttpActionResult DeleteOrder(int orderID)
        {
            WebResponseContentModel<bool> deleteOrder = new WebResponseContentModel<bool>();
            try
            {
                deleteOrder.Content = orderRepo.DeleteOrder(orderID);

                if (deleteOrder.Content)
                    deleteOrder.IsRequestSuccesful = true;
                else
                {
                    deleteOrder.IsRequestSuccesful = false;
                    deleteOrder.ValidationError = ValidationExceptionError.res_04;
                }
            }
            catch (Exception ex)
            {
                deleteOrder.IsRequestSuccesful = false;
                deleteOrder.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(deleteOrder);
            }

            return Json(deleteOrder);
        }

        [HttpGet]
        public IHttpActionResult DeleteOrderPosition(int orderPosID)
        {
            WebResponseContentModel<bool> deleteOrderPos = new WebResponseContentModel<bool>();
            try
            {
                deleteOrderPos.Content = orderRepo.DeleteOrderPosition(orderPosID);

                if (deleteOrderPos.Content)
                    deleteOrderPos.IsRequestSuccesful = true;
                else
                {
                    deleteOrderPos.IsRequestSuccesful = false;
                    deleteOrderPos.ValidationError = ValidationExceptionError.res_04;
                }
            }
            catch (Exception ex)
            {
                deleteOrderPos.IsRequestSuccesful = false;
                deleteOrderPos.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(deleteOrderPos);
            }

            return Json(deleteOrderPos);
        }

        [HttpGet]
        public IHttpActionResult ResetOrderStatusByID(int orderID)
        {
            WebResponseContentModel<bool> resetOrder = new WebResponseContentModel<bool>();
            try
            {
                orderRepo.ResetOrderStatusByID(orderID);
                resetOrder.IsRequestSuccesful = true;
                resetOrder.Content = true;

            }
            catch (Exception ex)
            {
                resetOrder.IsRequestSuccesful = false;
                resetOrder.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(resetOrder);
            }

            return Json(resetOrder);
        }

        [HttpGet]
        public IHttpActionResult RunPantheon(string sFile, string sArgs)
        {
            WebResponseContentModel<bool> resetOrder = new WebResponseContentModel<bool>();
            try
            {
                orderRepo.LaunchPantheonCreatePDF(sFile, sArgs);
                resetOrder.IsRequestSuccesful = true;
                resetOrder.Content = true;

            }
            catch (Exception ex)
            {
                resetOrder.IsRequestSuccesful = false;
                resetOrder.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(resetOrder);
            }

            return Json(resetOrder);
        }

        [HttpGet]
        public IHttpActionResult ChangeConfigValue(string sConfigName, string sConfigValue)
        {
            WebResponseContentModel<bool> resetOrder = new WebResponseContentModel<bool>();
            try
            {
                var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                if (config.AppSettings.Settings[sConfigName] != null)
                {
                    config.AppSettings.Settings[sConfigName].Value = sConfigValue;
                }
                else { config.AppSettings.Settings.Add(sConfigName, sConfigValue); }
                config.Save();

                resetOrder.IsRequestSuccesful = true;
                resetOrder.Content = true;

            }
            catch (Exception ex)
            {
                resetOrder.IsRequestSuccesful = false;
                resetOrder.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(resetOrder);
            }

            return Json(resetOrder);
        }



        [HttpGet]
        public IHttpActionResult GetConfigValue(string sConfigName)
        {
            WebResponseContentModel<string> resetOrder = new WebResponseContentModel<string>();
            Del<string> responseStatusHandler = ProcessContentModel;
            try
            {
                string sReturn = "";

                var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                if (config.AppSettings.Settings[sConfigName] != null)
                {
                    sReturn = config.AppSettings.Settings[sConfigName].Value;
                }
                else { sReturn = ""; }

                resetOrder.Content = sReturn;
                responseStatusHandler(resetOrder);


            }
            catch (Exception ex)
            {
                resetOrder.IsRequestSuccesful = false;
                resetOrder.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(resetOrder);
            }

            return Json(resetOrder);
        }
        #region MS SQL FUNCTIONS

        [HttpGet]
        public IHttpActionResult GetProductByName(string name)
        {
            WebResponseContentModel<List<ProductModel>> tmpUser = new WebResponseContentModel<List<ProductModel>>();
            Del<List<ProductModel>> responseStatusHandler = ProcessContentModel;

            try
            {
                tmpUser.Content = mssqlRepo.GetProductByName(name);
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
        public IHttpActionResult GetProductBySupplierAndName(string supplier, string name)
        {
            WebResponseContentModel<List<ProductModel>> tmpUser = new WebResponseContentModel<List<ProductModel>>();
            Del<List<ProductModel>> responseStatusHandler = ProcessContentModel;

            try
            {
                tmpUser.Content = mssqlRepo.GetProductBySupplierAndName(supplier, name);
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

        #region "Admin"

        [HttpGet]
        public IHttpActionResult CreatePDFAndSendPDOOrdersMultiple()
        {
            WebResponseContentModel<bool> tmpUser = new WebResponseContentModel<bool>();
            Del<bool> responseStatusHandler = ProcessContentModel;
            try
            {
                orderRepo.CreatePDFAndSendPDOOrdersMultiple();
                tmpUser.Content = true;
                responseStatusHandler(tmpUser);
            }
            catch (Exception ex)
            {
                tmpUser.Content = false;
                responseStatusHandler(tmpUser, ex);
                return Json(tmpUser);
            }

            return Json(tmpUser);
        }
        #endregion
    }
}