using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatabaseWebService.Domain.Abstract;
using DatabaseWebService.Models.Client;
using DatabaseWebService.Models;
using DatabaseWebService.Resources;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using DatabaseWebService.Common;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.Models.Event;

namespace DatabaseWebService.Domain.Concrete
{
    public class ClientRepository : IClientRepository
    {
        AnalizaProdajeEntities context = new AnalizaProdajeEntities();

        private readonly IEventRepository eventRepo;

        public ClientRepository(IEventRepository eventRepository, AnalizaProdajeEntities dummyEntites)
        {
            eventRepo = eventRepository;
            context = dummyEntites;
        }

        public List<ClientSimpleModel> GetAllClients()
        {
            try
            {//loj => left outer join
                var query = from client in context.Stranka
                            join clientEmployee in context.StrankaZaposleni on client.idStranka equals clientEmployee.idStranka into loj
                            from subClient in loj.DefaultIfEmpty()
                            select new ClientSimpleModel
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
                                GenerirajERacun = client.GenerirajERacun.HasValue ? client.GenerirajERacun.Value : 0,
                                JavniZavod = client.JavniZavod.HasValue ? client.JavniZavod.Value : 0,
                                SecondID = client.SecondID.HasValue ? client.SecondID.Value : 0,
                                ts = client.ts.HasValue ? client.ts.Value : DateTime.MinValue,
                                tsIDOsebe = client.tsIDOsebe.HasValue ? client.tsIDOsebe.Value : 0,
                                idOsebe = (subClient == null ? 0 : subClient.Osebe.idOsebe),
                                ImeInPriimekZaposlen = (subClient == null ? String.Empty : subClient.Osebe.Ime + " " + subClient.Osebe.Priimek),
                                Aktivnost = client.AKTIVNOST.HasValue ? client.AKTIVNOST.Value : 0,
                                LastStatusDogodekID = client.LastStatusID.HasValue ? client.LastStatusID.Value : 0,
                                tsLastStatus = client.tsLastStatus.HasValue ? client.tsLastStatus.Value : DateTime.MinValue,
                                LastDogodekNaziv = (from status in context.StatusDogodek
                                                     where status.idStatusDogodek == client.LastStatusID
                                                     select new EventStatusModel
                                                     {
                                                         Koda = status.Koda,
                                                         Naziv = status.Naziv,
                                                         ts = status.ts.HasValue ? status.ts.Value : DateTime.MinValue,
                                                         tsIDOsebe = status.tsIDOsebe.HasValue ? status.tsIDOsebe.Value : 0
                                                     }).FirstOrDefault().Naziv,
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<ClientSimpleModel> GetAllClients(int employeeID)
        {
            try
            {
                var query = from client in context.Stranka
                            join clientEmployee in context.StrankaZaposleni
                            on client.idStranka equals clientEmployee.idStranka
                            where clientEmployee.idOsebe == employeeID
                            select new ClientSimpleModel
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
                                GenerirajERacun = client.GenerirajERacun.HasValue ? client.GenerirajERacun.Value : 0,
                                JavniZavod = client.JavniZavod.HasValue ? client.JavniZavod.Value : 0,
                                SecondID = client.SecondID.HasValue ? client.SecondID.Value : 0,
                                ts = client.ts.HasValue ? client.ts.Value : DateTime.MinValue,
                                tsIDOsebe = client.tsIDOsebe.HasValue ? client.tsIDOsebe.Value : 0,
                                Aktivnost = client.AKTIVNOST.HasValue ? client.AKTIVNOST.Value : 0,
                                LastStatusDogodekID = client.LastStatusID.HasValue ? client.LastStatusID.Value : 0,
                                tsLastStatus = client.tsLastStatus.HasValue ? client.tsLastStatus.Value : DateTime.MinValue,
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public ClientFullModel GetClientByID(int clientID)
        {
            try
            {
                var query = from client in context.Stranka
                            where client.idStranka.Equals(clientID)
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
                                GenerirajERacun = client.GenerirajERacun.HasValue ? client.GenerirajERacun.Value : 0,
                                JavniZavod = client.JavniZavod.HasValue ? client.JavniZavod.Value : 0,
                                SecondID = client.SecondID.HasValue ? client.SecondID.Value : 0,
                                ts = client.ts.HasValue ? client.ts.Value : DateTime.MinValue,
                                tsIDOsebe = client.tsIDOsebe.HasValue ? client.tsIDOsebe.Value : 0,
                                Aktivnost = client.AKTIVNOST.HasValue ? client.AKTIVNOST.Value : 0,
                                LastStatusDogodekID = client.LastStatusID.HasValue ? client.LastStatusID.Value : 0,
                                tsLastStatus = client.tsLastStatus.HasValue ? client.tsLastStatus.Value : DateTime.MinValue,
                                LastStatusDogodek = (from status in context.StatusDogodek
                                              where status.idStatusDogodek == client.LastStatusID
                                              select new EventStatusModel
                                              {
                                                  Koda = status.Koda,
                                                  Naziv = status.Naziv,                                                                                               
                                                  ts = status.ts.HasValue ? status.ts.Value : DateTime.MinValue,
                                                  tsIDOsebe = status.tsIDOsebe.HasValue ? status.tsIDOsebe.Value : 0
                                              }).FirstOrDefault(),
                            };

                ClientFullModel model = query.FirstOrDefault();
                model.KontaktneOsebe = new List<ContactPersonModel>();
                model.KontaktneOsebe = GetContactPersonModelList(clientID);

                model.Plan = new List<PlanModel>();
                model.Plan = GetPlanModelList(clientID);

                model.StrankaZaposleni = new List<ClientEmployeeModel>();
                model.StrankaZaposleni = GetClientEmployeeModelList(clientID);

                model.Dogodek = new List<EventSimpleModel>();
                model.Dogodek = eventRepo.GetEventsListByClientID(clientID);

                model.Naprave = new List<DevicesModel>();
                model.Naprave = GetDevicesModelList(clientID);

                model.Opombe = new List<NotesModel>();
                model.Opombe = GetNotesModelList(clientID);

                model.StrankaKategorija = new List<ClientCategorieModel>();
                model.StrankaKategorija = GetClientCategorieModelList(clientID);

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public ClientFullModel GetClientByID(int clientID, int employeeID)
        {
            try
            {
                var query = from client in context.Stranka
                            join clientEmployee in context.StrankaZaposleni
                            on client.idStranka equals clientEmployee.idStranka
                            where client.idStranka.Equals(clientID) && clientEmployee.idOsebe.Value.Equals(employeeID)
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
                                GenerirajERacun = client.GenerirajERacun.HasValue ? client.GenerirajERacun.Value : 0,
                                JavniZavod = client.JavniZavod.HasValue ? client.JavniZavod.Value : 0,
                                SecondID = client.SecondID.HasValue ? client.SecondID.Value : 0,
                                ts = client.ts.HasValue ? client.ts.Value : DateTime.MinValue,
                                tsIDOsebe = client.tsIDOsebe.HasValue ? client.tsIDOsebe.Value : 0,
                                //TODO: Add collections of Dogodek, KontaktneOsebe, Nadzor, Plan, StrankaZaposleni
                            };

                ClientFullModel model = query.FirstOrDefault();
                if (model != null)
                {
                    model.KontaktneOsebe = new List<ContactPersonModel>();
                    model.KontaktneOsebe = GetContactPersonModelList(clientID);

                    model.Plan = new List<PlanModel>();
                    model.Plan = GetPlanModelList(clientID);

                    model.StrankaZaposleni = new List<ClientEmployeeModel>();
                    model.StrankaZaposleni = GetClientEmployeeModelList(clientID);

                    model.Dogodek = new List<EventSimpleModel>();
                    model.Dogodek = eventRepo.GetEventsListByClientID(clientID);

                    model.Naprave = new List<DevicesModel>();
                    model.Naprave = GetDevicesModelList(clientID);

                    model.StrankaKategorija = new List<ClientCategorieModel>();
                    model.StrankaKategorija = GetClientCategorieModelList(clientID);
                }

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public ClientSimpleModel GetClientSimpleModelByCode(int clientID)
        {
            try
            {
                var query = from client in context.Stranka
                            where client.idStranka.Equals(clientID)
                            select new ClientSimpleModel
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
                                GenerirajERacun = client.GenerirajERacun.HasValue ? client.GenerirajERacun.Value : 0,
                                JavniZavod = client.JavniZavod.HasValue ? client.JavniZavod.Value : 0,
                                SecondID = client.SecondID.HasValue ? client.SecondID.Value : 0,
                                ts = client.ts.HasValue ? client.ts.Value : DateTime.MinValue,
                                tsIDOsebe = client.tsIDOsebe.HasValue ? client.tsIDOsebe.Value : 0,
                                Aktivnost = client.AKTIVNOST.HasValue ? client.AKTIVNOST.Value : 0,
                                LastStatusDogodekID = client.LastStatusID.HasValue ? client.LastStatusID.Value : 0,
                                tsLastStatus = client.tsLastStatus.HasValue ? client.tsLastStatus.Value : DateTime.MinValue,
                                LastDogodekNaziv = (from status in context.StatusDogodek
                                                     where status.idStatusDogodek == client.LastStatusID
                                                     select new EventStatusModel
                                                     {
                                                         Koda = status.Koda,
                                                         Naziv = status.Naziv,
                                                         ts = status.ts.HasValue ? status.ts.Value : DateTime.MinValue,
                                                         tsIDOsebe = status.tsIDOsebe.HasValue ? status.tsIDOsebe.Value : 0
                                                     }).FirstOrDefault().Naziv,
                            };

                ClientSimpleModel model = query.FirstOrDefault();
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        private List<PlanModel> GetPlanModelList(int clientID)
        {
            try
            {
                var query = from plan in context.PlanStranka
                            where plan.IDStranka.Value.Equals(clientID)
                            select new PlanModel
                             {
                                 idKategorija = plan.idKategorija.HasValue ? plan.idKategorija.Value : 0,
                                 idPlan = plan.idPlanStranka,
                                 IDStranka = plan.IDStranka.HasValue ? plan.IDStranka.Value : 0,
                                 Kategorija = plan.Kategorija.Naziv,
                                 LetniZnesek = plan.LetniZnesek.HasValue ? plan.LetniZnesek.Value : 0,
                                 Leto = plan.Leto.HasValue ? plan.Leto.Value : 0,
                                 Stranka = plan.Stranka.NazivPrvi,
                                 ts = plan.ts.HasValue ? plan.ts.Value : DateTime.Now,
                                 tsIDOsebe = plan.tsIDOsebe.HasValue ? plan.tsIDOsebe.Value : 0
                             };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        private List<ContactPersonModel> GetContactPersonModelList(int clientID)
        {
            try
            {
                var query = from contact in context.KontaktneOsebe
                            where contact.idStranka.Value.Equals(clientID)
                            select new ContactPersonModel
                            {
                                idKontaktneOsebe = contact.idKontaktneOsebe,
                                idStranka = contact.idStranka.HasValue ? contact.idStranka.Value : 0,
                                NazivKontaktneOsebe = contact.NazivKontaktneOsebe,
                                Stranka = contact.Stranka.NazivPrvi,
                                Telefon = contact.Telefon,
                                GSM = contact.GSM,
                                Email = contact.Email,
                                DelovnoMesto = contact.DelovnoMesto,
                                ts = contact.ts.HasValue ? contact.ts.Value : DateTime.MinValue,
                                tsIDOsebe = contact.tsIDOsebe.HasValue ? contact.tsIDOsebe.Value : 0,
                                ZaporednaStevika = contact.ZaporednaStevika.HasValue ? contact.ZaporednaStevika.Value : 0,
                                Fax = contact.Fax,
                                Opombe = contact.Opombe,
                                RojstniDatum = contact.RojstniDatum.HasValue ? contact.RojstniDatum.Value : DateTime.MinValue,
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<ClientEmployeeModel> GetClientEmployeeModelList(int clientID)
        {
            try
            {
                var query = from cE in context.StrankaZaposleni
                            where cE.idStranka.Value.Equals(clientID)
                            group cE by cE into clientEmployee
                            select new ClientEmployeeModel
                            {
                                idOsebe = clientEmployee.Key.idOsebe.HasValue ? clientEmployee.Key.idOsebe.Value : 0,
                                idStranka = clientEmployee.Key.idStranka.HasValue ? clientEmployee.Key.idStranka.Value : 0,
                                idStrankaOsebe = clientEmployee.Key.idStrankaOsebe,
                                ts = clientEmployee.Key.ts.HasValue ? clientEmployee.Key.ts.Value : DateTime.MinValue,
                                tsIDOsebe = clientEmployee.Key.tsIDOsebe.HasValue ? clientEmployee.Key.tsIDOsebe.Value : 0,
                                oseba = (from employee in clientEmployee
                                         group employee by employee.Osebe into person
                                         select new EmployeeSimpleModel
                                         {

                                             DatumRojstva = person.Key.DatumRojstva.HasValue ? person.Key.DatumZaposlitve.Value : DateTime.MinValue,
                                             DatumZaposlitve = person.Key.DatumZaposlitve.HasValue ? person.Key.DatumZaposlitve.Value : DateTime.MinValue,
                                             DelovnoMesto = person.Key.DelovnoMesto,
                                             Email = person.Key.Email,
                                             Geslo = person.Key.Geslo,
                                             idOsebe = person.Key.idOsebe,
                                             Ime = person.Key.Ime,
                                             Naslov = person.Key.Naslov,
                                             Priimek = person.Key.Priimek,
                                             TelefonGSM = person.Key.TelefonGSM,
                                             ts = person.Key.ts.HasValue ? person.Key.ts.Value : DateTime.MinValue,
                                             tsIDOsebe = person.Key.tsIDOsebe.HasValue ? person.Key.tsIDOsebe.Value : 0,
                                             UporabniskoIme = person.Key.UporabniskoIme,
                                             Zunanji = person.Key.Zunanji.HasValue ? person.Key.Zunanji.Value : 0
                                         }).FirstOrDefault()
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        private List<DevicesModel> GetDevicesModelList(int clientID)
        {
            try
            {
                var query = from device in context.Naprava
                            where device.idStranka.Value.Equals(clientID)
                            select new DevicesModel
                            {
                                idNaprava = device.idNaprava,
                                idStranka = device.idStranka,
                                Koda = device.Koda,
                                Naziv = device.Naziv,
                                Opis = device.Opis,
                                Stranka = device.Stranka.NazivPrvi + " " + device.Stranka.NazivDrugi,
                                ts = device.ts.HasValue ? device.ts.Value : DateTime.MinValue,
                                tsIDOsebe = device.tsIDOsebe.HasValue ? device.tsIDOsebe.Value : 0
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        private List<NotesModel> GetNotesModelList(int clientID)
        {
            try
            {
                var query = from device in context.OpombaStranka
                            where device.idStranka.Value.Equals(clientID)
                            select new NotesModel
                            {
                                idOpombaStranka = device.idOpombaStranka,
                                idStranka = device.idStranka,
                                Opomba = device.Opomba,
                                Stranka = device.Stranka.NazivPrvi + " " + device.Stranka.NazivDrugi,
                                ts = device.ts.HasValue ? device.ts.Value : DateTime.MinValue,
                                tsIDOsebe = device.tsIDOsebe.HasValue ? device.tsIDOsebe.Value : 0,
                                VneselOseba = context.Osebe.Where(o => o.idOsebe == device.tsIDOsebe.Value).FirstOrDefault().Priimek,
                //VneselOseba = (from PrijOseba in context.Osebe
                //               where PrijOseba.idOsebe == device.tsIDOsebe
                //               select new EmployeeSimpleModel
                //               {
                //                   Ime = PrijOseba.Ime,
                //                   Priimek = PrijOseba.Priimek
                //               }).FirstOrDefault().Priimek,

            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<ClientCategorieModel> GetClientCategorieModelList(int clientID)
        {
            try
            {
                var query = from cC in context.StrankaKategorija
                            where cC.idStranka.Equals(clientID)
                            group cC by cC into clientCategorie
                            select new ClientCategorieModel
                            {
                                idStrankaKategorija = clientCategorie.Key.idStrankaKategorija,
                                idKategorija = clientCategorie.Key.idKategorija,
                                idStranka = clientCategorie.Key.idStranka,
                                ts = clientCategorie.Key.ts.HasValue ? clientCategorie.Key.ts.Value : DateTime.MinValue,
                                tsIDOseba = clientCategorie.Key.tsIDOseba.HasValue ? clientCategorie.Key.tsIDOseba.Value : 0,
                                Stranka = clientCategorie.Key.Stranka.NazivPrvi,
                                HasChartDataForCategorie = false,
                                Kategorija = (from categorie in clientCategorie
                                              group categorie by categorie.Kategorija into cat
                                              select new CategorieModel
                                              {
                                                  idKategorija = cat.Key.idKategorija,
                                                  Koda = cat.Key.Koda,
                                                  Naziv = cat.Key.Naziv,
                                                  ts = cat.Key.ts.HasValue ? cat.Key.ts.Value : DateTime.MinValue,
                                                  tsIDOsebe = cat.Key.tsIDOsebe.HasValue ? cat.Key.tsIDOsebe.Value : 0
                                              }).FirstOrDefault()
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public int SaveClient(ClientFullModel model, bool updateRecord = true)
        {
            try
            {
                Stranka client = new Stranka();
                client.idStranka = model.idStranka;
                client.KodaStranke = model.KodaStranke;
                client.NazivPrvi = model.NazivPrvi;
                client.NazivDrugi = model.NazivDrugi;
                client.Naslov = model.Naslov;
                client.StevPoste = model.StevPoste;
                client.NazivPoste = model.NazivPoste;
                client.Email = model.Email;
                client.Telefon = model.Telefon;
                client.FAX = model.FAX;
                client.InternetniNalov = model.InternetniNalov;
                client.KontaktnaOseba = model.KontaktnaOseba;
                client.RokPlacila = model.RokPlacila;
                client.RangStranke = model.RangStranke;
                client.AKTIVNOST = model.Aktivnost;
                client.LastStatusID = model.LastStatusDogodekID;
                client.tsLastStatus = model.tsLastStatus;
                /*client.ts = model.ts;
                client.tsIDOsebe = model.tsIDOsebe;*/

                client.Dogodek = new List<Dogodek>();
                client.KontaktneOsebe = new List<KontaktneOsebe>();
                client.Nadzor = new List<Nadzor>();
                client.PlanStranka = new List<PlanStranka>();
                client.StrankaZaposleni = new List<StrankaZaposleni>();

                if (client.idStranka == 0)
                {
                    client.ts = DateTime.Now;
                    client.tsIDOsebe = model.tsIDOsebe;
                    client.StrankaKategorija = GetClientCategories(client.idStranka, model.tsIDOsebe);
                    context.Stranka.Add(client);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        Stranka original = context.Stranka.Where(s => s.idStranka == client.idStranka).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(client);
                        context.SaveChanges();
                    }
                }

                if (model.StrankaZaposleni.Count > 0)
                {
                    model.StrankaZaposleni[0].idStranka = client.idStranka;

                    if (model.StrankaZaposleni[0].idStrankaOsebe == 0)
                        SaveClientEmployee(model.StrankaZaposleni[0], false);
                    else
                        SaveClientEmployee(model.StrankaZaposleni[0]);
                }

                return client.idStranka;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public bool DeleteClient(int clientID)
        {
            try
            {
                var client = context.Stranka.Where(s => s.idStranka == clientID).FirstOrDefault();

                if (client != null)
                {
                    context.Stranka.Remove(client);
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


        public int SaveClientPlan(PlanModel model, bool updateRecord = true)
        {
            PlanStranka plan = new PlanStranka();

            try
            {

                plan.idPlanStranka = model.idPlan;
                plan.idKategorija = model.idKategorija;
                plan.IDStranka = model.IDStranka;
                plan.LetniZnesek = model.LetniZnesek;
                plan.Leto = model.Leto;
                /*plan.ts = model.ts;
                plan.tsIDOsebe = model.tsIDOsebe;*/

                if (plan.idPlanStranka == 0)
                {

                    plan.ts = DateTime.Now;
                    plan.tsIDOsebe = model.tsIDOsebe;

                    context.PlanStranka.Add(plan);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        PlanStranka original = context.PlanStranka.Where(p => p.idPlanStranka == plan.idPlanStranka).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(plan);
                        context.SaveChanges();
                    }
                }

                return plan.idPlanStranka;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }


        public bool DeletePlan(int planID, int clientID)
        {
            try
            {
                var plan = context.PlanStranka.Where(p => p.idPlanStranka == planID && p.IDStranka == clientID).FirstOrDefault();

                if (plan != null)
                {
                    context.PlanStranka.Remove(plan);
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


        public int SaveContactPerson(ContactPersonModel model, bool updateRecord = true)
        {
            KontaktneOsebe kontOseba = new KontaktneOsebe();

            try
            {
                kontOseba.idKontaktneOsebe = model.idKontaktneOsebe;
                kontOseba.idStranka = model.idStranka;
                kontOseba.NazivKontaktneOsebe = model.NazivKontaktneOsebe;
                kontOseba.Telefon = model.Telefon;
                kontOseba.GSM = model.GSM;
                kontOseba.Email = model.Email;
                kontOseba.DelovnoMesto = model.DelovnoMesto;
                kontOseba.ZaporednaStevika = model.ZaporednaStevika;
                kontOseba.Fax = model.Fax;
                kontOseba.Opombe = model.Opombe;
                kontOseba.RojstniDatum = model.RojstniDatum;

                if (kontOseba.idKontaktneOsebe == 0)
                {

                    kontOseba.ts = DateTime.Now;
                    kontOseba.tsIDOsebe = model.tsIDOsebe;

                    context.KontaktneOsebe.Add(kontOseba);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        KontaktneOsebe original = context.KontaktneOsebe.Where(ko => ko.idKontaktneOsebe == kontOseba.idKontaktneOsebe).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(kontOseba);
                        context.SaveChanges();
                    }
                }

                return kontOseba.idKontaktneOsebe;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public bool DeleteContactPerson(int contactPersonID, int clientID)
        {
            try
            {
                var contactPerson = context.KontaktneOsebe.Where(ko => ko.idKontaktneOsebe == contactPersonID && ko.idStranka == clientID).FirstOrDefault();

                if (contactPerson != null)
                {
                    context.KontaktneOsebe.Remove(contactPerson);
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


        public List<CategorieModel> GetCategories()
        {
            try
            {
                var query = from categorie in context.Kategorija
                            where categorie.idKategorija.Equals(categorie.idKategorija)
                            select new CategorieModel
                            {
                                idKategorija = categorie.idKategorija,
                                Koda = categorie.Koda,
                                Naziv = categorie.Naziv,
                                ts = categorie.ts.HasValue ? categorie.ts.Value : DateTime.MinValue,
                                tsIDOsebe = categorie.tsIDOsebe.HasValue ? categorie.tsIDOsebe.Value : 0
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        private List<StrankaKategorija> GetClientCategories(int clientID, int employeeID)
        {
            try
            {
                List<CategorieModel> list = GetCategories();
                List<StrankaKategorija> returnList = new List<StrankaKategorija>();
                foreach (var item in list)
                {
                    StrankaKategorija kat = new StrankaKategorija();
                    kat.idKategorija = item.idKategorija;
                    kat.idStranka = clientID;
                    kat.ts = DateTime.Now;
                    kat.tsIDOseba = employeeID;
                    
                    returnList.Add(kat);
                }

                return returnList;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<CategorieModel> GetClientFreeCategories(int clientID, int catToSkip = 0)
        {
            try
            {
                // context.Kategorija.Where(cat => !context.StrankaKategorija.Any(sK => sK.idKategorija == cat.idKategorija && sK.idStranka == clientID)).ToList();
                var query = from categorie in context.Kategorija
                            where !context.StrankaKategorija.Any(sK => sK.idKategorija == categorie.idKategorija && sK.idStranka == clientID)
                            select new CategorieModel
                            {
                                idKategorija = categorie.idKategorija,
                                Koda = categorie.Koda,
                                Naziv = categorie.Naziv,
                                ts = categorie.ts.HasValue ? categorie.ts.Value : DateTime.MinValue,
                                tsIDOsebe = categorie.tsIDOsebe.HasValue ? categorie.tsIDOsebe.Value : 0
                            };

                List<CategorieModel> list = query.ToList();

                if (catToSkip > 0)
                    list.Add(GetCategorieByID(catToSkip));

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public CategorieModel GetCategorieByID(int categorieID)
        {
            try
            {
                var query = from categorie in context.Kategorija
                            where categorie.idKategorija.Equals(categorieID)
                            select new CategorieModel
                            {
                                idKategorija = categorie.idKategorija,
                                Koda = categorie.Koda,
                                Naziv = categorie.Naziv,
                                ts = categorie.ts.HasValue ? categorie.ts.Value : DateTime.MinValue,
                                tsIDOsebe = categorie.tsIDOsebe.HasValue ? categorie.tsIDOsebe.Value : 0
                            };

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }


        public int SaveClientEmployee(ClientEmployeeModel model, bool updateRecord = true)
        {
            StrankaZaposleni strZap = new StrankaZaposleni();

            try
            {
                strZap.idStrankaOsebe = model.idStrankaOsebe;
                strZap.idStranka = model.idStranka;
                strZap.idOsebe = model.idOsebe;

                if (strZap.idStrankaOsebe == 0)
                {

                    strZap.ts = DateTime.Now;
                    strZap.tsIDOsebe = model.tsIDOsebe;

                    context.StrankaZaposleni.Add(strZap);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        StrankaZaposleni original = context.StrankaZaposleni.Where(sz => sz.idStrankaOsebe == strZap.idStrankaOsebe).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(strZap);
                        context.SaveChanges();
                    }
                }

                return strZap.idStrankaOsebe;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public bool DeleteClientEmployee(int clientID, int employeeID)
        {
            try
            {
                var clientEmployee = context.StrankaZaposleni.Where(sz => sz.idStranka == clientID && sz.idOsebe == employeeID).FirstOrDefault();

                if (clientEmployee != null)
                {
                    context.StrankaZaposleni.Remove(clientEmployee);
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


        public bool ClientEmployeeExist(int clientID, int employeeID)
        {
            try
            {
                var clientEmployee = context.StrankaZaposleni.Where(sz => sz.idStranka == clientID && sz.idOsebe == employeeID).FirstOrDefault();

                if (clientEmployee != null)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public int SaveDevice(DevicesModel model, bool updateRecord = true)
        {
            Naprava device = new Naprava();

            try
            {
                device.idNaprava = model.idNaprava;
                device.idStranka = model.idStranka;
                device.Koda = model.Koda;
                device.Naziv = model.Naziv;
                device.Opis = model.Opis;

                if (device.idNaprava == 0)
                {
                    device.ts = DateTime.Now;
                    device.tsIDOsebe = model.tsIDOsebe;
                    context.Naprava.Add(device);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        Naprava original = context.Naprava.Where(n => n.idNaprava == device.idNaprava).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(device);
                        context.SaveChanges();
                    }
                }

                return device.idNaprava;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public bool DeleteDevice(int deviceID, int clientID)
        {
            try
            {
                var device = context.Naprava.Where(n => n.idNaprava == deviceID && n.idStranka == clientID).FirstOrDefault();

                if (device != null)
                {
                    context.Naprava.Remove(device);
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

        public int GetClientsCountByCode(string code)
        {
            try
            {
                code = code.Substring(0, 3);
                int result = 0;
                var query = context.Stranka.Where(np => np.KodaStranke.StartsWith(code)).ToList();
                foreach (var item in query)
                {
                    int clientCode = 0;
                    int.TryParse(item.KodaStranke.Substring(3), out clientCode);

                    if (clientCode > result)
                        result = clientCode;

                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public int SaveNotes(NotesModel model, bool updateRecord = true)
        {
            OpombaStranka notes = new OpombaStranka();

            try
            {
                notes.idOpombaStranka = model.idOpombaStranka;
                notes.idStranka = model.idStranka;
                notes.Opomba = model.Opomba;
         
                if (notes.idOpombaStranka == 0)
                {
                    notes.ts = DateTime.Now;
                    notes.tsIDOsebe = model.tsIDOsebe;
                    context.OpombaStranka.Add(notes);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        OpombaStranka original = context.OpombaStranka.Where(n => n.idOpombaStranka == notes.idOpombaStranka).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(notes);
                        context.SaveChanges();
                    }
                }

                return notes.idOpombaStranka;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public bool DeleteNotes(int NotesID, int clientID)
        {
            try
            {
                var notes = context.OpombaStranka.Where(n => n.idOpombaStranka == NotesID && n.idStranka == clientID).FirstOrDefault();

                if (notes != null)
                {
                    context.OpombaStranka.Remove(notes);
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

        public int GetNotesCountByCode(string code)
        {
            try
            {
                code = code.Substring(0, 3);
                int result = 0;
                var query = context.Stranka.Where(np => np.KodaStranke.StartsWith(code)).ToList();
                foreach (var item in query)
                {
                    int clientCode = 0;
                    int.TryParse(item.KodaStranke.Substring(3), out clientCode);

                    if (clientCode > result)
                        result = clientCode;

                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public int GetDevicesCountByCode(string code)
        {
            try
            {
                code = code.Substring(0, 3);
                int result = 0;
                var query = context.Naprava.Where(np => np.Koda.StartsWith(code)).ToList();

                foreach (var item in query)
                {
                    int deviceCode = Convert.ToInt32(item.Koda.Substring(3));
                    if (deviceCode > result)
                        result = deviceCode;

                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public int SaveClientCategorie(ClientCategorieModel model, bool updateRecord = true)
        {
            StrankaKategorija clientCat = new StrankaKategorija();

            try
            {
                clientCat.idStrankaKategorija = model.idStrankaKategorija;
                clientCat.idKategorija = model.idKategorija;
                clientCat.idStranka = model.idStranka;

                if (clientCat.idStrankaKategorija == 0)
                {

                    clientCat.ts = DateTime.Now;
                    clientCat.tsIDOseba = model.tsIDOseba;

                    context.StrankaKategorija.Add(clientCat);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        StrankaKategorija original = context.StrankaKategorija.Where(sK => sK.idStrankaKategorija == clientCat.idStrankaKategorija).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(clientCat);
                        context.SaveChanges();
                    }
                }

                return clientCat.idStrankaKategorija;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }


        public bool DeleteClientCategorie(int clientID, int clientCategorieID)
        {
            try
            {
                var clientCategorie = context.StrankaKategorija.Where(sK => sK.idStranka == clientID && sK.idStrankaKategorija == clientCategorieID).FirstOrDefault();

                if (clientCategorie != null)
                {
                    context.StrankaKategorija.Remove(clientCategorie);
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

        public List<ClientSimpleModel> FilterClientsByPropertyNames(string propertyName, string containsValue)
        {
            try
            {
                var clients = context.Stranka.Where(LinqExpressionHelper.GetContainsExpression<Stranka>(propertyName, containsValue)).ToList();

                var query = from client in clients
                            where client.idStranka.Equals(client.idStranka)
                            select new ClientSimpleModel
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
                                GenerirajERacun = client.GenerirajERacun.HasValue ? client.GenerirajERacun.Value : 0,
                                JavniZavod = client.JavniZavod.HasValue ? client.JavniZavod.Value : 0,
                                SecondID = client.SecondID.HasValue ? client.SecondID.Value : 0,
                                ts = client.ts.HasValue ? client.ts.Value : DateTime.MinValue,
                                tsIDOsebe = client.tsIDOsebe.HasValue ? client.tsIDOsebe.Value : 0,
                                Aktivnost = client.AKTIVNOST.HasValue ? client.AKTIVNOST.Value : 0,
                                LastStatusDogodekID = client.LastStatusID.HasValue ? client.LastStatusID.Value : 0,
                                tsLastStatus = client.tsLastStatus.HasValue ? client.tsLastStatus.Value : DateTime.MinValue
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }
    }
}