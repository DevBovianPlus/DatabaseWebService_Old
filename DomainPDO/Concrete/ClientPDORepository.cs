using DatabaseWebService.DomainPDO.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsOTP.Client;
using DatabaseWebService.ModelsPDO;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.DomainPDO.Concrete
{
    public class ClientPDORepository : IClientPDORepository
    {
        GrafolitPDOEntities context;

        public ClientPDORepository(GrafolitPDOEntities _context)
        {
            context = _context;
        }

        public List<ClientSimpleModel> GetAllClients()
        {
            try
            {//loj => left outer join
                var query = from client in context.Stranka_PDO
                            select new ClientSimpleModel
                            {
                                idStranka = client.StrankaID,
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
                                StatusDomacTuji = ((client.StatusDomacTuji.HasValue && client.StatusDomacTuji.Value) ? "DOMAČ" : ""),
                                Zavezanec_DA_NE = ((client.Zavezanec_DA_NE.HasValue && client.Zavezanec_DA_NE.Value) ? "DA" : "NE"),
                                IdentifikacijskaStev = client.IdentifikacijskaStev,
                                Clan_EU = ((client.Clan_EU.HasValue && client.Clan_EU.Value) ? "DA" : "NE"),
                                BIC = client.BIC,
                                KodaPlacila = client.KodaPlacila,
                                DrzavaStranke = client.DrzavaStranke,
                                Neaktivna = client.Neaktivna,
                                GenerirajERacun = client.GenerirajERacun.HasValue ? client.GenerirajERacun.Value : 0,
                                JavniZavod = client.JavniZavod.HasValue ? client.JavniZavod.Value : 0,
                                ts = client.ts.HasValue ? client.ts.Value : DateTime.MinValue,
                                tsIDOsebe = client.tsIDOsebe.HasValue ? client.tsIDOsebe.Value : 0,
                                //idOsebe = (subClient == null ? 0 : subClient.Osebe_PDO.OsebaID),
                                //ImeInPriimekZaposlen = (subClient == null ? String.Empty : subClient.Osebe_PDO.Ime + " " + subClient.Osebe_PDO.Priimek),
                                TipStranka = client.TipStranka_PDO != null ? client.TipStranka_PDO.Naziv : "",
                                Jezik = client.Jeziki != null ? client.Jeziki.Naziv : ""
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
                var query = from client in context.Stranka_PDO
                            join clientEmployee in context.StrankaZaposleni_PDO
                            on client.StrankaID equals clientEmployee.StrankaID
                            where clientEmployee.OsebeID == employeeID
                            select new ClientSimpleModel
                            {
                                idStranka = client.StrankaID,
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
                                StatusDomacTuji = ((client.StatusDomacTuji.HasValue && client.StatusDomacTuji.Value) ? "DOMAČ" : ""),
                                Zavezanec_DA_NE = ((client.Zavezanec_DA_NE.HasValue && client.Zavezanec_DA_NE.Value) ? "DA" : "NE"),
                                IdentifikacijskaStev = client.IdentifikacijskaStev,
                                Clan_EU = ((client.Clan_EU.HasValue && client.Clan_EU.Value) ? "DA" : "NE"),
                                BIC = client.BIC,
                                KodaPlacila = client.KodaPlacila,
                                DrzavaStranke = client.DrzavaStranke,
                                Neaktivna = client.Neaktivna,
                                GenerirajERacun = client.GenerirajERacun.HasValue ? client.GenerirajERacun.Value : 0,
                                JavniZavod = client.JavniZavod.HasValue ? client.JavniZavod.Value : 0,
                                ts = client.ts.HasValue ? client.ts.Value : DateTime.MinValue,
                                tsIDOsebe = client.tsIDOsebe.HasValue ? client.tsIDOsebe.Value : 0,
                                //                                ImeInPriimekZaposlen = (subClient == null ? String.Empty : subClient.Osebe_OTP.Ime + " " + subClient.Osebe_OTP.Priimek),
                                TipStranka = client.TipStranka_PDO != null ? client.TipStranka_PDO.Naziv : "",
                                Jezik = client.Jeziki != null ? client.TipStranka_PDO.Naziv : ""
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
                var query = from client in context.Stranka_PDO
                            where client.StrankaID.Equals(clientID)
                            select new ClientFullModel
                            {
                                idStranka = client.StrankaID,
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
                                StatusDomacTuji = ((client.StatusDomacTuji.HasValue && client.StatusDomacTuji.Value) ? "DOMAČ" : ""),
                                Zavezanec_DA_NE = ((client.Zavezanec_DA_NE.HasValue && client.Zavezanec_DA_NE.Value) ? "DA" : "NE"),
                                IdentifikacijskaStev = client.IdentifikacijskaStev,
                                Clan_EU = ((client.Clan_EU.HasValue && client.Clan_EU.Value) ? "DA" : "NE"),
                                BIC = client.BIC,
                                KodaPlacila = client.KodaPlacila,
                                DrzavaStranke = client.DrzavaStranke,
                                Neaktivna = client.Neaktivna,
                                PrivzetaEM = client.PrivzetaEM,
                                ZadnjaIzbranaKategorija = client.ZadnjaIzbranaKategorija,
                                GenerirajERacun = client.GenerirajERacun.HasValue ? client.GenerirajERacun.Value : 0,
                                JavniZavod = client.JavniZavod.HasValue ? client.JavniZavod.Value : 0,
                                ts = client.ts.HasValue ? client.ts.Value : DateTime.MinValue,
                                tsIDOsebe = client.tsIDOsebe.HasValue ? client.tsIDOsebe.Value : 0,
                                //TODO: Add collections of Dogodek, KontaktneOsebe, Nadzor, Plan, StrankaZaposleni
                                TipStrankaID = client.TipStrankaID,
                                TipStranka = (from clientType in context.TipStranka_PDO
                                              where clientType.TipStrankaID == client.TipStrankaID
                                              select new ClientType
                                              {
                                                  Koda = clientType.Koda,
                                                  Naziv = clientType.Naziv,
                                                  Opis = clientType.Opis,
                                                  TipStrankaID = clientType.TipStrankaID,
                                                  ts = clientType.ts.HasValue ? clientType.ts.Value : DateTime.MinValue,
                                                  tsIDOseba = clientType.tsIDOseba.HasValue ? clientType.tsIDOseba.Value : 0
                                              }).FirstOrDefault(),
                                JezikID = client.JezikID.HasValue ? client.JezikID.Value : 0,
                                Jezik = (from jez in context.Jeziki
                                         where jez.JezikID == client.JezikID
                                         select new LanguageModel
                                         {
                                             Koda = jez.Koda,
                                             Naziv = jez.Naziv,
                                             ts = jez.ts.HasValue ? jez.ts.Value : DateTime.MinValue,
                                         }).FirstOrDefault(),
                            };

                ClientFullModel model = query.FirstOrDefault();
                model.KontaktneOsebe = new List<ContactPersonModel>();
                model.KontaktneOsebe = GetContactPersonModelList(clientID);


                model.StrankaZaposleni = new List<ClientEmployeeModel>();
                model.StrankaZaposleni = GetClientEmployeeModelList(clientID);

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public ClientFullModel GetClientByCode(string sKoda)
        {
            try
            {
                var query = from client in context.Stranka_PDO
                            where client.KodaStranke.Equals(sKoda)
                            select new ClientFullModel
                            {
                                idStranka = client.StrankaID,
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
                                StatusDomacTuji = ((client.StatusDomacTuji.HasValue && client.StatusDomacTuji.Value) ? "DOMAČ" : ""),
                                Zavezanec_DA_NE = ((client.Zavezanec_DA_NE.HasValue && client.Zavezanec_DA_NE.Value) ? "DA" : "NE"),
                                IdentifikacijskaStev = client.IdentifikacijskaStev,
                                Clan_EU = ((client.Clan_EU.HasValue && client.Clan_EU.Value) ? "DA" : "NE"),
                                BIC = client.BIC,
                                KodaPlacila = client.KodaPlacila,
                                DrzavaStranke = client.DrzavaStranke,
                                Neaktivna = client.Neaktivna,
                                PrivzetaEM = client.PrivzetaEM,
                                ZadnjaIzbranaKategorija = client.ZadnjaIzbranaKategorija,
                                GenerirajERacun = client.GenerirajERacun.HasValue ? client.GenerirajERacun.Value : 0,
                                JavniZavod = client.JavniZavod.HasValue ? client.JavniZavod.Value : 0,
                                ts = client.ts.HasValue ? client.ts.Value : DateTime.MinValue,
                                tsIDOsebe = client.tsIDOsebe.HasValue ? client.tsIDOsebe.Value : 0,
                                //TODO: Add collections of Dogodek, KontaktneOsebe, Nadzor, Plan, StrankaZaposleni
                                TipStrankaID = client.TipStrankaID,
                                TipStranka = (from clientType in context.TipStranka_PDO
                                              where clientType.TipStrankaID == client.TipStrankaID
                                              select new ClientType
                                              {
                                                  Koda = clientType.Koda,
                                                  Naziv = clientType.Naziv,
                                                  Opis = clientType.Opis,
                                                  TipStrankaID = clientType.TipStrankaID,
                                                  ts = clientType.ts.HasValue ? clientType.ts.Value : DateTime.MinValue,
                                                  tsIDOseba = clientType.tsIDOseba.HasValue ? clientType.tsIDOseba.Value : 0
                                              }).FirstOrDefault(),
                                JezikID = client.JezikID.HasValue ? client.JezikID.Value : 0,
                                Jezik = (from jez in context.Jeziki
                                         where jez.JezikID == client.JezikID
                                         select new LanguageModel
                                         {
                                             Koda = jez.Koda,
                                             Naziv = jez.Naziv,
                                             ts = jez.ts.HasValue ? jez.ts.Value : DateTime.MinValue,
                                         }).FirstOrDefault(),
                            };

                ClientFullModel model = query.FirstOrDefault();
                model.KontaktneOsebe = new List<ContactPersonModel>();

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
                var query = from client in context.Stranka_PDO
                            join clientEmployee in context.StrankaZaposleni_PDO
                            on client.StrankaID equals clientEmployee.StrankaID
                            where client.StrankaID.Equals(clientID) && clientEmployee.OsebeID.Equals(employeeID)
                            select new ClientFullModel
                            {
                                idStranka = client.StrankaID,
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
                                StatusDomacTuji = ((client.StatusDomacTuji.HasValue && client.StatusDomacTuji.Value) ? "DOMAČ" : ""),
                                Zavezanec_DA_NE = ((client.Zavezanec_DA_NE.HasValue && client.Zavezanec_DA_NE.Value) ? "DA" : "NE"),
                                IdentifikacijskaStev = client.IdentifikacijskaStev,
                                Clan_EU = ((client.Clan_EU.HasValue && client.Clan_EU.Value) ? "DA" : "NE"),
                                BIC = client.BIC,
                                KodaPlacila = client.KodaPlacila,
                                DrzavaStranke = client.DrzavaStranke,
                                Neaktivna = client.Neaktivna,
                                GenerirajERacun = client.GenerirajERacun.HasValue ? client.GenerirajERacun.Value : 0,
                                JavniZavod = client.JavniZavod.HasValue ? client.JavniZavod.Value : 0,
                                ts = client.ts.HasValue ? client.ts.Value : DateTime.MinValue,
                                tsIDOsebe = client.tsIDOsebe.HasValue ? client.tsIDOsebe.Value : 0,
                                //TODO: Add collections of Dogodek, KontaktneOsebe, Nadzor, Plan, StrankaZaposleni
                                TipStrankaID = client.TipStrankaID,
                                TipStranka = (from clientType in context.TipStranka_PDO
                                              where clientType.TipStrankaID == client.TipStrankaID
                                              select new ClientType
                                              {
                                                  Koda = clientType.Koda,
                                                  Naziv = clientType.Naziv,
                                                  Opis = clientType.Opis,
                                                  TipStrankaID = clientType.TipStrankaID,
                                                  ts = clientType.ts.HasValue ? clientType.ts.Value : DateTime.MinValue,
                                                  tsIDOseba = clientType.tsIDOseba.HasValue ? clientType.tsIDOseba.Value : 0
                                              }).FirstOrDefault(),
                                JezikID = client.JezikID.HasValue ? client.JezikID.Value : 0,
                                Jezik = (from jez in context.Jeziki
                                         where jez.JezikID == client.JezikID
                                         select new LanguageModel
                                         {
                                             Koda = jez.Koda,
                                             Naziv = jez.Naziv,
                                             ts = jez.ts.HasValue ? jez.ts.Value : DateTime.MinValue,
                                         }).FirstOrDefault(),
                            };

                ClientFullModel model = query.FirstOrDefault();
                if (model != null)
                {
                    model.KontaktneOsebe = new List<ContactPersonModel>();
                    model.KontaktneOsebe = GetContactPersonModelList(clientID);

                    model.StrankaZaposleni = new List<ClientEmployeeModel>();
                    model.StrankaZaposleni = GetClientEmployeeModelList(clientID);
                }

                return model;
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
                Stranka_PDO client = new Stranka_PDO();
                client.StrankaID = model.idStranka;
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
                client.ts = model.ts.CompareTo(DateTime.MinValue) == 0 ? (DateTime?)null : model.ts;
                client.tsIDOsebe = model.tsIDOsebe;
                client.TipStrankaID = model.TipStrankaID;
                client.JezikID = model.JezikID;
                client.PrivzetaEM = model.PrivzetaEM;
                client.ZadnjaIzbranaKategorija = model.ZadnjaIzbranaKategorija;
                client.KontaktnaOseba_PDO = new List<KontaktnaOseba_PDO>();
                client.StrankaZaposleni_PDO = new List<StrankaZaposleni_PDO>();
                client.tsUpdate = DateTime.Now;
                //client.tsUpdateUserID = model.tsIDOsebe;

                if (client.StrankaID == 0)
                {
                    client.ts = DateTime.Now;
                    client.tsIDOsebe = model.tsIDOsebe;
                    context.Stranka_PDO.Add(client);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        Stranka_PDO original = context.Stranka_PDO.Where(s => s.StrankaID == client.StrankaID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(client);
                        context.SaveChanges();
                    }
                }

                if ((model.StrankaZaposleni != null) && (model.StrankaZaposleni.Count > 0))
                {
                    model.StrankaZaposleni[0].idStranka = client.StrankaID;

                    if (model.StrankaZaposleni[0].idStrankaOsebe == 0)
                        SaveClientEmployee(model.StrankaZaposleni[0], false);
                    else
                        SaveClientEmployee(model.StrankaZaposleni[0]);
                }

                return client.StrankaID;
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
                var client = context.Stranka_PDO.Where(s => s.StrankaID == clientID).FirstOrDefault();

                if (client != null)
                {
                    context.Stranka_PDO.Remove(client);
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

        public ContactPersonModel GetContactPersonByID(int contactPersonID)
        {
            try
            {
                var query = from contact in context.KontaktnaOseba_PDO
                            where contact.KontaktnaOsebaID == contactPersonID
                            select new ContactPersonModel
                            {
                                idKontaktneOsebe = contact.KontaktnaOsebaID,
                                idStranka = contact.StrankaID.HasValue ? contact.StrankaID.Value : 0,
                                NazivKontaktneOsebe = contact.Naziv,
                                NazivPodpis = contact.NazivPodpis,
                                Stranka = contact.Stranka_PDO.NazivPrvi,
                                Telefon = contact.Telefon,
                                GSM = contact.GSM,
                                Email = contact.Email,
                                ts = contact.ts.HasValue ? contact.ts.Value : DateTime.MinValue,
                                tsIDOsebe = contact.tsIDOsebe.HasValue ? contact.tsIDOsebe.Value : 0,
                                Fax = contact.Fax,
                                Opombe = contact.Opombe
                            };

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public int SaveContactPerson(ContactPersonModel model, bool updateRecord = true)
        {
            KontaktnaOseba_PDO kontOseba = new KontaktnaOseba_PDO();

            try
            {
                kontOseba.KontaktnaOsebaID = model.idKontaktneOsebe;
                kontOseba.StrankaID = model.idStranka;
                kontOseba.Naziv = model.NazivKontaktneOsebe;
                kontOseba.NazivPodpis = model.NazivPodpis;
                kontOseba.Telefon = model.Telefon;
                kontOseba.GSM = model.GSM;
                kontOseba.Email = model.Email;
                kontOseba.ts = model.ts.Equals(DateTime.MinValue) ? (DateTime?)null : model.ts;
                kontOseba.tsIDOsebe = model.tsIDOsebe;
                kontOseba.tsUpdate = DateTime.Now;
                kontOseba.tsUpdateUserID = model.tsIDOsebe;
                kontOseba.Fax = model.Fax;
                kontOseba.Opombe = model.Opombe;
                kontOseba.IsNabava = model.IsNabava;

                if (kontOseba.KontaktnaOsebaID == 0)
                {

                    kontOseba.ts = DateTime.Now;
                    kontOseba.tsIDOsebe = model.tsIDOsebe;

                    context.KontaktnaOseba_PDO.Add(kontOseba);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        KontaktnaOseba_PDO original = context.KontaktnaOseba_PDO.Where(ko => ko.KontaktnaOsebaID == kontOseba.KontaktnaOsebaID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(kontOseba);
                        context.SaveChanges();
                    }
                }

                return kontOseba.KontaktnaOsebaID;
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
                var contactPerson = context.KontaktnaOseba_PDO.Where(ko => ko.KontaktnaOsebaID == contactPersonID && ko.StrankaID == clientID).FirstOrDefault();

                if (contactPerson != null)
                {
                    context.KontaktnaOseba_PDO.Remove(contactPerson);
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

        public List<ClientEmployeeModel> GetClientEmployeeModelList(int clientID)
        {
            try
            {
                var query = from cE in context.StrankaZaposleni_PDO
                            where cE.StrankaID.Equals(clientID)
                            group cE by cE into clientEmployee
                            select new ClientEmployeeModel
                            {
                                idOsebe = clientEmployee.Key.OsebeID,
                                idStranka = clientEmployee.Key.StrankaID,
                                idStrankaOsebe = clientEmployee.Key.StrankaOsebeID,
                                ts = clientEmployee.Key.ts.HasValue ? clientEmployee.Key.ts.Value : DateTime.MinValue,
                                tsIDOsebe = clientEmployee.Key.tsIDOsebe.HasValue ? clientEmployee.Key.tsIDOsebe.Value : 0,
                                oseba = (from employee in clientEmployee
                                         group employee by employee.Osebe_PDO into person
                                         select new EmployeeSimpleModel
                                         {

                                             DatumRojstva = person.Key.DatumRojstva.HasValue ? person.Key.DatumZaposlitve.Value : DateTime.MinValue,
                                             DatumZaposlitve = person.Key.DatumZaposlitve.HasValue ? person.Key.DatumZaposlitve.Value : DateTime.MinValue,
                                             DelovnoMesto = person.Key.DelovnoMesto,
                                             Email = person.Key.Email,
                                             Geslo = person.Key.Geslo,
                                             idOsebe = person.Key.OsebaID,
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

        public int SaveClientEmployee(ClientEmployeeModel model, bool updateRecord = true)
        {
            StrankaZaposleni_PDO strZap = new StrankaZaposleni_PDO();

            try
            {
                strZap.StrankaOsebeID = model.idStrankaOsebe;
                strZap.StrankaID = model.idStranka;
                strZap.OsebeID = model.idOsebe;

                if (strZap.StrankaOsebeID == 0)
                {

                    strZap.ts = DateTime.Now;
                    strZap.tsIDOsebe = model.tsIDOsebe;

                    context.StrankaZaposleni_PDO.Add(strZap);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        StrankaZaposleni_PDO original = context.StrankaZaposleni_PDO.Where(sz => sz.StrankaOsebeID == strZap.StrankaOsebeID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(strZap);
                        context.SaveChanges();
                    }
                }

                return strZap.StrankaOsebeID;
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
                var clientEmployee = context.StrankaZaposleni_PDO.Where(sz => sz.StrankaID == clientID && sz.OsebeID == employeeID).FirstOrDefault();

                if (clientEmployee != null)
                {
                    context.StrankaZaposleni_PDO.Remove(clientEmployee);
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
                var clientEmployee = context.StrankaZaposleni_PDO.Where(sz => sz.StrankaID == clientID && sz.OsebeID == employeeID).FirstOrDefault();

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

        public List<ContactPersonModel> GetContactPersonModelList(int clientID)
        {
            try
            {
                var query = from contact in context.KontaktnaOseba_PDO
                            where contact.StrankaID.Value.Equals(clientID)
                            select new ContactPersonModel
                            {
                                idKontaktneOsebe = contact.KontaktnaOsebaID,
                                idStranka = contact.StrankaID.HasValue ? contact.StrankaID.Value : 0,
                                NazivKontaktneOsebe = contact.Naziv,
                                NazivPodpis = contact.NazivPodpis,
                                Stranka = contact.Stranka_PDO.NazivPrvi,
                                Telefon = contact.Telefon,
                                GSM = contact.GSM,
                                Email = contact.Email,
                                ts = contact.ts.HasValue ? contact.ts.Value : DateTime.MinValue,
                                tsIDOsebe = contact.tsIDOsebe.HasValue ? contact.tsIDOsebe.Value : 0,
                                Fax = contact.Fax,
                                Opombe = contact.Opombe,
                                IsNabava = contact.IsNabava.HasValue ? contact.IsNabava.Value : false

                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<ContactPersonModel> GetContactPersonModelListByName(string SupplierName)
        {
            try
            {
                var query = from contact in context.KontaktnaOseba_PDO
                            where contact.Naziv.Equals(SupplierName)
                            select new ContactPersonModel
                            {
                                idKontaktneOsebe = contact.KontaktnaOsebaID,
                                idStranka = contact.StrankaID.HasValue ? contact.StrankaID.Value : 0,
                                NazivKontaktneOsebe = contact.Naziv,
                                NazivPodpis = contact.NazivPodpis,
                                Stranka = contact.Stranka_PDO.NazivPrvi,
                                Telefon = contact.Telefon,
                                GSM = contact.GSM,
                                Email = contact.Email,
                                ts = contact.ts.HasValue ? contact.ts.Value : DateTime.MinValue,
                                tsIDOsebe = contact.tsIDOsebe.HasValue ? contact.tsIDOsebe.Value : 0,
                                Fax = contact.Fax,
                                Opombe = contact.Opombe
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<ClientSimpleModel> GetAllClientsByType(string typeCode, int employeeID = 0)
        {
            try
            {
                ClientType type = GetClientTypeByCode(typeCode);

                var query = from client in context.Stranka_PDO
                            join clientEmployee in context.StrankaZaposleni_PDO
                            on client.StrankaID equals clientEmployee.StrankaID
                            where clientEmployee.OsebeID == (employeeID > 0 ? employeeID : clientEmployee.OsebeID) && client.TipStrankaID == type.TipStrankaID
                            select new ClientSimpleModel
                            {
                                idStranka = client.StrankaID,
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
                                StatusDomacTuji = ((client.StatusDomacTuji.HasValue && client.StatusDomacTuji.Value) ? "DOMAČ" : ""),
                                Zavezanec_DA_NE = ((client.Zavezanec_DA_NE.HasValue && client.Zavezanec_DA_NE.Value) ? "DA" : "NE"),
                                IdentifikacijskaStev = client.IdentifikacijskaStev,
                                Clan_EU = ((client.Clan_EU.HasValue && client.Clan_EU.Value) ? "DA" : "NE"),
                                BIC = client.BIC,
                                KodaPlacila = client.KodaPlacila,
                                DrzavaStranke = client.DrzavaStranke,
                                Neaktivna = client.Neaktivna,
                                GenerirajERacun = client.GenerirajERacun.HasValue ? client.GenerirajERacun.Value : 0,
                                JavniZavod = client.JavniZavod.HasValue ? client.JavniZavod.Value : 0,
                                ts = client.ts.HasValue ? client.ts.Value : DateTime.MinValue,
                                tsIDOsebe = client.tsIDOsebe.HasValue ? client.tsIDOsebe.Value : 0,
                                //                                ImeInPriimekZaposlen = (subClient == null ? String.Empty : subClient.Osebe_OTP.Ime + " " + subClient.Osebe_OTP.Priimek),
                                TipStranka = client.TipStranka_PDO != null ? client.TipStranka_PDO.Naziv : "",
                                Jezik = client.Jeziki != null ? client.TipStranka_PDO.Naziv : ""
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public ClientType GetClientTypeByCode(string typeCode)
        {
            try
            {
                var query = from type in context.TipStranka_PDO
                            where type.Koda == typeCode
                            select new ClientType
                            {
                                Koda = type.Koda,
                                Naziv = type.Naziv,
                                Opis = type.Opis,
                                TipStrankaID = type.TipStrankaID,
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

        public ClientType GetClientTypeByID(int typeID)
        {
            try
            {
                var query = from type in context.TipStranka_PDO
                            where type.TipStrankaID == typeID
                            select new ClientType
                            {
                                Koda = type.Koda,
                                Naziv = type.Naziv,
                                Opis = type.Opis,
                                TipStrankaID = type.TipStrankaID,
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

        public List<ClientType> GetClientTypes()
        {
            try
            {
                var query = from type in context.TipStranka_PDO
                            select new ClientType
                            {
                                Koda = type.Koda,
                                Naziv = type.Naziv,
                                Opis = type.Opis,
                                TipStrankaID = type.TipStrankaID,
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

        public List<LanguageModel> GetLanguages()
        {
            try
            {
                var query = from type in context.Jeziki
                            select new LanguageModel
                            {
                                JezikID = type.JezikID,
                                Koda = type.Koda,
                                Naziv = type.Naziv,
                                ts = type.ts.HasValue ? type.ts.Value : DateTime.MinValue
                            };

                return query.ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<DepartmentModel> GetDepartments()
        {
            try
            {
                var query = from depart in context.Oddelek
                            select new DepartmentModel
                            {
                                OddelekID = depart.OddelekID,
                                Koda = depart.Koda,
                                Naziv = depart.Naziv,
                                ts = depart.ts.HasValue ? depart.ts.Value : DateTime.MinValue
                            };

                return query.ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public ClientFullModel GetClientByName(string clientName)
        {
            try
            {
                var query = from client in context.Stranka_PDO
                            where client.NazivPrvi.Equals(clientName)
                            select new ClientFullModel
                            {
                                idStranka = client.StrankaID,
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
                                StatusDomacTuji = ((client.StatusDomacTuji.HasValue && client.StatusDomacTuji.Value) ? "DOMAČ" : ""),
                                Zavezanec_DA_NE = ((client.Zavezanec_DA_NE.HasValue && client.Zavezanec_DA_NE.Value) ? "DA" : "NE"),
                                IdentifikacijskaStev = client.IdentifikacijskaStev,
                                Clan_EU = ((client.Clan_EU.HasValue && client.Clan_EU.Value) ? "DA" : "NE"),
                                BIC = client.BIC,
                                KodaPlacila = client.KodaPlacila,
                                DrzavaStranke = client.DrzavaStranke,
                                Neaktivna = client.Neaktivna,
                                PrivzetaEM = client.PrivzetaEM,
                                ZadnjaIzbranaKategorija = client.ZadnjaIzbranaKategorija,
                                GenerirajERacun = client.GenerirajERacun.HasValue ? client.GenerirajERacun.Value : 0,
                                JavniZavod = client.JavniZavod.HasValue ? client.JavniZavod.Value : 0,
                                ts = client.ts.HasValue ? client.ts.Value : DateTime.MinValue,
                                tsIDOsebe = client.tsIDOsebe.HasValue ? client.tsIDOsebe.Value : 0,
                                //TODO: Add collections of Dogodek, KontaktneOsebe, Nadzor, Plan, StrankaZaposleni
                                TipStrankaID = client.TipStrankaID,
                                TipStranka = (from clientType in context.TipStranka_PDO
                                              where clientType.TipStrankaID == client.TipStrankaID
                                              select new ClientType
                                              {
                                                  Koda = clientType.Koda,
                                                  Naziv = clientType.Naziv,
                                                  Opis = clientType.Opis,
                                                  TipStrankaID = clientType.TipStrankaID,
                                                  ts = clientType.ts.HasValue ? clientType.ts.Value : DateTime.MinValue,
                                                  tsIDOseba = clientType.tsIDOseba.HasValue ? clientType.tsIDOseba.Value : 0
                                              }).FirstOrDefault(),
                                JezikID = client.JezikID.HasValue ? client.JezikID.Value : 0,
                                Jezik = (from jez in context.Jeziki
                                         where jez.JezikID == client.JezikID
                                         select new LanguageModel
                                         {
                                             Koda = jez.Koda,
                                             Naziv = jez.Naziv,
                                             ts = jez.ts.HasValue ? jez.ts.Value : DateTime.MinValue,
                                         }).FirstOrDefault(),
                            };

                ClientFullModel model = query.FirstOrDefault();

                if (model != null)
                {
                    model.KontaktneOsebe = new List<ContactPersonModel>();
                    model.KontaktneOsebe = GetContactPersonModelList(model.idStranka);
                }

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public ClientFullModel GetClientByNameOrInsert(string clientName)
        {
            try
            {
                ClientFullModel client = GetClientByName(clientName);



                if (client == null)
                {
                    Stranka_PDO clientNew = new Stranka_PDO();
                    clientNew.NazivPrvi = clientName;
                    ClientType type = GetClientTypeByCode(Common.Enums.Enums.TypeOfClient.DOBAVITELJ.ToString());
                    clientNew.TipStrankaID = (type != null) ? type.TipStrankaID : 1;
                    context.Stranka_PDO.Add(clientNew);
                    context.SaveChanges();
                    client = GetClientByName(clientName);
                }

                return client;



            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_07, ex);
            }
        }
    }
}