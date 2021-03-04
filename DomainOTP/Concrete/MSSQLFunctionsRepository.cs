using DatabaseWebService.Common;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.DomainNOZ;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.DomainPDO;
using DatabaseWebService.ModelsOTP.Client;
using DatabaseWebService.ModelsOTP.Order;
using DatabaseWebService.ModelsOTP.Recall;
using DatabaseWebService.ModelsOTP.Tender;
using DatabaseWebService.ModelsPDO.Inquiry;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using static DatabaseWebService.Common.Enums.Enums;

namespace DatabaseWebService.DomainOTP.Concrete
{
    public class MSSQLFunctionsRepository : IMSSQLFunctionsRepository
    {
        GrafolitOTPEntities context;
        GrafolitPDOEntities contextPDO;
        GrafolitNOZEntities contextNOZ;

        public MSSQLFunctionsRepository(GrafolitOTPEntities _context, GrafolitPDOEntities _contextPDO, GrafolitNOZEntities _contextNOZ)
        {
            context = _context;
            contextPDO = _contextPDO;
            contextNOZ = _contextNOZ;
        }

        public List<SupplierModel> GetListOfSupplier()
        {//TODO: pridobimo seznam tudi tistih dobaviteljev ki jih imamo v tabelo stranke_otp in so tipa SKLADISCE
            try
            {
                var query = from dob in context.SeznamDobaviteljev()
                            select new SupplierModel
                            {
                                Dobavitelj = dob.Dobavitelj,
                                Kraj = dob.Kraj,
                                Naslov = dob.Naslov,
                                Posta = dob.Posta
                            };

                //Seznam dobaviteljev, ki imajo v Tabeli Stranka_OTP polje TipStranke SKLADISCE
                string skladisceCode = Enums.TypeOfClient.SKLADISCE.ToString();
                var listOfClientsTypeSKLADISCE = from client in context.Stranka_OTP
                                                 where client.TipStranka.Koda == skladisceCode
                                                 select new SupplierModel
                                                 {
                                                     Dobavitelj = client.NazivPrvi,
                                                     Kraj = client.NazivPoste,
                                                     Naslov = client.Naslov,
                                                     Posta = client.StevPoste,
                                                     StrankaSkladisceID = client.idStranka
                                                 };

                List<SupplierModel> model = query.ToList();
                model.AddRange(listOfClientsTypeSKLADISCE.ToList());

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<DisconnectedInvoicesModel> GetDisconnectedInvoices()
        {
            try
            {
                int tempNum = 0;

                var query = from povfak in context.SeznamNepovezanihFaktur()
                            select new DisconnectedInvoicesModel
                            {
                                Kljuc = povfak.Kljuc,
                                acKey = povfak.acKey,
                                Datum = povfak.Datum.HasValue ? povfak.Datum.Value : DateTime.MinValue,
                                Valuta = povfak.Valuta,
                                Kupec = povfak.Kupec,
                                Prevzemnik = povfak.Prevzemnik,
                                Kolicina = povfak.Kolicina.HasValue ? povfak.Kolicina.Value : 0,
                                ZnesekFakture = povfak.ZnesekFakture
                            };


                List<DisconnectedInvoicesModel> model = query.ToList();
                model = model.OrderBy(dt => dt.Kljuc).ToList();
                foreach (var item in model)
                {
                    item.TempID = ++tempNum;
                }

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }


        private List<OrderPositionModelNew> SetApplicationToOrderPosition(List<OrderPositionModelNew> listPositions)
        {
            foreach (OrderPositionModelNew itmPos in listPositions)
            {
                //DataTypesHelper.LogThis("Before PDO naročila");
                string sOrderNumber = (itmPos.Narocilnica != null && itmPos.Narocilnica.Length > 0) ? itmPos.Narocilnica : "";
                //DataTypesHelper.LogThis("1");
                if (sOrderNumber.Length == 0) continue;
                sOrderNumber = sOrderNumber.Trim().Replace("-", "");
                //DataTypesHelper.LogThis("2");

                // preverimmo ali je naročilnica med PDO naročili            
                var queryPDO = from narItm in contextPDO.Narocilo_PDO where narItm.NarociloStevilka_P != null && narItm.NarociloStevilka_P.Trim() == sOrderNumber select narItm;


                //DataTypesHelper.LogThis("3");
                var itmPosPDO = queryPDO.FirstOrDefault();
                itmPos.TipAplikacije = (itmPosPDO != null) ? Enums.AppType.PDO.ToString() : "";
                //DataTypesHelper.LogThis("4");
                if (itmPosPDO != null)
                {
                    itmPos.SortGledeNaTipApp = (itmPos.Order_Confirm != null && itmPos.Order_Confirm.Length > 0) ? 0 : 1;
                    continue;
                }
                //DataTypesHelper.LogThis("Before NOZ naročila");
                // preverimo ali je med NOZ naročili
                var queryNOZ = from narItm in contextNOZ.NarociloOptimalnihZalog where narItm.NarociloID_P != null && narItm.NarociloID_P.Trim() == sOrderNumber select narItm;
                //DataTypesHelper.LogThis("1");
                var itmPosNOZ = queryNOZ.FirstOrDefault();
                //DataTypesHelper.LogThis("2");
                itmPos.TipAplikacije = (itmPosNOZ != null) ? Enums.AppType.NOZ.ToString() : "";
                if (itmPosNOZ != null)
                {
                    itmPos.SortGledeNaTipApp = (itmPos.Order_Confirm != null && itmPos.Order_Confirm.Length > 0) ? 2 : 3;
                    continue;
                }
                //DataTypesHelper.LogThis("3");

                itmPos.SortGledeNaTipApp = 4;
                //DataTypesHelper.LogThis("4");

            }

            return listPositions;
        }

        public List<OrderPositionModelNew> GetListOfOpenedOrderPositions(string supplier, int clientID = 0)
        {//TODO: pridobimo še vse pozicije iz tabele lastna zaloga če je izbran dobavitelj iz naše tabele stranka_otp i da je tipa SKLADISCE
            try
            {
                if (supplier == null || supplier.Length == 0) return null;

                DataTypesHelper.LogThis("GetListOfOpenedOrderPositions for : " + supplier);

                supplier = supplier.Replace("|", "&");

                var query = from np in context.SeznamPozicijOdprtihNarocilnicGledeNaDobavitelja(supplier.Trim())
                            select new OrderPositionModelNew
                            {
                                UniqueID = string.Concat("-", np.Narocilnica, SqlFunctions.StringConvert((double)np.St_Pozicija)),
                                //UniqueID = SqlFunctions.StringConvert((double)np.St_Pozicija),
                                Artikel = np.Artikel,
                                Datum_Dobave = np.Datum_Dobave,
                                Datum_narocila = np.Datum_narocila.HasValue ? np.Datum_narocila.Value : DateTime.MinValue,
                                Dobavitelj = np.Dobavitelj,
                                Kupec = np.Kupec,
                                Naroceno = np.Naroceno,
                                Narocilnica = np.Narocilnica,
                                Order_Confirm = np.Order_Confirm,
                                Prevzeto = np.Prevzeto,
                                Razlika = np.Razlika.HasValue ? np.Razlika.Value : 0,
                                St_Pozicija = np.St_Pozicija,
                                Tovarna = np.Tovarna,
                                Tip = np.Tip,
                                Zaloga = np.Zaloga.HasValue ? np.Zaloga.Value : 0,
                                Interno = np.Interno,
                                Dovoljeno_Odpoklicati = np.Dovoljeno_odpoklicati,
                                Proizvedeno = np.Proizvedeno,
                                Kategorija = np.Kategorija == null ? "" : np.Kategorija,
                                Ident = np.Ident,
                                Kupec_Kraj = np.Kupec_Kraj,
                                Kupec_Naslov = np.Kupec_Naslov,
                                Kupec_Posta = np.Kupec_Posta,
                                EnotaMere = np.Enota_Mere
                            };

                //var pos = query.Where(o => o.Order_Confirm == "15-4988/30").FirstOrDefault();
                List<OrderPositionModelNew> list = query.ToList();
                int count = 1;
                foreach (var item in list)
                {
                    item.tempID = count;
                    count++;
                }
                //DataTypesHelper.LogThis("count : " + count);

                string kodaPotrjen = Enums.StatusOfRecall.POTRJEN.ToString();
                int statusPotrjen = context.StatusOdpoklica.Where(so => so.Koda == kodaPotrjen).FirstOrDefault().StatusOdpoklicaID;

                string kodaDelnoPrevzet = Enums.StatusOfRecall.DELNO_PREVZET.ToString();
                int statusDelnoPrevzet = context.StatusOdpoklica.Where(so => so.Koda == kodaDelnoPrevzet).FirstOrDefault().StatusOdpoklicaID;
                //DataTypesHelper.LogThis("Before CheckPositionQuantity");
                CheckPositionQuantity(list, statusPotrjen, statusDelnoPrevzet);
                //DataTypesHelper.LogThis("After CheckPositionQuantity");
                // preverimo za katero aplikacijo gre
                //DataTypesHelper.LogThis("Before SetApplicationToOrderPosition");
                list = SetApplicationToOrderPosition(list);
                //DataTypesHelper.LogThis("After SetApplicationToOrderPosition");
                list = list.OrderBy(p => p.Datum_Dobave).OrderBy(p1 => p1.SortGledeNaTipApp).ToList();

                #region
                /*for (int i = list.Count - 1; i >= 0; i--)
                {
                    OrderPositionModelNew item = list[i];
                    //OdpoklicPozicija rPos = context.OdpoklicPozicija.Where(op => op.MaterialIdent == item.Ident && op.Odpoklic.StatusID == statusPotrjen).OrderByDescending(op => op.DatumVnosa).FirstOrDefault();
                    List<OdpoklicPozicija> currentKolicinaOTPList = context.OdpoklicPozicija.Where(op => op.MaterialIdent == item.Ident &&
                        !op.StatusPrevzeto.Value &&
                        (op.Odpoklic.StatusID == statusPotrjen || op.Odpoklic.StatusID == statusDelnoPrevzet)).ToList();

                    decimal? currentRecallQuantity = 0;
                    if (currentKolicinaOTPList.Count > 0)
                        currentRecallQuantity = currentKolicinaOTPList.Sum(op => op.Kolicina);

                    //pridobimo vse pozicije odpoklicev za posamezno pozicijo naročilnice
                    List<OdpoklicPozicija> posByOrderPosNum = context.OdpoklicPozicija.Where(op => op.MaterialIdent == item.Ident &&
                        op.NarociloID == item.Narocilnica &&
                        op.NarociloPozicijaID == item.St_Pozicija &&
                        !op.StatusPrevzeto.Value &&
                        (op.Odpoklic.StatusID == statusPotrjen || op.Odpoklic.StatusID == statusDelnoPrevzet)).ToList();

                    //seštejemo odpoklicano količino za pozicijo naročilnice
                    item.OdpoklicKolicinaOTP = posByOrderPosNum.Sum(op => op.Kolicina);

                    if (item.OdpoklicKolicinaOTP == item.Naroceno)
                        list.RemoveAt(i);
                    else if (currentRecallQuantity.HasValue && item.Naroceno == (currentRecallQuantity + item.Zaloga))
                    {
                        item.VsotaOdpoklicKolicinaOTP = (currentRecallQuantity.Value - item.OdpoklicKolicinaOTP);//rPos.KolicinaOTP.Value;
                    }
                    else if (currentRecallQuantity.HasValue && currentRecallQuantity.Value < item.Naroceno)
                    {
                        //list[i].Naroceno -= rPos.KolicinaOTP.HasValue ? rPos.KolicinaOTP.Value : 0;
                        //list[i].Razlika = list[i].Naroceno - list[i].Prevzeto;//TODO: Kaj če bo večja količina prevzeta kot naročena? (negativna razlika????)
                        item.VsotaOdpoklicKolicinaOTP = (currentRecallQuantity.Value - item.OdpoklicKolicinaOTP);
                    }
                    else if (currentRecallQuantity.HasValue && currentRecallQuantity.Value > item.Naroceno)
                    {
                        item.VsotaOdpoklicKolicinaOTP = (currentRecallQuantity.Value - item.OdpoklicKolicinaOTP);
                        //TODO: na večih pozicijah naročilnic potrebno odšteti in po potrebi le-to odstraniti iz seznama
                        /**decimal tempQuantity = rPos.KolicinaOTP.Value;
                        for (int j = list.Count - 1; j >= 0; j--)
                        {
                            OrderPositionModelNew obj = list[j];
                            if (obj.Ident == item.Ident)
                            {
                                if (tempQuantity > obj.Naroceno)
                                {
                                    tempQuantity -= obj.Naroceno;
                                    list.RemoveAt(j);
                                }
                                else if (tempQuantity == obj.Naroceno)
                                {
                                    tempQuantity -= obj.Naroceno;
                                    list.RemoveAt(j);
                                }
                                else if (tempQuantity < obj.Naroceno)
                                {
                                    obj.Naroceno -= tempQuantity;
                                    obj.Razlika = obj.Naroceno - obj.Prevzeto;//TODO: Kaj če bo večja količina prevzeta kot naročena? (negativna razlika????)
                                }
                            }
                        }
                    }
                }*/
                #endregion
                //DataTypesHelper.LogThis("Before clientID : " + clientID.ToString());
                if (clientID > 0)
                {
                    var lastnoSkladisce = from ow in context.LastnaZaloga
                                          where ow.LastnoSkladisceID == clientID
                                          select new OrderPositionModelNew
                                          {
                                              Artikel = ow.Material,
                                              // Datum_Dobave = np.Datum_Dobave,
                                              //Datum_narocila = np.Datum_narocila.HasValue ? np.Datum_narocila.Value : DateTime.MinValue,
                                              //Dobavitelj = np.Dobavitelj,
                                              Kupec = ow.KupecNaziv,
                                              Naroceno = ow.KolicinaIzNarocila,
                                              Narocilnica = ow.NarociloID,
                                              Order_Confirm = ow.OC,
                                              Prevzeto = ow.KolicinaPrevzeta.HasValue ? ow.KolicinaPrevzeta.Value : 0,
                                              Razlika = ow.KolicinaRazlika.HasValue ? ow.KolicinaRazlika.Value : 0,
                                              St_Pozicija = ow.NarociloPozicijaID,
                                              //Tovarna = np.Tovarna,
                                              Tip = ow.TipNaziv,
                                              Zaloga = ow.TrenutnaZaloga.HasValue ? ow.TrenutnaZaloga.Value : 0,
                                              Interno = ow.Interno,
                                              Dovoljeno_Odpoklicati = ow.OptimalnaZaloga.HasValue ? (int)ow.OptimalnaZaloga.Value : 0,
                                              Proizvedeno = ow.Proizvedeno.HasValue ? ow.Proizvedeno.Value : 0,
                                              Ident = ow.MaterialIdent,
                                              Kupec_Kraj = ow.KupecKraj,
                                              Kupec_Naslov = ow.KupecNaslov,
                                              Kupec_Posta = ow.KupecPosta,
                                              OdpoklicID = ow.OdpoklicID,
                                              EnotaMere = ow.EnotaMere
                                          };

                    List<OrderPositionModelNew> newList = lastnoSkladisce.ToList();


                    foreach (var item in newList)
                    {
                        item.tempID = count;
                        count++;
                    }

                    CheckPositionQuantity(newList, statusPotrjen, statusDelnoPrevzet, true);

                    list.AddRange(newList);
                }
                //DataTypesHelper.LogThis("After Client");
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<OrderPositionModelNew> GetListOfOrderNumber10()
        {//TODO: pridobimo še vse pozicije iz tabele lastna zaloga če je izbran dobavitelj iz naše tabele stranka_otp i da je tipa SKLADISCE
            try
            {
                DataTypesHelper.LogThis("GetListOfOrderNumber10");


                var query = from np in context.SeznamPozicijNarocilnic10ZaOdpoklic()
                            select new OrderPositionModelNew
                            {
                                Status = np.Status,
                                ZeljeniRokDobave = np.ZeljeniRokDobave.HasValue ? np.ZeljeniRokDobave.Value : DateTime.MinValue,
                                Narocilnica = np.StevilkaDokumenta,
                                Kupec = np.Stranka,
                                Dobavitelj = np.Dobavitelj == null ? "" : np.Dobavitelj,
                                Kategorija = np.Kategorija == null ? "" : np.Kategorija,
                                Ident = np.KodaArtikla,
                                Artikel = np.NazivArtikla,
                                Naroceno = np.NarocenaKolicina,
                                EnotaMere = np.EnotaMere,
                                PotrjeniRokDobave = np.PotrjeniRokDobave.HasValue ? np.PotrjeniRokDobave.Value : DateTime.MinValue
                            };
                List<OrderPositionModelNew> list = query.ToList();

                int count = 1;
                foreach (var item in list)
                {
                    item.tempID = count;
                    count++;
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        private List<OrderPositionModelNew> CheckPositionQuantity(List<OrderPositionModelNew> list, int statusPotrjen, int statusDelnoPrevzet, bool ownStockRecall = false)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                OrderPositionModelNew item = list[i];

                //OdpoklicPozicija rPos = context.OdpoklicPozicija.Where(op => op.MaterialIdent == item.Ident && op.Odpoklic.StatusID == statusPotrjen).OrderByDescending(op => op.DatumVnosa).FirstOrDefault();
                List<OdpoklicPozicija> currentKolicinaOTPList = context.OdpoklicPozicija.Where(op => op.MaterialIdent == item.Ident &&
                    !op.StatusPrevzeto.Value &&
                    (op.OdpoklicIzLastneZaloge.HasValue && op.OdpoklicIzLastneZaloge.Value == ownStockRecall) &&
                    (op.Odpoklic.StatusID == statusPotrjen || op.Odpoklic.StatusID == statusDelnoPrevzet)).ToList();

                decimal? currentRecallQuantity = 0;
                if (currentKolicinaOTPList.Count > 0)
                    currentRecallQuantity = currentKolicinaOTPList.Sum(op => op.Kolicina);

                //pridobimo vse pozicije odpoklicev za posamezno pozicijo naročilnice
                List<OdpoklicPozicija> posByOrderPosNum = context.OdpoklicPozicija.Where(op => op.MaterialIdent == item.Ident &&
                    op.NarociloID == item.Narocilnica &&
                    op.NarociloPozicijaID == item.St_Pozicija &&
                    !op.StatusPrevzeto.Value &&
                    op.OdpoklicIzLastneZaloge.Value == ownStockRecall &&
                    (op.Odpoklic.StatusID == statusPotrjen || op.Odpoklic.StatusID == statusDelnoPrevzet)).ToList();

                //seštejemo odpoklicano količino za pozicijo naročilnice
                item.OdpoklicKolicinaOTP = posByOrderPosNum.Sum(op => op.Kolicina);

                decimal primerjalnaKolicina = 0;

                if (item.Proizvedeno > item.Naroceno)
                    primerjalnaKolicina = item.Proizvedeno;
                else
                    primerjalnaKolicina = item.Naroceno;


                if (item.OdpoklicKolicinaOTP == primerjalnaKolicina)
                    list.RemoveAt(i);
                else if (currentRecallQuantity.HasValue && primerjalnaKolicina == (currentRecallQuantity + item.Zaloga))
                {
                    item.VsotaOdpoklicKolicinaOTP = (currentRecallQuantity.Value - item.OdpoklicKolicinaOTP);//rPos.KolicinaOTP.Value;
                }
                else if (currentRecallQuantity.HasValue && currentRecallQuantity.Value < primerjalnaKolicina)
                {
                    //list[i].Naroceno -= rPos.KolicinaOTP.HasValue ? rPos.KolicinaOTP.Value : 0;
                    //list[i].Razlika = list[i].Naroceno - list[i].Prevzeto;//TODO: Kaj če bo večja količina prevzeta kot naročena? (negativna razlika????)
                    item.VsotaOdpoklicKolicinaOTP = (currentRecallQuantity.Value - item.OdpoklicKolicinaOTP);
                }
                else if (currentRecallQuantity.HasValue && currentRecallQuantity.Value > primerjalnaKolicina)
                {
                    item.VsotaOdpoklicKolicinaOTP = (currentRecallQuantity.Value - item.OdpoklicKolicinaOTP);
                    //TODO: na večih pozicijah naročilnic potrebno odšteti in po potrebi le-to odstraniti iz seznama
                    /**decimal tempQuantity = rPos.KolicinaOTP.Value;
                    for (int j = list.Count - 1; j >= 0; j--)
                    {
                        OrderPositionModelNew obj = list[j];
                        if (obj.Ident == item.Ident)
                        {
                            if (tempQuantity > obj.Naroceno)
                            {
                                tempQuantity -= obj.Naroceno;
                                list.RemoveAt(j);
                            }
                            else if (tempQuantity == obj.Naroceno)
                            {
                                tempQuantity -= obj.Naroceno;
                                list.RemoveAt(j);
                            }
                            else if (tempQuantity < obj.Naroceno)
                            {
                                obj.Naroceno -= tempQuantity;
                                obj.Razlika = obj.Naroceno - obj.Prevzeto;//TODO: Kaj če bo večja količina prevzeta kot naročena? (negativna razlika????)
                            }
                        }
                    }*/
                }

                if (item.OdpoklicKolicinaOTP == 0) item.OdpoklicKolicinaOTP = primerjalnaKolicina;

            }

            return list;
        }

        public TransportCountModel GetTransportCounByTransporterAndRoute(TransportCountModel model)
        {
            try
            {

                var obj = context.GetLastYearRecallCountBySuplierAndRoute(model.RelacijaID, model.PrevoznikID).FirstOrDefault();

                if (obj != null)
                    return new TransportCountModel() { PrevoznikID = obj.DobaviteljID.Value, RelacijaID = obj.RelacijaID.Value, StPotrjenihOdpoklicevNaRelacijoZaPrevoznika = obj.St_Relacija_Dobavitelj.Value, StPotrjenihOdpoklicevNaRelacijoZaVsePrevoznike = obj.St_Relacija.Value };
                else
                    return new TransportCountModel();

            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public CreateOrderDocument GetOrderDocumentData(string OrderDocXML)
        {
            CreateOrderDocument _coData = new CreateOrderDocument();


            ObjectParameter opExportPath = new ObjectParameter("p_cExportPath", "");
            ObjectParameter opPDFFileName = new ObjectParameter("p_cKey", "");
            ObjectParameter opErrorDesc = new ObjectParameter("p_cError", "");



            var obj = context.DodajPantheonDokument(OrderDocXML, opExportPath, opPDFFileName, opErrorDesc);


            string sExportPath = Convert.ToString(opExportPath.Value);
            string sPDFFileName = Convert.ToString(opPDFFileName.Value);
            string sErrorDesc = Convert.ToString(opErrorDesc.Value);

            DataTypesHelper.LogThis("Številka naročilnice: " + sPDFFileName);

            if (sErrorDesc.Length > 0) DataTypesHelper.LogThis("Error create naročilnica : " + sErrorDesc);

            if (sErrorDesc.Length > 0) throw new Exception(ValidationExceptionError.res_28 + "<br><br>" + sErrorDesc);

            _coData.ExportPath = sExportPath;
            _coData.PDFFile = sPDFFileName;
            _coData.ErrorDesc = sErrorDesc;


            return _coData;
        }

        public CreateOrderDocument GetOrderDocumentDataSupplierOrderAndLinkInvoice(string OrderDocXML, string InvoicesDocXML, string sCreatedOrderNo = "")
        {
            CreateOrderDocument _coData = new CreateOrderDocument();


            ObjectParameter opReturnOrderNo = new ObjectParameter("p_cKey", "");
            ObjectParameter opErrorDesc = new ObjectParameter("p_cError", "");


            if (sCreatedOrderNo.Length == 0)
            {
                var obj = context.DodajPantheonSupplierOrderAndLinkInvoice(OrderDocXML, "", opReturnOrderNo, opErrorDesc);
            }
            string sOrderNo = (sCreatedOrderNo.Length > 0 ? sCreatedOrderNo : Convert.ToString(opReturnOrderNo.Value));
            string sErrorDesc = Convert.ToString(opErrorDesc.Value);

            if (sOrderNo != null && sOrderNo.Length > 0)
                InvoicesDocXML = InvoicesDocXML.Replace("xxStOrderxx", sOrderNo);
            else
            {
                if (sErrorDesc.Length > 0) DataTypesHelper.LogThis("Error create naročilnica : " + sErrorDesc);
            }

            var obj1 = context.DodajPantheonSupplierOrderAndLinkInvoice("", InvoicesDocXML, opReturnOrderNo, opErrorDesc);

            sErrorDesc = Convert.ToString(opErrorDesc.Value);

            DataTypesHelper.LogThis("Številka naročilnice: " + sOrderNo);

            context.OsveziPantheonLinkedInvoicesByOrderNo(sOrderNo);

            if (sErrorDesc.Length > 0) DataTypesHelper.LogThis("Error create naročilnica : " + sErrorDesc);

            if (sErrorDesc.Length > 0) throw new Exception(ValidationExceptionError.res_28 + "<br><br>" + sErrorDesc);

            _coData.ErrorDesc = sErrorDesc;
            _coData.OrderNumber = sOrderNo;


            return _coData;
        }

        public List<ProductCategory> GetCategoryList()
        {
            int tempNum = 0;
            var query = from c in context.GetCategoryListOTP()
                        select new ProductCategory
                        {
                            Naziv = c
                        };

            var list = query.ToList();
            foreach (var item in list)
            {
                item.TempID = tempNum++;
            }

            return list;
        }


    }
}