using DatabaseWebService.Common;
using DatabaseWebService.Domain.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DatabaseWebService.Controllers
{
    public class ChartController : ApiController
    {
        private IChartsRepository chartsRepo;
     
        public ChartController(IChartsRepository ichartRepo)
        {
            chartsRepo = ichartRepo;
        }

        [HttpGet]
        public IHttpActionResult GetChartsData(int clientID, int categorieID, int period, int type, string dateFrom, string dateTo)
        {
            WebResponseContentModel<ChartRenderModel> tmpUser = new WebResponseContentModel<ChartRenderModel>();
            DateTime dateFROM = new DateTime();
            DateTime dateTO = new DateTime();

            if (!String.IsNullOrEmpty(dateFrom)) dateFROM = DateTime.Parse(dateFrom);
            if (!String.IsNullOrEmpty(dateTo)) dateTO = DateTime.Parse(dateTo);

            try
            {
                tmpUser.Content = chartsRepo.GetDataForChart(clientID, categorieID, period, type, dateFROM, dateTO);
                
                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_25;
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
        public IHttpActionResult GetChartsDataForAllTypes(int clientID, int categorieID, int period, string dateFrom, string dateTo)
        {
            WebResponseContentModel<List<ChartRenderModel>> tmpUser = new WebResponseContentModel<List<ChartRenderModel>>();

            DateTime dateFROM = new DateTime();
            DateTime dateTO = new DateTime();

            if (!String.IsNullOrEmpty(dateFrom)) dateFROM = DateTime.Parse(dateFrom);
            if (!String.IsNullOrEmpty(dateTo)) dateTO = DateTime.Parse(dateTo);

            try
            {
                tmpUser.Content = chartsRepo.GetDataChartAllTypes(clientID, categorieID, period, dateFROM, dateTO);

                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_25;
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
        public IHttpActionResult GetChartsDataFromSQLFunction(int clientID, int categorieID, int period, int type, string dateFrom, string dateTo)
        {
            WebResponseContentModel<ChartRenderModel> tmpUser = new WebResponseContentModel<ChartRenderModel>();
            DateTime? dateFROM = null;
            DateTime? dateTO = null;

            if (!String.IsNullOrEmpty(dateFrom)) dateFROM = DateTime.Parse(dateFrom);
            if (!String.IsNullOrEmpty(dateTo)) dateTO = DateTime.Parse(dateTo);

            try
            {
                tmpUser.Content = chartsRepo.GetDataForChartFromSQLFunction(clientID, categorieID, period, type, dateFROM, dateTO);

                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_25;
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
        public IHttpActionResult GetChartsDataForAllTypesSQLFunction(int clientID, int categorieID, int period, string dateFrom, string dateTo)
        {
            WebResponseContentModel<List<ChartRenderModel>> tmpUser = new WebResponseContentModel<List<ChartRenderModel>>();

            DateTime? dateFROM = null;
            DateTime? dateTO = null;

            if (!String.IsNullOrEmpty(dateFrom)) dateFROM = DateTime.Parse(dateFrom);
            if (!String.IsNullOrEmpty(dateTo)) dateTO = DateTime.Parse(dateTo);

            try
            {
                tmpUser.Content = chartsRepo.GetDataChartAllTypesSQLFunction(clientID, categorieID, period, dateFROM, dateTO);

                if (tmpUser.Content != null)
                {
                    tmpUser.IsRequestSuccesful = true;
                    tmpUser.ValidationError = "";
                }
                else
                {
                    tmpUser.IsRequestSuccesful = false;
                    tmpUser.ValidationError = ValidationExceptionError.res_25;
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
