using DatabaseWebService.Common;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsOTP.Client;
using DatabaseWebService.ModelsOTP.Route;
using DatabaseWebService.ModelsOTP.Tender;
using DatabaseWebService.Resources;
using System;
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

        public List<TenderFullModel> GetTenderList()
        {
            try
            {
                var query = from tender in context.Razpis
                            where tender.RazpisKreiran.Value.Equals(true)
                            select new TenderFullModel
                            {
                                CenaSkupaj = tender.CenaSkupaj,
                                DatumRazpisa = tender.DatumRazpisa,
                                Naziv = tender.Naziv,
                                RazpisID = tender.RazpisID,
                                ts = tender.ts.HasValue ? tender.ts.Value : DateTime.MinValue,
                                tsIDOseba = tender.tsIDOseba.HasValue ? tender.tsIDOseba.Value : 0,
                                RazpisKreiran = tender.RazpisKreiran.HasValue? tender.RazpisKreiran.Value : false
                            };

                List<TenderFullModel> model = query.ToList();
                foreach (var item in model)
                {
                    item.RazpisPozicija = GetTenderPositionModelByID(item.RazpisID);
                }

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<TenderPositionModel> GetTenderPositionModelByID(int tenderID)
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
                                ts = tender.ts.HasValue ? tender.ts.Value : DateTime.MinValue,
                                PotDokumenta = tender.PotDokumenta
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
                                DatumRazpisa = tender.DatumRazpisa,
                                Naziv = tender.Naziv,
                                RazpisID = tender.RazpisID,
                                ts = tender.ts.HasValue ? tender.ts.Value : DateTime.MinValue,
                                tsIDOseba = tender.tsIDOseba.HasValue ? tender.tsIDOseba.Value : 0,
                                RazpisKreiran = tender.RazpisKreiran.HasValue ? tender.RazpisKreiran.Value : false
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

        public int SaveTender(TenderFullModel model, bool updateRecord = true)
        {
            try
            {
                Razpis tender = new Razpis();
                tender.CenaSkupaj = model.CenaSkupaj;
                tender.DatumRazpisa = model.DatumRazpisa;
                tender.RazpisID = model.RazpisID;
                tender.Naziv = model.Naziv;
                tender.ts = model.ts.CompareTo(DateTime.MinValue) == 0 ? (DateTime?)null : model.ts;
                tender.tsIDOseba = model.tsIDOseba;
                tender.RazpisKreiran = model.RazpisKreiran;

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
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public void SaveTenderPositions(List<TenderPositionModel> positions, int tenderID)
        {
            try
            {
                int cnt = 0;

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
                    tenderPos.ts = item.ts.CompareTo(DateTime.MinValue) == 0 ? (DateTime?)null : item.ts;

                    cnt++;

                    if (tenderPos.RazpisPozicijaID == 0)
                    {
                        tenderPos.ts = DateTime.Now;
                        //Nastavimo polje PrevoznikAktualnaCena na true ker je ta zadnja cena za tega prevoznika s to relacijo
                        tenderPos.PrevoznikAktualnaCena = true;
                        //Nastavimo vse prejšnje pozicije razpisa z enakim prevoznikom in relacijo polje PrevoznikAktualnaCena na false
                        context.RazpisPozicija.Where(rp => rp.StrankaID == item.StrankaID && rp.RelacijaID == item.RelacijaID).ToList().ForEach(rp => rp.PrevoznikAktualnaCena = false);
                        context.RazpisPozicija.Add(tenderPos);
                    }
                    else
                    {
                        RazpisPozicija original = context.RazpisPozicija.Where(r => r.RazpisPozicijaID == tenderPos.RazpisPozicijaID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(tenderPos);
                    }
                }

                var tender = context.Razpis.Where(t => t.RazpisID == tenderID).FirstOrDefault();
                tender.RazpisKreiran = true;

                context.SaveChanges();
            }
            catch (Exception ex)
            {
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

                    if(tenderPos == null)
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
                    item.StPotrjenihOdpoklicev = context.Odpoklic.Where(od => od.DobaviteljID == item.PrevoznikID &&
                        od.RelacijaID == item.RelacijaID &&
                        od.StatusID != statusDelovna &&
                        od.StatusID != statusVOdobritvi &&
                        od.ts.Value >= previousYear &&
                        od.ts.Value <= currentDate).Count();

                    item.StVsehOdpoklicevZaRelacijo = context.Odpoklic.Where(od => od.RelacijaID == item.RelacijaID &&
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

                model.StPotrjenihOdpoklicev = context.Odpoklic.Where(od => od.DobaviteljID == model.PrevoznikID &&
                        od.RelacijaID == model.RelacijaID &&
                        od.StatusID != statusDelovna &&
                        od.StatusID != statusVOdobritvi &&
                        od.ts.Value >= previousYear &&
                        od.ts.Value <= currentDate).Count();

                model.StVsehOdpoklicevZaRelacijo = context.Odpoklic.Where(od => od.RelacijaID == model.RelacijaID &&
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
                        if(obj == null)//če prevoznik že obstaja v tabeli ga ne dodamo v tenderPositions seznam, če pa ne pa ga dodamo.
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
    }
}