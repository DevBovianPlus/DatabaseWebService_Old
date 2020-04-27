using DatabaseWebService.Common;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsOTP.Client;
using DatabaseWebService.ModelsOTP.Recall;
using DatabaseWebService.ModelsOTP.Route;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.DomainOTP.Concrete
{
    public class RecallRepository : IRecallRepository
    {
        GrafolitOTPEntities context;
        ISystemMessageEventsRepository_OTP messageEventRepo;

        public RecallRepository(GrafolitOTPEntities _context, ISystemMessageEventsRepository_OTP _messageEventRepo)
        {
            context = _context;
            messageEventRepo = _messageEventRepo;
        }

        public List<RecallModel> GetAllRecalls()
        {
            try
            {
                var query = from recall in context.Odpoklic
                            select new RecallModel
                            {
                                CenaPrevoza = recall.CenaPrevoza.HasValue ? recall.CenaPrevoza.Value : 0,
                                DobaviteljID = recall.DobaviteljID.HasValue ? recall.DobaviteljID.Value : 0,
                                DobaviteljNaziv = recall.DobaviteljNaziv,
                                KolicinaSkupno = recall.KolicinaSkupno,
                                OdpoklicID = recall.OdpoklicID,
                                RelacijaID = recall.RelacijaID.HasValue ? recall.RelacijaID.Value : 0,
                                RelacijaNaziv = recall.Relacija != null ? recall.Relacija.Naziv : "",
                                StatusID = recall.StatusID,
                                StatusNaziv = recall.StatusOdpoklica != null ? recall.StatusOdpoklica.Naziv : "",
                                StatusKoda = recall.StatusOdpoklica != null ? recall.StatusOdpoklica.Koda : "",
                                ts = recall.ts.HasValue ? recall.ts.Value : DateTime.MinValue,
                                tsIDOseba = recall.tsIDOseba.HasValue ? recall.tsIDOseba.Value : 0,
                                OdpoklicStevilka = recall.OdpoklicStevilka.HasValue ? recall.OdpoklicStevilka.Value : 0,
                                DobaviteljUrediTransport = recall.DobaviteljUrediTransport.HasValue ? recall.DobaviteljUrediTransport.Value : false,
                                DobaviteljKraj = recall.DobaviteljKraj,
                                DobaviteljNaslov = recall.DobaviteljNaslov,
                                DobaviteljPosta = recall.DobaviteljPosta
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public RecallFullModel GetRecallFullModelByID(int recallID)
        {
            try
            {
                var query = from recall in context.Odpoklic
                            where recall.OdpoklicID == recallID
                            select new RecallFullModel
                            {
                                CenaPrevoza = recall.CenaPrevoza.HasValue ? recall.CenaPrevoza.Value : 0,
                                DobaviteljID = recall.DobaviteljID.HasValue ? recall.DobaviteljID.Value : 0,
                                KolicinaSkupno = recall.KolicinaSkupno,                                
                                OdpoklicID = recall.OdpoklicID,
                                RelacijaID = recall.RelacijaID.HasValue ? recall.RelacijaID.Value : 0,
                                StatusID = recall.StatusID,
                                Opis = recall.Opis,
                                ts = recall.ts.HasValue ? recall.ts.Value : DateTime.MinValue,
                                tsIDOseba = recall.tsIDOseba.HasValue ? recall.tsIDOseba.Value : 0,
                                PaleteSkupaj = recall.PaleteSkupaj.HasValue ? recall.PaleteSkupaj.Value : 0,
                                SoferNaziv = recall.SoferNaziv,
                                Registracija = recall.Registracija,
                                OdobritevKomentar = recall.OdobritevKomentar,
                                RazpisPozicijaID = recall.RazpisPozicijaID.HasValue ? recall.RazpisPozicijaID.Value : 0,
                                OdpoklicStevilka = recall.OdpoklicStevilka.HasValue ? recall.OdpoklicStevilka.Value : 0,
                                StatusOdpoklica = (from status in context.StatusOdpoklica
                                                   where status.StatusOdpoklicaID == recall.StatusID
                                                   select new RecallStatus
                                                   {
                                                       Koda = status.Koda,
                                                       Naziv = status.Naziv,
                                                       Opis = status.Opis,
                                                       StatusOdpoklicaID = status.StatusOdpoklicaID,
                                                       ts = recall.ts.HasValue ? recall.ts.Value : DateTime.MinValue,
                                                       tsIDOseba = recall.tsIDOseba.HasValue ? recall.tsIDOseba.Value : 0
                                                   }).FirstOrDefault(),
                                Relacija = (from route in context.Relacija
                                            where route.RelacijaID == recall.RelacijaID
                                            select new RouteModel
                                            {
                                                Datum = route.Datum.HasValue ? route.Datum.Value : DateTime.MinValue,
                                                Dolzina = route.Dolzina,
                                                Koda = route.Koda,
                                                Naziv = route.Naziv,
                                                RelacijaID = route.RelacijaID,
                                                ts = recall.ts.HasValue ? recall.ts.Value : DateTime.MinValue,
                                                tsIDOsebe = recall.tsIDOseba.HasValue ? recall.tsIDOseba.Value : 0
                                            }).FirstOrDefault(),
                                Dobavitelj = (from client in context.Stranka_OTP
                                              where client.idStranka == recall.DobaviteljID
                                              select new ClientFullModel
                                              {
                                                  idStranka = client.idStranka,
                                                  KodaStranke = client.KodaStranke,
                                                  NazivPrvi = client.NazivPrvi,
                                                  NazivDrugi = client.NazivDrugi,
                                                  Naslov = client.Naslov,
                                                  StevPoste = client.StevPoste,
                                                  NazivPoste = client.NazivPoste,
                                                  Email = client.Email,
                                                  Telefon = client.Telefon,
                                                  FAX = client.FAX,
                                                  InternetniNalov = client.InternetniNalov,
                                                  KontaktnaOseba = client.KontaktnaOseba,
                                                  RokPlacila = client.RokPlacila,
                                                  TRR = client.TRR,
                                                  DavcnaStev = client.DavcnaStev,
                                                  MaticnaStev = client.MaticnaStev,
                                                  RangStranke = client.RangStranke,
                                                  StatusDomacTuji = client.StatusDomacTuji,
                                                  Zavezanec_DA_NE = client.Zavezanec_DA_NE,
                                                  IdentifikacijskaStev = client.IdentifikacijskaStev,
                                                  Clan_EU = client.Clan_EU,
                                                  BIC = client.BIC,
                                                  KodaPlacila = client.KodaPlacila,
                                                  StatusKupecDobavitelj = client.StatusKupecDobavitelj,
                                                  DrzavaStranke = client.DrzavaStranke,
                                                  Neaktivna = client.Neaktivna,
                                                  Activity = client.Activity.HasValue ? client.Activity.Value : 0,
                                                  GenerirajERacun = client.GenerirajERacun.HasValue ? client.GenerirajERacun.Value : 0,
                                                  JavniZavod = client.JavniZavod.HasValue ? client.JavniZavod.Value : 0,
                                                  ts = client.ts.HasValue ? client.ts.Value : DateTime.MinValue,
                                                  tsIDOsebe = client.tsIDOsebe.HasValue ? client.tsIDOsebe.Value : 0,
                                                  //TODO: Add collections of Dogodek, KontaktneOsebe, Nadzor, Plan, StrankaZaposleni
                                                  TipStrankaID = client.TipID,
                                                  TipStranka = (from clientType in context.TipStranka
                                                                where clientType.TipStrankaID == client.TipID
                                                                select new ClientType
                                                                {
                                                                    Koda = clientType.Koda,
                                                                    Naziv = clientType.Naziv,
                                                                    Opis = clientType.Opis,
                                                                    TipStrankaID = clientType.TipStrankaID,
                                                                    ts = clientType.ts.HasValue ? clientType.ts.Value : DateTime.MinValue,
                                                                    tsIDOseba = clientType.tsIDOseba.HasValue ? clientType.tsIDOseba.Value : 0
                                                                }).FirstOrDefault()
                                              }).FirstOrDefault(),
                                UserID = recall.UserID.HasValue ? recall.UserID.Value : 0,
                                User = (from employee in context.Osebe_OTP
                                        where employee.idOsebe == recall.UserID
                                        select new EmployeeSimpleModel
                                        {
                                            DatumRojstva = employee.DatumRojstva.HasValue ? employee.DatumRojstva.Value : DateTime.MinValue,
                                            DatumZaposlitve = employee.DatumZaposlitve.HasValue ? employee.DatumZaposlitve.Value : DateTime.MinValue,
                                            DelovnoMesto = employee.DelovnoMesto,
                                            Email = employee.Email,
                                            Geslo = employee.Geslo,
                                            idOsebe = employee.idOsebe,
                                            idVloga = employee.idVloga.HasValue ? employee.idVloga.Value : 0,
                                            Ime = employee.Ime,
                                            Naslov = employee.Naslov,
                                            Priimek = employee.Priimek,
                                            TelefonGSM = employee.TelefonGSM,
                                            UporabniskoIme = employee.UporabniskoIme
                                        }).FirstOrDefault(),
                                DobaviteljNaziv = recall.DobaviteljNaziv,
                                DobaviteljNaslov = recall.DobaviteljNaslov,
                                DobaviteljPosta = recall.DobaviteljPosta,
                                DobaviteljKraj = recall.DobaviteljKraj,
                                DatumNaklada = recall.DatumNaklada.HasValue ? recall.DatumNaklada.Value : DateTime.MinValue,
                                DatumRazklada = recall.DatumRazklada.HasValue ? recall.DatumRazklada.Value : DateTime.MinValue,
                                DobaviteljUrediTransport = recall.DobaviteljUrediTransport.HasValue ? recall.DobaviteljUrediTransport.Value : false,
                                RazlogOdobritveSistem = recall.RazlogOdobritveSistem,
                                LastenPrevoz = recall.LastenPrevoz.HasValue ? recall.LastenPrevoz.Value : false,
                                TipPrevozaID = recall.TipPrevoza.HasValue ? recall.TipPrevoza.Value : 0,
                                TipPrevoza = (from transportType in context.TipPrevoza
                                              where transportType.TipPrevozaID == recall.TipPrevoza
                                              select new ClientTransportType
                                              {
                                                  DovoljenaTeza = transportType.DovoljenaTeza.HasValue ? transportType.DovoljenaTeza.Value : 0,
                                                  Koda = transportType.Koda,
                                                  Naziv = transportType.Naziv,
                                                  Opombe = transportType.Opombe,
                                                  ShranjevanjePozicij = transportType.ShranjevanjePozicij.HasValue ? transportType.ShranjevanjePozicij.Value : false,
                                                  TipPrevozaID = transportType.TipPrevozaID,
                                                  ts = transportType.ts.HasValue ? transportType.ts.Value : DateTime.MinValue,
                                                  tsIDPrijave = transportType.tsIDPrijave.HasValue ? transportType.tsIDPrijave.Value : 0
                                              }).FirstOrDefault(),
                                LastnoSkladisceID = recall.LastnoSkladisceID.HasValue ? recall.LastnoSkladisceID.Value : 0,
                                Prevozniki = recall.Prevozniki,
                                KupecUrediTransport = recall.KupecUrediTransport.HasValue ? recall.KupecUrediTransport.Value : false,
                                PovprasevanjePoslanoPrevoznikom = recall.PovprasevanjePoslanoPrevoznikom.HasValue ? recall.PovprasevanjePoslanoPrevoznikom.Value : false,
                                PrevoznikOddalNajnizjoCeno = recall.PrevoznikOddalNajnizjoCeno.HasValue ? recall.PrevoznikOddalNajnizjoCeno.Value : false,
                                OpombaZaPovprasevnjePrevoznikom = recall.OpombaZaPovprasevnjePrevoznikom,
                                P_CreateOrder = recall.P_CreateOrder.HasValue ? recall.P_CreateOrder.Value : DateTime.MinValue,
                                P_LastTSCreatePDFPantheon = recall.P_LastTSCreatePDFPantheon.HasValue ? recall.P_LastTSCreatePDFPantheon.Value : DateTime.MinValue,
                                P_GetPDFOrderFile = recall.P_GetPDFOrderFile.HasValue ? recall.P_GetPDFOrderFile.Value : DateTime.MinValue,
                                P_TransportOrderPDFDocPath = recall.P_TransportOrderPDFDocPath,
                                P_TransportOrderPDFName = recall.P_TransportOrderPDFName,
                                P_UnsuccCountCreatePDFPantheon = recall.P_UnsuccCountCreatePDFPantheon.HasValue ? recall.P_UnsuccCountCreatePDFPantheon.Value : 0,
                                P_SendWarningToAdmin = recall.P_SendWarningToAmin.HasValue ? recall.P_SendWarningToAmin.Value : 0,
                            };

                RecallFullModel model = query.FirstOrDefault();
                if (model != null)
                {
                    model.OdpoklicPozicija = GetRecallPositionsByID(recallID);
                    string statusDelovna = Enums.StatusOfRecall.DELOVNA.ToString();
                    int statusDelovnaID = context.StatusOdpoklica.Where(so => so.Koda == statusDelovna).FirstOrDefault().StatusOdpoklicaID;

                    foreach (var item in model.OdpoklicPozicija)
                    {
                        if (model.StatusID == statusDelovnaID)
                        {
                            item.KolicinaOTPPozicijaNarocilnice = GetSumRecallPosQntyByOrderAndOrderPosition(item);
                            item.KolicinaOTP = GetSumRecallPosQuantity(item.MaterialIdent, model.OdpoklicPozicija) - item.KolicinaOTPPozicijaNarocilnice;
                        }
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }


        public List<RecallPositionModel> GetRecallPositionsByID(int recallID)
        {
            try
            {
                var query = from position in context.OdpoklicPozicija
                            where position.OdpoklicID == recallID
                            select new RecallPositionModel
                            {
                                Kolicina = position.Kolicina,
                                StPalet = position.StPalet.HasValue ? position.StPalet.Value : 0,
                                KolicinaIzNarocila = position.KolicinaIzNarocila,
                                Material = position.Material,
                                NarociloID = position.NarociloID,
                                NarociloPozicijaID = position.NarociloPozicijaID,
                                OdpoklicID = position.OdpoklicID,
                                OdpoklicPozicijaID = position.OdpoklicPozicijaID,
                                StatusKolicine = position.StatusKolicine,
                                ts = position.ts.HasValue ? position.ts.Value : DateTime.MinValue,
                                tsIDOseba = position.tsIDOseba.HasValue ? position.tsIDOseba.Value : 0,
                                TipID = position.TipID,
                                TipOdpoklica = (from type in context.TipOdpoklica
                                                where type.TipOdpoklicaID == position.TipID
                                                select new RecallType
                                                {
                                                    Koda = type.Koda,
                                                    Naziv = type.Naziv,
                                                    Opis = type.Opis,
                                                    TipOdpoklicaID = type.TipOdpoklicaID,
                                                    ts = position.ts.HasValue ? position.ts.Value : DateTime.MinValue,
                                                    tsIDOseba = position.tsIDOseba.HasValue ? position.tsIDOseba.Value : 0
                                                }).FirstOrDefault(),
                                OC = position.OC,
                                KolicinaPrevzeta = position.KolicinaPrevzeta.HasValue ? position.KolicinaPrevzeta.Value : 0,
                                KolicinaRazlika = position.KolicinaRazlika.HasValue ? position.KolicinaRazlika.Value : 0,
                                Palete = position.Palete.HasValue ? position.Palete.Value : 0,
                                KupecNaziv = position.KupecNaziv,
                                KupecViden = position.KupecViden.HasValue ? position.KupecViden.Value : 0,
                                OptimalnaZaloga = position.OptimalnaZaloga.HasValue ? position.OptimalnaZaloga.Value : 0,
                                TrenutnaZaloga = position.TrenutnaZaloga.HasValue ? position.TrenutnaZaloga.Value : 0,
                                Interno = position.Interno,
                                TipNaziv = position.TipNaziv,
                                Proizvedeno = position.Proizvedeno.HasValue ? position.Proizvedeno.Value : 0,
                                MaterialIdent = position.MaterialIdent,
                                DatumVnosa = position.DatumVnosa.HasValue ? position.DatumVnosa.Value : DateTime.MinValue,
                                KolicinaOTP = position.KolicinaOTP.HasValue ? position.KolicinaOTP.Value : 0,
                                StatusPrevzeto = position.StatusPrevzeto.HasValue ? position.StatusPrevzeto.Value : false,
                                ZaporednaStevilka = position.ZaporednaStevilka.HasValue ? position.ZaporednaStevilka.Value : 0,
                                KolicinaOTPPozicijaNarocilnice = position.KolicinaOTPPozicijaNarocilnice.HasValue ? position.KolicinaOTPPozicijaNarocilnice.Value : 0,
                                KupecNaslov = position.KupecNaslov,
                                KupecKraj = position.KupecKraj,
                                KupecPosta = position.KupecPosta,
                                OdpoklicIzLastneZaloge = position.OdpoklicIzLastneZaloge.HasValue ? position.OdpoklicIzLastneZaloge.Value : false,
                                PrvotniOdpoklicPozicijaID = position.PrvotniOdpoklicPozicijaID.HasValue ? position.PrvotniOdpoklicPozicijaID.Value : 0,
                                Split = position.Split.HasValue ? position.Split.Value : false,
                                EnotaMere = position.EnotaMere,
                                TransportnaKolicina = position.TransportnaKolicina.HasValue ? position.TransportnaKolicina.Value : 0
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public int SaveRecall(RecallFullModel model, bool updateRecord = true)
        {
            try
            {
                Odpoklic recall = new Odpoklic();
                recall.CenaPrevoza = model.CenaPrevoza;
                recall.DobaviteljID = model.DobaviteljID <= 0 ? (int?)null : model.DobaviteljID;
                recall.KolicinaSkupno = model.KolicinaSkupno;
                recall.OdpoklicID = model.OdpoklicID;
                recall.RelacijaID = model.RelacijaID <= 0 ? (int?)null : model.RelacijaID;
                recall.StatusID = model.StatusID;
                recall.ts = model.ts.CompareTo(DateTime.MinValue) == 0 ? (DateTime?)null : model.ts;
                recall.tsIDOseba = model.tsIDOseba;
                recall.Opis = model.Opis;
                recall.PaleteSkupaj = model.PaleteSkupaj;
                recall.SoferNaziv = model.SoferNaziv;
                recall.Registracija = model.Registracija;
                recall.OdobritevKomentar = model.OdobritevKomentar;
                recall.RazpisPozicijaID = model.RazpisPozicijaID;
                recall.OdpoklicStevilka = model.OdpoklicStevilka;
                recall.UserID = model.UserID;
                recall.DatumNaklada = model.DatumNaklada;
                recall.DatumNaklada = model.DatumNaklada > (DateTime.MinValue) ? model.DatumNaklada : (DateTime?)null;
                recall.DatumRazklada = model.DatumRazklada;
                recall.DatumRazklada = model.DatumRazklada > (DateTime.MinValue) ? model.DatumRazklada : (DateTime?)null;

                recall.DobaviteljNaziv = model.DobaviteljNaziv;
                recall.DobaviteljNaslov = model.DobaviteljNaslov;
                recall.DobaviteljPosta = model.DobaviteljPosta;
                recall.DobaviteljKraj = model.DobaviteljKraj;
                recall.DobaviteljUrediTransport = model.DobaviteljUrediTransport;
                recall.RazlogOdobritveSistem = model.RazlogOdobritveSistem;
                recall.LastenPrevoz = model.LastenPrevoz;
                recall.TipPrevoza = model.TipPrevozaID <= 0 ? (int?)null : model.TipPrevozaID;
                recall.LastnoSkladisceID = model.LastnoSkladisceID <= 0 ? (int?)null : model.LastnoSkladisceID;
                recall.Prevozniki = model.Prevozniki;
                recall.KupecUrediTransport = model.KupecUrediTransport;
                recall.PovprasevanjePoslanoPrevoznikom = model.PovprasevanjePoslanoPrevoznikom;
                recall.PrevoznikOddalNajnizjoCeno = model.PrevoznikOddalNajnizjoCeno;
                recall.OpombaZaPovprasevnjePrevoznikom = model.OpombaZaPovprasevnjePrevoznikom;



                // Save for Create Order procedure info
                if (model.P_CreateOrder.Year > 2000) recall.P_CreateOrder = model.P_CreateOrder;
                if (model.P_LastTSCreatePDFPantheon.Year > 2000 ) recall.P_LastTSCreatePDFPantheon = model.P_LastTSCreatePDFPantheon;
                recall.P_TransportOrderPDFDocPath = model.P_TransportOrderPDFDocPath;
                recall.P_TransportOrderPDFName = model.P_TransportOrderPDFName;
                recall.P_UnsuccCountCreatePDFPantheon = model.P_UnsuccCountCreatePDFPantheon;
                recall.P_SendWarningToAmin = model.P_SendWarningToAdmin;

                if (recall.OdpoklicID == 0)
                {
                    recall.ts = DateTime.Now;
                    int NextStevilka =  GetNextOdpoklicStevilka();
                    recall.OdpoklicStevilka = NextStevilka;
                    model.OdpoklicStevilka = NextStevilka;
                    context.Odpoklic.Add(recall);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        Odpoklic original = context.Odpoklic.Where(o => o.OdpoklicID == recall.OdpoklicID).FirstOrDefault();
                        string statusWork = Enums.StatusOfRecall.DELOVNA.ToString();
                        int idStatus = context.StatusOdpoklica.Where(so => so.Koda == statusWork).FirstOrDefault().StatusOdpoklicaID;
                        //Če ponovno odpremo odpoklic je potrebno pobristi vrstice iz LastneZaloge

                        if (original.StatusOdpoklica.Koda == Enums.StatusOfRecall.POTRJEN.ToString() && recall.StatusID == idStatus && (recall.LastnoSkladisceID.HasValue && recall.LastnoSkladisceID.Value > 0))
                        {
                            var originalRecall = context.LastnaZaloga.Where(lz => lz.OdpoklicID == recall.OdpoklicID).ToList();

                            foreach (var item in originalRecall)
                            {
                                context.LastnaZaloga.Remove(item);
                            }
                        }

                        context.Entry(original).CurrentValues.SetValues(recall);
                        context.SaveChanges();
                    }
                }

                if (model.OdpoklicPozicija != null && model.OdpoklicPozicija.Count > 0)
                {
                    int ownWarehouseID = 0;
                    bool? savePos = recall.TipPrevoza.HasValue ? context.TipPrevoza.Where(tp => tp.TipPrevozaID == recall.TipPrevoza).FirstOrDefault().ShranjevanjePozicij : false;

                    if (model.LastnoSkladisceID > 0 && (savePos.HasValue && savePos.Value))
                        ownWarehouseID = recall.LastnoSkladisceID.Value;

                    SaveRecallPosition(model.OdpoklicPozicija, recall.OdpoklicID, ownWarehouseID);
                }

                //todo: save to lastna zaloga

                return recall.OdpoklicID;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public bool DeleteRecall(int recallID)
        {
            try
            {
                var recall = context.Odpoklic.Where(o => o.OdpoklicID == recallID).FirstOrDefault();

                if (recall != null)
                {
                    // check if there is any PrijavaPrevoznika
                    var lsPrijavaPrevoznika = GetCarriersInquiry(recall.OdpoklicID);
                    // delete all data in PrijavaPrevoznika
                    if (lsPrijavaPrevoznika.Count > 0)
                    {
                        foreach (CarrierInquiryModel pp  in lsPrijavaPrevoznika)
                        {
                            DeleteCarrierInquiry(pp.PrijavaPrevoznikaID);
                        }
                    }

                    context.Odpoklic.Remove(recall);
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


        public int SaveRecallPosition(RecallPositionModel model, bool updateRecord = true)
        {
            try
            {
                OdpoklicPozicija recallPos = new OdpoklicPozicija();
                recallPos.Kolicina = model.Kolicina;
                recallPos.KolicinaIzNarocila = model.KolicinaIzNarocila;
                recallPos.Material = model.Material;
                recallPos.NarociloID = model.NarociloID;
                recallPos.NarociloPozicijaID = model.NarociloPozicijaID;
                recallPos.OdpoklicID = model.OdpoklicID;
                recallPos.OdpoklicPozicijaID = model.OdpoklicPozicijaID;
                recallPos.StatusKolicine = model.StatusKolicine;
                recallPos.ts = model.ts.CompareTo(DateTime.MinValue) == 0 ? (DateTime?)null : model.ts;
                recallPos.tsIDOseba = model.tsIDOseba;
                recallPos.OC = model.OC;
                recallPos.KolicinaPrevzeta = model.KolicinaPrevzeta <= 0 ? (decimal?)null : model.KolicinaPrevzeta;
                recallPos.KolicinaRazlika = model.KolicinaRazlika <= 0 ? (decimal?)null : model.KolicinaRazlika;
                recallPos.Palete = model.Palete <= 0 ? (decimal?)null : model.Palete;
                recallPos.TipID = model.TipID;
                recallPos.KupecNaziv = model.KupecNaziv;
                recallPos.KupecViden = model.KupecViden;
                recallPos.OptimalnaZaloga = model.OptimalnaZaloga;
                recallPos.TrenutnaZaloga = model.TrenutnaZaloga;
                recallPos.Interno = model.Interno;
                recallPos.Proizvedeno = model.Proizvedeno;
                recallPos.MaterialIdent = model.MaterialIdent;
                recallPos.DatumVnosa = model.DatumVnosa.CompareTo(DateTime.MinValue) == 0 ? (DateTime?)null : model.DatumVnosa;
                recallPos.KolicinaOTP = model.KolicinaOTP <= 0 ? (decimal?)null : model.KolicinaOTP;
                recallPos.StatusPrevzeto = model.StatusPrevzeto;
                recallPos.ZaporednaStevilka = model.ZaporednaStevilka;
                recallPos.KolicinaOTPPozicijaNarocilnice = model.KolicinaOTPPozicijaNarocilnice;
                recallPos.KupecNaslov = model.KupecNaslov;
                recallPos.KupecKraj = model.KupecKraj;
                recallPos.KupecPosta = model.KupecPosta;
                recallPos.OdpoklicIzLastneZaloge = model.OdpoklicIzLastneZaloge;
                recallPos.PrvotniOdpoklicPozicijaID = model.PrvotniOdpoklicPozicijaID <= 0 ? (int?)null : model.PrvotniOdpoklicPozicijaID;
                recallPos.Split = model.Split;
                recallPos.TransportnaKolicina = model.TransportnaKolicina <= 0 ? (decimal?)null : model.TransportnaKolicina;
                recallPos.EnotaMere = model.EnotaMere;

                if (recallPos.OdpoklicPozicijaID == 0)
                {
                    recallPos.KolicinaOTP = GetSumRecallPosQuantity(recallPos.MaterialIdent);
                    recallPos.ts = DateTime.Now;
                    recallPos.DatumVnosa = DateTime.Now;
                    context.OdpoklicPozicija.Add(recallPos);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        OdpoklicPozicija original = context.OdpoklicPozicija.Where(op => op.OdpoklicPozicijaID == recallPos.OdpoklicPozicijaID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(recallPos);
                        context.SaveChanges();
                    }
                }
                return recallPos.OdpoklicPozicijaID;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }


        public void SaveRecallPosition(List<RecallPositionModel> model, int recallID = 0, int ownStockWarehouseID = 0)
        {
            try
            {
                OdpoklicPozicija recallPos = null;

                bool isRecallConfirmed = HasRecallStatus(recallID, Enums.StatusOfRecall.POTRJEN.ToString());
                bool isRecallVOdobritvi = HasRecallStatus(recallID, Enums.StatusOfRecall.V_ODOBRITEV.ToString());

                foreach (var item in model)
                {
                    recallPos = new OdpoklicPozicija();
                    recallPos.Kolicina = item.Kolicina;
                    recallPos.StPalet = item.StPalet;
                    recallPos.KolicinaIzNarocila = item.KolicinaIzNarocila;
                    recallPos.Material = item.Material;
                    recallPos.NarociloID = item.NarociloID;
                    recallPos.NarociloPozicijaID = item.NarociloPozicijaID;
                    recallPos.OdpoklicID = recallID > 0 ? recallID : item.OdpoklicID;
                    recallPos.OdpoklicPozicijaID = item.OdpoklicPozicijaID;
                    recallPos.StatusKolicine = item.StatusKolicine;
                    recallPos.ts = item.ts.CompareTo(DateTime.MinValue) == 0 ? (DateTime?)null : item.ts;
                    recallPos.tsIDOseba = item.tsIDOseba;

                    recallPos.OC = item.OC;
                    recallPos.KolicinaPrevzeta = item.KolicinaPrevzeta <= 0 ? (decimal?)null : item.KolicinaPrevzeta;
                    recallPos.KolicinaRazlika = item.KolicinaRazlika <= 0 ? (decimal?)null : item.KolicinaRazlika;
                    recallPos.Palete = item.Palete <= 0 ? (decimal?)null : item.Palete;
                    recallPos.TipID = item.TipID;
                    recallPos.KupecNaziv = item.KupecNaziv;
                    recallPos.KupecViden = item.KupecViden;
                    recallPos.OptimalnaZaloga = item.OptimalnaZaloga;
                    recallPos.TrenutnaZaloga = item.TrenutnaZaloga;
                    recallPos.TipNaziv = item.TipNaziv;
                    recallPos.Interno = item.Interno;
                    recallPos.Proizvedeno = item.Proizvedeno;
                    recallPos.MaterialIdent = item.MaterialIdent;
                    recallPos.DatumVnosa = item.DatumVnosa.CompareTo(DateTime.MinValue) == 0 ? (DateTime?)null : item.DatumVnosa;
                    //recallPos.KolicinaOTP = item.KolicinaOTP <= 0 ? (decimal?)null : item.KolicinaOTP;
                    recallPos.StatusPrevzeto = item.StatusPrevzeto;
                    recallPos.ZaporednaStevilka = item.ZaporednaStevilka;
                    recallPos.KupecNaslov = item.KupecNaslov;
                    recallPos.KupecKraj = item.KupecKraj;
                    recallPos.KupecPosta = item.KupecPosta;
                    recallPos.OdpoklicIzLastneZaloge = item.OdpoklicIzLastneZaloge;
                    recallPos.PrvotniOdpoklicPozicijaID = item.PrvotniOdpoklicPozicijaID <= 0 ? (int?)null : item.PrvotniOdpoklicPozicijaID;
                    recallPos.Split = item.Split;
                    recallPos.TransportnaKolicina = item.TransportnaKolicina <= 0 ? (decimal?)null : item.TransportnaKolicina;
                    recallPos.EnotaMere = item.EnotaMere;

                    //if (recallPos.TipID <= 0)
                    //  throw new Exception(" Izberi tip prevoza na poziciji odpoklica");

                    //ko bo status v delovni verziji je potrebno ob vsakem odpiranju odpoklica preračunati KolicinoOTP in KolicinoOTPPozicijaNarocilnice 
                    if (isRecallConfirmed || isRecallVOdobritvi)
                    {
                        recallPos.KolicinaOTPPozicijaNarocilnice = GetSumRecallPosQntyByOrderAndOrderPosition(item);
                        recallPos.KolicinaOTP = GetSumRecallPosQuantity(recallPos.MaterialIdent, model) - recallPos.KolicinaOTPPozicijaNarocilnice;//kolicinaOTP ne sme vsebovati odpoklicane količine iz trenutne pozicije. Zato ker jo shranimo v polje KolicinaOTPPozicijaNarocilnice
                    }

                    if (recallPos.OdpoklicPozicijaID == 0)
                    {
                        recallPos.ts = DateTime.Now;
                        recallPos.DatumVnosa = DateTime.Now;
                        context.OdpoklicPozicija.Add(recallPos);
                    }
                    else
                    {
                        OdpoklicPozicija original = context.OdpoklicPozicija.Where(op => op.OdpoklicPozicijaID == recallPos.OdpoklicPozicijaID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(recallPos);
                    }

                    //V lastno zalogo prenašmo pozicije samo takrat kadar je izbrano skladišče in da je odpoklic v statusu Potrjen.
                    if (ownStockWarehouseID > 0 && isRecallConfirmed && recallPos.OdpoklicPozicijaID == 0)
                    {
                        LastnaZaloga ownStock = new LastnaZaloga();
                        PropertyCopyHelper<OdpoklicPozicija, LastnaZaloga>.Copy(recallPos, ownStock);

                        ownStock.idLastnaZaloga = 0;
                        ownStock.LastnoSkladisceID = ownStockWarehouseID;
                        context.LastnaZaloga.Add(ownStock);

                    }

                    //Če smo naredili nov odpoklic iz delno prevzetih odpoklicev
                    //če je odpoklic potrejen je potrebno odpoklice iz katerih smo prenesli še ne prevzete pozicije zapreti (nastavi status PONOVNI_ODPOKLIC)
                    if (isRecallConfirmed && (recallPos.PrvotniOdpoklicPozicijaID.HasValue && recallPos.PrvotniOdpoklicPozicijaID.Value > 0))
                    {
                        //poiščemo pozicijo odpoklica in preverimo če ima odpoklic status delno prevzet. Če ga ima potem spremenimo status na PONOVNI_ODPOKLIC
                        var originalRecallPos = context.OdpoklicPozicija.Where(op => op.OdpoklicPozicijaID == recallPos.PrvotniOdpoklicPozicijaID.Value).FirstOrDefault();
                        if (originalRecallPos != null && originalRecallPos.Odpoklic.StatusOdpoklica.Koda == Enums.StatusOfRecall.DELNO_PREVZET.ToString())
                        {
                            originalRecallPos.Odpoklic.StatusID = GetRecallStatusByCode(Enums.StatusOfRecall.PONOVNI_ODPOKLIC.ToString()).StatusOdpoklicaID;
                            context.Entry(originalRecallPos).CurrentValues.SetValues(originalRecallPos);
                        }
                    }
                }
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }


        public bool DeleteRecallPosition(int recallPosID)
        {
            try
            {
                var recallPos = context.OdpoklicPozicija.Where(rp => rp.OdpoklicPozicijaID == recallPosID).FirstOrDefault();

                if (recallPos != null)
                {
                    context.OdpoklicPozicija.Remove(recallPos);
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

        public RecallPositionModel GetRecallPositionByID(int recallPositionID)
        {
            try
            {
                var query = from position in context.OdpoklicPozicija
                            where position.OdpoklicPozicijaID == recallPositionID
                            select new RecallPositionModel
                            {
                                Kolicina = position.Kolicina,
                                KolicinaIzNarocila = position.KolicinaIzNarocila,
                                Material = position.Material,
                                NarociloID = position.NarociloID,
                                NarociloPozicijaID = position.NarociloPozicijaID,
                                OdpoklicID = position.OdpoklicID,
                                OdpoklicPozicijaID = position.OdpoklicPozicijaID,
                                StatusKolicine = position.StatusKolicine,
                                ts = position.ts.HasValue ? position.ts.Value : DateTime.MinValue,
                                tsIDOseba = position.tsIDOseba.HasValue ? position.tsIDOseba.Value : 0,
                                OptimalnaZaloga = position.OptimalnaZaloga.HasValue ? position.OptimalnaZaloga.Value : 0,
                                TrenutnaZaloga = position.TrenutnaZaloga.HasValue ? position.TrenutnaZaloga.Value : 0,
                                Interno = position.Interno,
                                Proizvedeno = position.Proizvedeno.HasValue ? position.Proizvedeno.Value : 0,
                                MaterialIdent = position.MaterialIdent,
                                DatumVnosa = position.DatumVnosa.HasValue ? position.DatumVnosa.Value : DateTime.MinValue,
                                KolicinaOTP = position.KolicinaOTP.HasValue ? position.KolicinaOTP.Value : 0,
                                StatusPrevzeto = position.StatusPrevzeto.HasValue ? position.StatusPrevzeto.Value : false,
                                ZaporednaStevilka = position.ZaporednaStevilka.HasValue ? position.ZaporednaStevilka.Value : 0,
                                KolicinaOTPPozicijaNarocilnice = position.KolicinaOTPPozicijaNarocilnice.HasValue ? position.KolicinaOTPPozicijaNarocilnice.Value : 0,
                                KupecNaslov = position.KupecNaslov,
                                KupecKraj = position.KupecKraj,
                                KupecPosta = position.KupecPosta,
                                OdpoklicIzLastneZaloge = position.OdpoklicIzLastneZaloge.HasValue ? position.OdpoklicIzLastneZaloge.Value : false,
                                PrvotniOdpoklicPozicijaID = position.PrvotniOdpoklicPozicijaID.HasValue ? position.PrvotniOdpoklicPozicijaID.Value : 0,
                                Split = position.Split.HasValue ? position.Split.Value : false,
                                EnotaMere = position.EnotaMere,
                                TransportnaKolicina = position.TransportnaKolicina.HasValue ? position.TransportnaKolicina.Value : 0
                            };

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }


        public RecallType GetRecallTypeByID(int typeId)
        {
            try
            {
                var query = from type in context.TipOdpoklica
                            where type.TipOdpoklicaID == typeId
                            select new RecallType
                            {
                                Koda = type.Koda,
                                Naziv = type.Naziv,
                                Opis = type.Opis,
                                TipOdpoklicaID = type.TipOdpoklicaID,
                                ts = type.ts.HasValue ? type.ts.Value : DateTime.MinValue,
                                tsIDOseba = type.tsIDOseba.HasValue ? type.tsIDOseba.Value : 0
                            };

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public RecallType GetRecallTypeByCode(string typeCode)
        {
            try
            {
                var query = from type in context.TipOdpoklica
                            where type.Koda == typeCode
                            select new RecallType
                            {
                                Koda = type.Koda,
                                Naziv = type.Naziv,
                                Opis = type.Opis,
                                TipOdpoklicaID = type.TipOdpoklicaID,
                                ts = type.ts.HasValue ? type.ts.Value : DateTime.MinValue,
                                tsIDOseba = type.tsIDOseba.HasValue ? type.tsIDOseba.Value : 0
                            };

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<RecallType> GetRecallTypes()
        {
            try
            {
                var query = from type in context.TipOdpoklica
                            select new RecallType
                            {
                                Koda = type.Koda,
                                Naziv = type.Naziv,
                                Opis = type.Opis,
                                TipOdpoklicaID = type.TipOdpoklicaID,
                                ts = type.ts.HasValue ? type.ts.Value : DateTime.MinValue,
                                tsIDOseba = type.tsIDOseba.HasValue ? type.tsIDOseba.Value : 0
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }


        public List<RecallStatus> GetRecallStatuses()
        {
            try
            {
                var query = from type in context.StatusOdpoklica
                            select new RecallStatus
                            {
                                Koda = type.Koda,
                                Naziv = type.Naziv,
                                Opis = type.Opis,
                                StatusOdpoklicaID = type.StatusOdpoklicaID,
                                ts = type.ts.HasValue ? type.ts.Value : DateTime.MinValue,
                                tsIDOseba = type.tsIDOseba.HasValue ? type.tsIDOseba.Value : 0
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public RecallStatus GetRecallStatusByID(int statusID)
        {
            try
            {
                var query = from type in context.StatusOdpoklica
                            where type.StatusOdpoklicaID == statusID
                            select new RecallStatus
                            {
                                Koda = type.Koda,
                                Naziv = type.Naziv,
                                Opis = type.Opis,
                                StatusOdpoklicaID = type.StatusOdpoklicaID,
                                ts = type.ts.HasValue ? type.ts.Value : DateTime.MinValue,
                                tsIDOseba = type.tsIDOseba.HasValue ? type.tsIDOseba.Value : 0
                            };

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public RecallStatus GetRecallStatusByCode(string statusCode)
        {
            try
            {
                var query = from type in context.StatusOdpoklica
                            where type.Koda == statusCode
                            select new RecallStatus
                            {
                                Koda = type.Koda,
                                Naziv = type.Naziv,
                                Opis = type.Opis,
                                StatusOdpoklicaID = type.StatusOdpoklicaID,
                                ts = type.ts.HasValue ? type.ts.Value : DateTime.MinValue,
                                tsIDOseba = type.tsIDOseba.HasValue ? type.tsIDOseba.Value : 0
                            };

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        /// <summary>
        /// Pridobimo vsoto količine za isti material (KolicinaOTP) -
        /// </summary>
        /// <param name="materialIdent"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private decimal GetSumRecallPosQuantity(string materialIdent, List<RecallPositionModel> model = null)
        {
            try
            {
                string confirmRecall = Enums.StatusOfRecall.POTRJEN.ToString();

                if (model != null)
                {
                    //pridobimo odpoklicano količino iz trenutnih pozicij ki jih želimo shraniti - če imamo iste artikle na odpoklicu
                    decimal recallQnty = model.Where(op => op.MaterialIdent == materialIdent).Sum(op => op.Kolicina);
                    decimal previousRecallQnty = 0;

                    //poiščemo vse pozicije odpoklica, ki imajo isti materialIdent in niso prevzete ter da je status odpoklica potrjen
                    List<OdpoklicPozicija> pos = context.OdpoklicPozicija.Where(op => op.MaterialIdent == materialIdent && !op.StatusPrevzeto.Value && op.Odpoklic.StatusOdpoklica.Koda == confirmRecall).ToList();

                    if (pos != null)
                    {
                        //pobrišemo vse pozicije odpoklica ki smo jih že sešteli zgoraj pri recallQnty (ko potrjujemo odpoklic - ko že imamo pozicijo shranjeno v bazi)
                        foreach (var item in model)
                        {
                            OdpoklicPozicija temp = pos.Where(rPos => rPos.OdpoklicPozicijaID == item.OdpoklicPozicijaID).FirstOrDefault();
                            if (temp != null)
                                pos.Remove(temp);
                        }

                        previousRecallQnty = pos.Sum(op => op.Kolicina);
                    }

                    return (/*recallQnty + */previousRecallQnty);
                }
                else
                    return context.OdpoklicPozicija.Where(op => op.MaterialIdent == materialIdent && !op.StatusPrevzeto.Value && op.Odpoklic.StatusOdpoklica.Koda == confirmRecall).Sum(op => op.Kolicina);
            }
            catch (Exception ex)
            {
                throw new Exception("GetSumRecallPosQuantity Method Error! ", ex);
            }
        }

        /// <summary>
        /// Pridobimo vsoto količine za isti material, na isti naročilnici z isto pozicijo naročilnice. V enem odpoklicu je lahko samo en takšen zapis (ne more biti več istih pozicij iz istega naročila na enem odpoklicu)
        /// </summary>
        /// <returns></returns>
        private decimal GetSumRecallPosQntyByOrderAndOrderPosition(RecallPositionModel model)
        {
            try
            {
                string kodaPotrjen = Enums.StatusOfRecall.POTRJEN.ToString();
                int statusPotrjen = context.StatusOdpoklica.Where(so => so.Koda == kodaPotrjen).FirstOrDefault().StatusOdpoklicaID;

                string kodaDelnoPrevzet = Enums.StatusOfRecall.DELNO_PREVZET.ToString();
                int statusDelnoPrevzet = context.StatusOdpoklica.Where(so => so.Koda == kodaDelnoPrevzet).FirstOrDefault().StatusOdpoklicaID;

                if (model != null)
                {
                    //pridobimo vse pozicije odpoklicev ki so si enaki v identu, narocilnici in narocilnici pozicije ter da niso prevzeti
                    List<OdpoklicPozicija> posByOrderPosNum = context.OdpoklicPozicija.Where(op => op.MaterialIdent == model.MaterialIdent &&
                       op.NarociloID == model.NarociloID &&
                       op.NarociloPozicijaID == model.NarociloPozicijaID &&
                       !op.StatusPrevzeto.Value &&
                       (op.Odpoklic.StatusID == statusPotrjen || op.Odpoklic.StatusID == statusDelnoPrevzet) &&
                       op.OdpoklicPozicijaID != model.OdpoklicPozicijaID).ToList();

                    //seštejemo odpoklicano količino za pozicijo naročilnice in še prištejemo trenutno odpoklicano količino
                    return posByOrderPosNum.Sum(op => op.Kolicina);// +model.Kolicina;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception("GetSumRecallPosQntyByOrderAndOrderPosition Method Error! ", ex);
            }
        }

        public decimal GetLatestKolicinaOTPForProduct(string materialIdent)
        {
            try
            {
                string confirmRecall = Enums.StatusOfRecall.POTRJEN.ToString();
                decimal kolicinaOTP = 0;
                OdpoklicPozicija position = context.OdpoklicPozicija.Where(op => op.MaterialIdent == materialIdent && !op.StatusPrevzeto.Value && op.Odpoklic.StatusOdpoklica.Koda == confirmRecall).OrderByDescending(op => op.DatumVnosa).FirstOrDefault();
                if (position != null)
                    kolicinaOTP = position.KolicinaOTP.HasValue ? position.KolicinaOTP.Value : 0;
                return kolicinaOTP;
            }
            catch (Exception ex)
            {
                throw new Exception("GetLatestKolicinaOTPForProduct Method Error! ", ex);
            }
        }
        public List<MaterialModel> GetLatestKolicinaOTPForProduct(List<MaterialModel> model)
        {
            try
            {
                string confirmRecall = Enums.StatusOfRecall.POTRJEN.ToString();
                foreach (var item in model)
                {
                    OdpoklicPozicija position = context.OdpoklicPozicija.Where(op => op.MaterialIdent == item.Ident && !op.StatusPrevzeto.Value && op.Odpoklic.StatusOdpoklica.Koda == confirmRecall).OrderByDescending(op => op.DatumVnosa).FirstOrDefault();
                    if (position != null)
                        item.KolicinaOTP = position.KolicinaOTP.HasValue ? position.KolicinaOTP.Value : 0;
                }

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception("GetLatestKolicinaOTPForProduct Method Error! ", ex);
            }
        }

        private int GetNextOdpoklicStevilka()
        {
            try
            {
                int? maxNum = context.Odpoklic.Max(o => o.OdpoklicStevilka);

                if (maxNum.HasValue)
                    return maxNum.Value + 1;
                else
                    return 1;
            }
            catch (Exception ex)
            {
                throw new Exception("GetNextOdpoklicStevilka Method Error! ", ex);
            }
        }

        private bool HasRecallStatus(int recallID, string statusCode)
        {
            var obj = context.Odpoklic.Where(o => o.OdpoklicID == recallID).FirstOrDefault();

            if (obj != null)
            {
                string kodaStatus = context.StatusOdpoklica.Where(so => so.StatusOdpoklicaID == obj.StatusID).FirstOrDefault().Koda;
                return (kodaStatus == statusCode);
            }
            else
            {
                return false;
            }
        }

        public List<RecallPositionModel> GetRecallPosFromPartialOverTakeRecalls(List<int> recallIDs)
        {
            try
            {
                List<RecallPositionModel> returnList = new List<RecallPositionModel>();
                foreach (var item in recallIDs)
                {
                    var query = from rPos in context.OdpoklicPozicija
                                where rPos.OdpoklicID == item && !rPos.StatusPrevzeto.Value
                                select new RecallPositionModel
                                {
                                    Kolicina = rPos.Kolicina,
                                    KolicinaIzNarocila = rPos.KolicinaIzNarocila,
                                    Material = rPos.Material,
                                    NarociloID = rPos.NarociloID,
                                    NarociloPozicijaID = rPos.NarociloPozicijaID,
                                    OdpoklicID = rPos.OdpoklicID,
                                    OdpoklicPozicijaID = rPos.OdpoklicPozicijaID,
                                    StatusKolicine = rPos.StatusKolicine,
                                    ts = rPos.ts.HasValue ? rPos.ts.Value : DateTime.MinValue,
                                    tsIDOseba = rPos.tsIDOseba.HasValue ? rPos.tsIDOseba.Value : 0,
                                    OptimalnaZaloga = rPos.OptimalnaZaloga.HasValue ? rPos.OptimalnaZaloga.Value : 0,
                                    TrenutnaZaloga = rPos.TrenutnaZaloga.HasValue ? rPos.TrenutnaZaloga.Value : 0,
                                    Interno = rPos.Interno,
                                    Proizvedeno = rPos.Proizvedeno.HasValue ? rPos.Proizvedeno.Value : 0,
                                    MaterialIdent = rPos.MaterialIdent,
                                    DatumVnosa = rPos.DatumVnosa.HasValue ? rPos.DatumVnosa.Value : DateTime.MinValue,
                                    KolicinaOTP = rPos.KolicinaOTP.HasValue ? rPos.KolicinaOTP.Value : 0,
                                    StatusPrevzeto = rPos.StatusPrevzeto.HasValue ? rPos.StatusPrevzeto.Value : false,
                                    ZaporednaStevilka = rPos.ZaporednaStevilka.HasValue ? rPos.ZaporednaStevilka.Value : 0,
                                    KolicinaOTPPozicijaNarocilnice = rPos.KolicinaOTPPozicijaNarocilnice.HasValue ? rPos.KolicinaOTPPozicijaNarocilnice.Value : 0,
                                    KupecNaslov = rPos.KupecNaslov,
                                    KupecKraj = rPos.KupecKraj,
                                    KupecPosta = rPos.KupecPosta,
                                    OdpoklicIzLastneZaloge = rPos.OdpoklicIzLastneZaloge.HasValue ? rPos.OdpoklicIzLastneZaloge.Value : false,
                                    PrvotniOdpoklicPozicijaID = rPos.PrvotniOdpoklicPozicijaID.HasValue ? rPos.PrvotniOdpoklicPozicijaID.Value : 0,
                                    Split = rPos.Split.HasValue ? rPos.Split.Value : false,
                                    EnotaMere = rPos.EnotaMere,
                                    TransportnaKolicina = rPos.TransportnaKolicina.HasValue ? rPos.TransportnaKolicina.Value : 0
                                };

                    returnList.AddRange(query.ToList());
                }

                return returnList;
            }
            catch (Exception ex)
            {
                throw new Exception("Kreiranje odpoklicev iz delno prevzeti odpoklicev! ", ex);
            }
        }

        public bool ResetSequentialNumInRecallPos()
        {
            try
            {
                var query = from recallPos in context.OdpoklicPozicija
                            group recallPos by recallPos.OdpoklicID into gRecallPos
                            select gRecallPos;


                int sequentialNumber = 0;
                foreach (var item in query)
                {
                    sequentialNumber = 1;
                    IEnumerable<OdpoklicPozicija> pos = item.Select(group => group);
                    pos.OrderBy(p => p.DatumVnosa).ToList().ForEach(p => p.ZaporednaStevilka = sequentialNumber++);
                }
                context.SaveChanges();

                return true;

            }
            catch (Exception ex)
            {
                throw new Exception("Resetiranje ZaporednaStevilka na odpoklic pozicija", ex);
            }
        }

        public List<RecallModel> GetAllTakeOverRecalls()
        {
            try
            {
                string prevzet = Enums.StatusOfRecall.PREVZET.ToString();
                int statusID = context.StatusOdpoklica.Where(so => so.Koda == prevzet).FirstOrDefault().StatusOdpoklicaID;

                var query = from recall in context.Odpoklic
                            where recall.StatusID == statusID
                            select new RecallModel
                            {
                                CenaPrevoza = recall.CenaPrevoza.HasValue ? recall.CenaPrevoza.Value : 0,
                                DobaviteljID = recall.DobaviteljID.HasValue ? recall.DobaviteljID.Value : 0,
                                DobaviteljNaziv = recall.DobaviteljNaziv,
                                KolicinaSkupno = recall.KolicinaSkupno,
                                OdpoklicID = recall.OdpoklicID,
                                RelacijaID = recall.RelacijaID.HasValue ? recall.RelacijaID.Value : 0,
                                RelacijaNaziv = recall.Relacija != null ? recall.Relacija.Naziv : "",
                                StatusID = recall.StatusID,
                                StatusNaziv = recall.StatusOdpoklica != null ? recall.StatusOdpoklica.Naziv : "",
                                StatusKoda = recall.StatusOdpoklica != null ? recall.StatusOdpoklica.Koda : "",
                                ts = recall.ts.HasValue ? recall.ts.Value : DateTime.MinValue,
                                tsIDOseba = recall.tsIDOseba.HasValue ? recall.tsIDOseba.Value : 0,
                                OdpoklicStevilka = recall.OdpoklicStevilka.HasValue ? recall.OdpoklicStevilka.Value : 0,
                                DobaviteljUrediTransport = recall.DobaviteljUrediTransport.HasValue ? recall.DobaviteljUrediTransport.Value : false,
                                DobaviteljKraj = recall.DobaviteljKraj,
                                DobaviteljNaslov = recall.DobaviteljNaslov,
                                DobaviteljPosta = recall.DobaviteljPosta
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<RecallModel> GetAllNonTakeOverRecalls()
        {
            try
            {
                string prevzet = Enums.StatusOfRecall.PREVZET.ToString();
                int statusID = context.StatusOdpoklica.Where(so => so.Koda == prevzet).FirstOrDefault().StatusOdpoklicaID;

                var query = from recall in context.Odpoklic
                            where recall.StatusID != statusID
                            select new RecallModel
                            {
                                CenaPrevoza = recall.CenaPrevoza.HasValue ? recall.CenaPrevoza.Value : 0,
                                DobaviteljID = recall.DobaviteljID.HasValue ? recall.DobaviteljID.Value : 0,
                                DobaviteljNaziv = recall.DobaviteljNaziv,
                                KolicinaSkupno = recall.KolicinaSkupno,
                                OdpoklicID = recall.OdpoklicID,
                                RelacijaID = recall.RelacijaID.HasValue ? recall.RelacijaID.Value : 0,
                                RelacijaNaziv = recall.Relacija != null ? recall.Relacija.Naziv : "",
                                StatusID = recall.StatusID,
                                StatusNaziv = recall.StatusOdpoklica != null ? recall.StatusOdpoklica.Naziv : "",
                                StatusKoda = recall.StatusOdpoklica != null ? recall.StatusOdpoklica.Koda : "",
                                ts = recall.ts.HasValue ? recall.ts.Value : DateTime.MinValue,
                                tsIDOseba = recall.tsIDOseba.HasValue ? recall.tsIDOseba.Value : 0,
                                OdpoklicStevilka = recall.OdpoklicStevilka.HasValue ? recall.OdpoklicStevilka.Value : 0,
                                DobaviteljUrediTransport = recall.DobaviteljUrediTransport.HasValue ? recall.DobaviteljUrediTransport.Value : false,
                                DobaviteljKraj = recall.DobaviteljKraj,
                                DobaviteljNaslov = recall.DobaviteljNaslov,
                                DobaviteljPosta = recall.DobaviteljPosta,
                                P_TransportOrderPDFName = recall.P_TransportOrderPDFName != null ? recall.P_TransportOrderPDFName : "",
                                P_UnsuccCountCreatePDFPantheon = recall.P_UnsuccCountCreatePDFPantheon.HasValue ? recall.P_UnsuccCountCreatePDFPantheon.Value : 0,
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        #region Recalls for carriers to submit their prices based on our tender values

        private string IsRecallStillValid(Odpoklic recall)
        {
            try
            {
                if (recall != null)
                {
                    if (recall.StatusOdpoklica.Koda == Enums.StatusOfRecall.POTRJEN.ToString() && recall.PovprasevanjePoslanoPrevoznikom.Value)
                        return "Odpoklic je že potrjen!";
                    else if (DateTime.Now > recall.DatumNaklada)
                        return "Prijava cene na razpis je že potekla!";
                    else
                        return "";
                }
                else
                    return "Odpolkic ne obstaja več!";
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public string IsPriceSubmittingStillValid(int prijavaPrevoznikaID)
        {
            try
            {
                var prijavaPrevoznika = context.PrijavaPrevoznika.Where(pp => pp.PrijavaPrevoznikaID == prijavaPrevoznikaID).FirstOrDefault();
                if (prijavaPrevoznika != null)
                {
                    if (prijavaPrevoznika.Odpoklic.StatusOdpoklica.Koda == Enums.StatusOfRecall.POTRJEN.ToString() &&
                        prijavaPrevoznika.Odpoklic.PovprasevanjePoslanoPrevoznikom.Value)
                        return "Odpoklic je že potrjen!";
                    else if (DateTime.Now > prijavaPrevoznika.Odpoklic.DatumNaklada)
                        return "Prijava cene na razpis je že potekla!";
                    else
                        return "OK";
                }
                else
                    return "Odpolkic ne obstaja več!";
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public bool SubmitPriceToPrijavaPrevoznika(int prijavaPrevoznikaID, decimal newPrice)
        {
            try
            {
                var prijavaPrevoznika = context.PrijavaPrevoznika.Where(pp => pp.PrijavaPrevoznikaID == prijavaPrevoznikaID).FirstOrDefault();
                if (prijavaPrevoznika != null)
                {
                    if (!String.IsNullOrEmpty(IsRecallStillValid(prijavaPrevoznika.Odpoklic))) return false;//odpoklic ne sprejema več novih cen za prevoz

                    context.Entry(prijavaPrevoznika).Entity.PrijavljenaCena = newPrice;
                    context.Entry(prijavaPrevoznika).Entity.DatumPrijave = DateTime.Now;

                    context.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }

        }

        public List<CarrierInquiryModel> GetCarriersInquiry(int recallID)
        {
            try
            {
                var query = from pp in context.PrijavaPrevoznika
                            where pp.OdpoklicID == recallID
                            select new CarrierInquiryModel
                            {
                                PrijavaPrevoznikaID = pp.PrijavaPrevoznikaID,
                                OdpoklicID = pp.OdpoklicID,
                                PrevoznikID = pp.PrevoznikID,
                                PrvotnaCena = pp.PrvotnaCena,
                                PrijavljenaCena = pp.PrijavljenaCena.HasValue ? pp.PrijavljenaCena.Value : 0,
                                DatumNaklada = pp.DatumNaklada,                                
                                DatumPosiljanjePrijav = pp.DatumPosiljanjePrijav,
                                DatumPrijave = pp.DatumPrijave.HasValue ? pp.DatumPrijave.Value : DateTime.MinValue,
                                ts = pp.ts.HasValue ? pp.ts.Value : DateTime.MinValue,
                                Odpoklic = (from o in context.Odpoklic
                                            where o.OdpoklicID == pp.OdpoklicID
                                            select new RecallModel
                                            {
                                                CenaPrevoza = o.CenaPrevoza.HasValue ? o.CenaPrevoza.Value : 0,
                                                DobaviteljID = o.DobaviteljID.HasValue ? o.DobaviteljID.Value : 0,
                                                DobaviteljKraj = o.DobaviteljKraj,
                                                DobaviteljNaslov = o.DobaviteljNaslov,
                                                DobaviteljNaziv = o.DobaviteljNaziv,
                                                DobaviteljPosta = o.DobaviteljPosta,
                                                DobaviteljUrediTransport = o.DobaviteljUrediTransport.HasValue ? o.DobaviteljUrediTransport.Value : false,
                                                KolicinaSkupno = o.KolicinaSkupno,
                                                OdpoklicID = o.OdpoklicID,
                                                OdpoklicStevilka = o.OdpoklicStevilka.HasValue ? o.OdpoklicStevilka.Value : 0,
                                                RelacijaID = o.RelacijaID.HasValue ? o.RelacijaID.Value : 0,
                                                RelacijaNaziv = o.Relacija.Naziv,
                                                StatusID = o.StatusID,
                                                StatusKoda = o.StatusOdpoklica.Koda,
                                                StatusNaziv = o.StatusOdpoklica.Naziv,
                                                ts = o.ts.HasValue ? o.ts.Value : DateTime.MinValue,
                                                tsIDOseba = o.tsIDOseba.HasValue ? o.tsIDOseba.Value : 0

                                            }).FirstOrDefault(),
                                Prevoznik = (from s in context.Stranka_OTP
                                             where s.idStranka == pp.PrevoznikID
                                             select new CarrierModel
                                             {
                                                 idStranka = s.idStranka,
                                                 Email = s.Email,
                                                 KodaStranke = s.KodaStranke,
                                                 Naslov = s.Naslov,
                                                 NazivDrugi = s.NazivDrugi,
                                                 NazivPoste = s.NazivPoste,
                                                 NazivPrvi = s.NazivPrvi,
                                                 StevPoste = s.StevPoste,
                                                 Telefon = s.Telefon
                                             }).FirstOrDefault(),
                                OdstopanjeVEUR = (pp.PrijavljenaCena.HasValue ? (pp.PrvotnaCena - pp.PrijavljenaCena.Value) : 0)
                            };

                return query.OrderBy(i => i.PrijavljenaCena <= 0).ThenBy(i => i.PrijavljenaCena).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public void ManualSelectCarrierForTransport(int prijavaPrevoznikaID)
        {
            try
            {
                var item = context.PrijavaPrevoznika.Where(pp => pp.PrijavaPrevoznikaID == prijavaPrevoznikaID).FirstOrDefault();
                if (item != null)
                {
                    string potrjen = Enums.StatusOfRecall.POTRJEN.ToString();

                    context.Entry(item.Odpoklic).Entity.PrevoznikOddalNajnizjoCeno = true;
                    context.Entry(item.Odpoklic).Entity.StatusID = context.StatusOdpoklica.Where(so => so.Koda == potrjen).FirstOrDefault().StatusOdpoklicaID;
                    context.Entry(item.Odpoklic).Entity.Prevozniki = "";
                    context.Entry(item.Odpoklic).Entity.DobaviteljID = item.PrevoznikID;
                    context.Entry(item.Odpoklic).Entity.CenaPrevoza = item.PrijavljenaCena;

                    context.Entry(item.Odpoklic).Entity.DatumPosiljanjaMailLogistika = DateTime.Now;

                    List<RazpisPozicija> tenderPositions = new List<RazpisPozicija>();
                    var test = (from tenderPosition in context.RazpisPozicija
                                where tenderPosition.RelacijaID == item.Odpoklic.RelacijaID
                                group tenderPosition by tenderPosition.StrankaID into transportGroup
                                select transportGroup).ToList();

                    //poiščemo prevoznika ki je oddal najnižjo ceno v seznamu razpisov pozicij, ki so grupirane po id prevoznika
                    var carrierTenderPos = test.Where(rp => rp.Key == item.PrevoznikID).FirstOrDefault();
                    if (carrierTenderPos != null)
                    {
                        var list = carrierTenderPos.ToList();//dobimo seznam RazpisPozicij za posameznega prevoznika

                        //uredimo seznam po datumu razpisa (padajoče) in izberemo prvo pozicijo (RazpisPozicija)
                        var tenderPos = list.OrderByDescending(o => o.Razpis.DatumRazpisa).Where(o => o.Cena > 0).FirstOrDefault();

                        if (tenderPos == null)
                            tenderPos = list.OrderByDescending(o => o.Razpis.DatumRazpisa).FirstOrDefault();

                        if (tenderPos != null)
                            context.Entry(item.Odpoklic).Entity.RazpisPozicijaID = tenderPos.RazpisPozicijaID;
                    }

                    context.SaveChanges();

                    //poslati mail izbranemu prevozniku
                    messageEventRepo.CreateEmailCarrierSelectedOrNot(item.Odpoklic, item);

                    //poslati mail ostalim prevoznikom da niso bili izbrani, ki so oddali ceno (PrijavljenaCena > 0)
                    var collection = context.PrijavaPrevoznika.Where(pp => pp.OdpoklicID == item.OdpoklicID &&
                    pp.PrijavljenaCena > 0 &&
                    pp.PrijavaPrevoznikaID != item.PrijavaPrevoznikaID).ToList();

                    foreach (var obj in collection)
                    {
                        messageEventRepo.CreateEmailCarrierSelectedOrNot(item.Odpoklic, obj, false);
                    }

                    //poslati mail logistiki o izbranem prevozniku
                    messageEventRepo.CreateEmailLogisticsCarrierSelected(item.Odpoklic, item);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public bool DeleteCarrierInquiry(int carrierInquiryID)
        {
            try
            {
                var carrierInquiry = context.PrijavaPrevoznika.Where(pp => pp.PrijavaPrevoznikaID == carrierInquiryID).FirstOrDefault();

                if (carrierInquiry != null)
                {
                    context.PrijavaPrevoznika.Remove(carrierInquiry);
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

        /// <summary>
        /// procedura ki resetira status na orderju, da ga bo service še enkrat vzel in obdelal
        /// </summary>
        public void ResetRecallStatusByID(int iRecallID)
        {
            try
            {
                RecallFullModel rfm = GetRecallFullModelByID(iRecallID);
                if (rfm != null)
                {
                    RecallStatus stat = GetRecallStatusByCode(Enums.StatusOfInquiry.USTVARJENO_NAROCILO.ToString());
                    if (stat != null)
                    {
                        rfm.StatusID = stat.StatusOdpoklicaID;
                        rfm.P_UnsuccCountCreatePDFPantheon = 0;
                        rfm.P_SendWarningToAdmin = 0;
                        SaveRecall(rfm, true);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ResetRecallStatusByID Method Error! ", ex);
            }
        }
        #endregion
    }
}