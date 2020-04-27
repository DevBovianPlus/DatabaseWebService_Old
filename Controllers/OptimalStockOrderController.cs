using DatabaseWebService.Common;
using DatabaseWebService.DomainNOZ.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.ModelsNOZ;
using DatabaseWebService.ModelsNOZ.OptimalStockOrder;
using DatabaseWebService.ModelsPDO.Inquiry;
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
    public class OptimalStockOrderController : ApiController
    {
        private IOptimalStockOrderRepository optimalStockOrderRepo;

        public delegate WebResponseContentModel<T> Del<T>(WebResponseContentModel<T> model, Exception ex = null);
        public OptimalStockOrderController(IOptimalStockOrderRepository _optimalStockOrderRepo)
        {
            optimalStockOrderRepo = _optimalStockOrderRepo;
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
        public IHttpActionResult GetOptimalStockTree(string productCategory, string color)
        {
            WebResponseContentModel<List<OptimalStockTreeHierarchy>> tmpUser = new WebResponseContentModel<List<OptimalStockTreeHierarchy>>();
            Del<List<OptimalStockTreeHierarchy>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = optimalStockOrderRepo.GetOptimalStockTree(productCategory, color);
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
        public IHttpActionResult GetCategoryList()
        {
            WebResponseContentModel<List<ProductCategory>> tmpUser = new WebResponseContentModel<List<ProductCategory>>();
            Del<List<ProductCategory>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = optimalStockOrderRepo.GetCategoryList();
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
        public IHttpActionResult GetColorListByCategory(string category)
        {
            
            WebResponseContentModel<List<ProductColor>> tmpUser = new WebResponseContentModel<List<ProductColor>>();
            Del<List<ProductColor>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = optimalStockOrderRepo.GetColorListByCategory(category);
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
        public IHttpActionResult GetProductsForSelectedOptimalStock(string color, [FromBody]object helperOptimalStock)
        {
            WebResponseContentModel<hlpOptimalStockOrderModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<hlpOptimalStockOrderModel>>(helperOptimalStock.ToString());

                if (model.Content != null)
                {
                    List<OptimalStockTreeHierarchy> lst = model.Content.SubCategoryWithProducts;
                    model.Content = optimalStockOrderRepo.GetProductsForSelectedOptimalStock(lst, color, model.Content);

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

        //[HttpPost]
        //public IHttpActionResult GetProductsForSelectedOptimalStock(string color, [FromBody]object filteredOptimalStockTreeData)
        //{
        //    WebResponseContentModel<List<OptimalStockTreeHierarchy>> model = null;
        //    try
        //    {
        //        model = JsonConvert.DeserializeObject<WebResponseContentModel<List<OptimalStockTreeHierarchy>>>(filteredOptimalStockTreeData.ToString());

        //        if (model.Content != null)
        //        {
        //            model.Content = optimalStockOrderRepo.GetProductsForSelectedOptimalStock(model.Content, color);

        //            model.IsRequestSuccesful = true;
        //        }
        //        else
        //        {
        //            model.IsRequestSuccesful = false;
        //            model.ValidationError = ValidationExceptionError.res_09;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        model.IsRequestSuccesful = false;
        //        model.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
        //        return Json(model);
        //    }

        //    return Json(model);
        //}


        #region Naročilo optimalnih zalog

        [HttpGet]
        public IHttpActionResult GetOptimalStockOrders()
        {
            WebResponseContentModel<List<OptimalStockOrderModel>> tmpUser = new WebResponseContentModel<List<OptimalStockOrderModel>>();
            Del<List<OptimalStockOrderModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = optimalStockOrderRepo.GetOptimalStockOrders();
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
        public IHttpActionResult GetOptimalStockOrderByID(int ID)
        {
            WebResponseContentModel<OptimalStockOrderModel> tmpUser = new WebResponseContentModel<OptimalStockOrderModel>();
            Del<OptimalStockOrderModel> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = optimalStockOrderRepo.GetOptimalStockOrderByID(ID);
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
        public IHttpActionResult SaveOptimalStockOrder([FromBody]object optimalStockOrderData, bool submitCopiedOrder)
        {
            WebResponseContentModel<OptimalStockOrderModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<OptimalStockOrderModel>>(optimalStockOrderData.ToString());

                if (model.Content != null)
                {
                    if (model.Content.NarociloOptimalnihZalogID > 0)//We update existing record in DB
                    {
                        optimalStockOrderRepo.SaveOptimalStockOrder(model.Content);

                        /*if (model.Content.EmployeeSubmitInquiry)
                        {
                            //TODO: Send mail to suppliers!
                            var employee = employeeRepo.GetEmployeeByID(model.Content.PovprasevanjeOddalID);
                            var group = inquiryRepo.GetInquiryPositionsGroupedBySupplier(model.Content.PovprasevanjeID);
                            messageEventsRepo.CreateEmailForSuppliers(group, employee, model.Content.PovprasevanjeStevilka);
                        }*/

                        //če prekopiramo naročilo in ga želimo oddati
                        if(submitCopiedOrder)
                            optimalStockOrderRepo.CreateXMLForPantheonNOZ(model.Content);// Create Order In Panteon
                    }
                    else // We add and save new recod to DB 
                    {
                        model.Content.NarociloOptimalnihZalogID = optimalStockOrderRepo.SaveOptimalStockOrder(model.Content, false);

                        /*if (model.Content.EmployeeSubmitInquiry)
                        {
                            //TODO: Send mail to suppliers!
                            var employee = employeeRepo.GetEmployeeByID(model.Content.PovprasevanjeOddalID);
                            var group = inquiryRepo.GetInquiryPositionsGroupedBySupplier(model.Content.PovprasevanjeID);
                            messageEventsRepo.CreateEmailForSuppliers(group, employee, model.Content.PovprasevanjeStevilka);
                        }*/

                        // Create Order In Panteon
                        optimalStockOrderRepo.CreateXMLForPantheonNOZ(model.Content);
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
        public IHttpActionResult DeleteOptimalStockOrder(int ID)
        {
            WebResponseContentModel<bool> deleteOrder = new WebResponseContentModel<bool>();
            try
            {
                deleteOrder.Content = optimalStockOrderRepo.DeleteOptimalStockOrder(ID);

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
        public IHttpActionResult CopyOptimalStockOrderByID(int optimalStockOrderID)
        {
            WebResponseContentModel<bool> optimalStockOrder = new WebResponseContentModel<bool>();
            Del<bool> responseStatusHandler = ProcessContentModel;
            try
            {
                optimalStockOrder.Content = optimalStockOrderRepo.CopyOptimalStockOrderByID(optimalStockOrderID);
                responseStatusHandler(optimalStockOrder);
            }
            catch (Exception ex)
            {
                responseStatusHandler(optimalStockOrder, ex);
                return Json(optimalStockOrder);
            }

            return Json(optimalStockOrder);
        }
        #endregion

        #region Naročilo optimalnih zalog pozicija

        [HttpGet]
        public IHttpActionResult GetOptimalStockOrderPositionsByOrderID(int orderID)
        {
            WebResponseContentModel<List<OptimalStockOrderPositionModel>> tmpUser = new WebResponseContentModel<List<OptimalStockOrderPositionModel>>();
            Del<List<OptimalStockOrderPositionModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = optimalStockOrderRepo.GetOptimalStockOrderPositionsByOrderID(orderID);
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
        public IHttpActionResult GetOptimalStockOrderPositionByID(int ID)
        {
            WebResponseContentModel<OptimalStockOrderPositionModel> tmpUser = new WebResponseContentModel<OptimalStockOrderPositionModel>();
            Del<OptimalStockOrderPositionModel> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = optimalStockOrderRepo.GetOptimalStockOrderPositionByID(ID);
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
        public IHttpActionResult SaveOptimalStockPositionOrder([FromBody]object optimalStockOrderPosData)
        {
            WebResponseContentModel<OptimalStockOrderPositionModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<OptimalStockOrderPositionModel>>(optimalStockOrderPosData.ToString());

                if (model.Content != null)
                {
                    if (model.Content.NarociloOptimalnihZalogPozicijaID > 0)//We update existing record in DB
                    {
                        optimalStockOrderRepo.SaveOptimalStockPositionOrder(model.Content);

                        /*if (model.Content.EmployeeSubmitInquiry)
                        {
                            //TODO: Send mail to suppliers!
                            var employee = employeeRepo.GetEmployeeByID(model.Content.PovprasevanjeOddalID);
                            var group = inquiryRepo.GetInquiryPositionsGroupedBySupplier(model.Content.PovprasevanjeID);
                            messageEventsRepo.CreateEmailForSuppliers(group, employee, model.Content.PovprasevanjeStevilka);
                        }*/
                    }
                    else // We add and save new recod to DB 
                    {
                        model.Content.NarociloOptimalnihZalogPozicijaID = optimalStockOrderRepo.SaveOptimalStockPositionOrder(model.Content, false);

                        /*if (model.Content.EmployeeSubmitInquiry)
                        {
                            //TODO: Send mail to suppliers!
                            var employee = employeeRepo.GetEmployeeByID(model.Content.PovprasevanjeOddalID);
                            var group = inquiryRepo.GetInquiryPositionsGroupedBySupplier(model.Content.PovprasevanjeID);
                            messageEventsRepo.CreateEmailForSuppliers(group, employee, model.Content.PovprasevanjeStevilka);
                        }*/
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
        public IHttpActionResult DeleteOptimalStockPosition(int ID)
        {
            WebResponseContentModel<bool> deleteOrderPos = new WebResponseContentModel<bool>();
            try
            {
                deleteOrderPos.Content = optimalStockOrderRepo.DeleteOptimalStockPosition(ID);

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

        #endregion

        #region Status Naročilo optimalnih zalog

        [HttpGet]
        public IHttpActionResult GetOptimalStockStatuses()
        {
            WebResponseContentModel<List<OptimalStockOrderStatusModel>> tmpUser = new WebResponseContentModel<List<OptimalStockOrderStatusModel>>();
            Del<List<OptimalStockOrderStatusModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = optimalStockOrderRepo.GetOptimalStockStatuses();
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
        public IHttpActionResult GetOptimalStockStatusByID(int statusID)
        {
            WebResponseContentModel<OptimalStockOrderStatusModel> tmpUser = new WebResponseContentModel<OptimalStockOrderStatusModel>();
            Del<OptimalStockOrderStatusModel> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = optimalStockOrderRepo.GetOptimalStockStatusByID(statusID);
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
