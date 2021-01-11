using DatabaseWebService.Common;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.DomainPDO.Abstract;
using DatabaseWebService.ModelsPDO;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.DomainPDO.Concrete
{
    public class DashboardPDORepository : IDashboardPDORepository
    {
        GrafolitPDOEntities context;


        public DashboardPDORepository(GrafolitPDOEntities _context)
        {
            context = _context;
        }

        public DashboardPDOModel GetDashboardData()
        {
            try
            {
                string statusPotrjen = Enums.StatusOfInquiry.ODDANO.ToString();
                string statusDeloven = Enums.StatusOfInquiry.DELOVNA.ToString();
                string statusVnabavi = Enums.StatusOfInquiry.POSLANO_V_NABAVO.ToString();

                var model = new DashboardPDOModel();

                model.InquiryCount = context.Povprasevanje.Count();
                model.ConfirmedInquiries = context.Povprasevanje.Where(i => i.StatusPovprasevanja.Koda == statusPotrjen).Count();
                model.InquiriesInProgress = context.Povprasevanje.Where(i => i.StatusPovprasevanja.Koda == statusDeloven).Count();
                model.InquiriesInPurchase = context.Povprasevanje.Where(i => i.StatusPovprasevanja.Koda == statusVnabavi).Count();
                model.SubmitedOrders = context.Narocilo_PDO.Count();


                //število odpoklicev v tekočem letu
                var query = from inquiry in context.Povprasevanje
                            where inquiry.ts.Value.Year == DateTime.Now.Year
                            orderby inquiry.ts.Value.Month
                            group inquiry by new { month = inquiry.ts.Value.Month }  into groupInquiries
                            
                            select new InquiriesInYear
                            {
                                Month = groupInquiries.Key.month,
                                Count = groupInquiries.Count()
                            };
                var OrdQuery = query.OrderBy(m => m.Month).ToList();
                model.CurrentYearInquiry = new List<object>();

                model.CurrentYearInquiry.Add(new object[]{
                    "MonthName", "Št. povpraševanj"
                });

                foreach (var item in OrdQuery.ToList())
                {
                    item.MonthName = DataTypesHelper.GetDateTimeMonthByNumber(item.Month);
                    model.CurrentYearInquiry.Add(new object[]{
                        item.MonthName, item.Count
                        });
                }

                if (model.CurrentYearInquiry.Count <= 1)
                    model.CurrentYearInquiry = new List<object>();


                //število povpraševanj na zaposlenega
                var query2 = from inquiry in context.Povprasevanje
                             where inquiry.ts.Value.Year == DateTime.Now.Year
                             group inquiry by inquiry.PovprasevajneOddal into groupInquiry
                             select new InquiriesInYear
                             {
                                 EmployeeName = (from e in context.Osebe_PDO
                                                 where e.OsebaID == groupInquiry.Key
                                                 select new
                                                 {
                                                     Name = e.Ime + " " + e.Priimek
                                                 }).FirstOrDefault().Name,
                                 Count = groupInquiry.Count()
                             };

                model.EmployeesInquiryCount = new List<object>();
                model.EmployeesInquiryCount.Add(new object[]{
                    "EmployeeName", "Št. povpraševanj"
                });

                foreach (var item in query2.ToList())
                {
                    model.EmployeesInquiryCount.Add(new object[]{
                        item.EmployeeName, item.Count
                        });
                }


                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_27, ex);
            }
        }
    }
}