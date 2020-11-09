using DatabaseWebService.Common;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.ModelsOTP.Route;
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
    public class RouteController : ApiController
    {
        private IRouteRepository routeRepo;

        public delegate WebResponseContentModel<T> Del<T>(WebResponseContentModel<T> model, Exception ex = null);
        public RouteController(IRouteRepository irouteRepo)
        {
            routeRepo = irouteRepo;
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
        public IHttpActionResult GetAllRoutes()
        {
            WebResponseContentModel<List<RouteModel>> tmpUser = new WebResponseContentModel<List<RouteModel>>();
            Del<List<RouteModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = routeRepo.GetAllRoutes();
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
        public IHttpActionResult GetAllRoutesTransportPricesByViewType(int iViewType, int iWeightType)
        {
            WebResponseContentModel<List<RouteTransporterPricesModel>> tmpUser = new WebResponseContentModel<List<RouteTransporterPricesModel>>();
            Del<List<RouteTransporterPricesModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = routeRepo.GetAllRoutesTransportPricesByViewType(iViewType, iWeightType);
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
        public IHttpActionResult GetRouteByID(int routeID)
        {
            WebResponseContentModel<RouteModel> tmpUser = new WebResponseContentModel<RouteModel>();
            Del<RouteModel> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = routeRepo.GetRouteByID(routeID);
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
        public IHttpActionResult SaveRoute([FromBody]object routeData)
        {
            WebResponseContentModel<RouteModel> model = null;
            try
            {
                model = JsonConvert.DeserializeObject<WebResponseContentModel<RouteModel>>(routeData.ToString());

                if (model.Content != null)
                {
                    if (model.Content.RelacijaID > 0)//We update existing record in DB
                        routeRepo.SaveRoute(model.Content);
                    else // We add and save new recod to DB 
                        model.Content.RelacijaID = routeRepo.SaveRoute(model.Content, false);

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
        public IHttpActionResult DeleteRoute(int routeID)
        {
            WebResponseContentModel<bool> deleteRoute = new WebResponseContentModel<bool>();
            try
            {
                deleteRoute.Content = routeRepo.DeleteRoute(routeID);

                if (deleteRoute.Content)
                    deleteRoute.IsRequestSuccesful = true;
                else
                {
                    deleteRoute.IsRequestSuccesful = false;
                    deleteRoute.ValidationError = ValidationExceptionError.res_04;
                }
            }
            catch (Exception ex)
            {
                deleteRoute.IsRequestSuccesful = false;
                deleteRoute.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
                return Json(deleteRoute);
            }

            return Json(deleteRoute);
        }

        [HttpGet]
        public IHttpActionResult GetRoutesByCarrierID(int carrierID)
        {
            WebResponseContentModel<List<RouteModel>> tmpUser = new WebResponseContentModel<List<RouteModel>>();
            Del<List<RouteModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = routeRepo.GetRoutesByCarrierID(carrierID);
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
        public IHttpActionResult GetRoutesByCarrierIDAndRouteID(int carrierID, int routeID)
        {
            WebResponseContentModel<List<RouteModel>> tmpUser = new WebResponseContentModel<List<RouteModel>>();
            Del<List<RouteModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = routeRepo.GetRoutesByCarrierIDAndRouteID(carrierID, routeID);
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
