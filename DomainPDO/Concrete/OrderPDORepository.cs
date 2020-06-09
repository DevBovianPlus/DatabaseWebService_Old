using DatabaseWebService.Common;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.DomainPDO.Abstract;
using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsOTP.Client;
using DatabaseWebService.ModelsOTP.Recall;
using DatabaseWebService.ModelsPDO;
using DatabaseWebService.ModelsPDO.Inquiry;
using DatabaseWebService.ModelsPDO.Order;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace DatabaseWebService.DomainPDO.Concrete
{
    public class OrderPDORepository : IOrderPDORepository
    {
        GrafolitPDOEntities context;

        IInquiryRepository inquiryRepo;
        IMSSQLPDOFunctionRepository sqlFunctionRepoPDO;
        ISystemEmailMessageRepository_PDO messageRepoPDO;
        IEmployeePDORepository employeePdoRep;

        public OrderPDORepository(GrafolitPDOEntities _context, IInquiryRepository iinquiryRepo, IMSSQLPDOFunctionRepository _sqlRepoPDO, ISystemEmailMessageRepository_PDO _messageRepoPDO, IEmployeePDORepository _employeePdoRep)
        {
            context = _context;
            inquiryRepo = iinquiryRepo;
            sqlFunctionRepoPDO = _sqlRepoPDO;
            messageRepoPDO = _messageRepoPDO;
            employeePdoRep = _employeePdoRep;
        }


        public List<OrderPDOFullModel> GetOrderList()
        {
            try
            {
                var query = from o in context.Narocilo_PDO
                            select new OrderPDOFullModel
                            {
                                DatumDobave = o.DatumDobave.HasValue ? o.DatumDobave.Value : DateTime.MinValue,
                                NarociloID = o.NarociloID,
                                NarociloStevilka_P = o.NarociloStevilka_P,
                                PovprasevanjeStevilka = o.PovprasevanjeStevilka,
                                Opombe = o.Opombe,
                                ts = o.ts.HasValue ? o.ts.Value : DateTime.MinValue,
                                tsIDOsebe = o.tsIDOsebe.HasValue ? o.tsIDOsebe.Value : 0,
                                tsUpdate = o.tsUpdate.HasValue ? o.tsUpdate.Value : DateTime.MinValue,
                                tsUpdateUserID = o.tsUpdateUserID.HasValue ? o.tsUpdateUserID.Value : 0,
                                P_UnsuccCountCreatePDFPantheon = o.P_UnsuccCountCreatePDFPantheon.HasValue ? o.P_UnsuccCountCreatePDFPantheon.Value : 0,
                                DobaviteljID = (from c in context.Stranka_PDO
                                                where c.StrankaID == (o.DobaviteljID.HasValue ? o.DobaviteljID.Value : 0)
                                                select new ClientSimpleModel
                                                {
                                                    NazivPrvi = c.NazivPrvi,
                                                    idStranka = c.StrankaID
                                                }).FirstOrDefault(),
                                StatusID = o.StatusID,
                                StatusModel = (from sp in context.StatusPovprasevanja
                                               where sp.StatusPovprasevanjaID == o.StatusID
                                               select new InquiryStatus
                                               {
                                                   Koda = sp.Koda,
                                                   Naziv = sp.Naziv,
                                                   Opis = sp.Opis,
                                                   StatusPovprasevanjaID = sp.StatusPovprasevanjaID,
                                                   ts = sp.ts.HasValue ? sp.ts.Value : DateTime.MinValue,
                                                   tsIDOseba = sp.tsIDOseba.HasValue ? sp.tsIDOseba.Value : 0,
                                                   tsUpdate = sp.tsUpdate.HasValue ? sp.tsUpdate.Value : DateTime.MinValue,
                                               }).FirstOrDefault(),
                                PovprasevanjeID = o.PovprasevanjeID,
                            };

                return query.OrderByDescending(s => s.ts).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public OrderPDOFullModel GetOrderByID(int oID)
        {
            try
            {
                var query = from o in context.Narocilo_PDO
                            where o.NarociloID == oID
                            select new OrderPDOFullModel
                            {
                                DatumDobave = o.DatumDobave.HasValue ? o.DatumDobave.Value : DateTime.MinValue,
                                NarociloID = o.NarociloID,
                                NarociloStevilka_P = o.NarociloStevilka_P,
                                Opombe = o.Opombe,
                                ts = o.ts.HasValue ? o.ts.Value : DateTime.MinValue,
                                tsIDOsebe = o.tsIDOsebe.HasValue ? o.tsIDOsebe.Value : 0,
                                tsUpdate = o.tsUpdate.HasValue ? o.tsUpdate.Value : DateTime.MinValue,
                                tsUpdateUserID = o.tsUpdateUserID.HasValue ? o.tsUpdateUserID.Value : 0,
                                StrankaDobaviteljID = o.DobaviteljID.HasValue ? o.DobaviteljID.Value : 0,
                                StatusID = o.StatusID.HasValue ? o.StatusID.Value : 0,

                                StatusModel = (from s in context.StatusPovprasevanja
                                               where s.StatusPovprasevanjaID == (o.StatusID.HasValue ? o.DobaviteljID.Value : 0)
                                               select new InquiryStatus
                                               {
                                                   Koda = s.Koda,
                                                   Naziv = s.Naziv,
                                                   Opis = s.Opis,
                                                   StatusPovprasevanjaID = s.StatusPovprasevanjaID,
                                                   ts = s.ts.HasValue ? s.ts.Value : DateTime.MinValue,
                                                   tsUpdate = s.tsUpdate.HasValue ? s.tsUpdate.Value : DateTime.MinValue,
                                                   tsIDOseba = s.tsIDOseba.HasValue ? s.tsIDOseba.Value : 0,
                                                   tsUpdateuserID = s.tsIDOseba.HasValue ? s.tsIDOseba.Value : 0
                                               }).FirstOrDefault(),

                                P_CreateOrder = o.P_CreateOrder.HasValue ? o.P_CreateOrder.Value : DateTime.MinValue,
                                P_LastTSCreatePDFPantheon = o.P_LastTSCreatePDFPantheon.HasValue ? o.P_LastTSCreatePDFPantheon.Value : DateTime.MinValue,
                                P_GetPDFOrderFile = o.P_GetPDFOrderFile.HasValue ? o.P_GetPDFOrderFile.Value : DateTime.MinValue,
                                P_TransportOrderPDFDocPath = o.P_TransportOrderPDFDocPath,
                                P_TransportOrderPDFName = o.P_TransportOrderPDFName,
                                P_UnsuccCountCreatePDFPantheon = o.P_UnsuccCountCreatePDFPantheon.HasValue ? o.P_UnsuccCountCreatePDFPantheon.Value : 0,
                                P_SendWarningToAdmin = o.P_SendWarningToAmin.HasValue ? o.P_SendWarningToAmin.Value : 0,
                                OddelekID = o.OddelekID.HasValue ? o.OddelekID.Value : 0,
                                OddelekNaziv = o.Oddelek.Naziv,
                                Oddelek = (from odd in context.Oddelek
                                           where odd.OddelekID == o.OddelekID
                                           select new DepartmentModel
                                           {
                                               Koda = odd.Koda,
                                               Naziv = odd.Naziv,
                                               ts = odd.ts.HasValue ? odd.ts.Value : DateTime.MinValue,
                                           }).FirstOrDefault(),
                                PovprasevanjeID = o.PovprasevanjeID,
                                PovprasevanjeStevilka = o.PovprasevanjeStevilka,
                            };

                var order = query.FirstOrDefault();

                if (order != null)
                {
                    order.NarociloPozicija_PDO = GetOrderPositionsByOrderID(oID);
                }

                return order;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }


        public List<OrderPDOPositionModel> GetOrderPositionsByOrderID(int oId)
        {
            try
            {
                var query = from op in context.NarociloPozicija_PDO
                            where op.NarociloID == oId
                            select new OrderPDOPositionModel
                            {
                                NarociloID = op.NarociloID,
                                ArtikelCena = op.ArtikelCena.HasValue ? op.ArtikelCena.Value : 0,
                                DatumDobave = op.DatumDobave.HasValue ? op.DatumDobave.Value : DateTime.MinValue,
                                IzbranDobaviteljID = op.IzbranDobaviteljID.HasValue ? op.IzbranDobaviteljID.Value : 0,
                                Dobavitelj = (from d in context.Stranka_PDO
                                              where d.StrankaID == op.IzbranDobaviteljID.Value
                                              select new ClientFullModel
                                              {
                                                  idStranka = d.StrankaID,
                                                  KodaStranke = d.KodaStranke,
                                                  NazivPrvi = d.NazivPrvi,
                                                  NazivDrugi = d.NazivDrugi,
                                                  Naslov = d.Naslov,
                                                  StevPoste = d.StevPoste,
                                                  NazivPoste = d.NazivPoste,
                                                  Email = d.Email,
                                                  Telefon = d.Telefon,
                                                  FAX = d.FAX,
                                                  InternetniNalov = d.InternetniNalov,
                                                  KontaktnaOseba = d.KontaktnaOseba,
                                                  RokPlacila = d.RokPlacila,
                                                  TRR = d.TRR,
                                                  DavcnaStev = d.DavcnaStev,
                                                  MaticnaStev = d.MaticnaStev,
                                                  StatusDomacTuji = ((d.StatusDomacTuji.HasValue && d.StatusDomacTuji.Value) ? "DOMAČ" : ""),
                                                  Zavezanec_DA_NE = ((d.Zavezanec_DA_NE.HasValue && d.Zavezanec_DA_NE.Value) ? "DA" : "NE"),
                                                  IdentifikacijskaStev = d.IdentifikacijskaStev,
                                                  Clan_EU = ((d.Clan_EU.HasValue && d.Clan_EU.Value) ? "DA" : "NE"),
                                                  BIC = d.BIC,
                                                  KodaPlacila = d.KodaPlacila,
                                                  DrzavaStranke = d.DrzavaStranke,
                                                  Neaktivna = d.Neaktivna,
                                                  GenerirajERacun = d.GenerirajERacun.HasValue ? d.GenerirajERacun.Value : 0,
                                                  JavniZavod = d.JavniZavod.HasValue ? d.JavniZavod.Value : 0,
                                                  ts = d.ts.HasValue ? d.ts.Value : DateTime.MinValue,
                                                  tsIDOsebe = d.tsIDOsebe.HasValue ? d.tsIDOsebe.Value : 0,
                                                  TipStrankaID = d.TipStrankaID,
                                                  TipStranka = (from tipS in context.TipStranka_PDO
                                                                where tipS.TipStrankaID == d.TipStrankaID
                                                                select new ClientType
                                                                {
                                                                    Koda = tipS.Koda,
                                                                    Naziv = tipS.Naziv,
                                                                    Opis = tipS.Opis,
                                                                    TipStrankaID = tipS.TipStrankaID,
                                                                    ts = tipS.ts.HasValue ? tipS.ts.Value : DateTime.MinValue,
                                                                    tsIDOseba = tipS.tsIDOseba.HasValue ? tipS.tsIDOseba.Value : 0
                                                                }).FirstOrDefault(),
                                                  /*KontaktneOsebe = (from cp in context.KontaktnaOseba_PDO
                                                                    where cp.StrankaID == d.StrankaID
                                                                    select new ContactPersonModel
                                                                    {
                                                                        DelovnoMesto = cp.DelovnoMesto,
                                                                        Email = cp.Email,
                                                                        Fax = cp.Fax,
                                                                        GSM = cp.GSM,
                                                                        idKontaktneOsebe = cp.KontaktnaOsebaID,
                                                                        NazivKontaktneOsebe = cp.NazivKontaktneOsebe,
                                                                        Opombe = cp.Opombe,
                                                                        Telefon = cp.Telefon,
                                                                        ts = cp.ts.HasValue ? cp.ts.Value : DateTime.MinValue,
                                                                        tsIDOsebe = cp.tsIDOsebe.HasValue ? cp.tsIDOsebe.Value : 0
                                                                    }).ToList(),*/
                                              }).FirstOrDefault(),
                                IzbraniArtikelIdent_P = op.Ident,
                                IzbraniArtikelNaziv_P = op.IzbraniArtikelNaziv_P,
                                NarociloPozicijaID = op.NarociloPozicijaID,
                                Opombe = op.Opombe,
                                PovprasevanjePozicijaID = op.PovprasevanjePozicijaID,
                                ts = op.ts.HasValue ? op.ts.Value : DateTime.MinValue,
                                tsIDOsebe = op.tsIDOsebe.HasValue ? op.tsIDOsebe.Value : 0,
                                tsUpdate = op.tsUpdate.HasValue ? op.tsUpdate.Value : DateTime.MinValue,
                                tsUpdateUserID = op.tsUpdateUserID.HasValue ? op.tsUpdateUserID.Value : 0,
                                KolicinavKG = op.KolicinavKG.HasValue ? op.KolicinavKG.Value : 0,
                                EnotaMere = op.EnotaMere,
                                Rabat = op.Rabat.HasValue ? op.Rabat.Value : 0,
                                OpombaNarocilnica = op.OpombaNarocilnica,
                                OddelekID = op.OddelekID.HasValue ? op.OddelekID.Value : 0,
                                OddelekNaziv = op.Oddelek.Naziv,
                                Oddelek = (from odd in context.Oddelek
                                           where odd.OddelekID == op.OddelekID
                                           select new DepartmentModel
                                           {
                                               Koda = odd.Koda,
                                               Naziv = odd.Naziv,
                                               ts = odd.ts.HasValue ? odd.ts.Value : DateTime.MinValue,
                                           }).FirstOrDefault(),
                                PrikaziKupca = op.PrikaziKupca.HasValue ? op.PrikaziKupca.Value : false,
                            };

                List<OrderPDOPositionModel> list = query.OrderBy(pos => pos.Dobavitelj).ToList();
                foreach (var item in list)
                {
                    item.PovprasevanjePozicija = inquiryRepo.GetInquiryPositionByID(item.PovprasevanjePozicijaID);
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }



        public InquiryFullModel GetOrderPositionsByInquiryIDForNewOrder(int iId)
        {
            try
            {
                InquiryFullModel inquery = new InquiryFullModel();

                inquery = inquiryRepo.GetInquiryByID(iId, false);

                inquery = inquiryRepo.GetInquiryPositionArtikelByInquiryID(inquery, iId, false);

                return inquery;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }



        public InquiryFullModel CheckPantheonArtikles(InquiryFullModel inquiry, bool updateRecord = true)
        {
            try
            {
                foreach (var item in inquiry.PovprasevanjePozicijaArtikel)
                {

                    var productList = sqlFunctionRepoPDO.GetProductBySupplierAndName(item.Dobavitelj.NazivPrvi, item.Naziv.Trim());

                    if (productList != null && productList.Count > 0)
                    {
                        if (productList.Count == 1)//če bo obstajalo več artiklov potem pustimo spodnja polja prazna. Tako bo uporabnik vedel da je potrebno izbrati artikel.
                        {
                            item.IzbraniArtikelNaziv_P = productList[0].Naziv;
                            item.IzbraniArtikelIdent_P = productList[0].StevilkaArtikel;
                        }

                        item.ArtikliPantheon = productList;
                    }
                }

                return inquiry;
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis("OrderPDORepository.SaveOrder Error: " + ex.ToString());
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public int SaveOrder(OrderPDOFullModel order, bool updateRecord = true, bool CreateXMLDoc = false)
        {
            try
            {
                Narocilo_PDO n = new Narocilo_PDO();
                n.DatumDobave = order.DatumDobave.Equals(DateTime.MinValue) ? (DateTime?)null : order.DatumDobave;
                n.NarociloID = order.NarociloID;
                n.NarociloStevilka_P = order.NarociloStevilka_P;
                n.Opombe = order.Opombe;
                n.tsUpdate = DateTime.Now;
                n.tsUpdateUserID = order.tsUpdateUserID;
                n.ts = order.ts.Equals(DateTime.MinValue) ? (DateTime?)null : order.ts;
                n.tsIDOsebe = order.tsIDOsebe;
                n.DobaviteljID = (order.StrankaDobaviteljID > 0) ? order.StrankaDobaviteljID : (int?)null;
                n.OddelekID = (order.OddelekID > 0) ? order.OddelekID : (int?)null;

                // Save for Create Order procedure info
                if (order.P_CreateOrder.Year > 2000) n.P_CreateOrder = order.P_CreateOrder;
                if (order.P_LastTSCreatePDFPantheon.Year > 2000) n.P_LastTSCreatePDFPantheon = order.P_LastTSCreatePDFPantheon;
                n.P_TransportOrderPDFDocPath = order.P_TransportOrderPDFDocPath;
                n.P_TransportOrderPDFName = order.P_TransportOrderPDFName;
                n.P_UnsuccCountCreatePDFPantheon = order.P_UnsuccCountCreatePDFPantheon;
                n.P_SendWarningToAmin = order.P_SendWarningToAdmin;

                //if (order.PovprasevanjeStatusID != null)
                //{
                //    InquiryStatus stat = GetPovprasevanjaStatusByID(order.PovprasevanjeStatusID);
                //    if (stat != null)
                //    {
                //    }
                //}

                if (CreateXMLDoc)
                {
                    InquiryStatus stat = inquiryRepo.GetPovprasevanjaStatusByCode(Enums.StatusOfInquiry.USTVARJENO_NAROCILO.ToString());
                    if (stat != null)
                    {
                        n.StatusID = stat.StatusPovprasevanjaID;
                    }
                }
                n.StatusID = order.StatusID;


                Povprasevanje inquiry = null;


                inquiry = context.Povprasevanje.Where(i => i.PovprasevanjeID == order.PovprasevanjeID).FirstOrDefault();
                if (inquiry != null)
                {
                    n.PovprasevanjeStevilka = inquiry.PovprasevanjeStevilka;
                    n.PovprasevanjeID = inquiry.PovprasevanjeID;

                    //inquiry.StatusID = order.StatusID.Value;

                }

                if (n.NarociloID == 0)
                {
                    n.ts = DateTime.Now;
                    n.tsIDOsebe = order.tsIDOsebe;
                    context.Narocilo_PDO.Add(n);


                }
                else
                {
                    if (updateRecord)
                    {
                        Narocilo_PDO original = context.Narocilo_PDO.Where(na => na.NarociloID == n.NarociloID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(n);
                    }
                }
                context.SaveChanges();

                inquiry = context.Povprasevanje.Where(i => i.PovprasevanjeID == order.PovprasevanjeID).FirstOrDefault();
                if ((inquiry != null) && CreateXMLDoc)
                {

                    inquiry.NarociloID = n.NarociloID;
                    inquiry.Narocila += n.NarociloID + ",";

                }

                context.SaveChanges();


                if (order.NarociloPozicija_PDO != null && order.NarociloPozicija_PDO.Count > 0)
                {
                    SaveOrderPositionsModel(order.NarociloPozicija_PDO, n.NarociloID);

                    // create XML for order in pantheon 
                    OrderPDOFullModel model = GetOrderByID(n.NarociloID);
                    if ((model != null) && (CreateXMLDoc))
                    {
                        CreateXMLForPantheon(model);
                    }

                }

                return n.NarociloID;
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis("OrderPDORepository.SaveOrder Error: " + ex.ToString());
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }



        public InquiryStatus GetPovprasevanjaStatusByID(int StatusID)
        {
            try
            {
                var query = from type in context.StatusPovprasevanja
                            where type.StatusPovprasevanjaID == StatusID
                            select new InquiryStatus
                            {
                                Koda = type.Koda,
                                Naziv = type.Naziv,
                                Opis = type.Opis,
                                StatusPovprasevanjaID = type.StatusPovprasevanjaID,
                                ts = type.ts.HasValue ? type.ts.Value : DateTime.MinValue,
                                tsUpdate = type.tsUpdate.HasValue ? type.tsUpdate.Value : DateTime.MinValue,
                                tsIDOseba = type.tsIDOseba.HasValue ? type.tsIDOseba.Value : 0,
                                tsUpdateuserID = type.tsIDOseba.HasValue ? type.tsIDOseba.Value : 0
                            };

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        private void CreateXMLForPantheon(OrderPDOFullModel model)
        {
            string xml = GetXMLForOrder(model);

            DataTypesHelper.LogThis(xml);

            DataTypesHelper.LogThis("Run Create order procedure _upJM_CreateSupplierOrder");
            // run store procedure _upJM_CreateSupplierOrder
            CreateOrderDocument coData = sqlFunctionRepoPDO.GetOrderDocumentData(xml);

            DataTypesHelper.LogThis("Update Create order Recall Data");
            // update odpoklic - uspešno kreirana naročilnica v pantheonu


            InquiryStatus stat = inquiryRepo.GetPovprasevanjaStatusByCode(Enums.StatusOfInquiry.USTVARJENO_NAROCILO.ToString());
            if (stat != null)
            {
                model.StatusID = stat.StatusPovprasevanjaID;
            }

            model.P_UnsuccCountCreatePDFPantheon = 0;
            model.P_CreateOrder = DateTime.Now;
            model.P_TransportOrderPDFName = coData.PDFFile.ToString();
            model.P_TransportOrderPDFDocPath = coData.ExportPath.ToString();
            model.NarociloStevilka_P = coData.PDFFile.ToString();

            SaveOrder(model, true, false);

        }

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

            DataTypesHelper.LogThis("PDO Config: PDF Path: " + ex1 + ", PantheonEXEFile: " + PanthExeFile + ", Args: " + PanthExeArgs + ", PanthDB: " + PanthDB + ", Timeout: " + sPanthExeTimeout);

            int timeout = Convert.ToInt32(sPanthExeTimeout);
            //string ex1 = "C:\\Temp\\OtpProject\\";
            PanthExeArgs = PanthExeArgs.Replace("DatabaseNameDB", "\"" + PanthDB + "\"");
            DataTypesHelper.LogThis("PDO Process info - Config");
            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            //startInfo.FileName = ex1 + "CreatePDFPantheon.bat";
            startInfo.FileName = PanthExeFile;
            startInfo.Arguments = PanthExeArgs;

            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            //startInfo.Arguments = "/lens";

            DataTypesHelper.LogThis("PDO Start File: " + startInfo.FileName.ToString());
            DataTypesHelper.LogThis("PDO Start Args: " + startInfo.Arguments.ToString());

            if (!File.Exists(startInfo.FileName))
            {
                DataTypesHelper.LogThis("PDO App doesnt exist: " + startInfo.FileName.ToString());
                return;
            }

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                DataTypesHelper.LogThis("PDO Run Process - Start");
                using (Process exeProcess = Process.Start(startInfo))
                {
                    DataTypesHelper.LogThis("PDO Process - Start timeout");
                    exeProcess.WaitForExit(timeout);
                    DataTypesHelper.LogThis("PDO Run Process - Succesfull");
                    if (exeProcess.HasExited == false)
                    {
                        DataTypesHelper.LogThis("PDO Process - Killed");
                        exeProcess.Kill();
                    }
                }

            }


            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message + "\r\n " + ex.Source + "\r\n " + ex.StackTrace);
            }


        }

        /// <summary>
        /// Launch the application with some options set.
        /// </summary>
        public void LaunchPantheonCreatePDF(string file, string sArgs)
        {
            // For the example
            //string ex1 = ConfigurationManager.AppSettings["PantheonCreatePDFPath"].ToString();
            //string ex1 = "C:\\Temp\\OtpProject\\";
            string sPanthExeTimeout = ConfigurationManager.AppSettings["PantheonEXETimeOut"];
            DataTypesHelper.LogThis("Process info - Config");
            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            if (file == null || file.Length == 0)
                file = "notepad.exe";
            startInfo.FileName = file;
            startInfo.Arguments = sArgs;

            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            //startInfo.Arguments = "/lens";

            int timeout = Convert.ToInt32(sPanthExeTimeout);


            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                DataTypesHelper.LogThis("Run Process - Start");
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit(timeout);
                    DataTypesHelper.LogThis("Run Process - Succesfull");
                    if (exeProcess != null)
                    {
                        DataTypesHelper.LogThis("Process - SessionId: " + exeProcess.SessionId);
                        DataTypesHelper.LogThis("Process - HasExited: " + exeProcess.HasExited);
                        if (exeProcess.HasExited == false)
                        {
                            DataTypesHelper.LogThis("Process - Kill: ");
                            exeProcess.Kill();
                            DataTypesHelper.LogThis("Process - after kill");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message + "\r\n " + ex.Source + "\r\n " + ex.StackTrace);
            }
        }

        private string GetXMLForOrder(OrderPDOFullModel model)
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
                xml.WriteStartElement("Document");

                xml.WriteElementString("timestamp", DateTime.Now.ToString());
                xml.WriteElementString("DocType", "0250");
                int iUsrID = model.tsIDOsebe > 0 ? model.tsIDOsebe : model.tsUpdateUserID;
                var employee = employeePdoRep.GetEmployeeByID(iUsrID);
                string ReferentID = (employee.PantheonUsrID != null && employee.PantheonUsrID.Length > 0) ? employee.PantheonUsrID : "";

                xml.WriteElementString("ReferentId", ReferentID);
                xml.WriteElementString("Department", model.OddelekNaziv.ToString());
                xml.WriteElementString("Supplier", model.NarociloPozicija_PDO[0].Dobavitelj.NazivPrvi);
                InquiryFullModel ifm = inquiryRepo.GetInquiryByID(model.NarociloPozicija_PDO[0].PovprasevanjePozicija.PovprasevanjeID, false);
                xml.WriteElementString("Buyer", ifm.KupecNaziv_P);
                xml.WriteElementString("OrderDate", DateTime.Now.ToString());
                xml.WriteElementString("LoadDate", model.DatumDobave.ToString());
                xml.WriteElementString("DeliveryDate", model.DatumDobave.ToString());
                xml.WriteElementString("Route", "");

                // define printtype
                string printType = (model.NarociloPozicija_PDO[0].PrikaziKupca) ? Enums.PrintType.A0Q.ToString() : Enums.PrintType.A0U.ToString();

                xml.WriteElementString("PrintType", printType);


                xml.WriteElementString("OrderPDFPath", ConfigurationManager.AppSettings["ServerOrderPDFPath"].ToString());
                xml.WriteElementString("OrderNote", model.Opombe);
                xml.WriteStartElement("Products");

                foreach (OrderPDOPositionModel _serv in model.NarociloPozicija_PDO)
                {
                    decimal dKolicina = 0;

                    dKolicina = (_serv.KolicinavKG > 0) ? _serv.KolicinavKG : 0;
                    _serv.OpombaNarocilnica = _serv.OpombaNarocilnica != null ? _serv.OpombaNarocilnica : "";
                    _serv.OddelekNaziv = _serv.OddelekNaziv == null ? model.OddelekNaziv.ToString() : _serv.OddelekNaziv.ToString();
                    xml.WriteStartElement("Product");
                    xml.WriteElementString("DeliveryDate", ((_serv.DatumDobave != null && !_serv.DatumDobave.Equals(DateTime.MinValue)) ? _serv.DatumDobave.ToString() : ""));
                    xml.WriteElementString("Department", _serv.OddelekNaziv.ToString());
                    xml.WriteElementString("Ident", _serv.IzbraniArtikelIdent_P);
                    xml.WriteElementString("Name", _serv.IzbraniArtikelNaziv_P);
                    xml.WriteElementString("Qty", dKolicina.ToString());
                    xml.WriteElementString("Price", _serv.ArtikelCena.ToString());
                    xml.WriteElementString("Rabat", _serv.Rabat.ToString());
                    xml.WriteElementString("Note", _serv.OpombaNarocilnica.ToString());
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

        public bool DeleteOrder(int orderId)
        {
            try
            {
                var order = context.Narocilo_PDO.Where(np => np.NarociloID == orderId).FirstOrDefault();

                if (order != null)
                {
                    context.Narocilo_PDO.Remove(order);
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

        private ProductModel GetArtikelFromPantheon(string supplier, string name)
        {


            List<ProductModel> products = sqlFunctionRepoPDO.GetProductBySupplierAndName(supplier, name);

            if (products.Count == 1)
            {
                return products[0];
            }

            return null;
        }

        public void SaveOrderPositionsModel(List<OrderPDOPositionModel> iOrderpos, int orderID, bool updateRecord = true)
        {
            try
            {
                foreach (var item in iOrderpos)
                {
                    NarociloPozicija_PDO np = new NarociloPozicija_PDO();
                    np.ArtikelCena = item.ArtikelCena > 0 ? item.ArtikelCena : (decimal?)null;
                    np.DatumDobave = item.DatumDobavePos.Equals(DateTime.MinValue) ? (DateTime?)null : item.DatumDobavePos;
                    np.IzbranDobaviteljID = item.IzbranDobaviteljID > 0 ? item.IzbranDobaviteljID : (int?)null;
                    np.IzbraniArtikelNaziv_P = item.IzbraniArtikelNaziv_P;
                    np.Ident = item.IzbraniArtikelIdent_P;
                    np.NarociloID = orderID;
                    np.NarociloPozicijaID = item.NarociloPozicijaID;
                    np.Opombe = item.Opombe;
                    np.PovprasevanjePozicijaID = item.PovprasevanjePozicijaID;
                    np.tsUpdate = DateTime.Now;
                    np.tsUpdateUserID = item.tsUpdateUserID;
                    np.ts = item.ts.Equals(DateTime.MinValue) ? (DateTime?)null : item.ts;
                    np.tsIDOsebe = item.tsIDOsebe;
                    np.OpombaNarocilnica = item.OpombaNarocilnica;
                    np.OddelekID = item.OddelekID > 0 ? item.OddelekID : (int?)null;

                    np.KolicinavKG = item.KolicinavKG > 0 ? item.KolicinavKG : (decimal?)null;
                    np.EnotaMere = item.EnotaMere;
                    np.Rabat = item.Rabat > 0 ? item.Rabat : (decimal?)null;
                    np.PrikaziKupca = item.PrikaziKupca;

                    if (np.NarociloPozicijaID == 0)
                    {
                        np.ts = DateTime.Now;
                        np.tsIDOsebe = item.tsIDOsebe;

                        context.NarociloPozicija_PDO.Add(np);
                    }
                    else
                    {
                        if (updateRecord)
                        {
                            NarociloPozicija_PDO original = context.NarociloPozicija_PDO.Where(npoz => npoz.NarociloPozicijaID == np.NarociloPozicijaID).FirstOrDefault();
                            context.Entry(original).CurrentValues.SetValues(np);
                        }
                    }



                }

                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public bool DeleteOrderPosition(int orderPosId)
        {
            try
            {
                var orderPos = context.NarociloPozicija_PDO.Where(np => np.NarociloPozicijaID == orderPosId).FirstOrDefault();

                if (orderPos != null)
                {
                    context.NarociloPozicija_PDO.Remove(orderPos);
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

        public IEnumerable<IGrouping<int, OrderPDOPositionModel>> GroupOrderPositionsBySupplier(OrderPDOFullModel model)
        {
            return model.NarociloPozicija_PDO.GroupBy(d => d.IzbranDobaviteljID);
        }

        /// <summary>
        /// Check all PDf for PDO Orders and send it through email
        /// </summary>
        public void CreatePDFAndSendPDOOrdersMultiple()
        {
            try
            {
                string prevzetKoda = Enums.StatusOfRecall.KREIRAN_POSLAN_PDF.ToString();
                int narociloKreirano = context.StatusPovprasevanja.SingleOrDefault(so => so.Koda == prevzetKoda).StatusPovprasevanjaID;
                List<Narocilo_PDO> listOfPDOOrders = context.Narocilo_PDO.Where(n => n.StatusID != narociloKreirano).ToList();

                foreach (var item in listOfPDOOrders)
                {
                    GetOrderPDFFilePDO(item.NarociloID);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("CreateAndSendOrdersMultiple Method Error! ", ex);
            }
        }

        /// <summary>
        /// procedura ki resetira status na orderju, da ga bo service še enkrat vzel in obdelal
        /// </summary>
        public void ResetOrderStatusByID(int iPDOOrderID)
        {
            try
            {
                OrderPDOFullModel rfm = GetOrderByID(iPDOOrderID);
                if (rfm != null)
                {
                    InquiryStatus stat = inquiryRepo.GetPovprasevanjaStatusByCode(Enums.StatusOfInquiry.USTVARJENO_NAROCILO.ToString());
                    if (stat != null)
                    {
                        rfm.StatusID = stat.StatusPovprasevanjaID;
                        rfm.P_UnsuccCountCreatePDFPantheon = 0;
                        rfm.P_SendWarningToAdmin = 0;
                        SaveOrder(rfm, true, false);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("CreateAndSendOrdersMultiple Method Error! ", ex);
            }
        }

        public List<InquiryModel> GetAllPurchases()
        {
            try
            {
                string sPoslanVNabavo = Enums.StatusOfInquiry.POSLANO_V_NABAVO.ToString();
                var query = from i in context.Povprasevanje
                            where i.StatusPovprasevanja.Koda == sPoslanVNabavo
                            select new InquiryModel
                            {
                                DatumOddajePovprasevanja = i.DatumOddajePovprasevanja.HasValue ? i.DatumOddajePovprasevanja.Value : DateTime.MinValue,
                                KupecID = i.KupecID,
                                Kupec = (from k in context.Stranka_PDO
                                         where k.StrankaID == i.KupecID
                                         select new ClientSimpleModel
                                         {
                                             idStranka = k.StrankaID,
                                             KodaStranke = k.KodaStranke,
                                             NazivPrvi = k.NazivPrvi,
                                             NazivDrugi = k.NazivDrugi,
                                             Naslov = k.Naslov,
                                             StevPoste = k.StevPoste,
                                             NazivPoste = k.NazivPoste,
                                             Email = k.Email,
                                             Telefon = k.Telefon,
                                             FAX = k.FAX,
                                             InternetniNalov = k.InternetniNalov,
                                             KontaktnaOseba = k.KontaktnaOseba,
                                             RokPlacila = k.RokPlacila,
                                             TRR = k.TRR,
                                             DavcnaStev = k.DavcnaStev,
                                             MaticnaStev = k.MaticnaStev,
                                             StatusDomacTuji = ((k.StatusDomacTuji.HasValue && k.StatusDomacTuji.Value) ? "DOMAČ" : ""),
                                             Zavezanec_DA_NE = ((k.Zavezanec_DA_NE.HasValue && k.Zavezanec_DA_NE.Value) ? "DA" : "NE"),
                                             IdentifikacijskaStev = k.IdentifikacijskaStev,
                                             Clan_EU = ((k.Clan_EU.HasValue && k.Clan_EU.Value) ? "DA" : "NE"),
                                             BIC = k.BIC,
                                             KodaPlacila = k.KodaPlacila,
                                             DrzavaStranke = k.DrzavaStranke,
                                             Neaktivna = k.Neaktivna,
                                             GenerirajERacun = k.GenerirajERacun.HasValue ? k.GenerirajERacun.Value : 0,
                                             JavniZavod = k.JavniZavod.HasValue ? k.JavniZavod.Value : 0,
                                             ts = k.ts.HasValue ? k.ts.Value : DateTime.MinValue,
                                             tsIDOsebe = k.tsIDOsebe.HasValue ? k.tsIDOsebe.Value : 0,
                                             //                                ImeInPriimekZaposlen = (subClient == null ? String.Empty : subClient.Osebe_OTP.Ime + " " + subClient.Osebe_OTP.Priimek),
                                             TipStranka = k.TipStranka_PDO != null ? k.TipStranka_PDO.Naziv : ""
                                         }).FirstOrDefault(),
                                NarociloID = i.NarociloID.HasValue ? i.NarociloID.Value : 0,
                                Naziv = i.Naziv,
                                PovprasevanjeID = i.PovprasevanjeID,
                                PovprasevanjeStevilka = i.PovprasevanjeStevilka,
                                StatusID = i.StatusID,
                                StatusPovprasevanja = (from sp in context.StatusPovprasevanja
                                                       where sp.StatusPovprasevanjaID == i.StatusID
                                                       select new InquiryStatusModel
                                                       {
                                                           Koda = sp.Koda,
                                                           Naziv = sp.Naziv,
                                                           Opis = sp.Opis,
                                                           StatusPovprasevanjaID = sp.StatusPovprasevanjaID,
                                                           ts = sp.ts.HasValue ? sp.ts.Value : DateTime.MinValue,
                                                           tsIDOseba = sp.tsIDOseba.HasValue ? sp.tsIDOseba.Value : 0,
                                                           tsUpdate = sp.tsUpdate.HasValue ? sp.tsUpdate.Value : DateTime.MinValue,
                                                           tsUpdateUserID = sp.tsUpdateUserID.HasValue ? sp.tsUpdateUserID.Value : 0,
                                                       }).FirstOrDefault(),
                                ts = i.ts.HasValue ? i.ts.Value : DateTime.MinValue,
                                tsIDOsebe = i.tsIDOsebe.HasValue ? i.tsIDOsebe.Value : 0,
                                tsUpdate = i.tsUpdate.HasValue ? i.tsUpdate.Value : DateTime.MinValue,
                                tsUpdateUserID = i.tsUpdateUserID.HasValue ? i.tsUpdateUserID.Value : 0,
                                KupecNaziv_P = i.KupecNaziv_P,
                                Dobavitelji = i.Dobavitelji,
                                Zakleni = i.Zakleni.HasValue ? i.Zakleni.Value : false,
                                ZaklenilUporabnik = i.ZaklenilUporabnik.HasValue ? i.ZaklenilUporabnik.Value : 0,
                                Narocilo = (from o in context.Narocilo_PDO
                                            where o.NarociloID == i.NarociloID
                                            select new OrderPDOFullModel
                                            {
                                                DatumDobave = o.DatumDobave.HasValue ? o.DatumDobave.Value : DateTime.MinValue,
                                                NarociloID = o.NarociloID,
                                                NarociloStevilka_P = o.NarociloStevilka_P
                                            }).FirstOrDefault(),
                                DatumPredvideneDobave = i.DatumPredvideneDobave.HasValue ? i.DatumPredvideneDobave.Value : DateTime.MinValue
                            };

                return query.OrderByDescending(p => p.ts).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public string GetOrderPDFFilePDO(int iPDOOrderID)
        {
            try
            {
                OrderPDOFullModel rfm = GetOrderByID(iPDOOrderID);
                string curFile = "";
                string sResult = "";

                if (rfm != null)
                {
                    rfm.NarociloStevilka_P = (rfm.NarociloStevilka_P != null) ? rfm.NarociloStevilka_P : "xxx";



                    //// če je 5x neuspešno, potem se pošlje mail Daniju
                    if ((rfm.P_UnsuccCountCreatePDFPantheon >= 5) && (rfm.P_SendWarningToAdmin == 0))
                    {
                        InquiryStatus stat = inquiryRepo.GetPovprasevanjaStatusByCode(Enums.StatusOfInquiry.ERR_ADMIN_MAIL.ToString());
                        if (stat != null)
                        {
                            rfm.StatusID = stat.StatusPovprasevanjaID;
                        }
                        messageRepoPDO.CreateEmailForAdmin_NoPDFForOrderPDO("", rfm.NarociloStevilka_P.ToString(), rfm.P_TransportOrderPDFName, false);
                        rfm.P_SendWarningToAdmin = 1;
                        SaveOrder(rfm, true, false);
                        return "NOT_EXIST";
                    }
                    else if (rfm.P_SendWarningToAdmin == 1)
                    {
                        InquiryStatus stat = inquiryRepo.GetPovprasevanjaStatusByCode(Enums.StatusOfInquiry.ERR_ADMIN_MAIL.ToString());
                        if (stat != null)
                        {
                            rfm.StatusID = stat.StatusPovprasevanjaID;
                        }

                        DataTypesHelper.LogThis("Za naročilo št. " + rfm.NarociloStevilka_P + " ni bilo kreirano PDF in je bil poslal že mail administratorju.");
                        return "NOT_EXIST";
                    }

                    //// Do some work
                    //TimeSpan timeDiff = DateTime.Now - rfm.P_LastTSCreatePDFPantheon;
                    //if (timeDiff.TotalMinutes < 5) return "";

                    curFile = ((rfm.P_TransportOrderPDFDocPath != null) && (rfm.P_TransportOrderPDFDocPath.Length > 0)) ? rfm.P_TransportOrderPDFDocPath : "";

                    sResult = File.Exists(curFile) ? "EXIST" : "NOT_EXIST";

                    if (sResult != "EXIST")
                    {
                        //InquiryStatus stat = GetPovprasevanjaStatusByCode(Enums.StatusOfInquiry.ERR_ORDER_NO_SEND.ToString());
                        //if (stat != null)
                        //{
                        //    rfm.StatusID = stat.StatusPovprasevanjaID;
                        //}
                        //DataTypesHelper.LogThis("NOT EXIST : " + rfm.NarociloStevilka_P);
                        //LaunchPantheonCreatePDF();
                        //rfm.P_UnsuccCountCreatePDFPantheon++;
                        //rfm.P_LastTSCreatePDFPantheon = DateTime.Now;
                    }
                    else
                    {
                        //InquiryStatus stat = GetPovprasevanjaStatusByCode(Enums.StatusOfInquiry.KREIRAN_POSLAN_PDF.ToString());
                        //if (stat != null)
                        //{
                        //    DataTypesHelper.LogThis("Exist send order to suplier: " + rfm.NarociloStevilka_P);
                        //    rfm.StatusID = stat.StatusPovprasevanjaID;
                        //    rfm.P_GetPDFOrderFile = DateTime.Now;
                        //    // Create mail for prevoznik  
                        //    messageRepoPDO.CreateEmailForSupplierOrder(rfm);
                        //}

                    }
                    DataTypesHelper.LogThis("Save order: " + rfm.NarociloStevilka_P);
                    SaveOrder(rfm, true, false);


                }

                return sResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}