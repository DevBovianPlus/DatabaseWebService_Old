using DatabaseWebService.Common.Enums;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsOTP;
using DatabaseWebService.ModelsOTP.Client;
using DatabaseWebService.ModelsOTP.Recall;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DatabaseWebService.DomainOTP.Concrete
{
    public class ClientOTPRepository : IClientOTPRepository
    {
        GrafolitOTPEntities context;

        public ClientOTPRepository(GrafolitOTPEntities _context)
        {
            context = _context;
        }

        public List<ClientSimpleModel> GetAllClients()
        {
            try
            {//loj => left outer join
                var query = from client in context.Stranka_OTP
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
                                ts = client.ts.HasValue ? client.ts.Value : DateTime.MinValue,
                                tsIDOsebe = client.tsIDOsebe.HasValue ? client.tsIDOsebe.Value : 0,
                                //idOsebe = (subClient == null ? 0 : subClient.Osebe_OTP.idOsebe),
                                //ImeInPriimekZaposlen = (subClient == null ? String.Empty : subClient.Osebe_OTP.Ime + " " + subClient.Osebe_OTP.Priimek),
                                TipStranka = client.TipStranka != null ? client.TipStranka.Naziv : "",
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
                var query = from client in context.Stranka_OTP
                            join clientEmployee in context.StrankaZaposleni_OTP
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
                                ts = client.ts.HasValue ? client.ts.Value : DateTime.MinValue,
                                tsIDOsebe = client.tsIDOsebe.HasValue ? client.tsIDOsebe.Value : 0,
                                //                                ImeInPriimekZaposlen = (subClient == null ? String.Empty : subClient.Osebe_OTP.Ime + " " + subClient.Osebe_OTP.Priimek),
                                TipStranka = client.TipStranka != null ? client.TipStranka.Naziv : "",
                                Jezik = client.Jeziki != null ? client.Jeziki.Naziv : ""
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
                var query = from client in context.Stranka_OTP
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
                                              }).FirstOrDefault(),
                                TipPrevozaID = client.TipPrevoza.HasValue ? client.TipPrevoza.Value : 0,
                                TipPrevoza = (from transportType in context.TipPrevoza
                                              where transportType.TipPrevozaID == client.TipPrevoza
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
                                JezikID = client.JezikID.HasValue ? client.JezikID.Value : 0,
                                JezikOTP = (from jez in context.Jeziki
                                         where jez.JezikID == client.JezikID
                                         select new LanguageModelOTP
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

        public ClientFullModel GetClientByID(int clientID, int employeeID)
        {
            try
            {
                var query = from client in context.Stranka_OTP
                            join clientEmployee in context.StrankaZaposleni_OTP
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
                                              }).FirstOrDefault(),
                                TipPrevozaID = client.TipPrevoza.HasValue ? client.TipPrevoza.Value : 0,
                                TipPrevoza = (from transportType in context.TipPrevoza
                                              where transportType.TipPrevozaID == client.TipPrevoza
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
                                JezikID = client.JezikID.HasValue ? client.JezikID.Value : 0,
                                JezikOTP = (from jez in context.Jeziki
                                            where jez.JezikID == client.JezikID
                                            select new LanguageModelOTP
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
                Stranka_OTP client = new Stranka_OTP();
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
                client.ts = model.ts.CompareTo(DateTime.MinValue) == 0 ? (DateTime?)null : model.ts;
                client.tsIDOsebe = model.tsIDOsebe;
                client.TipID = model.TipStrankaID;
                client.Activity = model.Activity;
                client.JezikID = model.JezikID;

                client.KontaktneOsebe_OTP = new List<KontaktneOsebe_OTP>();
                client.StrankaZaposleni_OTP = new List<StrankaZaposleni_OTP>();
                client.TipPrevoza = model.TipPrevozaID <= 0 ? (int?)null : model.TipPrevozaID;

                if (client.idStranka == 0)
                {
                    client.ts = DateTime.Now;
                    client.tsIDOsebe = model.tsIDOsebe;
                    context.Stranka_OTP.Add(client);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        Stranka_OTP original = context.Stranka_OTP.Where(s => s.idStranka == client.idStranka).FirstOrDefault();
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
                var client = context.Stranka_OTP.Where(s => s.idStranka == clientID).FirstOrDefault();

                if (client != null)
                {
                    context.Stranka_OTP.Remove(client);
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
            KontaktneOsebe_OTP kontOseba = new KontaktneOsebe_OTP();

            try
            {
                kontOseba.idKontaktneOsebe = model.idKontaktneOsebe;
                kontOseba.idStranka = model.idStranka;
                kontOseba.Naziv = model.NazivKontaktneOsebe;
                kontOseba.NazivKontaktneOsebe = model.NazivKontaktneOsebe;
                kontOseba.Telefon = model.Telefon;
                kontOseba.GSM = model.GSM;
                kontOseba.Email = model.Email;
                //kontOseba.DelovnoMesto = model.DelovnoMesto;
                //kontOseba.ZaporednaStevika = model.ZaporednaStevika;
                kontOseba.Fax = model.Fax;
                kontOseba.Opombe = model.Opombe;
                kontOseba.NazivPodpis = model.NazivPodpis;

                if (kontOseba.idKontaktneOsebe == 0)
                {

                    kontOseba.ts = DateTime.Now;
                    kontOseba.tsIDOsebe = model.tsIDOsebe;

                    context.KontaktneOsebe_OTP.Add(kontOseba);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        KontaktneOsebe_OTP original = context.KontaktneOsebe_OTP.Where(ko => ko.idKontaktneOsebe == kontOseba.idKontaktneOsebe).FirstOrDefault();
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
                var contactPerson = context.KontaktneOsebe_OTP.Where(ko => ko.idKontaktneOsebe == contactPersonID && ko.idStranka == clientID).FirstOrDefault();

                if (contactPerson != null)
                {
                    context.KontaktneOsebe_OTP.Remove(contactPerson);
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
                var query = from cE in context.StrankaZaposleni_OTP
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
                                         group employee by employee.Osebe_OTP into person
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

        public int SaveClientEmployee(ClientEmployeeModel model, bool updateRecord = true)
        {
            StrankaZaposleni_OTP strZap = new StrankaZaposleni_OTP();

            try
            {
                strZap.idStrankaOsebe = model.idStrankaOsebe;
                strZap.idStranka = model.idStranka;
                strZap.idOsebe = model.idOsebe;

                if (strZap.idStrankaOsebe == 0)
                {

                    strZap.ts = DateTime.Now;
                    strZap.tsIDOsebe = model.tsIDOsebe;

                    context.StrankaZaposleni_OTP.Add(strZap);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        StrankaZaposleni_OTP original = context.StrankaZaposleni_OTP.Where(sz => sz.idStrankaOsebe == strZap.idStrankaOsebe).FirstOrDefault();
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
                var clientEmployee = context.StrankaZaposleni_OTP.Where(sz => sz.idStranka == clientID && sz.idOsebe == employeeID).FirstOrDefault();

                if (clientEmployee != null)
                {
                    context.StrankaZaposleni_OTP.Remove(clientEmployee);
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
                var clientEmployee = context.StrankaZaposleni_OTP.Where(sz => sz.idStranka == clientID && sz.idOsebe == employeeID).FirstOrDefault();

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

        List<ContactPersonModel> GetContactPersonModelList(int clientID)
        {
            try
            {
                var query = from contact in context.KontaktneOsebe_OTP
                            where contact.idStranka.Value.Equals(clientID)
                            select new ContactPersonModel
                            {
                                idKontaktneOsebe = contact.idKontaktneOsebe,
                                idStranka = contact.idStranka.HasValue ? contact.idStranka.Value : 0,
                                NazivKontaktneOsebe = contact.Naziv,
                                Stranka = contact.Stranka_OTP.NazivPrvi,
                                Telefon = contact.Telefon,
                                GSM = contact.GSM,
                                Email = contact.Email,
                                //DelovnoMesto = contact.DelovnoMesto,
                                ts = contact.ts.HasValue ? contact.ts.Value : DateTime.MinValue,
                                tsIDOsebe = contact.tsIDOsebe.HasValue ? contact.tsIDOsebe.Value : 0,
                                //ZaporednaStevika = contact.ZaporednaStevika.HasValue ? contact.ZaporednaStevika.Value : 0,
                                Fax = contact.Fax,
                                Opombe = contact.Opombe,
                                NazivPodpis = contact.NazivPodpis                                
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }


        List<ContactPersonModel> IClientOTPRepository.GetContactPersonModelList(int clientID)
        {
            throw new NotImplementedException();
        }


        public List<ClientSimpleModel> GetAllClientsByType(string typeCode)
        {
            try
            {//loj => left outer join

                bool checkForPastYear = Convert.ToBoolean(ConfigurationManager.AppSettings["RouteRecallsPastYear"].ToString());
                DateTime dateStart = !checkForPastYear ? DateTime.Now.AddDays(-30).Date : DateTime.Now.AddYears(-1).Date;
                DateTime dateEnd = DateTime.Now.Date;
                string overtake = Enums.StatusOfRecall.PREVZET.ToString();
                string partialOvertake = Enums.StatusOfRecall.DELNO_PREVZET.ToString();
                string approvedStat = Enums.StatusOfRecall.POTRJEN.ToString();

                ClientType type = GetClientTypeByCode(typeCode);

                var query = from client in context.Stranka_OTP
                            join clientEmployee in context.StrankaZaposleni_OTP on client.idStranka equals clientEmployee.idStranka into loj
                            from subClient in loj.DefaultIfEmpty()
                            where client.TipID == type.TipStrankaID && client.Activity == 1
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
                                ts = client.ts.HasValue ? client.ts.Value : DateTime.MinValue,
                                tsIDOsebe = client.tsIDOsebe.HasValue ? client.tsIDOsebe.Value : 0,
                                idOsebe = (subClient == null ? 0 : subClient.Osebe_OTP.idOsebe),
                                ImeInPriimekZaposlen = (subClient == null ? String.Empty : subClient.Osebe_OTP.Ime + " " + subClient.Osebe_OTP.Priimek),
                                TipStranka = client.TipStranka != null ? client.TipStranka.Naziv : "",
                                RecallCount = (from recalls in context.Odpoklic
                                               where (recalls.DobaviteljID.Value == client.idStranka) && (recalls.ts.Value >= dateStart && recalls.ts.Value <= dateEnd) &&
                                               (recalls.StatusOdpoklica.Koda == approvedStat || recalls.StatusOdpoklica.Koda == overtake || recalls.StatusOdpoklica.Koda == partialOvertake)
                                               select recalls).Count() * (!checkForPastYear ? 12 : 1)
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<ClientSimpleModel> GetAllClientsByType(int employeeID, string typeCode)
        {
            try
            {
                ClientType type = GetClientTypeByCode(typeCode);

                var query = from client in context.Stranka_OTP
                            join clientEmployee in context.StrankaZaposleni_OTP
                            on client.idStranka equals clientEmployee.idStranka
                            where clientEmployee.idOsebe == employeeID && client.TipID == type.TipStrankaID
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
                                ts = client.ts.HasValue ? client.ts.Value : DateTime.MinValue,
                                tsIDOsebe = client.tsIDOsebe.HasValue ? client.tsIDOsebe.Value : 0,
                                //                                ImeInPriimekZaposlen = (subClient == null ? String.Empty : subClient.Osebe_OTP.Ime + " " + subClient.Osebe_OTP.Priimek),
                                TipStranka = client.TipStranka != null ? client.TipStranka.Naziv : ""
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
                var query = from type in context.TipStranka
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
                var query = from type in context.TipStranka
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
                var query = from type in context.TipStranka
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

        public List<ClientTransportType> GetClientTransportTypes()
        {
            try
            {
                var query = from transportType in context.TipPrevoza
                            select new ClientTransportType
                            {
                                Koda = transportType.Koda,
                                Naziv = transportType.Naziv,
                                Opombe = transportType.Opombe,
                                TipPrevozaID = transportType.TipPrevozaID,
                                DovoljenaTeza = transportType.DovoljenaTeza.HasValue ? transportType.DovoljenaTeza.Value : 0,
                                ShranjevanjePozicij = transportType.ShranjevanjePozicij.HasValue ? transportType.ShranjevanjePozicij.Value : false,
                                ts = transportType.ts.HasValue ? transportType.ts.Value : DateTime.MinValue,
                                tsIDPrijave = transportType.tsIDPrijave.HasValue ? transportType.tsIDPrijave.Value : 0
                            };

                return query.ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<ZbirnikTonModel> GetZbirnikTonList()
        {
            try
            {
                var query = from zbrirnikTon in context.ZbirnikTon
                            select new ZbirnikTonModel
                            {
                                Koda = zbrirnikTon.Koda,
                                Naziv = zbrirnikTon.Naziv,
                                ZbirnikTonID = zbrirnikTon.ZbirnikTonID,
                                TezaOd = zbrirnikTon.OdTeza.HasValue ? zbrirnikTon.OdTeza.Value : 0,
                                TezaDo = zbrirnikTon.DoTeza.HasValue ? zbrirnikTon.DoTeza.Value : 0,
                                ts = zbrirnikTon.ts.HasValue ? zbrirnikTon.ts.Value : DateTime.MinValue,
                                
                            };

                return query.ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public ClientTransportType GetClientTransportTypeByID(int id)
        {
            try
            {
                var query = from transportType in context.TipPrevoza
                            where transportType.TipPrevozaID == id
                            select new ClientTransportType
                            {
                                Koda = transportType.Koda,
                                Naziv = transportType.Naziv,
                                Opombe = transportType.Opombe,
                                TipPrevozaID = transportType.TipPrevozaID,
                                DovoljenaTeza = transportType.DovoljenaTeza.HasValue ? transportType.DovoljenaTeza.Value : 0,
                                ShranjevanjePozicij = transportType.ShranjevanjePozicij.HasValue ? transportType.ShranjevanjePozicij.Value : false,
                                ts = transportType.ts.HasValue ? transportType.ts.Value : DateTime.MinValue,
                                tsIDPrijave = transportType.tsIDPrijave.HasValue ? transportType.tsIDPrijave.Value : 0
                            };

                return query.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public int SaveClientTransportType(ClientTransportType model, bool updateRecord = true)
        {
            TipPrevoza tType = new TipPrevoza();

            try
            {
                tType.TipPrevozaID = model.TipPrevozaID;
                tType.Koda = model.Koda;
                tType.Naziv = model.Naziv;
                tType.Opombe = model.Opombe;
                tType.DovoljenaTeza = model.DovoljenaTeza;
                tType.ShranjevanjePozicij = model.ShranjevanjePozicij;
                tType.ts = model.ts == DateTime.MinValue ? (DateTime?)null : model.ts;
                tType.tsIDPrijave = model.tsIDPrijave;

                if (tType.TipPrevozaID == 0)
                {
                    tType.ts = DateTime.Now;

                    context.TipPrevoza.Add(tType);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        TipPrevoza original = context.TipPrevoza.Where(tp => tp.TipPrevozaID == tType.TipPrevozaID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(tType);
                        context.SaveChanges();
                    }
                }

                return tType.TipPrevozaID;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public bool DeleteClientTransportType(int trasportTypeID)
        {
            try
            {
                var transportType = context.TipPrevoza.Where(tp => tp.TipPrevozaID == trasportTypeID).FirstOrDefault();

                if (transportType != null)
                {
                    context.TipPrevoza.Remove(transportType);
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

        public List<LanguageModelOTP> GetLanguages()
        {
            try
            {
                var query = from type in context.Jeziki
                            select new LanguageModelOTP
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

        public ClientFullModel GetClientByName(string clientName)
        {
            try
            {
                var query = from client in context.Stranka_OTP
                            where client.NazivPrvi.Equals(clientName)
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
                                              }).FirstOrDefault(),
                                TipPrevozaID = client.TipPrevoza.HasValue ? client.TipPrevoza.Value : 0,
                                TipPrevoza = (from transportType in context.TipPrevoza
                                              where transportType.TipPrevozaID == client.TipPrevoza
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
                                              }).FirstOrDefault()
                            };

                ClientFullModel model = query.FirstOrDefault();

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }
    }
}