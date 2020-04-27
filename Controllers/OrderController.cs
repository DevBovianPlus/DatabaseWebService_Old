using DatabaseWebService.Common;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.ModelsOTP.Client;
using DatabaseWebService.ModelsOTP.Order;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DatabaseWebService.Controllers
{
    public class OrderController : ApiController
    {
        private IOrderRepository orderRepo;
        private IMSSQLFunctionsRepository sqlFunctionRepo;

        public delegate WebResponseContentModel<T> Del<T>(WebResponseContentModel<T> model, Exception ex = null);

        public OrderController(IOrderRepository iorderRepo, IMSSQLFunctionsRepository isqlFunctionRepo)
        {
            orderRepo = iorderRepo;
            sqlFunctionRepo = isqlFunctionRepo;
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
        public IHttpActionResult GetAllOrders()
        {
            WebResponseContentModel<List<OrderModel>> tmpUser = new WebResponseContentModel<List<OrderModel>>();

            try
            {
                tmpUser.Content = orderRepo.GetAllOrders();

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
        public IHttpActionResult GetAllOrdersPositions()
        {
            WebResponseContentModel<List<OrderPositionModel>> tmpUser = new WebResponseContentModel<List<OrderPositionModel>>();

            try
            {
                tmpUser.Content = orderRepo.GetAllOrdersPositions();

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
        public IHttpActionResult GetAllSuppliers()
        {
            WebResponseContentModel<List<SupplierModel>> tmpUser = new WebResponseContentModel<List<SupplierModel>>();
            Del<List<SupplierModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = sqlFunctionRepo.GetListOfSupplier();
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
        public IHttpActionResult GetListOfOpenedOrderPositionsBySupplier(string supplier, int clientID = 0)
        {
            WebResponseContentModel<List<OrderPositionModelNew>> tmpUser = new WebResponseContentModel<List<OrderPositionModelNew>>();
            Del<List<OrderPositionModelNew>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = sqlFunctionRepo.GetListOfOpenedOrderPositions(supplier, clientID);
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
