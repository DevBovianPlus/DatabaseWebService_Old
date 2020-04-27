using DatabaseWebService.Common.Enums;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.ModelsOTP.Route;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Objects;
using System.Linq;
using System.Web;

namespace DatabaseWebService.DomainOTP.Concrete
{
    public class RouteRepository : IRouteRepository
    {
        GrafolitOTPEntities context;

        public RouteRepository(GrafolitOTPEntities _context)
        {
            context = _context;
        }

        public List<RouteModel> GetAllRoutes()
        {
            try
            {
                bool checkForPastYear = Convert.ToBoolean(ConfigurationManager.AppSettings["RouteRecallsPastYear"].ToString());
                DateTime dateStart = !checkForPastYear ? DateTime.Now.AddDays(-30).Date : DateTime.Now.AddYears(-1).Date;
                DateTime dateEnd = DateTime.Now.Date;
                string overtake = Enums.StatusOfRecall.PREVZET.ToString();
                string partialOvertake = Enums.StatusOfRecall.DELNO_PREVZET.ToString();
                string approvedStat = Enums.StatusOfRecall.POTRJEN.ToString();

                var query = from route in context.Relacija
                            select new RouteModel
                            {
                                Datum = route.Datum.HasValue ? route.Datum.Value : DateTime.MinValue,
                                Dolzina = route.Dolzina,
                                Koda = route.Koda,
                                Naziv = route.Naziv,
                                RelacijaID = route.RelacijaID,
                                Opomba = route.Opomba,
                                RouteFirstRecallDate = (from recalls in context.Odpoklic
                                                        where recalls.RelacijaID.Value == route.RelacijaID
                                                        orderby recalls.OdpoklicID
                                                        select recalls).FirstOrDefault() != null ? (from recalls in context.Odpoklic
                                                                                                    where recalls.RelacijaID.Value == route.RelacijaID
                                                                                                    orderby recalls.OdpoklicID
                                                                                                    select recalls).FirstOrDefault().ts.Value : DateTime.Now,
                                ts = route.ts.HasValue ? route.ts.Value : DateTime.MinValue,
                                tsIDOsebe = route.tsIDOsebe.HasValue ? route.tsIDOsebe.Value : 0,
                                RecallCount = (from recalls in context.Odpoklic
                                               where (recalls.RelacijaID.Value == route.RelacijaID) && (recalls.ts.Value >= dateStart && recalls.ts.Value <= dateEnd) &&
                                               (recalls.StatusOdpoklica.Koda == approvedStat || recalls.StatusOdpoklica.Koda == overtake || recalls.StatusOdpoklica.Koda == partialOvertake)
                                               select recalls).Count(),
                                SupplierArrangesTransportRecallCount = (from recalls in context.Odpoklic
                                                                        where (recalls.RelacijaID.Value == route.RelacijaID) && !recalls.DobaviteljUrediTransport.Value && (recalls.ts.Value >= dateStart && recalls.ts.Value <= dateEnd) &&
                                                                        (recalls.StatusOdpoklica.Koda == approvedStat || recalls.StatusOdpoklica.Koda == overtake || recalls.StatusOdpoklica.Koda == partialOvertake)
                                                                        select recalls).Count() * (!checkForPastYear ? 12 : 1)
                            };

                var list = query.ToList();

                foreach (var item in list)
                {
                    int monthDiff = ((DateTime.Now.Year - item.RouteFirstRecallDate.Year) * 12) + DateTime.Now.Month - item.RouteFirstRecallDate.Month;
                    int diff = monthDiff > 0 ? monthDiff : 1;

                    if (item.RecallCount >= diff)
                        item.RecallCount = (item.RecallCount / diff) * (!checkForPastYear ? 12 : 1);
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public RouteModel GetRouteByID(int routeID)
        {
            try
            {
                var query = from route in context.Relacija
                            where route.RelacijaID == routeID
                            select new RouteModel
                            {
                                Datum = route.Datum.HasValue ? route.Datum.Value : DateTime.MinValue,
                                Dolzina = route.Dolzina,
                                Koda = route.Koda,
                                Naziv = route.Naziv,
                                RelacijaID = route.RelacijaID,
                                Opomba = route.Opomba,
                                ts = route.ts.HasValue ? route.ts.Value : DateTime.MinValue,
                                tsIDOsebe = route.tsIDOsebe.HasValue ? route.tsIDOsebe.Value : 0
                            };

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public int SaveRoute(RouteModel model, bool updateRecord = true)
        {
            try
            {
                Relacija route = new Relacija();
                route.Datum = model.Datum.CompareTo(DateTime.MinValue) == 0 ? (DateTime?)null : model.Datum;
                route.Dolzina = model.Dolzina;
                route.Koda = model.Koda;
                route.Naziv = model.Naziv;
                route.RelacijaID = model.RelacijaID;
                route.ts = model.ts.CompareTo(DateTime.MinValue) == 0 ? (DateTime?)null : model.ts;
                route.tsIDOsebe = model.tsIDOsebe;
                route.Opomba = model.Opomba;

                if (route.RelacijaID == 0)
                {
                    route.ts = DateTime.Now;
                    context.Relacija.Add(route);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        Relacija original = context.Relacija.Where(r => r.RelacijaID == route.RelacijaID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(route);
                        context.SaveChanges();
                    }
                }

                return route.RelacijaID;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public bool DeleteRoute(int routeID)
        {
            try
            {
                var route = context.Relacija.Where(r => r.RelacijaID == routeID).FirstOrDefault();

                if (route != null)
                {
                    context.Relacija.Remove(route);
                    context.SaveChanges();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_07, ex);
            }
        }

        public List<RouteModel> GetRoutesByCarrierID(int carrierID)
        {
            try
            {
                List<RouteModel> model = new List<RouteModel>();

                //Pridobimo vse unikatne (da smo dobili unikatne smo jih grupirali in tako lažje dobili tudi aktualno ceno) relacije za izbranega prevoznika
                var query = (from tenderPos in context.RazpisPozicija
                             where tenderPos.StrankaID == carrierID
                             group tenderPos by tenderPos.RelacijaID into tenderPosByRoute
                             select tenderPosByRoute).ToList();

                foreach (var item in query)
                {
                    RouteModel route = new RouteModel();

                    var obj = item.OrderByDescending(o => o.Razpis.DatumRazpisa).Where(o => o.Cena > 0).FirstOrDefault();

                    if (obj == null && item.Count() > 0)// če je obj null to pomeni da nimamo nobene cene vnešene ki bi bila večja od 0
                        obj = item.OrderByDescending(o => o.Razpis.DatumRazpisa).FirstOrDefault();

                    if (obj != null)
                    {
                        route.RelacijaID = obj.RelacijaID;
                        route.Naziv = obj.Relacija.Naziv;
                        route.Datum = obj.Razpis.DatumRazpisa;
                        route.Cena = obj.Cena.HasValue ? obj.Cena.Value : 0;
                        model.Add(route);
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_07, ex);
            }
        }

        public List<RouteModel> GetRoutesByCarrierIDAndRouteID(int carrierID, int routeID)
        {
            try
            {
                int temp = 0;
                var query = (from tenderPos in context.RazpisPozicija
                             where tenderPos.StrankaID == carrierID && tenderPos.RelacijaID == routeID
                             select new RouteModel
                             {
                                 RelacijaID = tenderPos.RelacijaID,
                                 Naziv = tenderPos.Relacija.Naziv,
                                 Datum = tenderPos.Razpis.DatumRazpisa,
                                 Cena = tenderPos.Cena.HasValue ? tenderPos.Cena.Value : 0
                             }).ToList();
                query.ForEach(r => r.TempID = ++temp);

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_07, ex);
            }
        }
    }
}