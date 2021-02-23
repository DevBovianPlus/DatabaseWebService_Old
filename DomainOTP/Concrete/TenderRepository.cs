using DatabaseWebService.Common;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsOTP.Client;
using DatabaseWebService.ModelsOTP.Route;
using DatabaseWebService.ModelsOTP.Tender;
using DatabaseWebService.Resources;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.DomainOTP.Concrete
{
    public class TenderRepository : ITenderRepository
    {
        GrafolitOTPEntities context;

        public TenderRepository(GrafolitOTPEntities _context)
        {
            context = _context;
        }

        public List<TenderFullModel> GetTenderList(string dtFrom, string dtTo, string FilterString)
        {
            try
            {
                DateTime? dtF = DataTypesHelper.ParseDateTime(dtFrom);
                DateTime? dtT = DataTypesHelper.ParseDateTime(dtTo);

                List<TenderFullModel> model = null;
                if (FilterString != null && FilterString.Length > 0)
                {
                    var query = from tender in context.Razpis
                                where (tender.DatumRazpisa >= dtF && tender.DatumRazpisa <= dtT) && (tender.RazpisPozicija.Any(rp => rp.Relacija.Naziv.Contains(FilterString)))
                                select new TenderFullModel
                                {
                                    CenaSkupaj = tender.CenaSkupaj,
                                    DatumRazpisa = tender.DatumRazpisa,
                                    Naziv = tender.Naziv,
                                    RazpisID = tender.RazpisID,
                                    ts = tender.ts.HasValue ? tender.ts.Value : DateTime.MinValue,
                                    tsIDOseba = tender.tsIDOseba.HasValue ? tender.tsIDOseba.Value : 0,
                                    RazpisKreiran = tender.RazpisKreiran.HasValue ? tender.RazpisKreiran.Value : false,
                                    PotRazpisa = tender.PotRazpisa,
                                    PodatkiZaExcell_JSon = tender.PodatkiZaExcell_JSon,
                                    GeneriranTender = tender.GeneriranTender.HasValue ? tender.GeneriranTender.Value : false,
                                    IsCiljnaCena = tender.IsCiljnaCena.HasValue ? tender.IsCiljnaCena.Value : false,
                                    IsNajcenejsiPrevoznik = tender.IsNajcenejsiPrevoznik.HasValue ? tender.IsNajcenejsiPrevoznik.Value : false,
                                };
                    model = query.ToList();
                }
                else
                {
                    var query = from tender in context.Razpis
                                where (tender.DatumRazpisa >= dtF && tender.DatumRazpisa <= dtT)
                                select new TenderFullModel
                                {
                                    CenaSkupaj = tender.CenaSkupaj,
                                    DatumRazpisa = tender.DatumRazpisa,
                                    Naziv = tender.Naziv,
                                    RazpisID = tender.RazpisID,
                                    ts = tender.ts.HasValue ? tender.ts.Value : DateTime.MinValue,
                                    tsIDOseba = tender.tsIDOseba.HasValue ? tender.tsIDOseba.Value : 0,
                                    RazpisKreiran = tender.RazpisKreiran.HasValue ? tender.RazpisKreiran.Value : false,
                                    PotRazpisa = tender.PotRazpisa,
                                    PodatkiZaExcell_JSon = tender.PodatkiZaExcell_JSon,
                                    GeneriranTender = tender.GeneriranTender.HasValue ? tender.GeneriranTender.Value : false,
                                    IsCiljnaCena = tender.IsCiljnaCena.HasValue ? tender.IsCiljnaCena.Value : false,
                                    IsNajcenejsiPrevoznik = tender.IsNajcenejsiPrevoznik.HasValue ? tender.IsNajcenejsiPrevoznik.Value : false,
                                };
                    model = query.ToList();
                }

                //foreach (var item in model)
                //{
                //    item.RazpisPozicija = GetTenderPositionModelByID(item.RazpisID, dtF.Value, dtT.Value);
                //}

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<TenderPositionChangeModel> GetTenderListPositionChanges()
        {
            try
            {
                var query = from tenderPosChange in context.RazpisPozicijaSpremembe
                            select new TenderPositionChangeModel
                            {
                                StaraCena = tenderPosChange.StaraCena.HasValue ? tenderPosChange.StaraCena.Value : 0,
                                NovaCena = tenderPosChange.NovaCena.HasValue ? tenderPosChange.NovaCena.Value : 0,
                                IDVnosOseba = tenderPosChange.IDVnosOseba,
                                IDSpremembeOseba = tenderPosChange.IDSpremembeOseba,
                                RazpisID = tenderPosChange.RazpisID,
                                RazpisPozicijaSpremembeID = tenderPosChange.RazpisPozicijaSpremembeID,
                                Relacija = (from r in context.Relacija
                                            where r.RelacijaID == tenderPosChange.RelacijaID
                                            select new RouteModel
                                            {
                                                Datum = r.Datum.HasValue ? r.Datum.Value : DateTime.MinValue,
                                                Dolzina = r.Dolzina,
                                                Koda = r.Koda,
                                                Naziv = r.Naziv,
                                                RelacijaID = r.RelacijaID,
                                                ts = r.ts.HasValue ? r.ts.Value : DateTime.MinValue,
                                                tsIDOsebe = r.tsIDOsebe.HasValue ? r.tsIDOsebe.Value : 0,
                                            }).FirstOrDefault(),
                                RelacijaID = tenderPosChange.RelacijaID,
                                Stranka = (from client in context.Stranka_OTP
                                           where client.idStranka == tenderPosChange.StrankaID
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
                                StrankaID = tenderPosChange.StrankaID,
                                SpremembeTS = tenderPosChange.SpremembeTS.HasValue ? tenderPosChange.SpremembeTS.Value : DateTime.MinValue,
                                VnosTS = tenderPosChange.VnosTS.HasValue ? tenderPosChange.VnosTS.Value : DateTime.MinValue,
                                ZbirnikTon = (from zt in context.ZbirnikTon
                                              where zt.ZbirnikTonID == tenderPosChange.ZbirnikTonID
                                              select new TonsModel
                                              {
                                                  Koda = zt.Koda,
                                                  Naziv = zt.Naziv,
                                                  ts = zt.ts.HasValue ? zt.ts.Value : DateTime.MinValue,
                                              }).FirstOrDefault(),
                                //OsebaVnos = (from employee in clientEmployee
                                //                     group employee by employee.Osebe_NOZ into person
                                //                     select new EmployeeSimpleModel
                                //                     {

                                //                         DatumRojstva = person.Key.DatumRojstva.HasValue ? person.Key.DatumZaposlitve.Value : DateTime.MinValue,
                                //                         DatumZaposlitve = person.Key.DatumZaposlitve.HasValue ? person.Key.DatumZaposlitve.Value : DateTime.MinValue,
                                //                         DelovnoMesto = person.Key.DelovnoMesto,
                                //                         Email = person.Key.Email,
                                //                         Geslo = person.Key.Geslo,
                                //                         idOsebe = person.Key.OsebaID,
                                //                         Ime = person.Key.Ime,
                                //                         Naslov = person.Key.Naslov,
                                //                         Priimek = person.Key.Priimek,
                                //                         TelefonGSM = person.Key.TelefonGSM,
                                //                         ts = person.Key.ts.HasValue ? person.Key.ts.Value : DateTime.MinValue,
                                //                         tsIDOsebe = person.Key.tsIDOsebe.HasValue ? person.Key.tsIDOsebe.Value : 0,
                                //                         UporabniskoIme = person.Key.UporabniskoIme,
                                //                         Zunanji = person.Key.Zunanji.HasValue ? person.Key.Zunanji.Value : 0
                                //                     }).FirstOrDefault()
                            };

                List<TenderPositionChangeModel> modelPosChange = query.ToList();
                return modelPosChange;
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message);
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<TenderPositionModel> GetTenderListPositionByTenderID(int tenderID)
        {
            try
            {
                var query = from tender in context.RazpisPozicija
                            where tender.RazpisID == tenderID
                            select new TenderPositionModel
                            {
                                Cena = tender.Cena.HasValue ? tender.Cena.Value : 0,
                                IDOseba = tender.IDOseba.HasValue ? tender.IDOseba.Value : 0,
                                RazpisID = tender.RazpisID,
                                RazpisPozicijaID = tender.RazpisPozicijaID,
                                Relacija = (from r in context.Relacija
                                            where r.RelacijaID == tender.RelacijaID
                                            select new RouteModel
                                            {
                                                Datum = r.Datum.HasValue ? r.Datum.Value : DateTime.MinValue,
                                                Dolzina = r.Dolzina,
                                                Koda = r.Koda,
                                                Naziv = r.Naziv,
                                                RelacijaID = r.RelacijaID,
                                                ts = tender.ts.HasValue ? r.ts.Value : DateTime.MinValue,
                                                tsIDOsebe = r.tsIDOsebe.HasValue ? r.tsIDOsebe.Value : 0,
                                            }).FirstOrDefault(),
                                RelacijaID = tender.RelacijaID,
                                Stranka = (from client in context.Stranka_OTP
                                           where client.idStranka == tender.StrankaID
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
                                StrankaID = tender.StrankaID,
                                ZbirnikTonID = tender.ZbirnikTonID.HasValue ? tender.ZbirnikTonID.Value : 0,
                                ts = tender.ts.HasValue ? tender.ts.Value : DateTime.MinValue,
                                PotDokumenta = tender.PotDokumenta,
                                ZbirnikTon = (from zt in context.ZbirnikTon
                                              where zt.ZbirnikTonID == tender.ZbirnikTonID
                                              select new TonsModel
                                              {
                                                  Koda = zt.Koda,
                                                  Naziv = zt.Naziv,
                                                  ts = tender.ts.HasValue ? zt.ts.Value : DateTime.MinValue,
                                              }).FirstOrDefault(),
                            };

                List<TenderPositionModel> modelPos = query.ToList();
                return modelPos;
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message);
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<TenderPositionModel> GetTenderPositionModelByID(int tenderID, DateTime? dtFrom = null, DateTime? dtTo = null)
        {
            try
            {
                var query = from tender in context.RazpisPozicija
                            where tender.RazpisID == tenderID && (tender.ts >= dtFrom && tender.ts <= dtTo)
                            select new TenderPositionModel
                            {
                                Cena = tender.Cena.HasValue ? tender.Cena.Value : 0,
                                IDOseba = tender.IDOseba.HasValue ? tender.IDOseba.Value : 0,
                                RazpisID = tender.RazpisID,
                                RazpisPozicijaID = tender.RazpisPozicijaID,
                                Relacija = (from r in context.Relacija
                                            where r.RelacijaID == tender.RelacijaID
                                            select new RouteModel
                                            {
                                                Datum = r.Datum.HasValue ? r.Datum.Value : DateTime.MinValue,
                                                Dolzina = r.Dolzina,
                                                Koda = r.Koda,
                                                Naziv = r.Naziv,
                                                RelacijaID = r.RelacijaID,
                                                ts = tender.ts.HasValue ? r.ts.Value : DateTime.MinValue,
                                                tsIDOsebe = r.tsIDOsebe.HasValue ? r.tsIDOsebe.Value : 0,
                                            }).FirstOrDefault(),
                                RelacijaID = tender.RelacijaID,
                                Stranka = (from client in context.Stranka_OTP
                                           where client.idStranka == tender.StrankaID
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
                                StrankaID = tender.StrankaID,
                                ZbirnikTonID = tender.ZbirnikTonID.HasValue ? tender.ZbirnikTonID.Value : 0,
                                ts = tender.ts.HasValue ? tender.ts.Value : DateTime.MinValue,
                                PotDokumenta = tender.PotDokumenta,
                                ZbirnikTon = (from zt in context.ZbirnikTon
                                              where zt.ZbirnikTonID == tender.ZbirnikTonID
                                              select new TonsModel
                                              {
                                                  Koda = zt.Koda,
                                                  Naziv = zt.Naziv,
                                                  ts = tender.ts.HasValue ? zt.ts.Value : DateTime.MinValue,
                                              }).FirstOrDefault(),
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message);
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }


        public TenderFullModel GetTenderModelByID(int tenderID)
        {
            try
            {
                var query = from tender in context.Razpis
                            where tender.RazpisID == tenderID
                            select new TenderFullModel
                            {
                                CenaSkupaj = tender.CenaSkupaj,
                                CiljnaCena = tender.CiljnaCena.HasValue ? tender.CiljnaCena.Value : 0,
                                DatumRazpisa = tender.DatumRazpisa,
                                Naziv = tender.Naziv,
                                RazpisID = tender.RazpisID,
                                ts = tender.ts.HasValue ? tender.ts.Value : DateTime.MinValue,
                                tsIDOseba = tender.tsIDOseba.HasValue ? tender.tsIDOseba.Value : 0,
                                RazpisKreiran = tender.RazpisKreiran.HasValue ? tender.RazpisKreiran.Value : false,
                                PotRazpisa = tender.PotRazpisa,
                                PodatkiZaExcell_JSon = tender.PodatkiZaExcell_JSon,
                            };

                TenderFullModel model = query.FirstOrDefault();
                if (model != null)
                {
                    model.RazpisPozicija = GetTenderPositionModelByID(model.RazpisID);
                }

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public TenderModel GetTenderSimpleModelByID(int tenderID)
        {
            try
            {
                var query = from tender in context.Razpis
                            where tender.RazpisID == tenderID
                            select new TenderModel
                            {
                                CenaSkupaj = tender.CenaSkupaj,
                                CiljnaCena = tender.CiljnaCena.HasValue ? tender.CiljnaCena.Value : 0,
                                DatumRazpisa = tender.DatumRazpisa,
                                Naziv = tender.Naziv,
                                RazpisID = tender.RazpisID,
                                ts = tender.ts.HasValue ? tender.ts.Value : DateTime.MinValue,
                                tsIDOseba = tender.tsIDOseba.HasValue ? tender.tsIDOseba.Value : 0,
                                RazpisKreiran = tender.RazpisKreiran.HasValue ? tender.RazpisKreiran.Value : false,
                                PotRazpisa = tender.PotRazpisa,
                                PodatkiZaExcell_JSon = tender.PodatkiZaExcell_JSon,
                                GeneriranTender = tender.GeneriranTender.HasValue ? tender.GeneriranTender.Value : false,
                                IsCiljnaCena = tender.IsCiljnaCena.HasValue ? tender.IsCiljnaCena.Value : false,
                                IsNajcenejsiPrevoznik = tender.IsNajcenejsiPrevoznik.HasValue ? tender.IsNajcenejsiPrevoznik.Value : false,
                            };

                TenderModel model = query.FirstOrDefault();


                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }


        public int SaveTender(TenderFullModel model, bool updateRecord = true)
        {
            try
            {
                DataTypesHelper.LogThis("Start - SaveTender");

                Razpis tender = new Razpis();
                tender.CenaSkupaj = model.CenaSkupaj;
                tender.CiljnaCena = model.CiljnaCena;
                tender.DatumRazpisa = model.DatumRazpisa;
                tender.RazpisID = model.RazpisID;
                tender.Naziv = model.Naziv;
                tender.ts = model.ts.CompareTo(DateTime.MinValue) == 0 ? (DateTime?)null : model.ts;
                tender.tsIDOseba = model.tsIDOseba;
                tender.RazpisKreiran = model.RazpisKreiran;
                tender.PotRazpisa = model.PotRazpisa;
                tender.PodatkiZaExcell_JSon = model.PodatkiZaExcell_JSon;
                tender.GeneriranTender = model.GeneriranTender;
                tender.IsCiljnaCena = model.IsCiljnaCena;
                tender.IsNajcenejsiPrevoznik = model.IsNajcenejsiPrevoznik;

                if (tender.RazpisID == 0)
                {
                    tender.ts = DateTime.Now;
                    tender.RazpisKreiran = false;
                    context.Razpis.Add(tender);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        Razpis original = context.Razpis.Where(r => r.RazpisID == tender.RazpisID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(tender);
                        context.SaveChanges();
                    }
                }
                return tender.RazpisID;
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message);
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public bool DeleteTender(int tenderID)
        {
            try
            {
                var tender = context.Razpis.Where(r => r.RazpisID == tenderID).FirstOrDefault();

                if (tender != null)
                {
                    context.Razpis.Remove(tender);
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

        public bool DeleteTenderPosition(int tenderPosID)
        {
            try
            {
                var tenderPosition = context.RazpisPozicija.Where(r => r.RazpisPozicijaID == tenderPosID).FirstOrDefault();

                if (tenderPosition != null)
                {
                    context.RazpisPozicija.Remove(tenderPosition);
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

        public int SaveTenderPosition(TenderPositionModel model, bool updateRecord = true)
        {
            try
            {
                RazpisPozicija tenderPos = new RazpisPozicija();
                tenderPos.Cena = model.Cena;
                tenderPos.PotDokumenta = model.PotDokumenta;
                tenderPos.IDOseba = model.IDOseba;
                tenderPos.RazpisID = model.RazpisID;
                tenderPos.RazpisPozicijaID = model.RazpisPozicijaID;
                tenderPos.RelacijaID = model.RelacijaID;
                tenderPos.StrankaID = model.StrankaID;
                tenderPos.ts = model.ts.CompareTo(DateTime.MinValue) == 0 ? (DateTime?)null : model.ts;


                if (tenderPos.RazpisPozicijaID == 0)
                {
                    tenderPos.ts = DateTime.Now;
                    //Nastavimo polje PrevoznikAktualnaCena na true ker je ta zadnja cena za tega prevoznika s to relacijo
                    tenderPos.PrevoznikAktualnaCena = true;
                    //Nastavimo vse prejšnje pozicije razpisa z enakim prevoznikom in relacijo polje PrevoznikAktualnaCena na false
                    context.RazpisPozicija.Where(rp => rp.StrankaID == model.StrankaID && rp.RelacijaID == model.RelacijaID).ToList().ForEach(rp => rp.PrevoznikAktualnaCena = false);
                    context.RazpisPozicija.Add(tenderPos);

                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        RazpisPozicija original = context.RazpisPozicija.Where(r => r.RazpisPozicijaID == tenderPos.RazpisPozicijaID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(tenderPos);
                        context.SaveChanges();
                    }
                }
                return tenderPos.RazpisPozicijaID;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public int SaveTenderWithTenderPositions(TenderFullModel model, bool updateRecord = true)
        {
            try
            {
                model.ts = (model.ts == DateTime.MinValue ? DateTime.Now : model.ts);

                int tenderID = SaveTender(model, updateRecord);
                SaveTenderPositions(model.RazpisPozicija, tenderID);
                return tenderID;
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis("Error :" + ex.ToString());
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }



        public void SaveTenderPositions(List<TenderPositionModel> positions, int tenderID)
        {
            try
            {

                DataTypesHelper.LogThis(DateTime.Now.ToString());

                int cnt = 0;
                int cntAdd = 0;
                int cntUpdate = 0;
                DataTypesHelper.LogThis("CNT :" + positions.Count);
                List<RazpisPozicija> listToAdd = new List<RazpisPozicija>();
                List<RazpisPozicija> listToUpdate = new List<RazpisPozicija>();


                foreach (var item in positions)
                {
                    RazpisPozicija tenderPos = new RazpisPozicija();
                    tenderPos.Cena = item.Cena;
                    tenderPos.PotDokumenta = item.PotDokumenta;
                    tenderPos.IDOseba = item.IDOseba;
                    tenderPos.RazpisID = tenderID;
                    tenderPos.RazpisPozicijaID = item.RazpisPozicijaID;
                    tenderPos.RelacijaID = item.RelacijaID;
                    tenderPos.StrankaID = item.StrankaID;
                    tenderPos.ZbirnikTonID = item.ZbirnikTonID;
                    tenderPos.ts = item.ts.CompareTo(DateTime.MinValue) == 0 ? (DateTime?)null : item.ts;

                    cnt++;

                    if (tenderPos.RazpisPozicijaID == 0)
                    {
                        cntAdd++;
                        tenderPos.ts = DateTime.Now;
                        //Nastavimo polje PrevoznikAktualnaCena na true ker je ta zadnja cena za tega prevoznika s to relacijo
                        tenderPos.PrevoznikAktualnaCena = true;
                        //Nastavimo vse prejšnje pozicije razpisa z enakim prevoznikom in relacijo polje PrevoznikAktualnaCena na false
                        //context.RazpisPozicija.Where(rp => rp.StrankaID == item.StrankaID && rp.RelacijaID == item.RelacijaID).ToList().ForEach(rp => rp.PrevoznikAktualnaCena = false);
                        //context.RazpisPozicija.Add(tenderPos);
                        listToAdd.Add(tenderPos);
                    }
                    else
                    {
                        cntUpdate++;
                        RazpisPozicija original = context.RazpisPozicija.Where(r => r.RazpisPozicijaID == tenderPos.RazpisPozicijaID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(tenderPos);
                        listToUpdate.Add(tenderPos);
                    }
                    int mod = cnt % 50;
                    if (mod == 0)
                    {
                        DataTypesHelper.LogThis("Izdelane pozicije št:" + cnt + " / " + positions.Count);
                    }

                    int modCommit = cnt % 1000;
                    if (modCommit == 0)
                    {
                        //context.SaveChanges();
                        DataTypesHelper.LogThis("Commit:" + cnt + " / " + positions.Count);
                    }

                }
                if (listToAdd.Count > 0) context.BulkInsert(listToAdd);
                if (listToUpdate.Count > 0) context.BulkUpdate(listToUpdate);


                var tender = context.Razpis.Where(t => t.RazpisID == tenderID).FirstOrDefault();
                tender.RazpisKreiran = true;
                DataTypesHelper.LogThis(DateTime.Now.ToString());
                //context.SaveChanges();
                DataTypesHelper.LogThis("CNT add :" + cntAdd);
                DataTypesHelper.LogThis("CNT update:" + cntUpdate);
                DataTypesHelper.LogThis("Save positions finished!");
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis("Error :" + ex.ToString());
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public void SaverPositionsChanges(List<TenderPositionChangeModel> positionsChanges)
        {
            try
            {

                DataTypesHelper.LogThis(DateTime.Now.ToString());


                List<RazpisPozicijaSpremembe> listToAdd = new List<RazpisPozicijaSpremembe>();



                foreach (var item in positionsChanges)
                {
                    RazpisPozicijaSpremembe tenderChangesPos = new RazpisPozicijaSpremembe();
                    tenderChangesPos.NovaCena = item.NovaCena;
                    tenderChangesPos.StaraCena = item.StaraCena;
                    tenderChangesPos.VnosTS = item.VnosTS;
                    tenderChangesPos.IDVnosOseba = (item.IDVnosOseba == 0) ? item.IDSpremembeOseba : 1;
                    tenderChangesPos.SpremembeTS = item.SpremembeTS;
                    tenderChangesPos.IDSpremembeOseba = item.IDSpremembeOseba;
                    tenderChangesPos.RazpisID = item.RazpisID;
                    tenderChangesPos.RazpisPozicijaSpremembeID = item.RazpisPozicijaSpremembeID;
                    tenderChangesPos.RelacijaID = item.RelacijaID;
                    tenderChangesPos.StrankaID = item.StrankaID;
                    tenderChangesPos.ZbirnikTonID = item.ZbirnikTonID;
                    listToAdd.Add(tenderChangesPos);
                }
                if (listToAdd.Count > 0) context.BulkInsert(listToAdd);

            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis("Error :" + ex.ToString());
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public void SaveTenders(List<TenderFullModel> model)
        {
            /*try
            {
                Razpis tender = null;

                foreach (var item in model)
                {
                    tender = new Razpis();
                    tender.Cena = item.Cena;
                    tender.DatumRazpisa = item.DatumRazpisa;
                    tender.PotDokumenta = item.PotDokumenta;
                    tender.RazpisID = item.RazpisID;
                    tender.RelacijaID = item.RelacijaID;
                    tender.StrankaID = item.StrankaID;
                    tender.ts = item.ts.CompareTo(DateTime.MinValue) == 0 ? (DateTime?)null : item.ts;
                    tender.tsIDOseba = item.tsIDOseba;

                    if (tender.RazpisID == 0)
                    {
                        tender.ts = DateTime.Now;
                        context.Razpis.Add(tender);
                        context.SaveChanges();
                    }
                    else
                    {
                        Razpis original = context.Razpis.Where(r => r.RazpisID == tender.RazpisID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(tender);
                        context.SaveChanges();
                    }
                }
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }*/
        }

        public List<TenderPositionModel> GetTenderListByRouteID(int routeID)
        {
            try
            {
                List<RazpisPozicija> tenderPositions = new List<RazpisPozicija>();
                var test = (from tenderPos in context.RazpisPozicija
                            where tenderPos.RelacijaID == routeID && tenderPos.Stranka_OTP.Activity == 1
                            group tenderPos by tenderPos.StrankaID into transportGroup
                            select transportGroup).ToList();

                foreach (var item in test)
                {
                    var list = item.ToList();//dobimo seznam RazpisPozicij za posameznega prevoznika

                    //uredimo seznam po datumu razpisa (padajoče) in izberemo prvo pozicijo (RazpisPozicija)
                    var tenderPos = list.OrderByDescending(o => o.Razpis.DatumRazpisa).Where(o => o.Cena > 0).FirstOrDefault();

                    if (tenderPos == null)
                        tenderPos = list.OrderByDescending(o => o.Razpis.DatumRazpisa).FirstOrDefault();

                    if (tenderPos != null)
                        tenderPositions.Add(tenderPos);
                }

                //pridobimo zadnji razpis (DatumRazpis) ki vsebuje podano relacijo
                /*var tmp = from tender in context.Razpis
                          join tenderPos in context.RazpisPozicija on tender.RazpisID equals tenderPos.RazpisID
                          where tenderPos.RelacijaID == routeID
                          group tender by tender into t
                          orderby t.Key.DatumRazpisa descending
                          select t;*/



                //poiščemo vse relacije ki so v razpisu in jih pošljemo uporanbiku
                var query = from tenderPos in tenderPositions //from tenderPos in tmp.FirstOrDefault().Key.RazpisPozicija
                                                              // where tenderPos.RelacijaID == routeID
                            select new TenderPositionModel
                            {
                                Cena = tenderPos.Cena.HasValue ? tenderPos.Cena.Value : 0,
                                PotDokumenta = tenderPos.PotDokumenta,
                                RazpisID = tenderPos.RazpisID,
                                RelacijaID = tenderPos.RelacijaID,
                                StrankaID = tenderPos.StrankaID,
                                ts = tenderPos.ts.HasValue ? tenderPos.ts.Value : DateTime.MinValue,
                                RazpisPozicijaID = tenderPos.RazpisPozicijaID,
                                IDOseba = tenderPos.IDOseba.HasValue ? tenderPos.IDOseba.Value : 0,
                                Relacija = (from r in context.Relacija
                                            where r.RelacijaID == tenderPos.RelacijaID
                                            select new RouteModel
                                            {
                                                Datum = r.Datum.HasValue ? r.Datum.Value : DateTime.MinValue,
                                                Dolzina = r.Dolzina,
                                                Koda = r.Koda,
                                                Naziv = r.Naziv,
                                                RelacijaID = r.RelacijaID,
                                                ts = tenderPos.ts.HasValue ? r.ts.Value : DateTime.MinValue,
                                                tsIDOsebe = r.tsIDOsebe.HasValue ? r.tsIDOsebe.Value : 0,
                                            }).FirstOrDefault(),
                                Stranka = (from client in context.Stranka_OTP
                                           where client.idStranka == tenderPos.StrankaID && client.Activity == 1
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

                                           }).FirstOrDefault()
                            };

                return query.OrderBy(rp => rp.Cena <= 0).ThenBy(rp => rp.Cena).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public TonsModel GetZbirnikTonByID(int ZbirnikTonID)
        {
            try
            {
                var query = from zbirnik in context.ZbirnikTon
                            where zbirnik.ZbirnikTonID == ZbirnikTonID
                            select new TonsModel
                            {

                                Koda = zbirnik.Koda,
                                Naziv = zbirnik.Naziv,
                                ts = zbirnik.ts.HasValue ? zbirnik.ts.Value : DateTime.MinValue,
                            };

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }


        public List<TenderPositionModel> GetTenderListByRouteIDAndTonsID(int routeID, int tonsID, bool bShowZeroTenders)
        {
            try
            {
                
                List<RazpisPozicija> tenderPositions = new List<RazpisPozicija>();
                var test = (from tenderPos in context.RazpisPozicija
                            where tenderPos.RelacijaID == routeID && tenderPos.ZbirnikTonID == tonsID && tenderPos.Stranka_OTP.Activity == 1
                            group tenderPos by tenderPos.StrankaID into transportGroup
                            select transportGroup).ToList();

                foreach (var item in test)
                {
                    var list = item.ToList();//dobimo seznam RazpisPozicij za posameznega prevoznika

                    //uredimo seznam po datumu razpisa (padajoče) in izberemo prvo pozicijo (RazpisPozicija)
                    var tenderPos = list.OrderByDescending(o => o.Razpis.DatumRazpisa).Where(o => o.Cena > 0).FirstOrDefault();

                    if (tenderPos == null)
                        tenderPos = list.OrderByDescending(o => o.Razpis.DatumRazpisa).FirstOrDefault();

                    if (tenderPos != null)
                        tenderPositions.Add(tenderPos);
                }


                //poiščemo vse relacije ki so v razpisu in jih pošljemo uporanbiku
                var query = from tenderPos in tenderPositions //from tenderPos in tmp.FirstOrDefault().Key.RazpisPozicija
                                                              // where tenderPos.RelacijaID == routeID
                            select new TenderPositionModel
                            {
                                Cena = tenderPos.Cena.HasValue ? tenderPos.Cena.Value : 0,
                                PotDokumenta = tenderPos.PotDokumenta,
                                RazpisID = tenderPos.RazpisID,
                                RelacijaID = tenderPos.RelacijaID,
                                StrankaID = tenderPos.StrankaID,
                                ts = tenderPos.ts.HasValue ? tenderPos.ts.Value : DateTime.MinValue,
                                RazpisPozicijaID = tenderPos.RazpisPozicijaID,
                                IDOseba = tenderPos.IDOseba.HasValue ? tenderPos.IDOseba.Value : 0,
                                Stranka = (from client in context.Stranka_OTP
                                           where client.idStranka == tenderPos.StrankaID && client.Activity == 1
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

                                           }).FirstOrDefault()
                            };
                var RetList = bShowZeroTenders ? query.Where(rp => rp.Cena > 0).ToList() : query;
                return RetList.OrderBy(rp => rp.Cena <= 0).ThenBy(rp => rp.Cena).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }
        public List<TenderPositionModel> GetTenderListByRouteIDAndTenderDate(int routeID, string TenderDate)
        {
            try
            {
                List<RazpisPozicija> tenderPositions = new List<RazpisPozicija>();

                DateTime dtTest = DateTime.Now;
                bool bIsDate = DateTime.TryParse(TenderDate, out dtTest);



                if (!bIsDate)
                {
                    var oData = (from tenderPos in context.RazpisPozicija
                                 where tenderPos.RelacijaID == routeID && tenderPos.Stranka_OTP.Activity == 1
                                 group tenderPos by tenderPos.StrankaID into transportGroup
                                 select transportGroup).ToList();
                    foreach (var item in oData)
                    {
                        var list = item.ToList();//dobimo seznam RazpisPozicij za posameznega prevoznika

                        //uredimo seznam po datumu razpisa (padajoče) in izberemo prvo pozicijo (RazpisPozicija)
                        var tenderPos = list.OrderByDescending(o => o.Razpis.DatumRazpisa).Where(o => o.Cena > 0).FirstOrDefault();

                        if (tenderPos == null)
                            tenderPos = list.OrderByDescending(o => o.Razpis.DatumRazpisa).FirstOrDefault();

                        if (tenderPos != null)
                            tenderPositions.Add(tenderPos);
                    }
                }
                else
                {
                    var oData = (from tenderPos in context.RazpisPozicija
                                 where tenderPos.RelacijaID == routeID && tenderPos.Stranka_OTP.Activity == 1 && tenderPos.ts < dtTest
                                 group tenderPos by tenderPos.StrankaID into transportGroup
                                 select transportGroup).ToList();
                    foreach (var item in oData)
                    {
                        var list = item.ToList();//dobimo seznam RazpisPozicij za posameznega prevoznika

                        //uredimo seznam po datumu razpisa (padajoče) in izberemo prvo pozicijo (RazpisPozicija)
                        var tenderPos = list.OrderByDescending(o => o.Razpis.DatumRazpisa).Where(o => o.Cena > 0).FirstOrDefault();

                        if (tenderPos == null)
                            tenderPos = list.OrderByDescending(o => o.Razpis.DatumRazpisa).FirstOrDefault();

                        if (tenderPos != null)
                            tenderPositions.Add(tenderPos);
                    }
                }

                //pridobimo zadnji razpis (DatumRazpis) ki vsebuje podano relacijo
                /*var tmp = from tender in context.Razpis
                          join tenderPos in context.RazpisPozicija on tender.RazpisID equals tenderPos.RazpisID
                          where tenderPos.RelacijaID == routeID
                          group tender by tender into t
                          orderby t.Key.DatumRazpisa descending
                          select t;*/



                //poiščemo vse relacije ki so v razpisu in jih pošljemo uporanbiku
                var query = from tenderPos in tenderPositions //from tenderPos in tmp.FirstOrDefault().Key.RazpisPozicija
                                                              // where tenderPos.RelacijaID == routeID
                            select new TenderPositionModel
                            {
                                Cena = tenderPos.Cena.HasValue ? tenderPos.Cena.Value : 0,
                                PotDokumenta = tenderPos.PotDokumenta,
                                RazpisID = tenderPos.RazpisID,
                                RelacijaID = tenderPos.RelacijaID,
                                StrankaID = tenderPos.StrankaID,
                                ts = tenderPos.ts.HasValue ? tenderPos.ts.Value : DateTime.MinValue,
                                RazpisPozicijaID = tenderPos.RazpisPozicijaID,
                                IDOseba = tenderPos.IDOseba.HasValue ? tenderPos.IDOseba.Value : 0,
                                Relacija = (from r in context.Relacija
                                            where r.RelacijaID == tenderPos.RelacijaID
                                            select new RouteModel
                                            {
                                                Datum = r.Datum.HasValue ? r.Datum.Value : DateTime.MinValue,
                                                Dolzina = r.Dolzina,
                                                Koda = r.Koda,
                                                Naziv = r.Naziv,
                                                RelacijaID = r.RelacijaID,
                                                ts = tenderPos.ts.HasValue ? r.ts.Value : DateTime.MinValue,
                                                tsIDOsebe = r.tsIDOsebe.HasValue ? r.tsIDOsebe.Value : 0,
                                            }).FirstOrDefault(),
                                Stranka = (from client in context.Stranka_OTP
                                           where client.idStranka == tenderPos.StrankaID && client.Activity == 1
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

                                           }).FirstOrDefault()
                            };

                return query.OrderBy(rp => rp.Cena <= 0).ThenBy(rp => rp.Cena).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public void DeleteTenderPositions(List<int> tenderPositionsID)
        {
            try
            {
                foreach (var item in tenderPositionsID)
                {
                    var tenderPosition = context.RazpisPozicija.Where(r => r.RazpisPozicijaID == item).FirstOrDefault();

                    if (tenderPosition != null)
                    {
                        context.RazpisPozicija.Remove(tenderPosition);
                    }
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_07, ex);
            }
        }

        public List<TransportCountModel> GetTransportCounByTransporterAndRoute(List<TransportCountModel> model)
        {
            try
            {
                string delovnaKoda = Enums.StatusOfRecall.DELOVNA.ToString();
                string vodobritviKoda = Enums.StatusOfRecall.V_ODOBRITEV.ToString();
                int statusDelovna = context.StatusOdpoklica.Where(so => so.Koda == delovnaKoda).FirstOrDefault().StatusOdpoklicaID;
                int statusVOdobritvi = context.StatusOdpoklica.Where(so => so.Koda == vodobritviKoda).FirstOrDefault().StatusOdpoklicaID;
                DateTime previousYear = DateTime.Now.AddYears(-1);
                DateTime currentDate = DateTime.Now;

                foreach (var item in model)
                {
                    item.StPotrjenihOdpoklicevNaRelacijoZaPrevoznika = context.Odpoklic.Where(od => od.DobaviteljID == item.PrevoznikID &&
                        od.RelacijaID == item.RelacijaID &&
                        od.StatusID != statusDelovna &&
                        od.StatusID != statusVOdobritvi &&
                        od.ts.Value >= previousYear &&
                        od.ts.Value <= currentDate).Count();

                    item.StPotrjenihOdpoklicevNaRelacijoZaVsePrevoznike = context.Odpoklic.Where(od => od.RelacijaID == item.RelacijaID &&
                        od.StatusID != statusDelovna &&
                        od.StatusID != statusVOdobritvi &&
                        od.ts.Value >= previousYear &&
                        od.ts.Value <= currentDate).Count();
                }
                /*var query = from r in context.Odpoklic
                            where model.Any(tcm => tcm.PrevoznikID == r.DobaviteljID && tcm.RelacijaID == r.RelacijaID)
                            select new TransportCountModel
                            {
                                PrevoznikID = r.DobaviteljID.HasValue ? r.DobaviteljID.Value : 0,
                                RelacijaID = r.RelacijaID.HasValue ? r.RelacijaID.Value : 0,
                                //StPotrjenihOdpoklicev = gr.Count()//,
                                NazivPrevoznik = r.Stranka_OTP.NazivPrvi,
                                RelacijaNaziv = r.Relacija.Naziv
                            };*/

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public TransportCountModel GetTransportCounByTransporterAndRoute(TransportCountModel model)
        {
            try
            {
                string delovnaKoda = Enums.StatusOfRecall.DELOVNA.ToString();
                string vodobritviKoda = Enums.StatusOfRecall.V_ODOBRITEV.ToString();
                int statusDelovna = context.StatusOdpoklica.Where(so => so.Koda == delovnaKoda).FirstOrDefault().StatusOdpoklicaID;
                int statusVOdobritvi = context.StatusOdpoklica.Where(so => so.Koda == vodobritviKoda).FirstOrDefault().StatusOdpoklicaID;
                DateTime previousYear = DateTime.Now.AddYears(-1);
                DateTime currentDate = DateTime.Now;

                model.StPotrjenihOdpoklicevNaRelacijoZaPrevoznika = context.Odpoklic.Where(od => od.DobaviteljID == model.PrevoznikID &&
                        od.RelacijaID == model.RelacijaID &&
                        od.StatusID != statusDelovna &&
                        od.StatusID != statusVOdobritvi &&
                        od.ts.Value >= previousYear &&
                        od.ts.Value <= currentDate).Count();

                model.StPotrjenihOdpoklicevNaRelacijoZaVsePrevoznike = context.Odpoklic.Where(od => od.RelacijaID == model.RelacijaID &&
                        od.StatusID != statusDelovna &&
                        od.StatusID != statusVOdobritvi &&
                        od.ts.Value >= previousYear &&
                        od.ts.Value <= currentDate).Count();

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public decimal GetLowestAndMostRecentPriceByRouteID(int routeID)
        {
            try
            {
                var tenderPos = GetTenderListByRouteID(routeID).FirstOrDefault();
                //var tenderPos = GetTenderListByRouteID(routeID);
                /* var item = context.RazpisPozicija.Where(rp => rp.RelacijaID == routeID && rp.Cena > 0).OrderBy(rp => rp.Cena).ThenBy(rp => rp.ts).FirstOrDefault();

                 if (item != null)
                     return item.Cena.HasValue ? item.Cena.Value : 0;*/

                if (tenderPos != null)
                    return tenderPos.Cena;

                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }


        public decimal GetLowestAndMostRecentPriceByRouteIDandZbirnikTonsID(int routeID, int ZbirnikTonID)
        {
            try
            {
                var tenderPos = GetTenderListByRouteIDAndTonsID(routeID, ZbirnikTonID, true).FirstOrDefault();
                //var tenderPos = GetTenderListByRouteID(routeID);
                /* var item = context.RazpisPozicija.Where(rp => rp.RelacijaID == routeID && rp.Cena > 0).OrderBy(rp => rp.Cena).ThenBy(rp => rp.ts).FirstOrDefault();

                 if (item != null)
                     return item.Cena.HasValue ? item.Cena.Value : 0;*/

                if (tenderPos != null)
                    return tenderPos.Cena;

                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<TenderPositionModel> GetTenderListByRouteIDAndRecallID(int routeID, int recallID)
        {
            try
            {
                List<RazpisPozicija> tenderPositions = new List<RazpisPozicija>();
                var test = (from tenderPos in context.RazpisPozicija
                            where tenderPos.RelacijaID == routeID
                            group tenderPos by tenderPos.StrankaID into transportGroup
                            select transportGroup).ToList();



                foreach (var item in test)
                {
                    var list = item.ToList();//dobimo seznam RazpisPozicij za posameznega prevoznika

                    //uredimo seznam po datumu razpisa (padajoče) in izberemo prvo pozicijo (RazpisPozicija)
                    var tenderPos = list.OrderByDescending(o => o.Razpis.DatumRazpisa).Where(o => o.Cena > 0).FirstOrDefault();

                    if (tenderPos == null)
                        tenderPos = list.OrderByDescending(o => o.Razpis.DatumRazpisa).FirstOrDefault();


                    if (tenderPos != null)
                    {
                        //Preverimo v tabelo Prijava PRevoznik če že obstaja trenutni prevoznik za odpoklic
                        var obj = context.PrijavaPrevoznika.Where(pp => pp.OdpoklicID == recallID && pp.PrevoznikID == item.Key).FirstOrDefault();
                        if (obj == null)//če prevoznik že obstaja v tabeli ga ne dodamo v tenderPositions seznam, če pa ne pa ga dodamo.
                            tenderPositions.Add(tenderPos);
                    }
                }


                //poiščemo vse relacije ki so v razpisu in jih pošljemo uporanbiku
                var query = from tenderPos in tenderPositions //from tenderPos in tmp.FirstOrDefault().Key.RazpisPozicija
                                                              // where tenderPos.RelacijaID == routeID
                            select new TenderPositionModel
                            {
                                Cena = tenderPos.Cena.HasValue ? tenderPos.Cena.Value : 0,
                                PotDokumenta = tenderPos.PotDokumenta,
                                RazpisID = tenderPos.RazpisID,
                                RelacijaID = tenderPos.RelacijaID,
                                StrankaID = tenderPos.StrankaID,
                                ts = tenderPos.ts.HasValue ? tenderPos.ts.Value : DateTime.MinValue,
                                RazpisPozicijaID = tenderPos.RazpisPozicijaID,
                                IDOseba = tenderPos.IDOseba.HasValue ? tenderPos.IDOseba.Value : 0,
                                Relacija = (from r in context.Relacija
                                            where r.RelacijaID == tenderPos.RelacijaID
                                            select new RouteModel
                                            {
                                                Datum = r.Datum.HasValue ? r.Datum.Value : DateTime.MinValue,
                                                Dolzina = r.Dolzina,
                                                Koda = r.Koda,
                                                Naziv = r.Naziv,
                                                RelacijaID = r.RelacijaID,
                                                ts = tenderPos.ts.HasValue ? r.ts.Value : DateTime.MinValue,
                                                tsIDOsebe = r.tsIDOsebe.HasValue ? r.tsIDOsebe.Value : 0,
                                            }).FirstOrDefault(),
                                Stranka = (from client in context.Stranka_OTP
                                           where client.idStranka == tenderPos.StrankaID
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

                                           }).FirstOrDefault()
                            };

                return query.OrderBy(rp => rp.Cena <= 0).ThenBy(rp => rp.Cena).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<TonsModel> GetAllTons()
        {
            try
            {
                var query = from zt in context.ZbirnikTon
                            orderby zt.SortIdx
                            select new TonsModel
                            {
                                ZbirnikTonID = zt.ZbirnikTonID,
                                Koda = zt.Koda,
                                Naziv = zt.Naziv,
                                ts = zt.ts.HasValue ? zt.ts.Value : DateTime.MinValue,
                            };

                List<TonsModel> model = query.ToList();

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public hlpTenderTransporterSelection PrepareDataForTenderTransport(hlpTenderTransporterSelection vTTModel)
        {
            decimal dNajnizjaCena = 0;
            TenderPositionModel model = null;
            int iCnt = 0;
            int iID = 0;
            hlpTenderCreateExcellData hlpTenderCreateExcellData = null;
            try
            {
                vTTModel.RazpisPozicija.Clear();
                hlpTenderCreateExcellData = (vTTModel.tTenderCreateExcellData != null) ? vTTModel.tTenderCreateExcellData : new hlpTenderCreateExcellData();



                foreach (var routeItem in vTTModel.SelectedRowsRoutes)
                {
                    iCnt++;

                    IList routeList = (IList)routeItem;
                    foreach (var tonsItem in vTTModel.SelectedRowsTons)
                    {
                        IList tonsList = (IList)tonsItem;
                        dNajnizjaCena = 0;

                        int iRelacijaID = DataTypesHelper.ParseInt(routeList[0]);
                        int iZbirnikTonID = DataTypesHelper.ParseInt(tonsList[0]);

                        List<TenderPositionModel> tenderRoutesPrices = GetTenderListByRouteIDAndTonsID(iRelacijaID, iZbirnikTonID, false);

                        if (tenderRoutesPrices.Count > 1)
                        {
                            TenderPositionModel bestPriceTP = tenderRoutesPrices.FirstOrDefault();
                            dNajnizjaCena = bestPriceTP.Cena;
                        }

                        // pridobimo vse, ki imajo najnižjo ceno
                        List<TenderPositionModel> BestPriceTenders = tenderRoutesPrices.Where(p => p.Cena == dNajnizjaCena).ToList();
                        if (dNajnizjaCena == 0) BestPriceTenders.Clear();

                        TransporterSimpleModel tsm = null;
                        RouteSimpleModel rsm = null;
                        TonsModel tm = null;

                        foreach (var carrierItem in vTTModel.SelectedRowsCarriers)
                        {
                            IList carrierList = (IList)carrierItem;

                            int iStrankaID = DataTypesHelper.ParseInt(carrierList[0]);


                            if (!vTTModel.CheapestTransporterTender)
                            {
                                // preverimo če obstaja med najboljšimi prevozniki
                                if (BestPriceTenders.Where(t => t.StrankaID == iStrankaID).FirstOrDefault() == null)
                                {
                                    // dodaj v excell model
                                    if (hlpTenderCreateExcellData.TransporterList == null) hlpTenderCreateExcellData.TransporterList = new List<TransporterSimpleModel>();

                                    tsm = hlpTenderCreateExcellData.TransporterList.Where(t => t.ClientID == iStrankaID).FirstOrDefault();
                                    if (tsm == null)
                                    {
                                        tsm = new TransporterSimpleModel();
                                        tsm.ClientID = iStrankaID;
                                        tsm.Naziv = DataTypesHelper.Parse(carrierList[1].ToString());
                                        hlpTenderCreateExcellData.TransporterList.Add(tsm);
                                    }

                                    if (tsm.RouteList == null) tsm.RouteList = new List<RouteSimpleModel>();
                                    rsm = tsm.RouteList.Where(t => t.RouteID == iRelacijaID).FirstOrDefault();
                                    if (rsm == null)
                                    {
                                        // dobi števila vseh prevozov
                                        TransportCountModel tcm = new TransportCountModel();
                                        tcm.PrevoznikID = iStrankaID;
                                        tcm.RelacijaID = iRelacijaID;

                                        tcm = GetTransportCounByTransporterAndRoute(tcm);

                                        rsm = new RouteSimpleModel();
                                        rsm.RouteID = iRelacijaID;
                                        rsm.Naziv = DataTypesHelper.Parse(routeList[1].ToString());
                                        if (tcm != null)
                                        {
                                            rsm.SteviloPrevozVLetuNaRelacijoVsiPrevozniki = tcm.StPotrjenihOdpoklicevNaRelacijoZaVsePrevoznike;
                                            rsm.SteviloPrevozVLetuNaRelacijoPrevoznik = tcm.StPotrjenihOdpoklicevNaRelacijoZaPrevoznika;
                                        }
                                        tsm.RouteList.Add(rsm);
                                    }

                                    if (rsm.TonsList == null) rsm.TonsList = new List<TonsModel>();
                                    tm = rsm.TonsList.Where(t => t.ZbirnikTonID == iZbirnikTonID).FirstOrDefault();
                                    if (tm == null)
                                    {
                                        tm = new TonsModel();
                                        tm.ZbirnikTonID = iZbirnikTonID;
                                        tm.Naziv = DataTypesHelper.Parse(tonsList[1].ToString());
                                        tm.NajnizjaCena = dNajnizjaCena;
                                        rsm.TonsList.Add(tm);
                                    }
                                }
                            }
                            else
                            {
                                // dodaj v excell model
                                if (hlpTenderCreateExcellData.TransporterList == null) hlpTenderCreateExcellData.TransporterList = new List<TransporterSimpleModel>();

                                tsm = hlpTenderCreateExcellData.TransporterList.Where(t => t.ClientID == iStrankaID).FirstOrDefault();
                                if (tsm == null)
                                {
                                    tsm = new TransporterSimpleModel();
                                    tsm.ClientID = iStrankaID;
                                    tsm.Naziv = DataTypesHelper.Parse(carrierList[1].ToString());
                                    hlpTenderCreateExcellData.TransporterList.Add(tsm);
                                }

                                if (tsm.RouteList == null) tsm.RouteList = new List<RouteSimpleModel>();
                                rsm = tsm.RouteList.Where(t => t.RouteID == iRelacijaID).FirstOrDefault();
                                if (rsm == null)
                                {
                                    rsm = new RouteSimpleModel();
                                    rsm.RouteID = iRelacijaID;
                                    rsm.Naziv = DataTypesHelper.Parse(routeList[1].ToString());
                                    tsm.RouteList.Add(rsm);
                                }

                                if (rsm.TonsList == null) rsm.TonsList = new List<TonsModel>();
                                tm = rsm.TonsList.Where(t => t.ZbirnikTonID == iZbirnikTonID).FirstOrDefault();
                                if (tm == null)
                                {
                                    tm = new TonsModel();
                                    tm.ZbirnikTonID = iZbirnikTonID;
                                    tm.Naziv = DataTypesHelper.Parse(tonsList[1].ToString());
                                    tm.NajnizjaCena = dNajnizjaCena;
                                    rsm.TonsList.Add(tm);
                                }
                            }
                        }

                    }
                    int mod = iCnt % 10;
                    if (mod == 0)
                    {
                        DataTypesHelper.LogThis("Izdelane lokacije št:" + iCnt + " / " + vTTModel.SelectedRowsRoutes.Count + " trenutna lokacija: " + routeList[1]);
                    }

                }

                vTTModel.tTenderCreateExcellData = hlpTenderCreateExcellData;
                // shrani pozicije razpisa
                foreach (var itemTransporter in hlpTenderCreateExcellData.TransporterList)
                {
                    foreach (var itemRoute in itemTransporter.RouteList)
                    {
                        foreach (var itemTons in itemRoute.TonsList)
                        {
                            model = new TenderPositionModel();
                            model.Cena = 0;
                            model.RazpisID = 0;
                            model.RazpisPozicijaID = 0;
                            model.RelacijaID = itemRoute.RouteID;
                            model.StrankaID = itemTransporter.ClientID;
                            model.ZbirnikTonID = itemTons.ZbirnikTonID;
                            model.IDOseba = vTTModel.tTenderCreateExcellData._TenderModel.tsIDOseba;
                            vTTModel.RazpisPozicija.Add(model);
                        }
                    }
                }

                vTTModel.tTenderCreateExcellData._TenderModel.IsNajcenejsiPrevoznik = vTTModel.CheapestTransporterTender;
                vTTModel.tTenderCreateExcellData._TenderModel.PodatkiZaExcell_JSon = JsonConvert.SerializeObject(vTTModel);
                vTTModel.tTenderCreateExcellData._TenderModel.RazpisPozicija = vTTModel.RazpisPozicija;
                iID = SaveTenderWithTenderPositions(vTTModel.tTenderCreateExcellData._TenderModel, true);

                vTTModel.tTenderCreateExcellData._TenderModel.GeneriranTender = true;
                vTTModel.tTenderCreateExcellData._TenderModel.RazpisKreiran = true;
                SaveTender(vTTModel.tTenderCreateExcellData._TenderModel, true);

                return vTTModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_07, ex);
            }
        }


    }
}