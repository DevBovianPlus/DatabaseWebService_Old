using DatabaseWebService.Common.Enums;
using DatabaseWebService.DomainPDO.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsOTP.Client;
using DatabaseWebService.ModelsPDO;
using DatabaseWebService.ModelsPDO.Inquiry;
using DatabaseWebService.ModelsPDO.Order;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.DomainPDO.Concrete
{
    public class InquiryRepository : IInquiryRepository
    {
        GrafolitPDOEntities context;
        IClientPDORepository clientPdoRepo;
        IMSSQLPDOFunctionRepository mssqlRepo;

        public InquiryRepository(GrafolitPDOEntities _context, IClientPDORepository iclientPdoRepo, IMSSQLPDOFunctionRepository imssqlRepo)
        {
            context = _context;
            clientPdoRepo = iclientPdoRepo;
            mssqlRepo = imssqlRepo;
        }

        public List<InquiryModel> GetInquiryList()
        {
            try
            {
                var query = from i in context.Povprasevanje
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
                                Narocila = i.Narocila,
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
                                DatumPredvideneDobave = i.DatumPredvideneDobave.HasValue ? i.DatumPredvideneDobave.Value : DateTime.MinValue,
                                PovprasevanjeOddal = (from employee in context.Osebe_PDO
                                                      where employee.OsebaID == i.PovprasevajneOddal.Value
                                                      select new EmployeeFullModel
                                                      {
                                                          DatumRojstva = employee.DatumRojstva.HasValue ? employee.DatumZaposlitve.Value : DateTime.MinValue,
                                                          DatumZaposlitve = employee.DatumZaposlitve.HasValue ? employee.DatumZaposlitve.Value : DateTime.MinValue,
                                                          DelovnoMesto = employee.DelovnoMesto,
                                                          Email = employee.Email,
                                                          Geslo = employee.Geslo,
                                                          idOsebe = employee.OsebaID,
                                                          Ime = employee.Ime,
                                                          Naslov = employee.Naslov,
                                                          Priimek = employee.Priimek,
                                                          TelefonGSM = employee.TelefonGSM,
                                                          ts = employee.ts.HasValue ? employee.ts.Value : DateTime.MinValue,
                                                          tsIDOsebe = employee.tsIDOsebe.HasValue ? employee.tsIDOsebe.Value : 0,
                                                          UporabniskoIme = employee.UporabniskoIme,
                                                          Zunanji = employee.Zunanji.HasValue ? employee.Zunanji.Value : 0,
                                                          idVloga = employee.VlogaID,
                                                          NadrejeniID = context.OsebeNadrejeni_PDO.Where(osn => osn.OsebaID == employee.OsebaID).FirstOrDefault() != null ? context.OsebeNadrejeni_PDO.Where(osn => osn.OsebaID == employee.OsebaID).FirstOrDefault().OsebeNadrejeniID : 0,
                                                          PDODostop = employee.PDODostop.HasValue ? employee.PDODostop.Value : false,
                                                      }).FirstOrDefault(),
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public InquiryFullModel GetInquiryByID(int iId, bool bOnlySelected, int iSelDobaviteljID = 0)
        {
            try
            {
                var query = from i in context.Povprasevanje
                            where i.PovprasevanjeID == iId
                            select new InquiryFullModel
                            {
                                DatumOddajePovprasevanja = i.DatumOddajePovprasevanja.HasValue ? i.DatumOddajePovprasevanja.Value : DateTime.MinValue,
                                KupecID = i.KupecID,
                                Kupec = (from k in context.Stranka_PDO
                                         where k.StrankaID == i.KupecID
                                         select new ClientFullModel
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
                                             TipStrankaID = k.TipStrankaID,
                                             TipStranka = (from tipS in context.TipStranka_PDO
                                                           where tipS.TipStrankaID == k.TipStrankaID
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
                                                               where cp.StrankaID == k.StrankaID
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
                                DatumPredvideneDobave = i.DatumPredvideneDobave.HasValue ? i.DatumPredvideneDobave.Value : DateTime.MinValue,
                                PovprasevanjeOddalID = i.PovprasevajneOddal.HasValue ? i.PovprasevajneOddal.Value : 0,
                                PovprasevanjeOddal = (from employee in context.Osebe_PDO
                                                      where employee.OsebaID == i.PovprasevajneOddal.Value
                                                      select new EmployeeFullModel
                                                      {
                                                          DatumRojstva = employee.DatumRojstva.HasValue ? employee.DatumZaposlitve.Value : DateTime.MinValue,
                                                          DatumZaposlitve = employee.DatumZaposlitve.HasValue ? employee.DatumZaposlitve.Value : DateTime.MinValue,
                                                          DelovnoMesto = employee.DelovnoMesto,
                                                          Email = employee.Email,
                                                          Geslo = employee.Geslo,
                                                          idOsebe = employee.OsebaID,
                                                          Ime = employee.Ime,
                                                          Naslov = employee.Naslov,
                                                          Priimek = employee.Priimek,
                                                          TelefonGSM = employee.TelefonGSM,
                                                          ts = employee.ts.HasValue ? employee.ts.Value : DateTime.MinValue,
                                                          tsIDOsebe = employee.tsIDOsebe.HasValue ? employee.tsIDOsebe.Value : 0,
                                                          UporabniskoIme = employee.UporabniskoIme,
                                                          Zunanji = employee.Zunanji.HasValue ? employee.Zunanji.Value : 0,
                                                          idVloga = employee.VlogaID,
                                                          NadrejeniID = context.OsebeNadrejeni_PDO.Where(osn => osn.OsebaID == employee.OsebaID).FirstOrDefault() != null ? context.OsebeNadrejeni_PDO.Where(osn => osn.OsebaID == employee.OsebaID).FirstOrDefault().OsebeNadrejeniID : 0,
                                                          PDODostop = employee.PDODostop.HasValue ? employee.PDODostop.Value : false,
                                                      }).FirstOrDefault(),
                                NotSendPDFAndEmailsToSupplier = i.NotSendPDFAndEmailsToSupplier.HasValue ? i.NotSendPDFAndEmailsToSupplier.Value : false,
                                OpombeNarocilnica = i.OpombaNarocilnica,
                                OddelekID = i.OddelekID.HasValue ? i.OddelekID.Value : 0,
                                OddelekNaziv = i.Oddelek.Naziv,
                                Oddelek = (from odd in context.Oddelek
                                           where odd.OddelekID == i.OddelekID
                                           select new DepartmentModel
                                           {
                                               Koda = odd.Koda,
                                               Naziv = odd.Naziv,
                                               ts = odd.ts.HasValue ? odd.ts.Value : DateTime.MinValue,
                                           }).FirstOrDefault(),

                            };

                InquiryFullModel model = query.FirstOrDefault();

                if (model != null)
                {
                    model.PovprasevanjePozicija = new List<InquiryPositionModel>();
                    model.PovprasevanjePozicija = GetInquiryPositionByInquiryID(model.PovprasevanjeID);
                    GetInquiryPositionArtikelByInquiryID(model, model.PovprasevanjeID, bOnlySelected, iSelDobaviteljID);

                }

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<InquiryPositionModel> GetInquiryPositionByInquiryID(int iId)
        {
            try
            {
                var query = from ip in context.PovprasevanjePozicija
                            where ip.PovprasevanjeID == iId
                            select new InquiryPositionModel
                            {
                                PovprasevanjePozicijaID = ip.PovprasevanjePozicijaID,
                                PovprasevanjeID = ip.PovprasevanjeID,
                                DobaviteljID = ip.DobaviteljID,
                                Dobavitelj = (from d in context.Stranka_PDO
                                              where d.StrankaID == ip.DobaviteljID
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
                                                  PrivzetaEM = d.PrivzetaEM,
                                                  ZadnjaIzbranaKategorija = d.ZadnjaIzbranaKategorija,
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
                                DobaviteljID_P = ip.DobaviteljID_P,
                                DobaviteljNaziv_P = ip.DobaviteljNaziv_P,
                                DobaviteljKontaktOsebe = ip.DobaviteljKontaktOsebe,
                                ObvesceneOsebe = ip.ObvesceneOsebe,
                                GrafolitID = context.Stranka_PDO.Where(s => s.KodaStranke == "GRAFOLIT").FirstOrDefault().StrankaID,
                                KupecViden = ip.KupecViden.HasValue ? ip.KupecViden.Value : false,
                                Artikli = ip.Artikli,
                                DatumPredvideneDobave = ip.DatumPredvideneDobave.HasValue ? ip.DatumPredvideneDobave.Value : DateTime.MinValue,
                                Opomba = ip.Opomba,
                                PotDokumenta = ip.PotDokumenta,
                                ts = ip.ts.HasValue ? ip.ts.Value : DateTime.MinValue,
                                tsIDOsebe = ip.tsIDOsebe.HasValue ? ip.tsIDOsebe.Value : 0,
                                tsUpdate = ip.tsUpdate.HasValue ? ip.tsUpdate.Value : DateTime.MinValue,
                                tsUpdateUserID = ip.tsUpdateUserID.HasValue ? ip.tsUpdateUserID.Value : 0,
                                EmailBody = ip.EmailBody,
                            };

                List<InquiryPositionModel> model = query.ToList();

                if (model != null && model.Count > 0)
                {
                    foreach (var item in model)
                    {
                        item.PovprasevanjePozicijaArtikel = (from ipa in context.PovprasevanjePozicijaArtikel
                                                             where ipa.PovprasevanjePozicijaID == item.PovprasevanjePozicijaID
                                                             select new InquiryPositionArtikelModel
                                                             {
                                                                 KategorijaNaziv = ipa.KategorijaNaziv,
                                                                 EnotaMere1 = ipa.EnotaMere1,
                                                                 EnotaMere2 = ipa.EnotaMere2,
                                                                 Kolicina1 = ipa.Kolicina1.HasValue ? ipa.Kolicina1.Value : 0,
                                                                 Kolicina2 = ipa.Kolicina2.HasValue ? ipa.Kolicina2.Value : 0,
                                                                 KolicinavKG = ipa.KolicinavKG.HasValue ? ipa.KolicinavKG.Value : 0,
                                                                 EnotaMere = ipa.EnotaMere,
                                                                 Naziv = ipa.Naziv,
                                                                 Opombe = ipa.Opombe,
                                                                 OpombaNarocilnica = ipa.OpombaNarocilnica,
                                                                 PovprasevanjePozicijaArtikelID = ipa.PovprasevanjePozicijaArtikelID,
                                                                 PovprasevanjePozicijaID = ipa.PovprasevanjePozicijaID,
                                                                 ts = ipa.ts.HasValue ? ipa.ts.Value : DateTime.MinValue,
                                                                 tsIDOsebe = ipa.tsIDOsebe.HasValue ? ipa.tsIDOsebe.Value : 0,
                                                                 tsUpdate = ipa.tsUpdate.HasValue ? ipa.tsUpdate.Value : DateTime.MinValue,
                                                                 tsUpdateUserID = ipa.tsUpdateUserID.HasValue ? ipa.tsUpdateUserID.Value : 0,
                                                             }).ToList();
                    }
                }


                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public bool DeleteInquiry(int iId)
        {
            try
            {
                var inquiry = context.Povprasevanje.Where(i => i.PovprasevanjeID == iId).FirstOrDefault();

                if (inquiry != null)
                {
                    context.Povprasevanje.Remove(inquiry);
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

        public bool CopyInquiryByID(int iId)
        {
            try
            {
                InquiryFullModel model = GetInquiryByID(iId, true);
                SaveInquiry(model, false, true);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_07, ex);
            }
        }



        public int SaveInquiry(InquiryFullModel model, bool updateRecord = true, bool cpyInquery = false)
        {
            try
            {
                Povprasevanje p = new Povprasevanje();
                p.DatumOddajePovprasevanja = model.DatumOddajePovprasevanja.Equals(DateTime.MinValue) ? (DateTime?)null : model.DatumOddajePovprasevanja;
                p.NarociloID = model.NarociloID <= 0 ? (int?)null : model.NarociloID;
                p.Naziv = model.Naziv;
                p.PovprasevanjeID = (cpyInquery) ? 0 : model.PovprasevanjeID;
                p.PovprasevanjeStevilka = model.PovprasevanjeStevilka;
                p.StatusID = model.StatusID;
                p.tsUpdate = DateTime.Now;
                p.tsUpdateUserID = model.tsUpdateUserID;
                p.KupecNaziv_P = model.KupecNaziv_P;
                p.KupecID = GetBuyerOrSupplierIDByName(model.KupecNaziv_P, Enums.TypeOfClient.KUPEC.ToString());
                p.Zakleni = model.Zakleni;
                p.ZaklenilUporabnik = model.ZaklenilUporabnik > 0 ? model.ZaklenilUporabnik : (int?)null;
                p.DatumPredvideneDobave = model.DatumPredvideneDobave.Equals(DateTime.MinValue) ? (DateTime?)null : model.DatumPredvideneDobave;
                p.ts = model.ts.Equals(DateTime.MinValue) ? (DateTime?)null : model.ts;
                p.tsIDOsebe = model.tsIDOsebe;
                p.PovprasevajneOddal = model.PovprasevanjeOddalID > 0 ? model.PovprasevanjeOddalID : (int?)null;
                p.NotSendPDFAndEmailsToSupplier = model.NotSendPDFAndEmailsToSupplier;
                p.OpombaNarocilnica = model.OpombeNarocilnica;
                p.OddelekID = model.OddelekID > 0 ? model.OddelekID : (int?)null;
                if (p.PovprasevanjeID == 0)
                {
                    p.PovprasevanjeStevilka = GetNextInquiryNum();
                    p.ts = DateTime.Now;
                    p.tsIDOsebe = model.tsIDOsebe;
                    if (cpyInquery)
                    {
                        p.Narocila = null;
                        p.NarociloID = null;
                        p.DatumOddajePovprasevanja = null;
                        p.DatumPredvideneDobave = null;
                        InquiryStatus stat = GetPovprasevanjaStatusByCode(Enums.StatusOfInquiry.DELOVNA.ToString());
                        if (stat != null)
                        {
                            p.StatusID = stat.StatusPovprasevanjaID;
                        }
                    }
                    context.Povprasevanje.Add(p);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        Povprasevanje original = context.Povprasevanje.Where(i => i.PovprasevanjeID == p.PovprasevanjeID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(p);
                        context.SaveChanges();
                    }
                }

                if (model.PovprasevanjePozicija != null && model.PovprasevanjePozicija.Count > 0)
                {
                    SaveInquiryPositionsModel(model.PovprasevanjePozicija, p.PovprasevanjeID, cpyInquery);
                    SaveSelectedArtikelForInquiryPosition(model);
                }

                return p.PovprasevanjeID;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public InquiryStatus GetPovprasevanjaStatusByCode(string statusCode)
        {
            try
            {
                var query = from type in context.StatusPovprasevanja
                            where type.Koda == statusCode
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


        public int SaveInquiryPurchase(InquiryFullModel model, bool updateRecord = true)
        {
            try
            {
                Povprasevanje p = new Povprasevanje();
                p.DatumOddajePovprasevanja = model.DatumOddajePovprasevanja.Equals(DateTime.MinValue) ? (DateTime?)null : model.DatumOddajePovprasevanja;
                p.NarociloID = model.NarociloID <= 0 ? (int?)null : model.NarociloID;
                p.Naziv = model.Naziv;
                p.PovprasevanjeID = model.PovprasevanjeID;
                p.PovprasevanjeStevilka = model.PovprasevanjeStevilka;
                p.StatusID = model.StatusID;
                p.tsUpdate = DateTime.Now;
                p.tsUpdateUserID = model.tsUpdateUserID;
                p.KupecNaziv_P = model.KupecNaziv_P;
                p.KupecID = GetBuyerOrSupplierIDByName(model.KupecNaziv_P, Enums.TypeOfClient.KUPEC.ToString());
                p.Zakleni = model.Zakleni;
                p.ZaklenilUporabnik = model.ZaklenilUporabnik > 0 ? model.ZaklenilUporabnik : (int?)null;
                p.DatumPredvideneDobave = model.DatumPredvideneDobave.Equals(DateTime.MinValue) ? (DateTime?)null : model.DatumPredvideneDobave;
                p.ts = model.ts.Equals(DateTime.MinValue) ? (DateTime?)null : model.ts;
                p.tsIDOsebe = model.tsIDOsebe;
                p.PovprasevajneOddal = model.PovprasevanjeOddalID > 0 ? model.PovprasevanjeOddalID : (int?)null;
                p.NotSendPDFAndEmailsToSupplier = model.NotSendPDFAndEmailsToSupplier;
                if (p.PovprasevanjeID == 0)
                {
                    p.PovprasevanjeStevilka = GetNextInquiryNum();
                    p.ts = DateTime.Now;
                    p.tsIDOsebe = model.tsIDOsebe;

                    context.Povprasevanje.Add(p);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        Povprasevanje original = context.Povprasevanje.Where(i => i.PovprasevanjeID == p.PovprasevanjeID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(p);
                        context.SaveChanges();
                    }
                }

                if (model.PovprasevanjePozicija != null && model.PovprasevanjePozicija.Count > 0)
                {
                    SaveInquiryPositionsModel(model.PovprasevanjePozicija, p.PovprasevanjeID);
                }

                return p.PovprasevanjeID;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        private int GetBuyerOrSupplierIDByName(string name, string code)
        {
            string compareBuyerName = name.Trim().ToLower();

            int clientTypeID = context.TipStranka_PDO.Where(tps => tps.Koda == code).FirstOrDefault().TipStrankaID;
            var query = (from c in context.Stranka_PDO
                         where c.TipStrankaID == clientTypeID && c.NazivPrvi.ToLower() == compareBuyerName
                         select c).FirstOrDefault();

            if (query != null)
                return query.StrankaID;
            else
            {
                Stranka_PDO buyer = new Stranka_PDO();
                buyer.StrankaID = 0;
                buyer.NazivPrvi = name.Trim();
                buyer.TipStrankaID = clientTypeID;
                buyer.ts = DateTime.Now;
                buyer.tsIDOsebe = 0;
                buyer.tsUpdate = DateTime.Now;
                buyer.tsUpdateUserID = 0;
                buyer.JezikID = 1;

                context.Stranka_PDO.Add(buyer);
                context.SaveChanges();

                return buyer.StrankaID;
            }
        }

        private ClientFullModel GetStrankaPDOByID(int iID)
        {
            var query = from client in context.Stranka_PDO
                        where client.StrankaID.Equals(iID)
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
                            PrivzetaEM = client.PrivzetaEM,
                            ZadnjaIzbranaKategorija = client.ZadnjaIzbranaKategorija,
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


            return model;
        }

        private string GetNextInquiryNum()
        {
            try
            {
                int num = 0;
                string prefix = "";
                var item = context.Nastavitve.OrderByDescending(n => n.ts.Value).FirstOrDefault();

                if (item != null && item.ts.Value.Year == DateTime.Now.Year)
                {
                    num = item.PovprasevanjeStevilcenjeStev.HasValue ? item.PovprasevanjeStevilcenjeStev.Value : 0;
                    prefix = String.IsNullOrEmpty(item.PovprasevanjeStevilcenjePredpona) ? DateTime.Now.Year.ToString() : item.PovprasevanjeStevilcenjePredpona;
                    num += 1;

                    item.PovprasevanjeStevilcenjePredpona = prefix;
                    item.PovprasevanjeStevilcenjeStev = num;
                }
                else
                {
                    num += 1;
                    prefix = DateTime.Now.Year.ToString();

                    Nastavitve nas = new Nastavitve();
                    nas.NastavitveID = 0;
                    nas.PovprasevanjeStevilcenjePredpona = prefix;
                    nas.PovprasevanjeStevilcenjeStev = num;
                    nas.ts = DateTime.Now;
                    nas.tsIDOsebe = 0;
                    nas.tsUpdate = DateTime.Now;
                    nas.tsUpdateUserID = 0;

                    context.Nastavitve.Add(nas);
                }

                context.SaveChanges();

                return prefix + "/" + num;
            }
            catch (Exception ex)
            {
                throw new Exception("GetNextPovprasevanjeStevilka Method Error! ", ex);
            }
        }

        /// <summary>
        /// Kopiraj vse podatke za izbrano pozicijo
        /// </summary>
        /// <param name="ipID"></param>
        /// <returns></returns>
        public InquiryPositionModel CopyInquiryPositionByID(int ipID)
        {
            try
            {
                var query = from ip in context.PovprasevanjePozicija
                            where ip.PovprasevanjePozicijaID == ipID
                            select new InquiryPositionModel
                            {
                                PovprasevanjeID = ip.PovprasevanjeID,
                                PovprasevanjePozicijaID = ip.PovprasevanjePozicijaID,
                                DobaviteljID = ip.DobaviteljID,
                                Dobavitelj = (from d in context.Stranka_PDO
                                              where d.StrankaID == ip.DobaviteljID
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
                                DobaviteljID_P = ip.DobaviteljID_P,
                                DobaviteljNaziv_P = ip.DobaviteljNaziv_P,
                                DobaviteljKontaktOsebe = ip.DobaviteljKontaktOsebe,
                                ObvesceneOsebe = ip.ObvesceneOsebe,
                                GrafolitID = context.Stranka_PDO.Where(s => s.KodaStranke == "GRAFOLIT").FirstOrDefault().StrankaID,
                                Artikli = ip.Artikli,
                                KupecViden = ip.KupecViden.HasValue ? ip.KupecViden.Value : false,
                                PotDokumenta = ip.PotDokumenta,
                                ts = ip.ts.HasValue ? ip.ts.Value : DateTime.MinValue,
                                tsIDOsebe = ip.tsIDOsebe.HasValue ? ip.tsIDOsebe.Value : 0,
                                tsUpdate = ip.tsUpdate.HasValue ? ip.tsUpdate.Value : DateTime.MinValue,
                                tsUpdateUserID = ip.tsUpdateUserID.HasValue ? ip.tsUpdateUserID.Value : 0
                            };

                InquiryPositionModel model = query.FirstOrDefault();

                int NewPovprasevanjePosID = 0;

                if (model != null)
                {
                    // označimo za ADD
                    model.PovprasevanjePozicijaID = 0;
                    NewPovprasevanjePosID = SaveInquiryPositionModel(model, false);
                    model.PovprasevanjePozicijaID = ipID;
                    model.PovprasevanjePozicijaArtikel = (from ipa in context.PovprasevanjePozicijaArtikel
                                                          where ipa.PovprasevanjePozicijaID == model.PovprasevanjePozicijaID
                                                          select new InquiryPositionArtikelModel
                                                          {
                                                              PovprasevanjePozicijaArtikelID = ipa.PovprasevanjePozicijaArtikelID,
                                                              PovprasevanjePozicijaID = ipa.PovprasevanjePozicijaID,
                                                              EnotaMere1 = ipa.EnotaMere1,
                                                              EnotaMere2 = ipa.EnotaMere2,
                                                              KategorijaNaziv = ipa.KategorijaNaziv,
                                                              Kolicina1 = ipa.Kolicina1.HasValue ? ipa.Kolicina1.Value : 0,
                                                              Kolicina2 = ipa.Kolicina2.HasValue ? ipa.Kolicina2.Value : 0,
                                                              Naziv = ipa.Naziv,
                                                              Opombe = ipa.Opombe,
                                                              OpombaNarocilnica = ipa.OpombaNarocilnica,
                                                              ts = ipa.ts.HasValue ? ipa.ts.Value : DateTime.MinValue,
                                                              tsIDOsebe = ipa.tsIDOsebe.HasValue ? ipa.tsIDOsebe.Value : 0,
                                                              tsUpdate = ipa.tsUpdate.HasValue ? ipa.tsUpdate.Value : DateTime.MinValue,
                                                              tsUpdateUserID = ipa.tsUpdateUserID.HasValue ? ipa.tsUpdateUserID.Value : 0,

                                                          }).ToList();

                }

                // Set povpraševanjepozicija za parent povezavo
                SetParentToPovprasevanjePozicijaArtikel(model, NewPovprasevanjePosID);
                model.PovprasevanjePozicijaID = NewPovprasevanjePosID;

                // shranimo še dobavitelj pozicije
                SaveInquiryPositionModel(model, false);

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        private void SetParentToPovprasevanjePozicijaArtikel(InquiryPositionModel model, int PovpPosID)
        {
            foreach (var item in model.PovprasevanjePozicijaArtikel)
            {
                item.PovprasevanjePozicijaArtikelID = 0;
                item.PovprasevanjePozicijaID = PovpPosID;
            }
        }

        public InquiryPositionModel GetInquiryPositionByID(int ipID)
        {
            try
            {
                var query = from ip in context.PovprasevanjePozicija
                            where ip.PovprasevanjePozicijaID == ipID
                            select new InquiryPositionModel
                            {
                                PovprasevanjeID = ip.PovprasevanjeID,
                                PovprasevanjePozicijaID = ip.PovprasevanjePozicijaID,
                                DobaviteljID = ip.DobaviteljID,
                                Dobavitelj = (from d in context.Stranka_PDO
                                              where d.StrankaID == ip.DobaviteljID
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
                                                  PrivzetaEM = d.PrivzetaEM,
                                                  ZadnjaIzbranaKategorija = d.ZadnjaIzbranaKategorija,
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
                                DobaviteljID_P = ip.DobaviteljID_P,
                                DobaviteljNaziv_P = ip.DobaviteljNaziv_P,
                                DobaviteljKontaktOsebe = ip.DobaviteljKontaktOsebe,
                                ObvesceneOsebe = ip.ObvesceneOsebe,
                                GrafolitID = context.Stranka_PDO.Where(s => s.KodaStranke == "GRAFOLIT").FirstOrDefault().StrankaID,
                                Artikli = ip.Artikli,
                                KupecViden = ip.KupecViden.HasValue ? ip.KupecViden.Value : false,
                                PotDokumenta = ip.PotDokumenta,
                                Opomba = ip.Opomba,
                                EmailBody = ip.EmailBody,
                                ts = ip.ts.HasValue ? ip.ts.Value : DateTime.MinValue,
                                tsIDOsebe = ip.tsIDOsebe.HasValue ? ip.tsIDOsebe.Value : 0,
                                tsUpdate = ip.tsUpdate.HasValue ? ip.tsUpdate.Value : DateTime.MinValue,
                                tsUpdateUserID = ip.tsUpdateUserID.HasValue ? ip.tsUpdateUserID.Value : 0,
                            };

                InquiryPositionModel model = query.FirstOrDefault();

                if (model != null)
                {
                    model.PovprasevanjePozicijaArtikel = (from ipa in context.PovprasevanjePozicijaArtikel
                                                          where ipa.PovprasevanjePozicijaID == model.PovprasevanjePozicijaID
                                                          select new InquiryPositionArtikelModel
                                                          {
                                                              PovprasevanjePozicijaArtikelID = ipa.PovprasevanjePozicijaArtikelID,
                                                              PovprasevanjePozicijaID = ipa.PovprasevanjePozicijaID,
                                                              EnotaMere1 = ipa.EnotaMere1,
                                                              EnotaMere2 = ipa.EnotaMere2,
                                                              KategorijaNaziv = ipa.KategorijaNaziv,
                                                              Kolicina1 = ipa.Kolicina1.HasValue ? ipa.Kolicina1.Value : 0,
                                                              Kolicina2 = ipa.Kolicina2.HasValue ? ipa.Kolicina2.Value : 0,
                                                              Naziv = ipa.Naziv,
                                                              Opombe = ipa.Opombe,
                                                              OpombaNarocilnica = ipa.OpombaNarocilnica,
                                                              ts = ipa.ts.HasValue ? ipa.ts.Value : DateTime.MinValue,
                                                              tsIDOsebe = ipa.tsIDOsebe.HasValue ? ipa.tsIDOsebe.Value : 0,
                                                              tsUpdate = ipa.tsUpdate.HasValue ? ipa.tsUpdate.Value : DateTime.MinValue,
                                                              DatumDobavePos = ipa.DatumDobavePos.HasValue ? ipa.DatumDobavePos.Value : DateTime.MinValue,
                                                              tsUpdateUserID = ipa.tsUpdateUserID.HasValue ? ipa.tsUpdateUserID.Value : 0,
                                                          }).ToList();

                }

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public int SaveInquiryPositionModel(InquiryPositionModel item, bool updateRecord = true)
        {
            try
            {
                PovprasevanjePozicija pp = new PovprasevanjePozicija();

                pp.PovprasevanjeID = item.PovprasevanjeID;
                pp.PovprasevanjePozicijaID = item.PovprasevanjePozicijaID;
                pp.DobaviteljID = GetBuyerOrSupplierIDByName(item.DobaviteljNaziv_P, Enums.TypeOfClient.DOBAVITELJ.ToString());
                pp.DobaviteljID_P = item.DobaviteljID_P;
                pp.DobaviteljNaziv_P = item.DobaviteljNaziv_P;
                pp.DobaviteljKontaktOsebe = item.DobaviteljKontaktOsebe;
                pp.ObvesceneOsebe = item.ObvesceneOsebe;
                pp.Artikli = item.Artikli;
                pp.KupecViden = item.KupecViden;
                pp.tsUpdate = DateTime.Now;
                pp.tsUpdateUserID = item.tsUpdateUserID;

                pp.ts = item.ts.Equals(DateTime.MinValue) ? (DateTime?)null : item.ts;
                pp.tsIDOsebe = item.tsIDOsebe;
                pp.Artikli = item.Artikli;
                pp.Opomba = item.Opomba;
                pp.PotDokumenta = item.PotDokumenta;
                pp.EmailBody = item.EmailBody;
                if (item.PovprasevanjePozicijaID == 0)
                {
                    pp.ts = DateTime.Now;
                    pp.tsIDOsebe = item.tsIDOsebe;

                    context.PovprasevanjePozicija.Add(pp);
                    context.SaveChanges();
                    item.PovprasevanjePozicijaID = pp.PovprasevanjePozicijaID;
                }
                else
                {
                    PovprasevanjePozicija original = context.PovprasevanjePozicija.Where(i => i.PovprasevanjePozicijaID == pp.PovprasevanjePozicijaID).FirstOrDefault();
                    context.Entry(original).CurrentValues.SetValues(pp);
                    context.SaveChanges();
                }
                pp.PovprasevanjePozicijaArtikel = GetSelectedArtikelForInquiryPosition(item);
                return pp.PovprasevanjePozicijaID;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public void SaveInquiryPositionsModel(List<InquiryPositionModel> ipos, int inquiry = 0, bool cpyInquery = false)
        {
            try
            {
                PovprasevanjePozicija pp = null;

                foreach (var item in ipos)
                {
                    pp = new PovprasevanjePozicija();

                    pp.PovprasevanjeID = inquiry;
                    pp.PovprasevanjePozicijaID = (cpyInquery) ? 0 : item.PovprasevanjePozicijaID;
                    pp.DobaviteljID = GetBuyerOrSupplierIDByName(item.DobaviteljNaziv_P, Enums.TypeOfClient.DOBAVITELJ.ToString());
                    pp.DobaviteljID_P = item.DobaviteljID_P;

                    pp.DobaviteljNaziv_P = item.DobaviteljNaziv_P;
                    pp.DobaviteljKontaktOsebe = item.DobaviteljKontaktOsebe;
                    pp.ObvesceneOsebe = item.ObvesceneOsebe;
                    pp.Artikli = item.Artikli;
                    pp.Opomba = item.Opomba;
                    pp.PotDokumenta = item.PotDokumenta;
                    pp.EmailBody = item.EmailBody;
                    pp.KupecViden = item.KupecViden;
                    pp.tsUpdate = DateTime.Now;
                    pp.tsUpdateUserID = item.tsUpdateUserID;
                    pp.PovprasevanjePozicijaArtikel = GetSelectedArtikelForInquiryPosition(item);
                    pp.ts = item.ts.Equals(DateTime.MinValue) ? (DateTime?)null : item.ts;
                    pp.tsIDOsebe = item.tsIDOsebe;
                    item.PovprasevanjePozicijaID = (cpyInquery) ? 0 : item.PovprasevanjePozicijaID;

                    if (item.PovprasevanjePozicijaID == 0)
                    {
                        pp.ts = DateTime.Now;
                        pp.tsIDOsebe = item.tsIDOsebe;

                        context.PovprasevanjePozicija.Add(pp);
                    }
                    else
                    {
                        PovprasevanjePozicija original = context.PovprasevanjePozicija.Where(i => i.PovprasevanjePozicijaID == pp.PovprasevanjePozicijaID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(pp);
                    }
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        private List<PovprasevanjePozicijaArtikel> SaveSelectedArtikelForInquiryPosition(InquiryFullModel model)
        {
            PovprasevanjePozicijaArtikel ppa = null;
            List<PovprasevanjePozicijaArtikel> list = new List<PovprasevanjePozicijaArtikel>();

            if (model.PovprasevanjePozicijaArtikel == null) return list;

            foreach (var item in model.PovprasevanjePozicijaArtikel)
            {
                ppa = new PovprasevanjePozicijaArtikel();
                ppa.PovprasevanjePozicijaArtikelID = item.PovprasevanjePozicijaArtikelID;
                ppa.PovprasevanjePozicijaID = item.PovprasevanjePozicijaID;
                ppa.EnotaMere1 = item.EnotaMere1;
                ppa.EnotaMere2 = item.EnotaMere2;
                ppa.KategorijaNaziv = item.KategorijaNaziv;
                ppa.Kolicina1 = item.Kolicina1 <= 0 ? (decimal?)null : item.Kolicina1;
                ppa.Kolicina2 = item.Kolicina2 <= 0 ? (decimal?)null : item.Kolicina2;
                ppa.Naziv = item.Naziv;
                ppa.Opombe = item.Opombe;
                ppa.tsUpdate = DateTime.Now;
                ppa.tsUpdateUserID = item.tsUpdateUserID;
                ppa.ts = item.ts.Equals(DateTime.MinValue) ? (DateTime?)null : item.ts;
                ppa.tsIDOsebe = item.tsIDOsebe;
                ppa.IzbranArtikel = item.IzbranArtikel;
                ppa.IzbraniArtikelNaziv_P = item.IzbraniArtikelNaziv_P;
                ppa.Ident = item.IzbraniArtikelIdent_P;
                ppa.ArtikelCena = item.ArtikelCena > 0 ? item.ArtikelCena : (decimal?)null;
                ppa.KolicinavKG = item.KolicinavKG > 0 ? item.KolicinavKG : (decimal?)null;
                ppa.Rabat = item.Rabat > 0 ? item.Rabat : (decimal?)null;
                ppa.OpombaNarocilnica = item.OpombaNarocilnica;
                ppa.OddelekID = (item.OddelekID > 0) ? item.OddelekID : (int?)null;
                ppa.EnotaMere = item.EnotaMere;
                ppa.DatumDobavePos = item.DatumDobavePos.Equals(DateTime.MinValue) ? (DateTime?)null : item.DatumDobavePos;

                if (ppa.PovprasevanjePozicijaArtikelID == 0)
                {
                    ppa.ts = DateTime.Now;
                    ppa.tsIDOsebe = item.tsIDOsebe;

                    context.PovprasevanjePozicijaArtikel.Add(ppa);
                }
                else
                {
                    PovprasevanjePozicijaArtikel original = context.PovprasevanjePozicijaArtikel.Where(ppodD => ppodD.PovprasevanjePozicijaArtikelID == ppa.PovprasevanjePozicijaArtikelID).FirstOrDefault();
                    if (original == null)
                        continue;
                    context.Entry(original).CurrentValues.SetValues(ppa);
                }
                list.Add(ppa);
            }
            context.SaveChanges();

            return list;
        }

        public InquiryFullModel GetInquiryPositionArtikelByInquiryID(InquiryFullModel inquery, int iId, bool isSelected, int iSelDobaviteljID = 0)
        {
            try
            {
                var query = from ppa in context.PovprasevanjePozicijaArtikel
                            join pp in context.PovprasevanjePozicija on ppa.PovprasevanjePozicijaID equals pp.PovprasevanjePozicijaID
                            where pp.PovprasevanjeID == iId && (isSelected ? ppa.IzbranArtikel == isSelected : true) && (iSelDobaviteljID > 0 ? pp.DobaviteljID == iSelDobaviteljID : true)
                            select new InquiryPositionArtikelModel
                            {
                                PovprasevanjePozicijaArtikelID = ppa.PovprasevanjePozicijaArtikelID,
                                PovprasevanjePozicijaID = pp.PovprasevanjePozicijaID,
                                IzbranDobaviteljID = pp.DobaviteljID,
                                Dobavitelj = (from d in context.Stranka_PDO
                                              where d.StrankaID == pp.DobaviteljID
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
                                ts = pp.ts.HasValue ? pp.ts.Value : DateTime.MinValue,
                                tsIDOsebe = pp.tsIDOsebe.HasValue ? pp.tsIDOsebe.Value : 0,
                                tsUpdate = pp.tsUpdate.HasValue ? pp.tsUpdate.Value : DateTime.MinValue,
                                tsUpdateUserID = pp.tsUpdateUserID.HasValue ? pp.tsUpdateUserID.Value : 0,

                                KategorijaNaziv = ppa.KategorijaNaziv,
                                Naziv = ppa.Naziv,
                                EnotaMere1 = ppa.EnotaMere1,
                                EnotaMere2 = ppa.EnotaMere2,
                                Kolicina1 = ppa.Kolicina1.HasValue ? ppa.Kolicina1.Value : 0,
                                Kolicina2 = ppa.Kolicina2.HasValue ? ppa.Kolicina2.Value : 0,
                                IzbraniArtikelNaziv_P = ppa.IzbraniArtikelNaziv_P,
                                IzbraniArtikelIdent_P = ppa.Ident,
                                ArtikelCena = ppa.ArtikelCena.HasValue ? ppa.ArtikelCena.Value : 0,
                                KolicinavKG = ppa.KolicinavKG.HasValue ? ppa.KolicinavKG.Value : 0,
                                EnotaMere = ppa.EnotaMere,
                                Rabat = ppa.Rabat.HasValue ? ppa.Rabat.Value : 0,
                                OpombaNarocilnica = ppa.OpombaNarocilnica,
                                Opombe = ppa.Opombe,
                                OddelekID = ppa.OddelekID.HasValue ? ppa.OddelekID.Value : 0,
                                IzbranArtikel = ppa.IzbranArtikel.HasValue ? ppa.IzbranArtikel.Value : false,
                                DatumDobavePos = ppa.DatumDobavePos.HasValue ? ppa.DatumDobavePos.Value : DateTime.MinValue,

                            };

                List<InquiryPositionArtikelModel> model = query.ToList();

                foreach (InquiryPositionArtikelModel item in model)
                {
                    item.ArtikliPantheon = mssqlRepo.GetProductBySupplierAndName(item.Dobavitelj.NazivPrvi, item.Naziv);
                }



                inquery.PovprasevanjePozicijaArtikel = model;

                return inquery;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        private List<PovprasevanjePozicijaArtikel> GetSelectedArtikelForInquiryPosition(InquiryPositionModel ipos)
        {
            PovprasevanjePozicijaArtikel ppa = null;
            List<PovprasevanjePozicijaArtikel> list = new List<PovprasevanjePozicijaArtikel>();

            if (ipos.PovprasevanjePozicijaArtikel == null) return list;

            foreach (var item in ipos.PovprasevanjePozicijaArtikel)
            {
                ppa = new PovprasevanjePozicijaArtikel();
                ppa.PovprasevanjePozicijaArtikelID = item.PovprasevanjePozicijaArtikelID;
                ppa.PovprasevanjePozicijaID = ipos.PovprasevanjePozicijaID;
                ppa.EnotaMere1 = item.EnotaMere1;
                ClientFullModel sDobavitelj = GetStrankaPDOByID(ipos.DobaviteljID);
                if (sDobavitelj != null)
                {
                    if (sDobavitelj.PrivzetaEM == null)
                    {
                        sDobavitelj.PrivzetaEM = ppa.EnotaMere1;
                    }
                    sDobavitelj.JezikID = sDobavitelj.JezikID > 0 ? sDobavitelj.JezikID : 1;
                    sDobavitelj.ZadnjaIzbranaKategorija = item.KategorijaNaziv;
                    clientPdoRepo.SaveClient(sDobavitelj, true);
                }
                ppa.EnotaMere2 = item.EnotaMere2;
                ppa.KategorijaNaziv = item.KategorijaNaziv;
                ppa.Kolicina1 = item.Kolicina1 <= 0 ? (decimal?)null : item.Kolicina1;
                ppa.Kolicina2 = item.Kolicina2 <= 0 ? (decimal?)null : item.Kolicina2;
                ppa.Naziv = item.Naziv;
                ppa.Opombe = item.Opombe;
                ppa.tsUpdate = DateTime.Now;
                ppa.tsUpdateUserID = item.tsUpdateUserID;
                ppa.ts = item.ts.Equals(DateTime.MinValue) ? (DateTime?)null : item.ts;
                ppa.tsIDOsebe = item.tsIDOsebe;

                ppa.IzbraniArtikelNaziv_P = item.IzbraniArtikelNaziv_P;
                ppa.Ident = item.IzbraniArtikelIdent_P;
                ppa.ArtikelCena = item.ArtikelCena > 0 ? item.ArtikelCena : (decimal?)null;
                ppa.KolicinavKG = item.KolicinavKG > 0 ? item.KolicinavKG : (decimal?)null;
                ppa.Rabat = item.Rabat > 0 ? item.Rabat : (decimal?)null;
                ppa.OpombaNarocilnica = item.OpombaNarocilnica;
                ppa.OddelekID = (item.OddelekID > 0) ? item.OddelekID : (int?)null;
                ppa.DatumDobavePos = item.DatumDobavePos.Equals(DateTime.MinValue) ? (DateTime?)null : item.DatumDobavePos;

                if (ppa.PovprasevanjePozicijaArtikelID == 0)
                {
                    ppa.ts = DateTime.Now;
                    ppa.tsIDOsebe = item.tsIDOsebe;

                    context.PovprasevanjePozicijaArtikel.Add(ppa);
                }
                else
                {
                    PovprasevanjePozicijaArtikel original = context.PovprasevanjePozicijaArtikel.Where(ppodD => ppodD.PovprasevanjePozicijaArtikelID == ppa.PovprasevanjePozicijaArtikelID).FirstOrDefault();
                    context.Entry(original).CurrentValues.SetValues(ppa);
                }

                list.Add(ppa);
                context.SaveChanges();
                item.PovprasevanjePozicijaID = ipos.PovprasevanjePozicijaID;
                item.PovprasevanjePozicijaArtikelID = ppa.PovprasevanjePozicijaArtikelID;
            }


            return list;
        }

        //public InquiryPositionArtikelModel GetInquiryPositionArtikelByInquiryPositionIDSupplierID(int iposID, int iSupplierID)
        //{
        //    try
        //    {
        //        var query = from ips in context.PovprasevanjePozicijaDobavitelj
        //                    where (ips.PovprasevanjePozicijaDobaviteljID == iposID && ips.DobaviteljID == iSupplierID)
        //                    select new InquiryPositionSupplierModel
        //                    {
        //                        DobaviteljID = ips.DobaviteljID.HasValue ? ips.DobaviteljID.Value : 0,                                
        //                        DobaviteljID_P = ips.DobaviteljID_P,
        //                        DobaviteljNaziv_P = ips.DobaviteljNaziv_P,
        //                        KupecViden = ips.KupecViden.HasValue ? ips.KupecViden.Value : false,
        //                        PovprasevanjePozicijaDobaviteljID = ips.PovprasevanjePozicijaDobaviteljID,
        //                        PovprasevanjePozicijaID = ips.PovprasevanjePozicijaID,
        //                        ts = ips.ts.HasValue ? ips.ts.Value : DateTime.MinValue,
        //                        tsIDOsebe = ips.tsIDOsebe.HasValue ? ips.tsIDOsebe.Value : 0,
        //                        tsUpdate = ips.tsUpdate.HasValue ? ips.tsUpdate.Value : DateTime.MinValue,
        //                        tsUpdateUserID = ips.tsUpdateUserID.HasValue ? ips.tsUpdateUserID.Value : 0,
        //                        PotDokumenta = ips.PotDokumenta
        //                    };

        //        return query.FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ValidationExceptionError.res_06, ex);
        //    }
        //}

        public List<InquiryPositionArtikelModel> GetInquiryPositionArtikelByInquiryPositionID(int iposID)
        {
            try
            {
                var query = from ipos in context.PovprasevanjePozicijaArtikel
                            where ipos.PovprasevanjePozicijaID == iposID
                            select new InquiryPositionArtikelModel
                            {
                                EnotaMere1 = ipos.EnotaMere1,
                                EnotaMere2 = ipos.EnotaMere2,
                                KategorijaNaziv = ipos.KategorijaNaziv,
                                Kolicina1 = ipos.Kolicina1.HasValue ? ipos.Kolicina1.Value : 0,
                                Kolicina2 = ipos.Kolicina2.HasValue ? ipos.Kolicina2.Value : 0,
                                Naziv = ipos.Naziv,
                                Opombe = ipos.Opombe,
                                PovprasevanjePozicijaArtikelID = ipos.PovprasevanjePozicijaArtikelID,
                                PovprasevanjePozicijaID = ipos.PovprasevanjePozicijaID,
                                ts = ipos.ts.HasValue ? ipos.ts.Value : DateTime.MinValue,
                                tsIDOsebe = ipos.tsIDOsebe.HasValue ? ipos.tsIDOsebe.Value : 0,
                                tsUpdate = ipos.tsUpdate.HasValue ? ipos.tsUpdate.Value : DateTime.MinValue,
                                tsUpdateUserID = ipos.tsUpdateUserID.HasValue ? ipos.tsUpdateUserID.Value : 0,
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public bool DeleteInquiryPositionArtikel(int ipaId)
        {
            try
            {
                var inquiryPosArt = context.PovprasevanjePozicijaArtikel.Where(i => i.PovprasevanjePozicijaArtikelID == ipaId).FirstOrDefault();

                if (inquiryPosArt != null)
                {
                    context.PovprasevanjePozicijaArtikel.Remove(inquiryPosArt);
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

        public bool DeleteInquiryPosition(int ipId)
        {
            try
            {
                // delete artikel first

                var lsInquiryArtikel = GetInquiryPositionArtikelByInquiryPositionID(ipId);

                if (lsInquiryArtikel.Count > 0)
                {
                    foreach (InquiryPositionArtikelModel ipa in lsInquiryArtikel)
                    {
                        DeleteInquiryPositionArtikel(ipa.PovprasevanjePozicijaArtikelID);
                    }
                }



                var inquiryPos = context.PovprasevanjePozicija.Where(i => i.PovprasevanjePozicijaID == ipId).FirstOrDefault();

                if (inquiryPos != null)
                {
                    context.PovprasevanjePozicija.Remove(inquiryPos);
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

        public bool DeleteInquiryPositionArtikles(List<InquiryPositionArtikelModel> itemsToDelete)
        {
            try
            {
                foreach (var item in itemsToDelete)
                {
                    if (item.PovprasevanjePozicijaArtikelID > 0)
                    {
                        var inquiryPosArt = context.PovprasevanjePozicijaArtikel.Where(ppd => ppd.PovprasevanjePozicijaArtikelID == item.PovprasevanjePozicijaArtikelID).FirstOrDefault();

                        if (inquiryPosArt != null)
                        {
                            context.PovprasevanjePozicijaArtikel.Remove(inquiryPosArt);
                            context.SaveChanges();

                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_07, ex);
            }
        }

        public InquiryStatusModel GetInquiryStatusByID(int statusID)
        {
            try
            {
                var query = from s in context.StatusPovprasevanja
                            where s.StatusPovprasevanjaID == statusID
                            select new InquiryStatusModel
                            {
                                Koda = s.Koda,
                                Naziv = s.Naziv,
                                Opis = s.Opis,
                                StatusPovprasevanjaID = s.StatusPovprasevanjaID,
                                ts = s.ts.HasValue ? s.ts.Value : DateTime.MinValue,
                                tsIDOseba = s.tsIDOseba.HasValue ? s.tsIDOseba.Value : 0,
                                tsUpdate = s.tsUpdate.HasValue ? s.tsUpdate.Value : DateTime.MinValue,
                                tsUpdateUserID = s.tsUpdateUserID.HasValue ? s.tsUpdateUserID.Value : 0,
                            };

                return query.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public InquiryStatusModel GetInquiryStatusByCode(string statusCode)
        {
            try
            {
                var query = from s in context.StatusPovprasevanja
                            where s.Koda == statusCode
                            select new InquiryStatusModel
                            {
                                Koda = s.Koda,
                                Naziv = s.Naziv,
                                Opis = s.Opis,
                                StatusPovprasevanjaID = s.StatusPovprasevanjaID,
                                ts = s.ts.HasValue ? s.ts.Value : DateTime.MinValue,
                                tsIDOseba = s.tsIDOseba.HasValue ? s.tsIDOseba.Value : 0,
                                tsUpdate = s.tsUpdate.HasValue ? s.tsUpdate.Value : DateTime.MinValue,
                                tsUpdateUserID = s.tsUpdateUserID.HasValue ? s.tsUpdateUserID.Value : 0,
                            };

                return query.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<InquiryStatusModel> GetInquiryStatuses()
        {
            try
            {
                var query = from s in context.StatusPovprasevanja
                            select new InquiryStatusModel
                            {
                                Koda = s.Koda,
                                Naziv = s.Naziv,
                                Opis = s.Opis,
                                StatusPovprasevanjaID = s.StatusPovprasevanjaID,
                                ts = s.ts.HasValue ? s.ts.Value : DateTime.MinValue,
                                tsIDOseba = s.tsIDOseba.HasValue ? s.tsIDOseba.Value : 0,
                                tsUpdate = s.tsUpdate.HasValue ? s.tsUpdate.Value : DateTime.MinValue,
                                tsUpdateUserID = s.tsUpdateUserID.HasValue ? s.tsUpdateUserID.Value : 0,
                            };

                return query.ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public bool LockInquiry(int inquiryID, int userID)
        {
            Povprasevanje original = context.Povprasevanje.Where(i => i.PovprasevanjeID == inquiryID).FirstOrDefault();
            original.Zakleni = true;
            original.ZaklenilUporabnik = userID;
            context.SaveChanges();

            return true;
        }

        public bool UnLockInquiry(int inquiryID, int userID)
        {
            Povprasevanje original = context.Povprasevanje.Where(i => i.PovprasevanjeID == inquiryID).FirstOrDefault();
            original.Zakleni = false;
            original.ZaklenilUporabnik = (int?)null;
            context.SaveChanges();

            return true;
        }

        public bool IsInquiryLocked(int inquiryID)
        {
            Povprasevanje original = context.Povprasevanje.Where(i => i.PovprasevanjeID == inquiryID).FirstOrDefault();

            return original.Zakleni.Value;
        }

        public bool UnLockInquiriesByUserID(int userID)
        {
            var list = context.Povprasevanje.Where(p => p.Zakleni.Value && p.ZaklenilUporabnik.Value == userID).ToList();
            foreach (var item in list)
            {
                item.Zakleni = false;
            }
            context.SaveChanges();
            return true;
        }

        public List<GroupedInquiryPositionsBySupplier> GetInquiryPositionsGroupedBySupplier(int inquiryID)
        {
            try
            {
                string supplierCode = Enums.TypeOfClient.DOBAVITELJ.ToString();
                string buyerCode = Enums.TypeOfClient.KUPEC.ToString();

                var query = from ips in context.PovprasevanjePozicija
                            where ips.PovprasevanjeID == inquiryID
                            //group ips by ips.DobaviteljID into inquiryPosition
                            select new GroupedInquiryPositionsBySupplier
                            {
                                Supplier = (from client in context.Stranka_PDO
                                            where client.StrankaID == ips.DobaviteljID && client.TipStranka_PDO.Koda == supplierCode
                                            select new ClientFullModel
                                            {
                                                idStranka = client.StrankaID,
                                                NazivPrvi = client.NazivPrvi,
                                                Email = client.Email,
                                                Telefon = client.Telefon,
                                                TipStrankaID = client.TipStrankaID,
                                                JezikID = client.JezikID.HasValue ? client.JezikID.Value : 0,
                                                Jezik = new LanguageModel
                                                {
                                                    Koda = client.Jeziki.Koda,
                                                    Naziv = client.Jeziki.Naziv,
                                                    ts = client.Jeziki.ts.HasValue ? client.Jeziki.ts.Value : DateTime.MinValue,
                                                },
                                            }).FirstOrDefault(),
                                InquiryPositionsArtikel = (from ipa in context.PovprasevanjePozicijaArtikel
                                                           where ipa.PovprasevanjePozicijaID == ips.PovprasevanjePozicijaID
                                                           select new InquiryPositionArtikelModel
                                                           {
                                                               EnotaMere1 = ipa.EnotaMere1,
                                                               EnotaMere2 = ipa.EnotaMere2,
                                                               KategorijaNaziv = ipa.KategorijaNaziv,
                                                               Kolicina1 = ipa.Kolicina1.HasValue ? ipa.Kolicina1.Value : 0,
                                                               Kolicina2 = ipa.Kolicina2.HasValue ? ipa.Kolicina2.Value : 0,
                                                               Naziv = ipa.Naziv,
                                                               Opombe = ipa.Opombe,
                                                               OpombaNarocilnica = ipa.OpombaNarocilnica,
                                                               PovprasevanjePozicijaID = ipa.PovprasevanjePozicijaID,
                                                               ts = ipa.ts.HasValue ? ipa.ts.Value : DateTime.MinValue,
                                                               tsIDOsebe = ipa.tsIDOsebe.HasValue ? ipa.tsIDOsebe.Value : 0,
                                                               tsUpdate = ipa.tsUpdate.HasValue ? ipa.tsUpdate.Value : DateTime.MinValue,
                                                               tsUpdateUserID = ipa.tsUpdateUserID.HasValue ? ipa.tsUpdateUserID.Value : 0,
                                                           }),
                                Buyer = (from inquiry in context.Povprasevanje
                                         where inquiry.PovprasevanjeID == inquiryID
                                         select new ClientFullModel
                                         {
                                             idStranka = inquiry.Stranka_PDO.StrankaID,
                                             NazivPrvi = inquiry.Stranka_PDO.NazivPrvi,
                                             Email = inquiry.Stranka_PDO.Email,
                                             Telefon = inquiry.Stranka_PDO.Telefon,
                                             TipStrankaID = inquiry.Stranka_PDO.TipStrankaID,
                                             JezikID = inquiry.Stranka_PDO.JezikID.HasValue ? inquiry.Stranka_PDO.JezikID.Value : 0,
                                             Jezik = new LanguageModel
                                             {
                                                 Koda = inquiry.Stranka_PDO.Jeziki.Koda,
                                                 Naziv = inquiry.Stranka_PDO.Jeziki.Naziv,
                                                 ts = inquiry.Stranka_PDO.Jeziki.ts.HasValue ? inquiry.Stranka_PDO.Jeziki.ts.Value : DateTime.MinValue,
                                             },
                                         }).FirstOrDefault(),
                                ReportFilePath = ips.PotDokumenta,
                                SelectedContactPersons = ips.DobaviteljKontaktOsebe,
                                SelectedGrafolitPersons = ips.ObvesceneOsebe,
                                KupecViden = ips.KupecViden,
                                EmailBody = ips.EmailBody,
                                PovprasevanjePozicijaID = ips.PovprasevanjePozicijaID,
                            };

                List<GroupedInquiryPositionsBySupplier> InquiryPositionsGroupedBySupplier = query.ToList();

                //fill emails for supplier
                GetEmailsForSelectedContactPersons(InquiryPositionsGroupedBySupplier, false);

                //fill emails for grafolit uporabniki
                GetEmailsForSelectedContactPersons(InquiryPositionsGroupedBySupplier,true);


                return InquiryPositionsGroupedBySupplier;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        private void GetEmailsForSelectedContactPersons(List<GroupedInquiryPositionsBySupplier> InquiryPositionsGroupedBySupplier, bool isObvesceneOsebe)
        {
            string sEmails = "";
            string sListEmails = "";

            foreach (GroupedInquiryPositionsBySupplier item in InquiryPositionsGroupedBySupplier)
            {
                sEmails = "";
                sListEmails = (isObvesceneOsebe ? item.SelectedGrafolitPersons : item.SelectedContactPersons);

                //string[] split = item.SelectedContactPersons.Split(';');
                string[] split = sListEmails.Split(';');

                if (split.Length > 0)
                {
                    foreach (string sContactName in split)
                    {
                        string sEmail = "";
                        var obj = context.KontaktnaOseba_PDO.Where(ppd => ppd.Naziv == sContactName).FirstOrDefault();
                        sEmail = obj.Email;
                        if (Common.DataTypesHelper.IsValidEmail(sEmail))
                            sEmails += obj.Email + ";";

                    }
                }

                if (sEmails.Length > 0)
                    sEmails = sEmails.Remove(sEmails.LastIndexOf(";"), 1);

                if (isObvesceneOsebe)
                {
                    item.SelectedGrafolitPersonsEmails = sEmails;
                }
                else
                {
                    item.SelectedContactPersonsEmails = sEmails;
                }
            }


        }

        public void SaveInquiryPositionPdfReport(GroupedInquiryPositionsBySupplier model)
        {
            try
            {

                var obj = context.PovprasevanjePozicija.Where(ppd => ppd.PovprasevanjePozicijaID == model.PovprasevanjePozicijaID && ppd.DobaviteljID == model.Supplier.idStranka).FirstOrDefault();

                if (obj != null)
                {
                    obj.PotDokumenta = model.ReportFilePath;
                    context.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }
    }
}