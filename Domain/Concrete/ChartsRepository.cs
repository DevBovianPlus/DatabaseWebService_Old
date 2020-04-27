using DatabaseWebService.Common.Enums;
using DatabaseWebService.Domain.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Domain.Concrete
{
    public class ChartsRepository : IChartsRepository
    {
        AnalizaProdajeEntities context = new AnalizaProdajeEntities();
        IEventRepository eventRepo;
        public ChartsRepository(AnalizaProdajeEntities dummyEntites, IEventRepository _eventRepo)
        {
            context = dummyEntites;
            eventRepo = _eventRepo;
        }

        public ChartRenderModel GetDataForChart(int clientID, int categorieID, int period, int type, DateTime? dateFROM = null, DateTime? dateTO = null)
        {
            try
            {
                ChartRenderModel model = new ChartRenderModel();

                if ((dateFROM != null && dateTO != null) && (dateFROM.Value.CompareTo(DateTime.MinValue) > 0 && dateTO.Value.CompareTo(DateTime.MinValue) > 0))
                {
                    model.chartRenderData = QueryChartDataForDateFromTO(clientID, categorieID, type, dateFROM.Value, dateTO.Value);
                }
                else
                    model.chartRenderData = QueryChartDataForPeriod(clientID, categorieID, period, type);

                if (model.chartRenderData.Count > 0)
                {
                    DateTime min = model.chartRenderData.Min(crd => crd.Datum);
                    DateTime max = model.chartRenderData.Max(crd => crd.Datum);
                    if (period == 2)//Year chart render
                    {
                        if (model.chartRenderData.Count > 0)
                            model.chartRenderData[model.chartRenderData.Count - 1].Datum = model.chartRenderData[model.chartRenderData.Count - 1].Datum.AddMonths(11);
                        max = max.AddMonths(12);
                    }
                    model.EventList = eventRepo.GetEventsByClientIDAndCategorieID(clientID, categorieID, min, max);
                }
                else
                    model.EventList = new List<EventSimpleModel>();

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        private List<ChartRenderSimple> QueryChartDataForDateFromTO(int clientID, int categorieID, int type, DateTime dateFROM, DateTime dateTO)
        {
            var query = from charts in context.IzpisGrafa
                        where charts.StrankaID.Equals(clientID) && charts.KategorijaID.Equals(categorieID) &&
                        (charts.Datum.Value.CompareTo(dateFROM) >= 0 && charts.Datum.Value.CompareTo(dateTO) <= 0) && charts.Tip.Value.Equals(type)
                        group charts by charts into chartsModel
                        select new ChartRenderSimple
                        {
                            Datum = chartsModel.Key.Datum.HasValue ? chartsModel.Key.Datum.Value : DateTime.MinValue,
                            EnotaMere = chartsModel.Key.EnotaMere,
                            IzpisGrafaID = chartsModel.Key.IzpisGrafaID,
                            KategorijaID = chartsModel.Key.KategorijaID,
                            Obdobje = chartsModel.Key.Obdobje.HasValue ? chartsModel.Key.Obdobje.Value : 0,
                            Opis = chartsModel.Key.Opis,
                            StrankaID = chartsModel.Key.StrankaID,
                            Tip = chartsModel.Key.Tip.HasValue ? chartsModel.Key.Tip.Value : 0,
                            Vrednost = chartsModel.Key.Vrednost.HasValue ? chartsModel.Key.Vrednost.Value : 0,
                        };

            return query.ToList();
        }

        private List<ChartRenderSimple> QueryChartDataForPeriod(int clientID, int categorieID, int period, int type)
        {
            var query = from charts in context.IzpisGrafa
                        where charts.StrankaID.Equals(clientID) && charts.KategorijaID.Equals(categorieID) &&
                        charts.Obdobje.Value.Equals(period) && charts.Tip.Value.Equals(type)
                        group charts by charts into chartsModel
                        select new ChartRenderSimple
                        {
                            Datum = chartsModel.Key.Datum.HasValue ? chartsModel.Key.Datum.Value : DateTime.MinValue,
                            EnotaMere = chartsModel.Key.EnotaMere,
                            IzpisGrafaID = chartsModel.Key.IzpisGrafaID,
                            KategorijaID = chartsModel.Key.KategorijaID,
                            Obdobje = chartsModel.Key.Obdobje.HasValue ? chartsModel.Key.Obdobje.Value : 0,
                            Opis = chartsModel.Key.Opis,
                            StrankaID = chartsModel.Key.StrankaID,
                            Tip = chartsModel.Key.Tip.HasValue ? chartsModel.Key.Tip.Value : 0,
                            Vrednost = chartsModel.Key.Vrednost.HasValue ? chartsModel.Key.Vrednost.Value : 0,
                        };

            return query.ToList();
        }

        public List<ChartRenderModel> GetDataChartAllTypes(int clientID, int categorieID, int period, DateTime dateFROM, DateTime dateTO)
        {
            List<ChartRenderModel> list = new List<ChartRenderModel>();

            try
            {
                foreach (var item in Enum.GetValues(typeof(Enums.ChartRenderType)))
                {
                    ChartRenderModel model = GetDataForChart(clientID, categorieID, period, (int)((Enums.ChartRenderType)item), dateFROM, dateTO);
                    list.Add(model);
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public ChartRenderModel GetDataForChartFromSQLFunction(int clientID, int categorieID, int period, int type, DateTime? dateFROM = null, DateTime? dateTO = null)
        {
            try
            {
                ChartRenderModel model = new ChartRenderModel();

                Stranka stranka = context.Stranka.Where(s => s.idStranka.Equals(clientID)).FirstOrDefault();
                Kategorija kategorija = context.Kategorija.Where(k => k.idKategorija.Equals(categorieID)).FirstOrDefault();

                //if the dates FROM and TO are not set, then we set the dates based on period
                if (dateFROM == null && dateTO == null)
                {
                    switch (period)
                    {
                        case (int)Enums.ChartRenderPeriod.LETNO:
                            dateTO = DateTime.Now;
                            dateFROM = new DateTime(dateTO.Value.AddYears(-5).Year, 1, 1);
                            break;
                        case (int)Enums.ChartRenderPeriod.MESECNO:
                            dateTO = DateTime.Now;
                            dateFROM = new DateTime(dateTO.Value.AddYears(-3).Year, 1, 1);
                            break;
                    }
                }


                List<IzrisGrafa_Result> list = context.IzrisGrafa(stranka.KodaStranke, kategorija.Koda, type, period, dateFROM, dateTO).ToList();

                var query = from db in list
                            select new ChartRenderSimple
                            {
                                Datum = db.Datum.Value,
                                EnotaMere = db.EnotaMere,
                                IzpisGrafaID = -1,
                                KategorijaID = db.KategorijaID.Value,
                                Obdobje = db.Obdobje.Value,
                                Opis = db.Opis,
                                StrankaID = db.StrankaID.Value,
                                Tip = db.Tip.Value,
                                Vrednost = db.Vrednost.Value
                            };
                            

                model.chartRenderData = query.ToList();

                if (model.chartRenderData.Count > 0)
                {
                    DateTime min = model.chartRenderData.Min(crd => crd.Datum);
                    DateTime max = model.chartRenderData.Max(crd => crd.Datum);
                    if (period == 2)//Year chart render
                    {
                        if (model.chartRenderData.Count > 0)
                            model.chartRenderData[model.chartRenderData.Count - 1].Datum = model.chartRenderData[model.chartRenderData.Count - 1].Datum.AddMonths(11);
                        max = max.AddMonths(12);
                    }
                    model.EventList = eventRepo.GetEventsByClientIDAndCategorieID(clientID, categorieID, min, max);
                }
                else
                    model.EventList = new List<EventSimpleModel>();

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<ChartRenderModel> GetDataChartAllTypesSQLFunction(int clientID, int categorieID, int period, DateTime? dateFROM, DateTime? dateTO)
        {
            List<ChartRenderModel> list = new List<ChartRenderModel>();

            try
            {
                foreach (var item in Enum.GetValues(typeof(Enums.ChartRenderType)))
                {
                    ChartRenderModel model = GetDataForChartFromSQLFunction(clientID, categorieID, period, (int)((Enums.ChartRenderType)item), dateFROM, dateTO);
                    list.Add(model);
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }
    }
}