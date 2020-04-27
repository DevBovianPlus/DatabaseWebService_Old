using DatabaseWebService.Common;
using DatabaseWebService.DomainDW.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.ModelsDW.CashFlow_Skupno;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DatabaseWebService.Controllers
{
    public class CashFlowController : ApiController
    {
        private ICashFlow_SkupnoRepository cashFlowSkupnoRepo;
        //public delegate T DelSaveEntry<T>(T model, bool updateRecord = true);
        public delegate WebResponseContentModel<T> Del<T>(WebResponseContentModel<T> model, Exception ex = null);

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
                    model.ValidationError = ValidationExceptionError.res_25;
                }
            }
            else
            {
                model.IsRequestSuccesful = false;
                model.ValidationError = ExceptionValidationHelper.GetExceptionSource(ex);
            }
            return model;
        }
        public CashFlowController(ICashFlow_SkupnoRepository icashFlowSkupnoRepo)
        {
            cashFlowSkupnoRepo = icashFlowSkupnoRepo;
        }

        [HttpGet]
        public IHttpActionResult GetCashFlow_SkupnoByDatumPlana(string datumPlana)
        {
            WebResponseContentModel<List<CashFlow_SkupnoModel>> tmpUser = new WebResponseContentModel<List<CashFlow_SkupnoModel>>();
            Del<List<CashFlow_SkupnoModel>> responseStatusHandler = ProcessContentModel;
            DateTime planDatum = new DateTime();

            if (!String.IsNullOrEmpty(datumPlana)) planDatum = DateTime.Parse(datumPlana);

            try
            {
                tmpUser.Content = cashFlowSkupnoRepo.GetCashFlow_SkupnoByDatumPlana(planDatum);
                responseStatusHandler(tmpUser);
                return Json(tmpUser);
            }
            catch (Exception ex)
            {
                responseStatusHandler(tmpUser, ex);
                return Json(tmpUser);
            }
        }
        [HttpGet]
        public IHttpActionResult GetCashFlow_SkupnoByDatum(string datumTeden)
        {
            WebResponseContentModel<List<CashFlow_SkupnoModel>> tmpUser = new WebResponseContentModel<List<CashFlow_SkupnoModel>>();
            Del<List<CashFlow_SkupnoModel>> responseStatusHandler = ProcessContentModel;
            DateTime datum = new DateTime();

            if (!String.IsNullOrEmpty(datumTeden)) datum = DateTime.Parse(datumTeden);

            try
            {
                tmpUser.Content = cashFlowSkupnoRepo.GetCashFlow_SkupnoByDatum(datum);
                responseStatusHandler(tmpUser);
                return Json(tmpUser);
            }
            catch (Exception ex)
            {
                responseStatusHandler(tmpUser, ex);
                return Json(tmpUser);
            }
        }
        [HttpGet]
        public IHttpActionResult GetCashFlow_SkupnoByVrsta(string vrsta)
        {
            WebResponseContentModel<List<CashFlow_SkupnoModel>> tmpUser = new WebResponseContentModel<List<CashFlow_SkupnoModel>>();
            Del<List<CashFlow_SkupnoModel>> responseStatusHandler = ProcessContentModel;
            try
            {
                tmpUser.Content = cashFlowSkupnoRepo.GetCashFlow_SkupnoByVrsta(vrsta);
                responseStatusHandler(tmpUser);
                return Json(tmpUser);
            }
            catch (Exception ex)
            {
                responseStatusHandler(tmpUser, ex);
                return Json(tmpUser);
            }
        }
    }
}
