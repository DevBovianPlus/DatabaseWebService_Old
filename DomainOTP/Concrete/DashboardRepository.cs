using DatabaseWebService.Common;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.ModelsOTP;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DatabaseWebService.DomainOTP.Concrete
{
    public class DashboardRepository : IDashboardRepository
    {
        GrafolitOTPEntities context;

        public DashboardRepository(GrafolitOTPEntities _context)
        {
            context = _context;
        }

        public DashboardDataModel GetDashboardData()
        {
            try
            {
                var model = new DashboardDataModel();
                string approvedStat = Enums.StatusOfRecall.POTRJEN.ToString();
                string rejectedStat = Enums.StatusOfRecall.ZAVRNJEN.ToString();
                string needApprovalStat = Enums.StatusOfRecall.V_ODOBRITEV.ToString();
                string clientTypeTransport = Enums.TypeOfClient.PREVOZNIK.ToString();
                string clientTypeWarehouse = Enums.TypeOfClient.SKLADISCE.ToString();
                string overtake = Enums.StatusOfRecall.PREVZET.ToString();
                string partialOvertake = Enums.StatusOfRecall.DELNO_PREVZET.ToString();

                model.AllRecalls = context.Odpoklic.Count();
                model.ApprovedRecalls = context.Odpoklic.Where(o => o.StatusOdpoklica.Koda == approvedStat).Count();
                model.RejectedRecalls = context.Odpoklic.Where(o => o.StatusOdpoklica.Koda == rejectedStat).Count();
                model.NeedsApproval = context.Odpoklic.Where(o => o.StatusOdpoklica.Koda == needApprovalStat).Count();
                model.Routes = context.Relacija.Count();
                model.Transporters = context.Stranka_OTP.Where(sotp => sotp.TipStranka.Koda == clientTypeTransport).Count();
                model.OwnWarehouse = context.Stranka_OTP.Where(sotp => sotp.TipStranka.Koda == clientTypeWarehouse).Count();

                //število odpoklicev v tekočem letu
                var query = from recalls in context.Odpoklic
                            where recalls.ts.Value.Year == DateTime.Now.Year
                            group recalls by new { month = recalls.ts.Value.Month } into groupRecall
                            select new RecallsInYear
                            {
                                Month = groupRecall.Key.month,
                                Count = groupRecall.Count()
                            };

                model.CurrentYearRecall = new List<object>();

                model.CurrentYearRecall.Add(new object[]{
                    "MonthName", "Št. odpoklicev"
                });

                foreach (var item in query.ToList())
                {
                    item.MonthName = DataTypesHelper.GetDateTimeMonthByNumber(item.Month);
                    model.CurrentYearRecall.Add(new object[]{
                        item.MonthName, item.Count
                        });
                }

                //število odpoklicev na zaposlenega
                var query2 = from recalls in context.Odpoklic
                             where recalls.ts.Value.Year == DateTime.Now.Year
                             group recalls by recalls.Osebe_OTP into groupRecall
                             select new RecallsInYear
                             {
                                 EmployeeName = groupRecall.Key.Ime + " " + groupRecall.Key.Priimek,
                                 Count = groupRecall.Count()
                             };

                model.EmployeesRecallCount = new List<object>();
                model.EmployeesRecallCount.Add(new object[]{
                    "EmployeeName", "Št. odpoklicev "
                });

                foreach (var item in query2.ToList())
                {
                    model.EmployeesRecallCount.Add(new object[]{
                        item.EmployeeName, item.Count
                        });
                }

                //gledamo odpoklice za 30 dni nazaj ali odpoklice 1 leta nazaj. In vsi odpoklici morajo biti odobreni, prevzeti ali delno prevzeti.
                bool checkForPastYear = Convert.ToBoolean(ConfigurationManager.AppSettings["RouteRecallsPastYear"].ToString());
                DateTime dateStart = !checkForPastYear ? DateTime.Now.AddDays(-30).Date : DateTime.Now.AddYears(-1).Date;
                DateTime dateEnd = DateTime.Now.Date;

                //število odpoklicev na posameznega prevoznika
                var query3 = from recalls in context.Odpoklic
                             where recalls.ts.Value >= dateStart && recalls.ts.Value <= dateEnd &&
                             (recalls.StatusOdpoklica.Koda == approvedStat || recalls.StatusOdpoklica.Koda == overtake || recalls.StatusOdpoklica.Koda == partialOvertake)
                             group recalls by recalls.Stranka_OTP1 into groupRecall
                             select new RecallsInYear
                             {
                                 Transporter = groupRecall.Key.NazivPrvi,
                                 Count = !checkForPastYear ? groupRecall.Count() * 12 : groupRecall.Count()
                             };

                model.TransporterRecallCount = new List<object>();
                model.TransporterRecallCount.Add(new object[]{
                    "Transporter", "Št. odpoklicov"
                });

                foreach (var item in query3.ToList())
                {
                    if (item.Count > 0)
                    {
                        model.TransporterRecallCount.Add(new object[]{
                        item.Transporter, item.Count
                        });
                    }
                }

                

                var query4 = from recalls in context.Odpoklic
                             where recalls.ts.Value >= dateStart && recalls.ts.Value <= dateEnd &&
                             (recalls.StatusOdpoklica.Koda == approvedStat || recalls.StatusOdpoklica.Koda == overtake || recalls.StatusOdpoklica.Koda == partialOvertake)
                             group recalls by recalls.Relacija into groupRecalls
                             select new RecallsInYear
                             {
                                 Route = groupRecalls.Key.Naziv,
                                 Count = !checkForPastYear ? groupRecalls.Count() * 12 : groupRecalls.Count()
                             };

                model.RouteRecallCount = new List<object>();
                model.RouteRecallCount.Add(new object[]{
                    "Relacija", "Št. odpoklicov"
                });

                foreach (var item in query4.OrderByDescending(o => o.Count).Take(10).ToList())
                {
                    if (item.Count > 0)
                    {
                        model.RouteRecallCount.Add(new object[]{
                        item.Route, item.Count
                        });
                    }
                }

                var query5 = from recalls in context.Odpoklic
                             where recalls.ts.Value.Year == DateTime.Now.Year && (recalls.StatusOdpoklica.Koda == approvedStat || recalls.StatusOdpoklica.Koda == overtake || recalls.StatusOdpoklica.Koda == partialOvertake)
                             group recalls by recalls.DobaviteljNaziv into groupRecalls
                             select new RecallsInYear
                             {
                                 Supplier = groupRecalls.Key,
                                 Count = groupRecalls.Count()
                             };

                model.SupplierRecallCount = new List<object>();
                model.SupplierRecallCount.Add(new object[]{
                    "Dobavitelj", "Št. odpoklicov"
                });

                foreach (var item in query5.OrderByDescending(o => o.Count).Take(10).ToList())
                {
                    if (item.Count > 0)
                    {
                        model.SupplierRecallCount.Add(new object[]{
                        item.Supplier, item.Count
                        });
                    }
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