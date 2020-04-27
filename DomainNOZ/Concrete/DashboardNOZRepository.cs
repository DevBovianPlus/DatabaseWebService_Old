using DatabaseWebService.Common;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.DomainNOZ.Abstract;
using DatabaseWebService.ModelsNOZ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.DomainNOZ.Concrete
{
    public class DashboardNOZRepository : IDashboardNOZRepository
    {

        GrafolitNOZEntities context;


        public DashboardNOZRepository(GrafolitNOZEntities _context)
        {
            context = _context;
        }

        public DashboardNOZModel GetDashboardData()
        {
            string statusOddan = Enums.StatusOfOptimalStock.ODDANO.ToString();
            string statusKreiran = Enums.StatusOfOptimalStock.USTVARJENO_NAROCILO.ToString();

            var model = new DashboardNOZModel();

            model.OrderCount = context.NarociloOptimalnihZalog.Count();
            model.SubmitedOrders = context.NarociloOptimalnihZalog.Where(i => i.StatusNarocilaOptimalnihZalog.Koda == statusOddan).Count();
            model.CreatedOrders = context.NarociloOptimalnihZalog.Where(i => i.StatusNarocilaOptimalnihZalog.Koda == statusKreiran).Count();

            //število naročil optimalnih zalog v tekočem letu
            var query = from order in context.NarociloOptimalnihZalog
                        where order.ts.Value.Year == DateTime.Now.Year
                        group order by new { month = order.ts.Value.Month } into groupOrders
                        select new OptimalStockOrdersInYear
                        {
                            Month = groupOrders.Key.month,
                            Count = groupOrders.Count()
                        };

            model.CurrentYearOrder = new List<object>();

            model.CurrentYearOrder.Add(new object[]{
                    "MonthName", "Št. naročil"
                });

            foreach (var item in query.ToList())
            {
                item.MonthName = DataTypesHelper.GetDateTimeMonthByNumber(item.Month);
                model.CurrentYearOrder.Add(new object[]{
                            item.MonthName, item.Count
                        });
            }

            if (model.CurrentYearOrder.Count <= 1)
                model.CurrentYearOrder = new List<object>();

            //število naročil na zaposlenega
            var query2 = from order in context.NarociloOptimalnihZalog
                         where order.ts.Value.Year == DateTime.Now.Year
                         group order by order.tsIDOsebe into groupOrders
                         select new OptimalStockOrdersInYear
                         {
                             EmployeeName = (from e in context.Osebe_NOZ
                                             where e.OsebaID == groupOrders.Key
                                             select new
                                             {
                                                 Name = e.Ime + " " + e.Priimek
                                             }).FirstOrDefault().Name,
                             Count = groupOrders.Count()
                         };

            model.EmployeesOrderCount = new List<object>();
            model.EmployeesOrderCount.Add(new object[]{
                    "EmployeeName", "Št. naročil"
                });

            foreach (var item in query2.ToList())
            {
                model.EmployeesOrderCount.Add(new object[]{
                            item.EmployeeName, item.Count
                        });
            }

            return model;
        }
    }
}