using DatabaseWebService.Common.Enums;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsOTP.Recall;
using DatabaseWebService.ModelsOTP.Route;
using DatabaseWebService.ModelsOTP.Tender;
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
        ITenderRepository tenderRepo;

        public RouteRepository(GrafolitOTPEntities _context, ITenderRepository _tenderRepo)
        {
            context = _context;
            tenderRepo = _tenderRepo;
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
                    if (item.Naziv.Length > 0)
                    {
                        int iPrviOklepaj = item.Naziv.IndexOf(")");
                        if (iPrviOklepaj > 0)
                        {
                            item.DrzavaKoda = item.Naziv.Substring(1, iPrviOklepaj - 1);
                        }

                        string[] split = item.Naziv.Split(' ');
                        if (split.Length > 1)
                        {
                            item.PostaKoda = split[1];
                        }
                    }

                    int monthDiff = ((DateTime.Now.Year - item.RouteFirstRecallDate.Year) * 12) + DateTime.Now.Month - item.RouteFirstRecallDate.Month;
                    int diff = monthDiff > 0 ? monthDiff : 1;

                    if (!(checkForPastYear))
                    {
                        if (item.RecallCount >= diff)
                            item.RecallCount = (item.RecallCount / diff) * (!checkForPastYear ? 12 : 1);
                    }
                }
                list = list.OrderBy(d => d.PostaKoda).ToList().OrderByDescending(p => p.DrzavaKoda).ToList();
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>        
        /// <param name="iViewType">Pregled 1 - Vsi odpoklici, 2 - Grafolit prevoz, 3 - Dobavitelj 4 - Kupec, 5 - Grafolit - Lastni prevoz</param>
        /// <param name="iWeightType">Teža odpoklica 1 - nad 20t, 2 - Pod 20t </param>
        /// <returns></returns>
        int SetRecalCountRelacijaByType(int iRelacijaID, int iZbirnikTonID, hlpViewRoutePricesModel vRPModel)
        {
            DateTime dateStart = vRPModel.DateFrom;
            DateTime dateEnd = vRPModel.DateTo;

            string overtake = Enums.StatusOfRecall.PREVZET.ToString();
            string partialOvertake = Enums.StatusOfRecall.DELNO_PREVZET.ToString();
            string approvedStat = Enums.StatusOfRecall.POTRJEN.ToString();

            int iWeightType = vRPModel.iWeightType;
            int iViewType = vRPModel.iViewType;
            int iRecalCount = 0;

            switch (iWeightType)
            {
                case 0:
                    {
                        switch (iViewType)
                        {
                            case 1:
                                iRecalCount = (from recalls in context.Odpoklic
                                               where (recalls.RelacijaID.Value == iRelacijaID) && (recalls.ts.Value >= dateStart && recalls.ts.Value <= dateEnd) && recalls.KolicinaSkupno > 20000 &&
                                               (recalls.StatusOdpoklica.Koda == approvedStat || recalls.StatusOdpoklica.Koda == overtake || recalls.StatusOdpoklica.Koda == partialOvertake)
                                               select recalls).Count();
                                break;
                            case 2:
                                iRecalCount = (from recalls in context.Odpoklic
                                               where (recalls.RelacijaID.Value == iRelacijaID) && (recalls.ts.Value >= dateStart && recalls.ts.Value <= dateEnd) &&
                                               ((!(bool)recalls.LastenPrevoz) && (!(bool)recalls.DobaviteljUrediTransport) && (!(bool)recalls.KupecUrediTransport)) && recalls.KolicinaSkupno > 20000 &&
                                               (recalls.StatusOdpoklica.Koda == approvedStat || recalls.StatusOdpoklica.Koda == overtake || recalls.StatusOdpoklica.Koda == partialOvertake)
                                               select recalls).Count();
                                break;
                            case 3:
                                iRecalCount = (from recalls in context.Odpoklic
                                               where (recalls.RelacijaID.Value == iRelacijaID) && (recalls.ts.Value >= dateStart && recalls.ts.Value <= dateEnd) && ((bool)recalls.DobaviteljUrediTransport) && recalls.KolicinaSkupno > 20000 &&
                                               (recalls.StatusOdpoklica.Koda == approvedStat || recalls.StatusOdpoklica.Koda == overtake || recalls.StatusOdpoklica.Koda == partialOvertake)
                                               select recalls).Count();
                                break;
                            case 4:
                                iRecalCount = (from recalls in context.Odpoklic
                                               where (recalls.RelacijaID.Value == iRelacijaID) && (recalls.ts.Value >= dateStart && recalls.ts.Value <= dateEnd) && ((bool)recalls.KupecUrediTransport) && recalls.KolicinaSkupno > 20000 &&
                                               (recalls.StatusOdpoklica.Koda == approvedStat || recalls.StatusOdpoklica.Koda == overtake || recalls.StatusOdpoklica.Koda == partialOvertake)
                                               select recalls).Count();
                                break;
                            case 5:
                                iRecalCount = (from recalls in context.Odpoklic
                                               where (recalls.RelacijaID.Value == iRelacijaID) && (recalls.ts.Value >= dateStart && recalls.ts.Value <= dateEnd) && (bool)recalls.LastenPrevoz && recalls.KolicinaSkupno > 20000 &&
                                               (recalls.StatusOdpoklica.Koda == approvedStat || recalls.StatusOdpoklica.Koda == overtake || recalls.StatusOdpoklica.Koda == partialOvertake)
                                               select recalls).Count();
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case 1:
                    {
                        switch (iViewType)
                        {
                            case 1:
                                iRecalCount = (from recalls in context.Odpoklic
                                               where (recalls.RelacijaID.Value == iRelacijaID && (iZbirnikTonID != 0 ? recalls.ZbirnikTonID == iZbirnikTonID : true)) && (recalls.ts.Value >= dateStart && recalls.ts.Value <= dateEnd) && recalls.KolicinaSkupno > 20000 &&
                                               (recalls.StatusOdpoklica.Koda == approvedStat || recalls.StatusOdpoklica.Koda == overtake || recalls.StatusOdpoklica.Koda == partialOvertake)
                                               select recalls).Count();
                                break;
                            case 2:
                                iRecalCount = (from recalls in context.Odpoklic
                                               where (recalls.RelacijaID.Value == iRelacijaID && (iZbirnikTonID != 0 ? recalls.ZbirnikTonID == iZbirnikTonID : true)) && (recalls.ts.Value >= dateStart && recalls.ts.Value <= dateEnd) &&
                                               ((!(bool)recalls.LastenPrevoz) && (!(bool)recalls.DobaviteljUrediTransport) && (!(bool)recalls.KupecUrediTransport)) && recalls.KolicinaSkupno > 20000 &&
                                               (recalls.StatusOdpoklica.Koda == approvedStat || recalls.StatusOdpoklica.Koda == overtake || recalls.StatusOdpoklica.Koda == partialOvertake)
                                               select recalls).Count();
                                break;
                            case 3:
                                iRecalCount = (from recalls in context.Odpoklic
                                               where (recalls.RelacijaID.Value == iRelacijaID && (iZbirnikTonID != 0 ? recalls.ZbirnikTonID == iZbirnikTonID : true)) && (recalls.ts.Value >= dateStart && recalls.ts.Value <= dateEnd) && ((bool)recalls.DobaviteljUrediTransport) && recalls.KolicinaSkupno > 20000 &&
                                               (recalls.StatusOdpoklica.Koda == approvedStat || recalls.StatusOdpoklica.Koda == overtake || recalls.StatusOdpoklica.Koda == partialOvertake)
                                               select recalls).Count();
                                break;
                            case 4:
                                iRecalCount = (from recalls in context.Odpoklic
                                               where (recalls.RelacijaID.Value == iRelacijaID && (iZbirnikTonID != 0 ? recalls.ZbirnikTonID == iZbirnikTonID : true)) && (recalls.ts.Value >= dateStart && recalls.ts.Value <= dateEnd) && ((bool)recalls.KupecUrediTransport) && recalls.KolicinaSkupno > 20000 &&
                                               (recalls.StatusOdpoklica.Koda == approvedStat || recalls.StatusOdpoklica.Koda == overtake || recalls.StatusOdpoklica.Koda == partialOvertake)
                                               select recalls).Count();
                                break;
                            case 5:
                                iRecalCount = (from recalls in context.Odpoklic
                                               where (recalls.RelacijaID.Value == iRelacijaID && (iZbirnikTonID != 0 ? recalls.ZbirnikTonID == iZbirnikTonID : true)) && (recalls.ts.Value >= dateStart && recalls.ts.Value <= dateEnd) && (bool)recalls.LastenPrevoz && recalls.KolicinaSkupno > 20000 &&
                                               (recalls.StatusOdpoklica.Koda == approvedStat || recalls.StatusOdpoklica.Koda == overtake || recalls.StatusOdpoklica.Koda == partialOvertake)
                                               select recalls).Count();
                                break;
                            default:
                                break;
                        }
                    }
                    break;

                case 2:
                    {
                        switch (iViewType)
                        {
                            case 1:
                                iRecalCount = (from recalls in context.Odpoklic
                                               where (recalls.RelacijaID.Value == iRelacijaID && (iZbirnikTonID != 0 ? recalls.ZbirnikTonID == iZbirnikTonID : true)) && (recalls.ts.Value >= dateStart && recalls.ts.Value <= dateEnd) && recalls.KolicinaSkupno < 20000 &&
                                               (recalls.StatusOdpoklica.Koda == approvedStat || recalls.StatusOdpoklica.Koda == overtake || recalls.StatusOdpoklica.Koda == partialOvertake)
                                               select recalls).Count();
                                break;
                            case 2:
                                iRecalCount = (from recalls in context.Odpoklic
                                               where (recalls.RelacijaID.Value == iRelacijaID && (iZbirnikTonID != 0 ? recalls.ZbirnikTonID == iZbirnikTonID : true)) && (recalls.ts.Value >= dateStart && recalls.ts.Value <= dateEnd) &&
                                               ((!(bool)recalls.LastenPrevoz) && (!(bool)recalls.DobaviteljUrediTransport) && (!(bool)recalls.KupecUrediTransport)) && recalls.KolicinaSkupno < 20000 &&
                                               (recalls.StatusOdpoklica.Koda == approvedStat || recalls.StatusOdpoklica.Koda == overtake || recalls.StatusOdpoklica.Koda == partialOvertake)
                                               select recalls).Count();
                                break;
                            case 3:
                                iRecalCount = (from recalls in context.Odpoklic
                                               where (recalls.RelacijaID.Value == iRelacijaID && (iZbirnikTonID != 0 ? recalls.ZbirnikTonID == iZbirnikTonID : true)) && (recalls.ts.Value >= dateStart && recalls.ts.Value <= dateEnd) &&
                                               ((bool)recalls.DobaviteljUrediTransport) && recalls.KolicinaSkupno < 20000 &&
                                               (recalls.StatusOdpoklica.Koda == approvedStat || recalls.StatusOdpoklica.Koda == overtake || recalls.StatusOdpoklica.Koda == partialOvertake)
                                               select recalls).Count();
                                break;
                            case 4:
                                iRecalCount = (from recalls in context.Odpoklic
                                               where (recalls.RelacijaID.Value == iRelacijaID && (iZbirnikTonID != 0 ? recalls.ZbirnikTonID == iZbirnikTonID : true)) && (recalls.ts.Value >= dateStart && recalls.ts.Value <= dateEnd) &&
                                               ((bool)recalls.KupecUrediTransport) && recalls.KolicinaSkupno < 20000 &&
                                               (recalls.StatusOdpoklica.Koda == approvedStat || recalls.StatusOdpoklica.Koda == overtake || recalls.StatusOdpoklica.Koda == partialOvertake)
                                               select recalls).Count();
                                break;
                            case 5:
                                iRecalCount = (from recalls in context.Odpoklic
                                               where (recalls.RelacijaID.Value == iRelacijaID && (iZbirnikTonID != 0 ? recalls.ZbirnikTonID == iZbirnikTonID : true)) && (recalls.ts.Value >= dateStart && recalls.ts.Value <= dateEnd)
                                               && (bool)recalls.LastenPrevoz && recalls.KolicinaSkupno < 20000 &&
                                               (recalls.StatusOdpoklica.Koda == approvedStat || recalls.StatusOdpoklica.Koda == overtake || recalls.StatusOdpoklica.Koda == partialOvertake)
                                               select recalls).Count();
                                break;
                            default:
                                break;
                        }

                    }
                    break;
            }
            return iRecalCount;
        }

        public hlpViewRoutePricesModel GetAllRoutesTransportPricesByViewType(hlpViewRoutePricesModel vRPModel)
        {
            try
            {
                List<RouteTransporterPricesModel> ReturnList = null;
                string overtake = Enums.StatusOfRecall.PREVZET.ToString();
                string partialOvertake = Enums.StatusOfRecall.DELNO_PREVZET.ToString();
                string approvedStat = Enums.StatusOfRecall.POTRJEN.ToString();

                ReturnList = new List<RouteTransporterPricesModel>();
                vRPModel.DateTo = vRPModel.DateTo.AddHours(23).AddMinutes(59);
                //var qOdpoklic = context.Odpoklic.Select(o => o.ts >= vRPModel.DateFrom && o.ts <= vRPModel.DateTo).ToList().Select(p => p.RelacijaID);
                var qOdpoklic1 = context.Odpoklic.Where(o => o.ts >= vRPModel.DateFrom && o.ts <= vRPModel.DateTo).Select(p => p.RelacijaID);





                var list1 = qOdpoklic1.ToList();


                var query = from route in context.Relacija
                            where list1.Contains(route.RelacijaID)
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
                                tsIDOsebe = route.tsIDOsebe.HasValue ? route.tsIDOsebe.Value : 0
                            };

                var list = query.ToList();
                List<RouteModel> rList = list;
                foreach (RouteModel rt in rList)
                {
                    rt.RecallCount = SetRecalCountRelacijaByType(rt.RelacijaID, 0, vRPModel);
                }

                if (vRPModel.iWeightType == 0)
                {
                    // zaključimo, ker rabimo samo število odpoklicov
                    // pri razpisih
                    vRPModel.lRouteList = rList;
                    return vRPModel;
                }

                int iTempID = 0;
                int iCnt = 0;
                int iCntTons = 0;
                int iMyOrder = 0;

                rList = rList.Where(o => o.RecallCount > 0).ToList();
                rList = rList.OrderByDescending(rp => rp.RecallCount).ToList();

                foreach (var item in rList)
                {
                    iMyOrder++;
                    iTempID++;
                    RouteTransporterPricesModel rtpm = new RouteTransporterPricesModel();
                    rtpm.TempID = iTempID;
                    rtpm.Relacija = item.Naziv;
                    rtpm.RecallCount = item.RecallCount;
                    rtpm.IsRoute = true;
                    rtpm.SortIndx = iMyOrder;
                    iCnt = 0;
                    ReturnList.Add(rtpm);

                    //List<TenderPositionModel> tenderRoutesPrices = tenderRepo.GetTenderListByRouteID(item.RelacijaID);

                    var query2 = from tenderPos in context.RazpisPozicija //from tenderPos in tmp.FirstOrDefault().Key.RazpisPozicija
                                 where tenderPos.RelacijaID == item.RelacijaID && tenderPos.Cena > 0 && (vRPModel.iWeightType == 1 ? tenderPos.ZbirnikTonID == 10 : tenderPos.ZbirnikTonID < 10)
                                 orderby tenderPos.ZbirnikTon.SortIdx ascending, tenderPos.Cena ascending
                                 select new TenderPositionModel
                                 {
                                     Cena = tenderPos.Cena.HasValue ? tenderPos.Cena.Value : 0,
                                     RazpisID = tenderPos.RazpisID,
                                     RelacijaID = tenderPos.RelacijaID,
                                     StrankaID = tenderPos.StrankaID,
                                     Stranka = (from client in context.Stranka_OTP
                                                where client.idStranka == tenderPos.StrankaID && client.Activity == 1
                                                select new ClientFullModel
                                                {
                                                    idStranka = client.idStranka,
                                                    KodaStranke = client.KodaStranke,
                                                    NazivPrvi = client.NazivPrvi,
                                                    NazivDrugi = client.NazivDrugi,
                                                    Naslov = client.Naslov
                                                }).FirstOrDefault(),
                                     ZbirnikTonID = tenderPos.ZbirnikTonID.HasValue ? tenderPos.ZbirnikTonID.Value : 0,
                                     ZbirnikTon = (from zt in context.ZbirnikTon
                                                   where zt.ZbirnikTonID == tenderPos.ZbirnikTonID
                                                   select new TonsModel
                                                   {
                                                       Koda = zt.Koda,
                                                       Naziv = zt.Naziv,
                                                       ts = tenderPos.ts.HasValue ? zt.ts.Value : DateTime.MinValue,
                                                   }).FirstOrDefault(),
                                 };

                    iCntTons = 0;
                    int iLastZTId = 0;
                    RouteTransporterPricesModel rtpmTonsItem = new RouteTransporterPricesModel();
                    foreach (var itm in query2)
                    {

                        if (iLastZTId == itm.ZbirnikTonID)
                            continue;
                        else
                        {
                            if (iCntTons == 4)
                            {
                                iCntTons = 0;
                                rtpmTonsItem = new RouteTransporterPricesModel();
                            }
                        }

                        iTempID++;

                        rtpmTonsItem.TempID = iTempID;
                        rtpmTonsItem.Relacija = itm.ZbirnikTon.Koda;
                        rtpmTonsItem.RecallCount = SetRecalCountRelacijaByType(itm.RelacijaID, itm.ZbirnikTonID, vRPModel); ;
                        if (rtpmTonsItem.RecallCount == 0 && iCntTons == 0)
                        {
                            continue;
                        }
                        rtpmTonsItem.IsRoute = false;
                        rtpmTonsItem.SortIndx = iMyOrder;

                        if (rtpmTonsItem.DodaneStrankeID == null) rtpmTonsItem.DodaneStrankeID = new List<int>();
                        if (!(rtpmTonsItem.DodaneStrankeID.Contains(itm.StrankaID)))
                        {
                            rtpmTonsItem.DodaneStrankeID.Add(itm.StrankaID);
                            iCntTons++;
                        }
                        else
                            continue;



                        switch (iCntTons)
                        {
                            case 1:
                                rtpmTonsItem.Prevoznik_1 = itm.Stranka.NazivPrvi;
                                rtpmTonsItem.Prevoznik_1_Cena = itm.Cena;
                                break;
                            case 2:
                                rtpmTonsItem.Prevoznik_2 = itm.Stranka.NazivPrvi;
                                rtpmTonsItem.Prevoznik_2_Cena = itm.Cena;
                                break;
                            case 3:
                                rtpmTonsItem.Prevoznik_3 = itm.Stranka.NazivPrvi;
                                rtpmTonsItem.Prevoznik_3_Cena = itm.Cena;
                                break;
                            case 4:
                                rtpmTonsItem.Prevoznik_4 = itm.Stranka.NazivPrvi;
                                rtpmTonsItem.Prevoznik_4_Cena = itm.Cena;
                                break;
                            default:
                                break;
                        }

                        if (iCntTons == 4)
                        {
                            ReturnList.Add(rtpmTonsItem);
                            iLastZTId = itm.ZbirnikTonID;
                            continue;
                        }
                        else if (query2.Count() < 4 && iCntTons == query2.Count())
                        {
                            ReturnList.Add(rtpmTonsItem);
                            iLastZTId = itm.ZbirnikTonID;
                            continue;
                        }


                    }


                }

                vRPModel.lRouteTransporterPriceModel = ReturnList.OrderBy(rp => rp.SortIndx).ToList();

                return vRPModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        //public hlpViewRoutePricesModel GetAllRoutesTransportPricesByViewType(hlpViewRoutePricesModel vRPModel)
        //{
        //    try
        //    {
        //        List<RouteTransporterPricesModel> ReturnList = null;
        //        string overtake = Enums.StatusOfRecall.PREVZET.ToString();
        //        string partialOvertake = Enums.StatusOfRecall.DELNO_PREVZET.ToString();
        //        string approvedStat = Enums.StatusOfRecall.POTRJEN.ToString();

        //        ReturnList = new List<RouteTransporterPricesModel>();

        //        //var qOdpoklic = context.Odpoklic.Select(o => o.ts >= vRPModel.DateFrom && o.ts <= vRPModel.DateTo).ToList().Select(p => p.RelacijaID);
        //        var qOdpoklic1 = context.Odpoklic.Where(o => o.ts >= vRPModel.DateFrom && o.ts <= vRPModel.DateTo).Select(p => p.RelacijaID);





        //        var list1 = qOdpoklic1.ToList();


        //        var query = from route in context.Relacija
        //                    where list1.Contains(route.RelacijaID)
        //                    select new RouteModel
        //                    {
        //                        Datum = route.Datum.HasValue ? route.Datum.Value : DateTime.MinValue,
        //                        Dolzina = route.Dolzina,
        //                        Koda = route.Koda,
        //                        Naziv = route.Naziv,
        //                        RelacijaID = route.RelacijaID,
        //                        Opomba = route.Opomba,
        //                        RouteFirstRecallDate = (from recalls in context.Odpoklic
        //                                                where recalls.RelacijaID.Value == route.RelacijaID
        //                                                orderby recalls.OdpoklicID
        //                                                select recalls).FirstOrDefault() != null ? (from recalls in context.Odpoklic
        //                                                                                            where recalls.RelacijaID.Value == route.RelacijaID
        //                                                                                            orderby recalls.OdpoklicID
        //                                                                                            select recalls).FirstOrDefault().ts.Value : DateTime.Now,
        //                        ts = route.ts.HasValue ? route.ts.Value : DateTime.MinValue,
        //                        tsIDOsebe = route.tsIDOsebe.HasValue ? route.tsIDOsebe.Value : 0
        //                    };

        //        var list = query.ToList();
        //        List<RouteModel> rList = list;
        //        foreach (RouteModel rt in rList)
        //        {
        //            rt.RecallCount = SetRecalCountRelacijaByType(rt.RelacijaID, 0, vRPModel);
        //        }


        //        int iTempID = 0;
        //        int iCnt = 0;
        //        int iCntTons = 0;
        //        int iMyOrder = 0;

        //        rList = rList.Where(o => o.RecallCount > 0).ToList();
        //        rList = rList.OrderByDescending(rp => rp.RecallCount).ToList();

        //        foreach (var item in rList)
        //        {
        //            iMyOrder++;
        //            iTempID++;
        //            RouteTransporterPricesModel rtpm = new RouteTransporterPricesModel();
        //            rtpm.TempID = iTempID;
        //            rtpm.Relacija = item.Naziv;
        //            rtpm.RecallCount = item.RecallCount;
        //            rtpm.IsRoute = true;
        //            rtpm.SortIndx = iMyOrder;
        //            iCnt = 0;
        //            ReturnList.Add(rtpm);

        //            //List<TenderPositionModel> tenderRoutesPrices = tenderRepo.GetTenderListByRouteID(item.RelacijaID);

        //            var query2 = from tenderPos in context.RazpisPozicija //from tenderPos in tmp.FirstOrDefault().Key.RazpisPozicija
        //                         where tenderPos.RelacijaID == item.RelacijaID && tenderPos.Cena > 0
        //                         orderby tenderPos.ZbirnikTonID ascending, tenderPos.Cena ascending
        //                         select new TenderPositionModel
        //                         {
        //                             Cena = tenderPos.Cena.HasValue ? tenderPos.Cena.Value : 0,
        //                             RazpisID = tenderPos.RazpisID,
        //                             RelacijaID = tenderPos.RelacijaID,
        //                             StrankaID = tenderPos.StrankaID,
        //                             Stranka = (from client in context.Stranka_OTP
        //                                        where client.idStranka == tenderPos.StrankaID && client.Activity == 1
        //                                        select new ClientFullModel
        //                                        {
        //                                            idStranka = client.idStranka,
        //                                            KodaStranke = client.KodaStranke,
        //                                            NazivPrvi = client.NazivPrvi,
        //                                            NazivDrugi = client.NazivDrugi,
        //                                            Naslov = client.Naslov
        //                                        }).FirstOrDefault(),
        //                             ZbirnikTonID = tenderPos.ZbirnikTonID.HasValue ? tenderPos.ZbirnikTonID.Value : 0,
        //                             ZbirnikTon = (from zt in context.ZbirnikTon
        //                                           where zt.ZbirnikTonID == tenderPos.ZbirnikTonID
        //                                           select new TonsModel
        //                                           {
        //                                               Koda = zt.Koda,
        //                                               Naziv = zt.Naziv,
        //                                               ts = tenderPos.ts.HasValue ? zt.ts.Value : DateTime.MinValue,
        //                                           }).FirstOrDefault(),
        //                         };

        //            iCntTons = 0;
        //            int iLastZTId = 0;
        //            RouteTransporterPricesModel rtpmTonsItem = new RouteTransporterPricesModel();
        //            foreach (var itm in query2)
        //            {
        //                iTempID++;

        //                rtpmTonsItem.TempID = iTempID;
        //                rtpmTonsItem.Relacija = itm.ZbirnikTon.Koda;
        //                rtpmTonsItem.RecallCount = SetRecalCountRelacijaByType(itm.RelacijaID, itm.ZbirnikTonID, vRPModel); ;
        //                rtpmTonsItem.IsRoute = false;
        //                rtpmTonsItem.SortIndx = iMyOrder;

        //                if (rtpmTonsItem.DodaneStrankeID == null) rtpmTonsItem.DodaneStrankeID = new List<int>();
        //                if (!(rtpmTonsItem.DodaneStrankeID.Contains(itm.StrankaID)))
        //                {
        //                    rtpmTonsItem.DodaneStrankeID.Add(itm.StrankaID);
        //                    iCntTons++;
        //                }
        //                else
        //                    continue;



        //                switch (iCntTons)
        //                {
        //                    case 1:
        //                        rtpmTonsItem.Prevoznik_1 = itm.Stranka.NazivPrvi;
        //                        rtpmTonsItem.Prevoznik_1_Cena = itm.Cena;
        //                        break;
        //                    case 2:
        //                        rtpmTonsItem.Prevoznik_2 = itm.Stranka.NazivPrvi;
        //                        rtpmTonsItem.Prevoznik_2_Cena = itm.Cena;
        //                        break;
        //                    case 3:
        //                        rtpmTonsItem.Prevoznik_3 = itm.Stranka.NazivPrvi;
        //                        rtpmTonsItem.Prevoznik_3_Cena = itm.Cena;
        //                        break;
        //                    case 4:
        //                        rtpmTonsItem.Prevoznik_4 = itm.Stranka.NazivPrvi;
        //                        rtpmTonsItem.Prevoznik_4_Cena = itm.Cena;
        //                        break;
        //                    default:
        //                        break;
        //                }
        //                iLastZTId = itm.ZbirnikTonID;
        //                if (iCntTons == 4)
        //                {
        //                    ReturnList.Add(rtpmTonsItem);
        //                    continue;
        //                }

        //            }


        //        }

        //        vRPModel.lRouteTransporterPriceModel = ReturnList.OrderBy(rp => rp.SortIndx).ToList();

        //        return vRPModel;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ValidationExceptionError.res_06, ex);
        //    }
        //}

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