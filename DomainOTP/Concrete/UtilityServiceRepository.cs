using DatabaseWebService.Common;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.ModelsOTP;
using DatabaseWebService.ModelsOTP.Order;
using DatabaseWebService.ModelsOTP.Recall;
using DatabaseWebService.ModelsOTP.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using DatabaseWebService.DomainPDO;
using DatabaseWebService.ModelsPDO.Order;
using DatabaseWebService.DomainPDO.Abstract;
using DatabaseWebService.ModelsPDO.Inquiry;

namespace DatabaseWebService.DomainOTP.Concrete
{
    public class UtilityServiceRepository : IUtilityServiceRepository
    {
        GrafolitOTPEntities context;
        GrafolitPDOEntities contextPDO;
        ISystemMessageEventsRepository_OTP messageRepo;
        //ISystemEmailMessageRepository_PDO messageRepoPDO;
        IRecallRepository recallRepo;
        //IOrderPDORepository orderPDORepo;
        IClientOTPRepository clientRepo;
        IMSSQLFunctionsRepository sqlFunctionRepo;
        

        public UtilityServiceRepository(GrafolitOTPEntities _context, GrafolitPDOEntities _contextPDO, ISystemMessageEventsRepository_OTP _messageRepo, IRecallRepository _recallRepo, IMSSQLFunctionsRepository _sqlRepo, IClientOTPRepository _clientRepo)
        {
            context = _context;
            contextPDO = _contextPDO;
            messageRepo = _messageRepo;
            //messageRepoPDO = _messageRepoPDO;
            recallRepo = _recallRepo;
            clientRepo = _clientRepo;
            sqlFunctionRepo = _sqlRepo;
            //orderPDORepo = _orderPDORepo;
        }

        private bool CheckIfExistStevilkaOdpoklica(string stevOdpoklica)
        {
            bool b = false;

            var query = (from pv in context.PrevzemiView
                         where pv.Stevilka_Odpoklica.Contains(stevOdpoklica)
                         select new
                         {
                             Datum_Prevzema = pv.Datum_Prevzema,
                             Artikel = pv.Artikel,
                             Dobavitelj = pv.Dobavitelj,
                             Stevilka_Odpoklica = pv.Stevilka_Odpoklica
                         }).OrderByDescending(o => o.Datum_Prevzema);

            var listOfPrevzemi = query.ToList();

            if (listOfPrevzemi.Count > 0) return true;

            return b;
        }

        public void CheckForOrderTakeOver2()
        {
            try
            {
                int iCnt = 0;
                string prevzetKoda = Enums.StatusOfRecall.PREVZET.ToString();
                //List<PrevzemiView> list = (from p in context.PrevzemiView select p).ToList();
                int prevzeto = context.StatusOdpoklica.SingleOrDefault(so => so.Koda == prevzetKoda).StatusOdpoklicaID;
                List<Odpoklic> listOfRecalls = context.Odpoklic.Where(o => o.StatusID != prevzeto).ToList();


                foreach (var item in listOfRecalls)
                {
                    string searchString = item.OdpoklicStevilka.ToString() + "-";//stari zapis odpoklica v bazi Grafolit55SI
                    string searchString2 = item.OdpoklicStevilka.ToString();//nov zapis za odpoklic v bazo Grafolit55SI


                    iCnt++;
                    if ((CheckIfExistStevilkaOdpoklica(searchString)) || (CheckIfExistStevilkaOdpoklica(searchString2)))
                    {
                        context.Entry(item).Entity.StatusID = prevzeto;
                        context.OdpoklicPozicija.Where(op => op.OdpoklicID == item.OdpoklicID).ToList().ForEach(op => context.Entry(op).Entity.StatusPrevzeto = true);
                    }

                }

                //foreach (var item in listOfRecalls)
                //{
                //    string searchString = item.OdpoklicStevilka.ToString() + "-";//stari zapis odpoklica v bazi Grafolit55SI
                //    string searchString2 = item.OdpoklicStevilka.ToString();//nov zapis za odpoklic v bazo Grafolit55SI
                //    var prevzemZVezajem = context.PrevzemiView.Any(pv => pv.Stevilka_Odpoklica.Contains(searchString));
                //    var prevzemBrezVezaja = context.PrevzemiView.Any(pv => pv.Stevilka_Odpoklica.Contains(searchString2));

                //    if (prevzemZVezajem || prevzemBrezVezaja)
                //    {
                //        context.Entry(item).Entity.StatusID = prevzeto;
                //        //vsaki poziciji odpoklica nastavimo StatusPrevzeto na true;
                //        context.OdpoklicPozicija.Where(op => op.OdpoklicID == item.OdpoklicID).ToList().ForEach(op => context.Entry(op).Entity.StatusPrevzeto = true);
                //    }
                //}
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("CheckForOrderTakeOver Method Error! ", ex);
            }
        }

        public void CreateAndSendOrdersMultiple()
        {
            try
            {
                string prevzetKoda = Enums.StatusOfRecall.USTVARJENO_NAROCILO.ToString();
                string errNoSend = Enums.StatusOfRecall.ERR_ORDER_NO_SEND.ToString();
                //List<PrevzemiView> list = (from p in context.PrevzemiView select p).ToList();
                int narociloKreirano = context.StatusOdpoklica.SingleOrDefault(so => so.Koda == prevzetKoda).StatusOdpoklicaID;
                int narociloNiPoslano = context.StatusOdpoklica.SingleOrDefault(so => so.Koda == errNoSend).StatusOdpoklicaID;
                List<Odpoklic> listOfRecalls = context.Odpoklic.Where(o => o.StatusID == narociloKreirano || o.StatusID == narociloNiPoslano).ToList();

                foreach (var item in listOfRecalls)
                {
                    GetOrderPDFFile(item.OdpoklicID);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("CreateAndSendOrdersMultiple Method Error! ", ex);
            }
        }

        /// <summary>
        ///// Check all PDf for PDO Orders and send it through email
        ///// </summary>
        //public void CreatePDFAndSendPDOOrdersMultiple()
        //{
        //    try
        //    {
        //        string prevzetKoda = Enums.StatusOfRecall.USTVARJENO_NAROCILO.ToString();                
        //        int narociloKreirano = contextPDO.StatusPovprasevanja.SingleOrDefault(so => so.Koda == prevzetKoda).StatusPovprasevanjaID;
        //        List<Narocilo_PDO> listOfPDOOrders = contextPDO.Narocilo_PDO.Where(n => n.StatusID == narociloKreirano).ToList();

        //        foreach (var item in listOfPDOOrders)
        //        {
        //            GetOrderPDFFilePDO(item.NarociloID);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("CreateAndSendOrdersMultiple Method Error! ", ex);
        //    }
        //}


        public void CheckForOrderTakeOver()
        {
            try
            {
                string prevzetoKoda = Enums.StatusOfRecall.PREVZET.ToString();
                List<PrevzemiView> list = (from p in context.PrevzemiView select p).ToList();

                int prevzeto = context.StatusOdpoklica.SingleOrDefault(so => so.Koda == prevzetoKoda).StatusOdpoklicaID;

                foreach (var item in list)
                {
                    string[] splitRecallNums = item.Stevilka_Odpoklica.Split(';');

                    foreach (var recallNum in splitRecallNums)
                    {
                        if (item.Stevilka_Odpoklica.Contains('-'))//preverjamo če imamo še stare zapise v view-u (300-1,2,3;302-1,2,3), 
                                                                  //kjer smo prevzemali vsako pozicijo posebaj. Nove zahteve so, da se zapre celoten odpoklic naenkrat,
                                                                  //zato bodo novi zaposi vsebovali (300;301;302)
                        {
                            string[] split = recallNum.Split('-');
                            int recall = DataTypesHelper.ParseInt(split[0]);
                            /*string[] recallPositions = split[1].Split(',');
                            foreach (var recallPosNum in recallPositions)
                            {
                                int recallPos = DataTypesHelper.ParseInt(split[0]);
                                var result = context.OdpoklicPozicija.SingleOrDefault(op => op.ZaporednaStevilka == recallPos && op.Odpoklic.OdpoklicStevilka == recall);

                                if (result != null)
                                    context.Entry(result).Entity.StatusPrevzeto = true;
                            }*/

                            Odpoklic odpoklic = context.Odpoklic.SingleOrDefault(o => o.OdpoklicStevilka == recall);
                            context.Entry(odpoklic).Entity.StatusID = prevzeto;

                            //if (odpoklic != null && odpoklic.OdpoklicPozicija.Count == recallPositions.Length)
                            /*else if (odpoklic != null && odpoklic.OdpoklicPozicija.Count > recallPositions.Length)
                                odpoklic.StatusID = context.StatusOdpoklica.SingleOrDefault(so => so.Koda == Enums.StatusOfRecall.DELNO_PREVZET.ToString()).StatusOdpoklicaID;
                                */
                        }
                        else
                        {
                            int recall = DataTypesHelper.ParseInt(recallNum);
                            Odpoklic odpoklic = context.Odpoklic.SingleOrDefault(o => o.OdpoklicStevilka == recall);
                            context.Entry(odpoklic).Entity.StatusID = prevzeto;
                        }
                    }
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("CheckForOrderTakeOver Method Error! ", ex);
            }
        }

        public void CheckRecallsForCarriersSubmittingPrices()
        {
            //pridobi vse odpoklice ki imajo flag PovprasevanjePolsnoPrevoznikom nastavlen na 1 in PrevoznikOddalNajnizjoCeno 0 ter DatumNaklada še ni potekel
            List<int> recalls = context.Odpoklic.Where(o => o.PovprasevanjePoslanoPrevoznikom.Value &&
           !o.PrevoznikOddalNajnizjoCeno.Value &&
           o.DatumNaklada > DateTime.Now).Select(o => o.OdpoklicID).ToList();//select uporabimo zato ker v spodnjem query kličemo funkcijo Any ki dovoljuje samo enostavne 

            //pridobimo vse prijave prevoznikov ki predstavljajo odpoklice v seznamu odpoklicev (recalls) in jih grupiramo po OdpoklicID polju
            List<IGrouping<Odpoklic, PrijavaPrevoznika>> listPrijavaPrevoznika = context.PrijavaPrevoznika.Where(pp => recalls.Any(o => o == pp.OdpoklicID)).ToList().GroupBy(g => g.Odpoklic).ToList();

            foreach (var item in listPrijavaPrevoznika)
            {
                //En teden pred datumom naklada
                DateTime oneWeekFromLoadingDate = item.Key.DatumNaklada.Value.AddDays(-7);

                if (oneWeekFromLoadingDate.Date == DateTime.Now.Date)//če je današnji dan enak tistemu ki je en teden pred datumom naklada
                {
                    DataTypesHelper.LogThis("Trenutni datum je 1 teden pred datumom naklada " + item.Key.DatumNaklada.Value.ToShortDateString() + " za " + item.Key.Relacija.Naziv);
                    CheckForLowestPriceInPrijavaPrevoznika(item);
                }
                else if (oneWeekFromLoadingDate.Date < DateTime.Now.Date && item.Key.DatumNaklada.Value.Date > DateTime.Now.Date)//če je današnji datum večji (pretekel) kot en teden pre datumom naklada in vseeno manjši od datuma naklada.
                {
                    DataTypesHelper.LogThis("Trenutni datum je pretekel Datum naklada -1 teden " + item.Key.DatumNaklada.Value.ToShortDateString() + " za " + item.Key.Relacija.Naziv);
                    //Implementiraj da se bo pošiljalo samo enkrat na dan.
                    //Pošljemo mail logistiki.
                    //Še vseeno zbiramo najnižjo ceno prevoznikov 
                    CheckForLowestPriceInPrijavaPrevoznika(item);
                }
            }
        }

        private void CheckForLowestPriceInPrijavaPrevoznika(IGrouping<Odpoklic, PrijavaPrevoznika> item)
        {
            string potrjen = Enums.StatusOfRecall.POTRJEN.ToString();
            //v seznamu PrijavaPrevoznika glede na odpoklic poiščemo takšnega, ki ima najmanjšo ceno
            var lowestPrice = item.Where(pp => pp.PrijavljenaCena > 0 && pp.PrijavljenaCena <= pp.PrvotnaCena)
                .OrderBy(pp => pp.PrijavljenaCena)
                .ThenBy(pp => pp.DatumPrijave)
                .FirstOrDefault();

            if (lowestPrice != null)//našli smo prevoznika z najnižjo ceno
            {
                DataTypesHelper.LogThis("Našli smo najnižjo ceno! " + lowestPrice.PrijavljenaCena.Value.ToString("N2") + " za prevoznika " + lowestPrice.Stranka_OTP.NazivPrvi);
                //Tukaj se bo že samo po sebi poslalo enkrat, ker imamo v prvem query-ju nastavljeno da je PrevoznikOddalNajnizjoCeno
                context.Entry(item.Key).Entity.PrevoznikOddalNajnizjoCeno = true;
                context.Entry(item.Key).Entity.StatusID = context.StatusOdpoklica.Where(so => so.Koda == potrjen).FirstOrDefault().StatusOdpoklicaID;
                context.Entry(item.Key).Entity.Prevozniki = "";
                context.Entry(item.Key).Entity.DobaviteljID = lowestPrice.PrevoznikID;
                context.Entry(item.Key).Entity.CenaPrevoza = lowestPrice.PrijavljenaCena;
                //bi bilo potrebno to ceno vnesti tudi v zadnji razpis za to relacijo za tega prevoznika???

                context.Entry(item.Key).Entity.DatumPosiljanjaMailLogistika = DateTime.Now;



                List<RazpisPozicija> tenderPositions = new List<RazpisPozicija>();
                var test = (from tenderPosition in context.RazpisPozicija
                            where tenderPosition.RelacijaID == item.Key.RelacijaID
                            group tenderPosition by tenderPosition.StrankaID into transportGroup
                            select transportGroup).ToList();

                //poiščemo prevoznika ki je oddal najnižjo ceno v seznamu razpisov pozicij, ki so grupirane po id prevoznika
                var carrierTenderPos = test.Where(rp => rp.Key == lowestPrice.PrevoznikID).FirstOrDefault();
                if (carrierTenderPos != null)
                {
                    var list = carrierTenderPos.ToList();//dobimo seznam RazpisPozicij za posameznega prevoznika

                    //uredimo seznam po datumu razpisa (padajoče) in izberemo prvo pozicijo (RazpisPozicija)
                    var tenderPos = list.OrderByDescending(o => o.Razpis.DatumRazpisa).Where(o => o.Cena > 0).FirstOrDefault();

                    if (tenderPos == null)
                        tenderPos = list.OrderByDescending(o => o.Razpis.DatumRazpisa).FirstOrDefault();

                    if (tenderPos != null)
                        context.Entry(item.Key).Entity.RazpisPozicijaID = tenderPos.RazpisPozicijaID;
                }


                context.SaveChanges();

                //poslati mail izbranemu prevozniku
                messageRepo.CreateEmailCarrierSelectedOrNot(item.Key, lowestPrice);

                //poslati mail ostalim prevoznikom da niso bili izbrani, ki so oddali ceno (PrijavljenaCena > 0)
                var collection = item.Where(pp => pp.PrijavljenaCena > 0 && pp.PrijavaPrevoznikaID != lowestPrice.PrijavaPrevoznikaID).ToList();
                foreach (var obj in collection)
                {
                    messageRepo.CreateEmailCarrierSelectedOrNot(item.Key, obj, false);
                }

                //poslati mail logistiki o izbranem prevozniku
                messageRepo.CreateEmailLogisticsCarrierSelected(item.Key, lowestPrice);

                // izdelaj Order XML za prevoznika
                //CreateOrderXMLByType("0240", item.Key.OdpoklicID);
            }
            else
            {//Nismo našli prevoznika z najnižjo ceno - KAJ ČE BI TO LOVILI V EKSTRA METODI, KJER BI PREGLEDALI VSE ODPOKLICE IN NE ENEGA PO ENEGA
             //Implementiraj da se bo pošiljalo samo enkrat na dan. Ker se bo windows service (UtilityService) zaganjal na 15 min.
             //Pošljemo mail logistiki
             /*bool mailSend = false;

             if (item.Key.DatumPosiljanjaMailLogistika.HasValue)
             {
                 //preveri če je že poteklo 24ur
                 if (item.Key.DatumPosiljanjaMailLogistika.Value.AddHours(24) <= DateTime.Now)
                 {
                     mailSend = true;
                     //Pošljemo mail logisti spet o neizbranem prevozniku
                 }
             }
             else
             {
                 mailSend = true;
                 //logisti še nismo poslali mail zato ga pošljemo takoj.(tema: neizbran prevoznik )

             }

             if (mailSend)
             {
                 context.Entry(item.Key).Entity.DatumPosiljanjaMailLogistika = DateTime.Now;
                 context.SaveChanges();
             }*/
            }
        }

        #region "XML Order generator"

        /// <summary>
        /// Launch the application with some options set.
        /// </summary>
        public void LaunchPantheonCreatePDF()
        {
            // For the example
            string ex1 = ConfigurationManager.AppSettings["PantheonCreatePDFPath"].ToString();
            string PanthExeFile = ConfigurationManager.AppSettings["PantheonEXEFile"].ToString();
            string PanthExeArgs = ConfigurationManager.AppSettings["PantheonEXEArgs"].ToString();
            string PanthDB = ConfigurationManager.AppSettings["PantheonDB"].ToString();
            string sPanthExeTimeout = ConfigurationManager.AppSettings["PantheonEXETimeOut"];

            DataTypesHelper.LogThis("OTP Config: PDF Path: " + ex1 + ", PantheonEXEFile: " + PanthExeFile + ", Args: " + PanthExeArgs + ", PanthDB: " + PanthDB + ", Timeout: " + sPanthExeTimeout);

            int timeout = Convert.ToInt32(sPanthExeTimeout);
            //string ex1 = "C:\\Temp\\OtpProject\\";
            PanthExeArgs = PanthExeArgs.Replace("DatabaseNameDB", "\"" + PanthDB + "\"");

            DataTypesHelper.LogThis("OTP Process info - Config");
            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            //startInfo.FileName = ex1 + "CreatePDFPantheon.bat";
            startInfo.FileName = PanthExeFile;
            startInfo.Arguments = PanthExeArgs;

            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            //startInfo.Arguments = "/lens";

            DataTypesHelper.LogThis("OTP Start File: " + startInfo.FileName.ToString());
            DataTypesHelper.LogThis("OTP Start Args: " + startInfo.Arguments.ToString());

            if (!File.Exists(startInfo.FileName))
            {
                DataTypesHelper.LogThis("OTP App doesnt exist: " + startInfo.FileName.ToString());
                return;
            }

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                DataTypesHelper.LogThis("OTP Run Process - Start");
                using (Process exeProcess = Process.Start(startInfo))
                {
                    DataTypesHelper.LogThis("OTP Process - Start timeout");
                    exeProcess.WaitForExit(timeout);
                    DataTypesHelper.LogThis("OTP Run Process - Succesfull");
                    if (exeProcess.HasExited == false)
                    {
                        DataTypesHelper.LogThis("OTP Process - Killed");
                        exeProcess.Kill();
                    }
                }
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message + "\r\n " + ex.Source + "\r\n " + ex.StackTrace);
            }
        }


        public string GetOrderPDFFile(int iRecallID)
        {
            try
            {
                RecallFullModel rfm = recallRepo.GetRecallFullModelByID(iRecallID);
                string curFile = "";
                string sResult = "";

                if (rfm != null)
                {
                    //če je 5x neuspešno, potem se pošlje mail Daniju
                    if ((rfm.P_UnsuccCountCreatePDFPantheon >= 5) && (rfm.P_SendWarningToAdmin == 0))
                    {
                        RecallStatus stat = recallRepo.GetRecallStatusByCode(Enums.StatusOfRecall.ERR_ADMIN_MAIL.ToString());
                        if (stat != null)
                        {
                            rfm.StatusID = stat.StatusOdpoklicaID;
                        }
                        messageRepo.CreateEmailForAdmin_NoPDFForOrderOTP("", rfm.OdpoklicStevilka.ToString(), rfm.P_TransportOrderPDFName);
                        rfm.P_SendWarningToAdmin = 1;
                        recallRepo.SaveRecall(rfm, true);
                        return "NOT_EXIST";
                    }
                    else if (rfm.P_SendWarningToAdmin == 1)
                    {
                        RecallStatus stat = recallRepo.GetRecallStatusByCode(Enums.StatusOfRecall.ERR_ADMIN_MAIL.ToString());
                        if (stat != null)
                        {
                            rfm.StatusID = stat.StatusOdpoklicaID;
                        }

                        DataTypesHelper.LogThis("Za naročilo št. " + rfm.OdpoklicStevilka + " ni bilo kreirano PDF in je bil poslal že mail administratorju.");
                        return "NOT_EXIST";
                    }
                    //TimeSpan timeDiff = DateTime.Now - rfm.P_LastTSCreatePDFPantheon;
                    //if (timeDiff.TotalMinutes < 5) return "";

                    curFile = ((rfm.P_TransportOrderPDFDocPath != null) && (rfm.P_TransportOrderPDFDocPath.Length > 0)) ? rfm.P_TransportOrderPDFDocPath : "";
                    sResult = File.Exists(curFile) ? "EXIST" : "NOT_EXIST";

         


                    if (sResult != "EXIST")
                    {
                        RecallStatus stat = recallRepo.GetRecallStatusByCode(Enums.StatusOfRecall.ERR_ORDER_NO_SEND.ToString());
                        if (stat != null)
                        {
                            rfm.StatusID = stat.StatusOdpoklicaID;
                        }
                        DataTypesHelper.LogThis("NOT EXIST : " + rfm.OdpoklicStevilka);

                        LaunchPantheonCreatePDF();
                        rfm.P_UnsuccCountCreatePDFPantheon++;
                        rfm.P_LastTSCreatePDFPantheon = DateTime.Now;
                    }
                    else
                    {
                        RecallStatus stat = recallRepo.GetRecallStatusByCode(Enums.StatusOfRecall.KREIRAN_POSLAN_PDF.ToString());
                        if (stat != null)
                        {
                            DataTypesHelper.LogThis("Exist send order to carriaer: " + rfm.OdpoklicStevilka);
                            rfm.StatusID = stat.StatusOdpoklicaID;
                            rfm.P_GetPDFOrderFile = DateTime.Now;
                            // Create mail for prevoznik
                            DataTypesHelper.LogThis("Start send email: " + rfm.OdpoklicStevilka);
                            messageRepo.CreateEmailForCarrierOrder(rfm);
                        }

                    }

                    recallRepo.SaveRecall(rfm, true);
                }
                return sResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public string GetOrderPDFFilePDO(int iPDOOrderID)
        //{
        //    try
        //    {
        //        OrderPDOFullModel rfm = orderPDORepo.GetOrderByID(iPDOOrderID);
        //        string curFile = "";
        //        string sResult = "";

        //        if (rfm != null)
        //        {
        //            rfm.NarociloStevilka_P = (rfm.NarociloStevilka_P != null) ? rfm.NarociloStevilka_P : "xxx";

        //            // če je 5x neuspešno, potem se pošlje mail Daniju
        //            if ((rfm.P_UnsuccCountCreatePDFPantheon >= 5) && (rfm.P_SendWarningToAdmin == 0))
        //            {

        //                messageRepo.CreateEmailForAdmin_NoPDFForOrder("", rfm.NarociloStevilka_P.ToString(), rfm.P_TransportOrderPDFName, false);
        //                rfm.P_SendWarningToAdmin = 1;
        //                return "NOT_EXIST";
        //            }
        //            else if (rfm.P_SendWarningToAdmin == 1)
        //            {
        //                DataTypesHelper.LogThis("Za naročilo št. " + rfm.NarociloStevilka_P + " ni bilo kreirano PDF in je bil poslal že mail administraotrju.");
        //                return "NOT_EXIST";
        //            }

        //            curFile = ((rfm.P_TransportOrderPDFDocPath != null) && (rfm.P_TransportOrderPDFDocPath.Length > 0)) ? rfm.P_TransportOrderPDFDocPath : "";

        //            sResult = File.Exists(curFile) ? "EXIST" : "NOT_EXIST";
        //        }

        //        if (sResult != "EXIST")
        //        {
        //            LaunchPantheonCreatePDF();
        //            rfm.P_UnsuccCountCreatePDFPantheon++;
        //            rfm.P_LastTSCreatePDFPantheon = DateTime.Now;
        //        }
        //        else
        //        {
        //            InquiryStatus stat = orderPDORepo.GetPovprasevanjaStatusByCode(Enums.StatusOfInquiry.KREIRAN_POSLAN_PDF.ToString());
        //            if (stat != null)
        //            {
        //                rfm.StatusID = stat.StatusPovprasevanjaID;
        //                rfm.P_GetPDFOrderFile = DateTime.Now;
        //                // Create mail for prevoznik
        //                messageRepoPDO.CreateEmailForSupplierOrder(rfm);
        //            }

        //        }

        //        orderPDORepo.SaveOrder(rfm, true, false);

        //        return sResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        public void CreateOrderTransport(CreateOrderModel model)
        {
            try
            {
                RecallFullModel rfm = recallRepo.GetRecallFullModelByID(model.RecallID);

                if (rfm.P_CreateOrder.Year < 2000)
                {
                    DataTypesHelper.LogThis("cREATE oRDER xml");
                    string xml = GetXMLForOrder(model);
                    DataTypesHelper.LogThis(xml);

                    DataTypesHelper.LogThis("Run Create order procedure _upJM_CreateSupplierOrder");
                    // run store procedure _upJM_CreateSupplierOrder
                    CreateOrderDocument coData = sqlFunctionRepo.GetOrderDocumentData(xml);


                    DataTypesHelper.LogThis("Update Create order Recall Data");
                    // update odpoklic - uspešno kreirana naročilnica v pantheonu


                    RecallStatus stat = recallRepo.GetRecallStatusByCode(Enums.StatusOfRecall.USTVARJENO_NAROCILO.ToString());
                    if (stat != null)
                    {
                        rfm.StatusID = stat.StatusOdpoklicaID;
                    }

                    rfm.P_UnsuccCountCreatePDFPantheon = 0;
                    rfm.P_CreateOrder = DateTime.Now;
                    rfm.P_TransportOrderPDFName = coData.PDFFile.ToString();
                    rfm.P_TransportOrderPDFDocPath = coData.ExportPath.ToString();
                    recallRepo.SaveRecall(rfm, true);

                    //DataTypesHelper.LogThis("Launch Create PDF Pantheon.exe");
                    // launch Pantheon EXE command
                    //LaunchPantheonCreatePDF();
                    DataTypesHelper.LogThis("Finish CreateOrderTransport");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetXMLForOrder(CreateOrderModel model)
        {

            var directory = AppDomain.CurrentDomain.BaseDirectory;
            //string sPath = directory + "OrderTransport_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm") + ".xml";
            //XmlTextWriter xml = new XmlTextWriter(sPath, Encoding.Unicode);
            MemoryStream stream = new MemoryStream(); // The writer closes this for us
            XmlTextWriter xml = new XmlTextWriter(stream, Encoding.Unicode);
            xml.Formatting = Formatting.Indented;
            xml.WriteStartDocument(true);

            try
            {
                xml.WriteStartElement("Recall");

                xml.WriteElementString("timestamp", DateTime.Now.ToString());
                xml.WriteElementString("DocType", model.TypeCode);
                xml.WriteElementString("Department", "");

                RecallFullModel rfm = recallRepo.GetRecallFullModelByID(model.RecallID);
                var TransportData = clientRepo.GetClientByID(rfm.DobaviteljID);
                if (ConfigurationManager.AppSettings["PantheonCreateOrderDefBuyer"] != null)
                    xml.WriteElementString("Buyer", ConfigurationManager.AppSettings["PantheonCreateOrderDefBuyer"].ToString());
                else
                    xml.WriteElementString("Buyer", "");

                //var employee = employeePdoRep.GetEmployeeByID(model.tsIDOsebe);
                //string ReferentID = employee.PantheonUsrID.Length > 0 ? employee.PantheonUsrID : "";

                xml.WriteElementString("Supplier", TransportData.NazivPrvi);
                xml.WriteElementString("OrderDate", DateTime.Now.ToString());
                xml.WriteElementString("LoadDate", rfm.DatumNaklada.ToString());
                xml.WriteElementString("Route", rfm.Relacija.Naziv);

                string lgn = (TransportData.JezikID > 0) ? TransportData.JezikOTP.Koda : "SLO";
                string printType = (lgn == "SLO") ? Enums.PrintType.A10.ToString() : Enums.PrintType.A0Q.ToString();
                xml.WriteElementString("PrintType", printType);

                xml.WriteElementString("OrderPDFPath", ConfigurationManager.AppSettings["ServerOrderPDFPath"].ToString());
                xml.WriteElementString("OrderNote", model.Note);
                xml.WriteStartElement("Products");

                foreach (ServiceListModel _serv in model.services)
                {
                    xml.WriteStartElement("Product");
                    xml.WriteElementString("Department", "");
                    xml.WriteElementString("Ident", _serv.Code);
                    xml.WriteElementString("Name", _serv.Name);
                    xml.WriteElementString("Qty", _serv.Quantity.ToString());
                    xml.WriteElementString("Price", _serv.Price.ToString());
                    xml.WriteElementString("Rabat", "");
                    xml.WriteElementString("Note", "");
                    xml.WriteEndElement();  // Product
                }

                xml.WriteEndElement();  // Products
                xml.WriteEndElement();  // Recall
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message + "\r\n " + ex.Source + "\r\n " + ex.StackTrace);
            }
            finally
            {
                xml.Flush();
            }
            string result;
            StreamReader reader = new StreamReader(stream, Encoding.Unicode, true);
            stream.Seek(0, SeekOrigin.Begin);
            result = reader.ReadToEnd();

            return result;
        }


        #endregion

        public void CheckForRecallsWithNoSubmitedPrices()
        {
            //poiščemo takšne odpoklice kejr je bilo poslano povpraševaje prevoznikom in še ni bilo določenega prevoznika z najnižjo ceno in
            //da datum naklada še vseeno večji od današnjega in 
            var list = context.Odpoklic.Where(o => o.PovprasevanjePoslanoPrevoznikom.Value &&
           !o.PrevoznikOddalNajnizjoCeno.Value &&
           o.DatumNaklada > DateTime.Now).Select(o => o.OdpoklicID).ToList();//select uporabimo zato ker v spodnjem query kličemo funkcijo Any ki dovoljuje samo enostavne 

            //pridobimo vse prijave prevoznikov ki predstavljajo odpoklice v seznamu odpoklicev (recalls) in jih grupiramo po OdpoklicID polju
            List<IGrouping<Odpoklic, PrijavaPrevoznika>> listPrijavaPrevoznika = context.PrijavaPrevoznika.Where(pp => list.Any(o => o == pp.OdpoklicID)).ToList().GroupBy(g => g.Odpoklic).ToList();
            List<IGrouping<Odpoklic, PrijavaPrevoznika>> newList = new List<IGrouping<Odpoklic, PrijavaPrevoznika>>();

            foreach (var item in listPrijavaPrevoznika)
            {
                //En teden pred datumom naklada
                DateTime oneWeekFromLoadingDate = item.Key.DatumNaklada.Value.AddDays(-7);

                if (oneWeekFromLoadingDate.Date < DateTime.Now.Date)
                {
                    if (item.Key.DatumPosiljanjaMailLogistika.HasValue && (DateTime.Now - item.Key.DatumPosiljanjaMailLogistika.Value).TotalDays >= 1)
                    {
                        newList.Add(item);
                        context.Entry(item.Key).Entity.DatumPosiljanjaMailLogistika = DateTime.Now;
                    }
                    else if (!item.Key.DatumPosiljanjaMailLogistika.HasValue)
                    {
                        newList.Add(item);
                        context.Entry(item.Key).Entity.DatumPosiljanjaMailLogistika = DateTime.Now;
                    }
                }
            }
            context.SaveChanges();

            messageRepo.CreateEmailLogisticsCarrierNotSelected(newList);
        }
    }
}