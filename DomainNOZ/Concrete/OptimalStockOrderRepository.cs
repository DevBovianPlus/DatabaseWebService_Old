using DatabaseWebService.Common;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.DomainNOZ.Abstract;
using DatabaseWebService.DomainPDO.Abstract;
using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DatabaseWebService.ModelsNOZ;
using DatabaseWebService.ModelsNOZ.OptimalStockOrder;
using DatabaseWebService.ModelsOTP.Recall;
using DatabaseWebService.ModelsPDO.Inquiry;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace DatabaseWebService.DomainNOZ.Concrete
{
    public class OptimalStockOrderRepository : IOptimalStockOrderRepository
    {
        GrafolitNOZEntities context;

        IMSSQLNOZFunctionRepository msSqlRepo;
        IEmployeeNOZRepository employeeNOZRepo;
        IClientNOZRepository clientNOZRepo;
        int indeks;
        public OptimalStockOrderRepository(GrafolitNOZEntities _context, IMSSQLNOZFunctionRepository _msSqlRepo, IEmployeeNOZRepository _employeeNOZRepo, IClientNOZRepository _clientNOZRepo)
        {
            context = _context;
            msSqlRepo = _msSqlRepo;
            employeeNOZRepo = _employeeNOZRepo;
            clientNOZRepo = _clientNOZRepo;
        }

        public void GetOptimalStockTree(List<string> productCategory, string color)
        {
            var sqlOptimalStock = msSqlRepo.GetListOptimalnaZaloga();

            foreach (var item in productCategory)
            {

            }
        }

        /// <summary>
        /// Uredimo seznam tako da so najprej gramatura Manjša od 10 s . v imenu gramature
        /// Nato pride vsi ki so manjši kot 100 in potem še tisti ki so večji ali enako kot 100
        /// </summary>
        /// <param name="listData"></param>
        /// <returns></returns>
        private List<GetListOptimalnaZaloga_Result> SetupOrderByGramaturaData(List<GetListOptimalnaZaloga_Result> listData)
        {
            listData = listData.OrderBy(l => l.Gramatura).ToList();

            var ListDecimalGramatura = listData.Where(s => s.Gramatura.Contains(".")).ToList();
            var ListDecimalGramaturaManjsiOd100 = listData.Where(g => !g.Gramatura.Contains(".")).Where(s => Convert.ToInt32(s.Gramatura) < 100).ToList();

            var ListDecimalGramaturaVecjiOd100 = listData.Where(g => !g.Gramatura.Contains(".")).Where(s => Convert.ToInt32(s.Gramatura) >= 100).ToList();


            List<GetListOptimalnaZaloga_Result> lNewResult = new List<GetListOptimalnaZaloga_Result>();

            lNewResult.AddRange(ListDecimalGramatura);
            lNewResult.AddRange(ListDecimalGramaturaManjsiOd100);
            lNewResult.AddRange(ListDecimalGramaturaVecjiOd100);

            return lNewResult;
        }

        public List<OptimalStockTreeHierarchy> GetOptimalStockTree(string productCategory, string color)
        {
            indeks = 0;
            //Pridobimo vse optimalne zaloge iz DW baze.
            var sqlOptimalStock = msSqlRepo.GetListOptimalnaZaloga();
            sqlOptimalStock = SetupOrderByGramaturaData(sqlOptimalStock);
            //sqlOptimalStock = sqlOptimalStock.OrderBy(l => l.Gramatura).ToList();

            if (!String.IsNullOrEmpty(color))
                sqlOptimalStock = sqlOptimalStock.Where(c => c.Barva == color).ToList();

            List<OptimalStockTreeHierarchy> list = new List<OptimalStockTreeHierarchy>();

            //pridobimo optimalne zaloge glede na kategorijo.
            var groupedByCategory = sqlOptimalStock.GroupBy(opt => opt.Kategorija).ToList();

            if (!String.IsNullOrEmpty(productCategory))
            {
                var categories = productCategory.Split(',').Where(s => !String.IsNullOrEmpty(s)).ToList();

                groupedByCategory = groupedByCategory.Where(gc => categories.Any(c => c.Trim() == gc.Key)).ToList();
            }

            foreach (var obj in groupedByCategory)
            {
                var element = obj.Select(s => s);

                var model = ConstructTree(element.First(), Enums.OptimalStockHierarchyLevel.Kategorija, null);
                foreach (var item in element.Skip(1))
                {
                    model = ConstructTree(item, Enums.OptimalStockHierarchyLevel.Gloss, model);
                }

                var leftLeaf = TraverseToLeftMostLeaf(model.Child[0]);
                CalculateOptimalStock(leftLeaf, new OptimalStockCalculationFields { optimalStock = 0, onStock = 0, orderInProgress = 0 });

                list.AddRange(CalculateOptimalStockParameters(GetTreeToList(model, null)));
                model = null;
                leftLeaf = null;
            }

            foreach (var item in list)
            {
                item.Parent = null;
                item.Child = null;
            }

            return list;
        }

        private OptimalStockTreeHierarchy CalculateOptimalStock(OptimalStockTreeHierarchy node, OptimalStockCalculationFields calc)
        {
            if (node == null)
                return null;

            if (node.IsLeaf)
            {
                calc.optimalStock = node.KolicinaOptimalna;
                calc.onStock = node.KolicinaZaloga;
                calc.orderInProgress = node.KolicinaNarocenoVTeku;

                //node.RazlikaZalogaOptimalna = calc.onStock - calc.optimalStock;
                //node.VsotaZalNarRazlikaOpt = ((calc.onStock + calc.orderInProgress) - calc.optimalStock);
                //node.VsotaZalNarKolicnikOpt = calc.optimalStock > 0 ? ((calc.onStock + calc.orderInProgress) / calc.optimalStock) : 0;

                node.IsProcessed = true;
                return CalculateOptimalStock(node.Parent, calc);
            }
            else
            {
                if (!node.IsProcessed)
                {
                    node.KolicinaOptimalna += calc.optimalStock;
                    node.KolicinaZaloga += calc.onStock;
                    node.KolicinaNarocenoVTeku += calc.orderInProgress;

                    //node.RazlikaZalogaOptimalna = node.KolicinaZaloga - node.KolicinaOptimalna;

                    //node.VsotaZalNarRazlikaOpt = ((node.KolicinaZaloga + node.KolicinaNarocenoVTeku) - node.KolicinaOptimalna);
                    //node.VsotaZalNarKolicnikOpt = node.KolicinaOptimalna > 0 ? ((node.KolicinaZaloga + node.KolicinaNarocenoVTeku) / node.KolicinaOptimalna) : 0;
                }

                var unprocessedNodes = node.Child.Where(c => !c.IsProcessed).ToList();

                if (unprocessedNodes.Count <= 0)
                {
                    node.IsProcessed = true;

                    if (node.Parent == null)
                        return node;
                    else
                        return CalculateOptimalStock(node.Parent, new OptimalStockCalculationFields { optimalStock = node.KolicinaOptimalna, onStock = node.KolicinaZaloga, orderInProgress = node.KolicinaNarocenoVTeku });
                }
                else
                {
                    var leftLeaf = TraverseToLeftMostLeaf(unprocessedNodes.First());
                    return CalculateOptimalStock(leftLeaf, new OptimalStockCalculationFields { optimalStock = 0, onStock = 0, orderInProgress = 0 });//ko gremo v golobino ni potrebno prenašati količine
                }
            }
        }

        private List<OptimalStockTreeHierarchy> CalculateOptimalStockParameters(List<OptimalStockTreeHierarchy> list)
        {
            foreach (var item in list)
            {
                item.RazlikaZalogaOptimalna = item.KolicinaZaloga - item.KolicinaOptimalna;
                item.VsotaZalNarRazlikaOpt = ((item.KolicinaZaloga + item.KolicinaNarocenoVTeku) - item.KolicinaOptimalna);
                item.VsotaZalNarKolicnikOpt = item.KolicinaOptimalna > 0 ? Math.Round(((item.KolicinaZaloga + item.KolicinaNarocenoVTeku) / item.KolicinaOptimalna), 2) : 0;
            }

            return list;
        }

        private OptimalStockTreeHierarchy TraverseToLeftMostLeaf(OptimalStockTreeHierarchy model)
        {
            OptimalStockTreeHierarchy leaf = null;
            if (model.IsLeaf)
            {
                return model;
            }
            else
                leaf = TraverseToLeftMostLeaf(model.Child.First());

            return leaf;
        }

        private List<OptimalStockTreeHierarchy> GetTreeToList(OptimalStockTreeHierarchy model, List<OptimalStockTreeHierarchy> list)
        {
            if (model.Parent == null)
            {
                list = new List<OptimalStockTreeHierarchy>();
                list.Add(model);
            }

            foreach (var item in model.Child)
            {
                list.Add(item);
                GetTreeToList(item, list);
            }

            return list;
        }

        private OptimalStockTreeHierarchy ConstructTree(GetListOptimalnaZaloga_Result currentItemToBeProcessed, Enums.OptimalStockHierarchyLevel level, OptimalStockTreeHierarchy model)
        {
            if (level == Enums.OptimalStockHierarchyLevel.Kategorija)//vemo da še ni korena
            {
                model = CreateObject(false, null, currentItemToBeProcessed.Kategorija);
                ConstructTree(currentItemToBeProcessed, Enums.OptimalStockHierarchyLevel.Gloss, model);
            }
            else if (level == Enums.OptimalStockHierarchyLevel.Gloss)
            {
                var item = model.Child.Where(c => c.Name == currentItemToBeProcessed.Gloss).FirstOrDefault();

                if (item != null)
                {
                    ConstructTree(currentItemToBeProcessed, Enums.OptimalStockHierarchyLevel.Gramatura, item);
                }
                else
                {
                    var newGloss = CreateObject(false, model, currentItemToBeProcessed.Gloss);
                    model.Child.Add(newGloss);
                    ConstructTree(currentItemToBeProcessed, Enums.OptimalStockHierarchyLevel.Gramatura, newGloss);
                }
            }
            else if (level == Enums.OptimalStockHierarchyLevel.Gramatura)
            {
                var item = model.Child.Where(c => c.Name == currentItemToBeProcessed.Gramatura).FirstOrDefault();

                if (item != null)
                {
                    ConstructTree(currentItemToBeProcessed, Enums.OptimalStockHierarchyLevel.Velikost, item);
                }
                else
                {
                    var newGramatura = CreateObject(false, model, currentItemToBeProcessed.Gramatura);
                    model.Child.Add(newGramatura);
                    ConstructTree(currentItemToBeProcessed, Enums.OptimalStockHierarchyLevel.Velikost, newGramatura);
                }
            }
            else if (level == Enums.OptimalStockHierarchyLevel.Velikost)
            {
                var item = model.Child.Where(c => c.Name == currentItemToBeProcessed.Velikost).FirstOrDefault();

                if (item != null)
                {
                    ConstructTree(currentItemToBeProcessed, Enums.OptimalStockHierarchyLevel.Tek, item);
                }
                else
                {
                    var newVelikost = CreateObject(false, model, currentItemToBeProcessed.Velikost);
                    model.Child.Add(newVelikost);
                    ConstructTree(currentItemToBeProcessed, Enums.OptimalStockHierarchyLevel.Tek, newVelikost);
                }
            }
            else//model je Tek
            {

                var newTek = CreateObject(true, model, currentItemToBeProcessed.Tek);
                newTek.KolicinaOptimalna = currentItemToBeProcessed.Kolicina_OPT.HasValue ? currentItemToBeProcessed.Kolicina_OPT.Value : 0;
                newTek.KolicinaZaloga = currentItemToBeProcessed.anStock.HasValue ? currentItemToBeProcessed.anStock.Value : 0;
                newTek.KolicinaNarocenoVTeku = currentItemToBeProcessed.VTekuKolicina.HasValue ? currentItemToBeProcessed.VTekuKolicina.Value : 0;

                model.Child.Add(newTek);

                return newTek;
            }

            return model;
        }

        private OptimalStockTreeHierarchy CreateObject(bool isleaf, OptimalStockTreeHierarchy parent, string name)
        {
            return new OptimalStockTreeHierarchy
            {
                ID = ++indeks,
                IsLeaf = isleaf,
                IsProcessed = false,
                Name = name,
                ParentID = parent != null ? parent.ID : 0,
                Parent = parent != null ? parent : null,
                Child = new List<OptimalStockTreeHierarchy>()
            };
        }

        public List<OptimalStockOrderModel> GetOptimalStockOrders()
        {
            try
            {
                var query = from i in context.NarociloOptimalnihZalog
                            select new OptimalStockOrderModel
                            {
                                DatumOddaje = i.DatumOddaje.HasValue ? i.DatumOddaje.Value : DateTime.MinValue,
                                StrankaID = i.StrankaID,
                                Stranka = (from k in context.Stranka_NOZ
                                           where k.StrankaID == i.StrankaID
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
                                               TipStranka = k.TipStranka_NOZ != null ? k.TipStranka_NOZ.Naziv : ""
                                           }).FirstOrDefault(),
                                NarociloID_P = i.NarociloID_P,
                                Naziv = i.Naziv,
                                NarociloOptimalnihZalogID = i.NarociloOptimalnihZalogID,
                                NarociloOptimalnihZalogStevilka = i.NarociloOptimalnihZalogStevilka,
                                StatusID = i.StatusID,
                                StatusNarocilaOptimalnihZalog = (from sp in context.StatusNarocilaOptimalnihZalog
                                                                 where sp.StatusNarocilaOptimalnihZalogID == i.StatusID
                                                                 select new OptimalStockOrderStatusModel
                                                                 {
                                                                     Koda = sp.Koda,
                                                                     Naziv = sp.Naziv,
                                                                     Opis = sp.Opis,
                                                                     StatusNarocilaOptimalnihZalogID = sp.StatusNarocilaOptimalnihZalogID,
                                                                     ts = sp.ts.HasValue ? sp.ts.Value : DateTime.MinValue,
                                                                     tsIDOseba = sp.tsIDOseba.HasValue ? sp.tsIDOseba.Value : 0,
                                                                     tsUpdate = sp.tsUpdate.HasValue ? sp.tsUpdate.Value : DateTime.MinValue,
                                                                     tsUpdateUserID = sp.tsUpdateUserID.HasValue ? sp.tsUpdateUserID.Value : 0,
                                                                 }).FirstOrDefault(),
                                ts = i.ts.HasValue ? i.ts.Value : DateTime.MinValue,
                                tsIDOsebe = i.tsIDOsebe.HasValue ? i.tsIDOsebe.Value : 0,
                                tsUpdate = i.tsUpdate.HasValue ? i.tsUpdate.Value : DateTime.MinValue,
                                tsUpdateUserID = i.tsUpdateUserID.HasValue ? i.tsUpdateUserID.Value : 0,
                                Kolicina = i.Kolicina,
                                NarociloOddal = i.NarociloOddal.HasValue ? i.NarociloOddal.Value : 0,
                                Opombe = i.Opombe,
                                PotDokumenta = i.PotDokumenta,
                                Zaposlen = (i.NarociloOddal.HasValue ? new EmployeeSimpleModel()
                                {
                                    Ime = i.Osebe_NOZ.Ime,
                                    Priimek = i.Osebe_NOZ.Priimek
                                } : new EmployeeSimpleModel() { Ime = "", Priimek = "" }),

                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public OptimalStockOrderModel GetOptimalStockOrderByID(int ID)
        {
            try
            {
                var query = from i in context.NarociloOptimalnihZalog
                            where i.NarociloOptimalnihZalogID == ID
                            select new OptimalStockOrderModel
                            {
                                DatumOddaje = i.DatumOddaje.HasValue ? i.DatumOddaje.Value : DateTime.MinValue,
                                StrankaID = i.StrankaID,
                                Stranka = (from k in context.Stranka_NOZ
                                           where k.StrankaID == i.StrankaID
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
                                               TipStranka = k.TipStranka_NOZ != null ? k.TipStranka_NOZ.Naziv : ""
                                           }).FirstOrDefault(),
                                NarociloID_P = i.NarociloID_P,
                                Naziv = i.Naziv,
                                NarociloOptimalnihZalogID = i.NarociloOptimalnihZalogID,
                                NarociloOptimalnihZalogStevilka = i.NarociloOptimalnihZalogStevilka,
                                StatusID = i.StatusID,
                                StatusNarocilaOptimalnihZalog = (from sp in context.StatusNarocilaOptimalnihZalog
                                                                 where sp.StatusNarocilaOptimalnihZalogID == i.StatusID
                                                                 select new OptimalStockOrderStatusModel
                                                                 {
                                                                     Koda = sp.Koda,
                                                                     Naziv = sp.Naziv,
                                                                     Opis = sp.Opis,
                                                                     StatusNarocilaOptimalnihZalogID = sp.StatusNarocilaOptimalnihZalogID,
                                                                     ts = sp.ts.HasValue ? sp.ts.Value : DateTime.MinValue,
                                                                     tsIDOseba = sp.tsIDOseba.HasValue ? sp.tsIDOseba.Value : 0,
                                                                     tsUpdate = sp.tsUpdate.HasValue ? sp.tsUpdate.Value : DateTime.MinValue,
                                                                     tsUpdateUserID = sp.tsUpdateUserID.HasValue ? sp.tsUpdateUserID.Value : 0,
                                                                 }).FirstOrDefault(),
                                ts = i.ts.HasValue ? i.ts.Value : DateTime.MinValue,
                                tsIDOsebe = i.tsIDOsebe.HasValue ? i.tsIDOsebe.Value : 0,
                                tsUpdate = i.tsUpdate.HasValue ? i.tsUpdate.Value : DateTime.MinValue,
                                tsUpdateUserID = i.tsUpdateUserID.HasValue ? i.tsUpdateUserID.Value : 0,
                                Kolicina = i.Kolicina,
                                NarociloOddal = i.NarociloOddal.HasValue ? i.NarociloOddal.Value : 0,
                                Opombe = i.Opombe,
                                PotDokumenta = i.PotDokumenta,
                                Zaposlen = (i.NarociloOddal.HasValue ? new EmployeeSimpleModel
                                {
                                    Ime = i.Osebe_NOZ.Ime,
                                    Priimek = i.Osebe_NOZ.Priimek
                                } : new EmployeeSimpleModel { Ime = "", Priimek = "" }),
                                NarociloOptimalnihZalogPozicija = (from pos in context.NarociloOptimalnihZalogPozicija
                                                                   where pos.NarociloOptimalnihZalogID == i.NarociloOptimalnihZalogID
                                                                   select new OptimalStockOrderPositionModel
                                                                   {
                                                                       KategorijaNaziv = pos.KategorijaNaziv,
                                                                       Kolicina = pos.Kolicina.HasValue ? pos.Kolicina.Value : 0,
                                                                       KolicinaPol = pos.KolicinaPol.HasValue ? pos.KolicinaPol.Value : 0,
                                                                       NarociloOptimalnihZalogID = pos.NarociloOptimalnihZalogID,
                                                                       NarociloOptimalnihZalogPozicijaID = pos.NarociloOptimalnihZalogPozicijaID,
                                                                       NazivArtikla = pos.NazivArtikla,
                                                                       Opombe = pos.Opombe,
                                                                       ts = pos.ts.HasValue ? pos.ts.Value : DateTime.MinValue,
                                                                       tsIDOsebe = pos.tsIDOsebe.HasValue ? pos.tsIDOsebe.Value : 0,
                                                                       tsUpdate = pos.tsUpdate.HasValue ? pos.tsUpdate.Value : DateTime.MinValue,
                                                                       tsUpdateUserID = pos.tsUpdateUserID.HasValue ? pos.tsUpdateUserID.Value : 0,
                                                                       IdentArtikla_P = pos.IdentArtikla_P,
                                                                   })
                            };

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public int SaveOptimalStockOrder(OptimalStockOrderModel model, bool updateRecord = true, bool copyOrder = false)
        {
            try
            {
                NarociloOptimalnihZalog noz = new NarociloOptimalnihZalog();
                noz.DatumOddaje = model.DatumOddaje.Equals(DateTime.MinValue) ? (DateTime?)null : model.DatumOddaje;
                noz.Kolicina = model.Kolicina;
                noz.NarociloID_P = model.NarociloID_P;
                noz.NarociloOddal = model.NarociloOddal <= 0 ? (int?)null : model.NarociloOddal;
                noz.NarociloOptimalnihZalogID = copyOrder ? 0 : model.NarociloOptimalnihZalogID;
                noz.NarociloOptimalnihZalogStevilka = model.NarociloOptimalnihZalogStevilka;
                noz.Naziv = model.Naziv;
                noz.Opombe = model.Opombe;
                noz.PotDokumenta = model.PotDokumenta;
                noz.StatusID = model.StatusID;
                noz.StrankaID = model.StrankaID;
                noz.ts = model.ts;
                noz.tsIDOsebe = model.tsIDOsebe;
                noz.tsUpdate = DateTime.Now;
                noz.tsUpdateUserID = model.tsUpdateUserID;

                if (noz.NarociloOptimalnihZalogID == 0)
                {
                    noz.NarociloOptimalnihZalogStevilka = GetNextOptimalStockOrderNum();
                    noz.ts = DateTime.Now;
                    noz.tsIDOsebe = model.tsIDOsebe;

                    if (copyOrder)
                    {
                        string statusCode = Enums.StatusOfOptimalStock.KOPIRANO_NAROCILO.ToString();
                        noz.DatumOddaje = (DateTime?)null;
                        noz.NarociloID_P = null;
                        noz.Naziv = "Kopirano naročilo optimalne zaloge dne, " + DateTime.Now.ToString("dd. MMMM yyyy");
                        noz.StatusID = context.StatusNarocilaOptimalnihZalog.Where(s => s.Koda == statusCode).FirstOrDefault().StatusNarocilaOptimalnihZalogID;
                        noz.NarociloOddal = (int?)null;
                    }

                    context.NarociloOptimalnihZalog.Add(noz);
                    context.SaveChanges();

                    model.NarociloOptimalnihZalogStevilka = noz.NarociloOptimalnihZalogStevilka;
                }
                else
                {
                    if (updateRecord)
                    {
                        NarociloOptimalnihZalog original = context.NarociloOptimalnihZalog.Where(i => i.NarociloOptimalnihZalogID == noz.NarociloOptimalnihZalogID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(noz);
                        context.SaveChanges();
                    }
                }

                if (model.NarociloOptimalnihZalogPozicija != null && model.NarociloOptimalnihZalogPozicija.Count() > 0)
                {
                    SaveOptimalStockPositionOrder(model.NarociloOptimalnihZalogPozicija.ToList(), noz.NarociloOptimalnihZalogID, copyOrder);
                }

                return noz.NarociloOptimalnihZalogID;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public bool DeleteOptimalStockOrder(int ID)
        {
            try
            {
                var optimalStock = context.NarociloOptimalnihZalog.Where(i => i.NarociloOptimalnihZalogID == ID).FirstOrDefault();

                if (optimalStock != null)
                {
                    context.NarociloOptimalnihZalog.Remove(optimalStock);
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

        public List<OptimalStockOrderPositionModel> GetOptimalStockOrderPositionsByOrderID(int orderID)
        {
            try
            {
                var query = from pos in context.NarociloOptimalnihZalogPozicija
                            where pos.NarociloOptimalnihZalogID == orderID
                            select new OptimalStockOrderPositionModel
                            {
                                KategorijaNaziv = pos.KategorijaNaziv,
                                Kolicina = pos.Kolicina.HasValue ? pos.Kolicina.Value : 0,
                                KolicinaPol = pos.KolicinaPol.HasValue ? pos.KolicinaPol.Value : 0,
                                NarociloOptimalnihZalogID = pos.NarociloOptimalnihZalogID,
                                NarociloOptimalnihZalogPozicijaID = pos.NarociloOptimalnihZalogPozicijaID,
                                NazivArtikla = pos.NazivArtikla,
                                Opombe = pos.Opombe,
                                ts = pos.ts.HasValue ? pos.ts.Value : DateTime.MinValue,
                                tsIDOsebe = pos.tsIDOsebe.HasValue ? pos.tsIDOsebe.Value : 0,
                                tsUpdate = pos.tsUpdate.HasValue ? pos.tsUpdate.Value : DateTime.MinValue,
                                tsUpdateUserID = pos.tsUpdateUserID.HasValue ? pos.tsUpdateUserID.Value : 0,
                                IdentArtikla_P = pos.IdentArtikla_P,
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public OptimalStockOrderPositionModel GetOptimalStockOrderPositionByID(int ID)
        {
            try
            {
                var query = from pos in context.NarociloOptimalnihZalogPozicija
                            where pos.NarociloOptimalnihZalogPozicijaID == ID
                            select new OptimalStockOrderPositionModel
                            {
                                KategorijaNaziv = pos.KategorijaNaziv,
                                Kolicina = pos.Kolicina.HasValue ? pos.Kolicina.Value : 0,
                                KolicinaPol = pos.KolicinaPol.HasValue ? pos.KolicinaPol.Value : 0,
                                NarociloOptimalnihZalogID = pos.NarociloOptimalnihZalogID,
                                NarociloOptimalnihZalogPozicijaID = pos.NarociloOptimalnihZalogPozicijaID,
                                NazivArtikla = pos.NazivArtikla,
                                Opombe = pos.Opombe,
                                ts = pos.ts.HasValue ? pos.ts.Value : DateTime.MinValue,
                                tsIDOsebe = pos.tsIDOsebe.HasValue ? pos.tsIDOsebe.Value : 0,
                                tsUpdate = pos.tsUpdate.HasValue ? pos.tsUpdate.Value : DateTime.MinValue,
                                tsUpdateUserID = pos.tsUpdateUserID.HasValue ? pos.tsUpdateUserID.Value : 0,
                                IdentArtikla_P = pos.IdentArtikla_P,
                            };

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public int SaveOptimalStockPositionOrder(OptimalStockOrderPositionModel model, bool updateRecord = true)
        {
            try
            {
                NarociloOptimalnihZalogPozicija nozp = new NarociloOptimalnihZalogPozicija();
                nozp.KategorijaNaziv = model.KategorijaNaziv;
                nozp.Kolicina = model.Kolicina;
                nozp.NarociloOptimalnihZalogID = model.NarociloOptimalnihZalogID;
                nozp.NarociloOptimalnihZalogPozicijaID = model.NarociloOptimalnihZalogPozicijaID;
                nozp.NazivArtikla = model.NazivArtikla;
                nozp.Opombe = model.Opombe;
                nozp.ts = model.ts;
                nozp.tsIDOsebe = model.tsIDOsebe;
                nozp.tsUpdate = DateTime.Now;
                nozp.tsUpdateUserID = model.tsUpdateUserID;
                nozp.IdentArtikla_P = model.IdentArtikla_P;

                if (nozp.NarociloOptimalnihZalogPozicijaID == 0)
                {
                    nozp.ts = DateTime.Now;
                    nozp.tsIDOsebe = model.tsIDOsebe;

                    context.NarociloOptimalnihZalogPozicija.Add(nozp);
                    context.SaveChanges();
                }
                else
                {
                    if (updateRecord)
                    {
                        NarociloOptimalnihZalogPozicija original = context.NarociloOptimalnihZalogPozicija.Where(i => i.NarociloOptimalnihZalogPozicijaID == nozp.NarociloOptimalnihZalogPozicijaID).FirstOrDefault();
                        context.Entry(original).CurrentValues.SetValues(nozp);
                        context.SaveChanges();
                    }
                }

                return nozp.NarociloOptimalnihZalogID;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public bool DeleteOptimalStockPosition(int ID)
        {
            try
            {
                var optimalStockPos = context.NarociloOptimalnihZalogPozicija.Where(i => i.NarociloOptimalnihZalogPozicijaID == ID).FirstOrDefault();

                if (optimalStockPos != null)
                {
                    context.NarociloOptimalnihZalogPozicija.Remove(optimalStockPos);
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

        private string GetNextOptimalStockOrderNum()
        {
            try
            {
                int num = 0;
                string prefix = "";
                var item = context.Nastavitve_NOZ.OrderByDescending(n => n.ts.Value).FirstOrDefault();

                if (item != null && item.ts.Value.Year == DateTime.Now.Year)
                {
                    num = item.NarociloOptimalnihZalogStev.HasValue ? item.NarociloOptimalnihZalogStev.Value : 0;
                    prefix = String.IsNullOrEmpty(item.NarociloOptimalnihZalogPredpona) ? DateTime.Now.Year.ToString() : item.NarociloOptimalnihZalogPredpona;
                    num += 1;

                    item.NarociloOptimalnihZalogPredpona = prefix;
                    item.NarociloOptimalnihZalogStev = num;
                }
                else
                {
                    num += 1;
                    prefix = DateTime.Now.Year.ToString();

                    Nastavitve_NOZ nas = new Nastavitve_NOZ();
                    nas.NastavitveID = 0;
                    nas.NarociloOptimalnihZalogPredpona = prefix;
                    nas.NarociloOptimalnihZalogStev = num;
                    nas.ts = DateTime.Now;
                    nas.tsIDOsebe = 0;
                    nas.tsUpdate = DateTime.Now;
                    nas.tsUpdateUserID = 0;

                    context.Nastavitve_NOZ.Add(nas);
                }

                context.SaveChanges();

                return prefix + "/" + num;
            }
            catch (Exception ex)
            {
                throw new Exception("GetNextPovprasevanjeStevilka Method Error! ", ex);
            }
        }

        public void SaveOptimalStockPositionOrder(List<OptimalStockOrderPositionModel> model, int orderID, bool copyOrder = false)
        {
            try
            {
                foreach (var item in model)
                {
                    if (copyOrder)
                        item.NarociloOptimalnihZalogPozicijaID = 0;

                    NarociloOptimalnihZalogPozicija nozp = context.NarociloOptimalnihZalogPozicija.Where(z => z.NarociloOptimalnihZalogPozicijaID == item.NarociloOptimalnihZalogPozicijaID).FirstOrDefault() ?? new NarociloOptimalnihZalogPozicija();
                    nozp.NarociloOptimalnihZalogID = orderID;
                    nozp.NarociloOptimalnihZalogPozicijaID = nozp.NarociloOptimalnihZalogPozicijaID > 0 ? nozp.NarociloOptimalnihZalogPozicijaID : item.NarociloOptimalnihZalogPozicijaID;
                    nozp.KategorijaNaziv = item.KategorijaNaziv;
                    nozp.Kolicina = item.Kolicina;
                    nozp.KolicinaPol = item.KolicinaPol;
                    nozp.NazivArtikla = item.NazivArtikla.Trim();
                    nozp.Opombe = item.Opombe;
                    nozp.ts = item.ts;
                    nozp.tsIDOsebe = item.tsIDOsebe;
                    nozp.tsUpdate = DateTime.Now;
                    nozp.tsUpdateUserID = item.tsUpdateUserID;
                    nozp.IdentArtikla_P = item.IdentArtikla_P;

                    if (nozp.NarociloOptimalnihZalogPozicijaID == 0)
                    {
                        nozp.ts = DateTime.Now;
                        nozp.tsIDOsebe = item.tsIDOsebe;

                        context.NarociloOptimalnihZalogPozicija.Add(nozp);
                    }
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_08, ex);
            }
        }

        public List<ProductCategory> GetCategoryList()
        {
            try
            {
                return msSqlRepo.GetCategoryList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public List<ProductColor> GetColorListByCategory(string category)
        {
            try
            {
                if (String.IsNullOrEmpty(category)) return null;
                List<string> categories = null;
                if (category.Contains(", "))
                    categories = category.Split(',').Where(s => !String.IsNullOrEmpty(s) && !String.IsNullOrWhiteSpace(s)).ToList();
                else
                    categories = new List<string> { category };

                List<ProductColor> list = new List<ProductColor>();

                foreach (var item in categories)
                {
                    var colors = msSqlRepo.GetColorListByCategory(item.Trim());

                    // najprej pogledam če je notri bela (WHITE) in je potrebno da je prva v seznamu
                    var itmW = colors.FirstOrDefault(g => g.Naziv == "WHITE");
                    if (itmW != null) list.Add(itmW);
                    colors.Remove(itmW);

                    list.AddRange(colors);
                }

                list = list.GroupBy(g => g.Naziv).Select(s => s.FirstOrDefault()).ToList();

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        /// <summary>
        /// Preverimo če že obstaja kategorija v seznamu, če obstaj dodamo vsoto, če ne jo domao v seznam
        /// </summary>
        /// <param name="sNazivPodKategorije"></param>
        /// <param name="lSubCategoryBySum"></param>
        /// <returns></returns>
        private int GetOrAddSubCategoryOnCalculateSum(SumSubCategoryModel sc, List<SumSubCategoryModel> lSubCategoryBySum, GetProductsByOptimalStockValuesModel product)
        {
            int hasStock = 0;

            SumSubCategoryModel findsc = lSubCategoryBySum.Where(pk => pk.NazivPodKategorije == sc.NazivPodKategorije).FirstOrDefault();


            if (findsc == null)
            {
                lSubCategoryBySum.Add(sc);
                findsc = sc;
            }
            else
            {
                findsc.VsotaLetneProdaje += sc.VsotaLetneProdaje;
            }

            // dodamo vse produkte v prodajno kategorijo, da lahko potem izberemo vsak artikel ne glede na podkategorijo
            if (findsc.ChildProducts == null) findsc.ChildProducts = new List<GetProductsByOptimalStockValuesModel>();
            GetProductStockByIdentModel pm = msSqlRepo.GetProductByIdent(product.IDENT);
            product.TrenutnaZaloga = (pm != null ? pm.Zaloga : 0);

            // seštevamo zalogo za podskupino
            findsc.VsotaZaloge += (pm != null ? pm.Zaloga : 0);

            // če ima zalogo je potrebno tega dobavitleja označiti, da ima zalogo
            hasStock = ((pm != null && pm.Zaloga > 0) ? 1 : 0);
            findsc.ChildProducts.Add(product);

            return hasStock;

        }

        private void AddOrUpdateSupplierHasStock(string sSuplier, string sSuplierAlter, hlpOptimalStockOrderModel hlpOptimalStock, int iHasStock, decimal dStock, int iHasProdaja)
        {
            if (!hlpOptimalStock.Suppliers.Exists(s => s.NazivPrvi.Contains(sSuplier)))
            {
                tempNum++;
                hlpOptimalStock.Suppliers.Add(new ClientSimpleModel { NazivPrvi = sSuplier, TempID = tempNum, HasStock = iHasStock, StockNumber = dStock, HasProdaja = iHasProdaja });
            }
            else
            {
                ClientSimpleModel Supplier = hlpOptimalStock.Suppliers.Where(s => s.NazivPrvi == sSuplier).FirstOrDefault();
                if (Supplier != null)
                {
                    Supplier.HasStock = (Supplier.HasStock != iHasStock && iHasStock == 1) ? Supplier.HasStock = iHasStock : Supplier.HasStock;
                    Supplier.HasProdaja = (Supplier.HasProdaja != iHasProdaja && iHasProdaja == 1) ? Supplier.HasProdaja = iHasProdaja : Supplier.HasProdaja;
                    if (Supplier.HasStock == 1)
                    {
                        Supplier.StockNumber += dStock;
                    }
                }
            }

            // če gre za SAPPI dobavitelja je potrebno dodati v seznam poddobaviteljev sSupplierAlter, zaradi prikaza potencialnih SAPPi dobaviteljev v 3 koraku
            if (sSuplier != sSuplierAlter)
            {
                ClientSimpleModel SelSupplier = hlpOptimalStock.Suppliers.Where(s => s.NazivPrvi == sSuplier).FirstOrDefault();
                if (SelSupplier != null)
                {
                    SelSupplier.SubSupplier = (SelSupplier.SubSupplier == null) ? new List<ClientSimpleModel>() : SelSupplier.SubSupplier;

                    if (!SelSupplier.SubSupplier.Exists(s => s.NazivPrvi.Contains(sSuplierAlter)))
                    {
                        tempNumAlterSupplier++;
                        SelSupplier.SubSupplier.Add(new ClientSimpleModel { NazivPrvi = sSuplierAlter, TempID = tempNumAlterSupplier, HasStock = iHasStock, StockNumber = dStock, HasProdaja = iHasProdaja });
                    }
                }

            }
        }

        // Globalni Števic za Potencialne Dobavitelje
        int tempNum = 0;
        int tempNumAlterSupplier = 0;
        /// <summary>
        /// Dobimo naziv za podgrupo, ki se prikaže v TV
        /// </summary>
        /// <param name="GroupID"></param>
        /// <returns></returns>
        private SumSubCategoryModel GetSubGroupBySales(int GroupID, hlpOptimalStockOrderModel hlpOptimalStock, OptimalStockColumnsModel columnValues)
        {

            int iHasStock = 0;

            List<SumSubCategoryModel> lSubCategoryBySum = new List<SumSubCategoryModel>();
            List<GetProductsByOptimalStockValuesModel> lProducts = msSqlRepo.GetProductSalesQtyByGroupID(GroupID);
            if (lProducts.Count == 0) return null;
            foreach (var item in lProducts)
            {
                item.DOBAVITELJ = item.DOBAVITELJ.Trim();
                item.DOBAVITELJ_ALTER = item.DOBAVITELJ.Trim();
                item.DOBAVITELJ = item.DOBAVITELJ.Contains("SAPPI") ? "SAPPI" : item.DOBAVITELJ;
                iHasStock = 0;
                SumSubCategoryModel sc = GetSubCategoryFromNaziv(item.IDENT, item.NAZIV, item.LetnaProdaja);
                item.NazivPodkategorije = sc.NazivPodKategorije;
                iHasStock = GetOrAddSubCategoryOnCalculateSum(sc, lSubCategoryBySum, item);

                // dodamo še potencialne dobavitelje

                item.DOBAVITELJ = item.DOBAVITELJ.Trim();
                if (hlpOptimalStock.Suppliers == null) hlpOptimalStock.Suppliers = new List<ClientSimpleModel>();

                AddOrUpdateSupplierHasStock(item.DOBAVITELJ.Trim(), item.DOBAVITELJ_ALTER, hlpOptimalStock, iHasStock, item.TrenutnaZaloga, 1);
            }



            // preverimo še vse produkte v skupini ali obstaja dobavitelj, katerega artikel je na zalogi in ni med najbolj prodajani
            var productsOnGoup = msSqlRepo.GetProductsByOptimalStockValues(columnValues);
            foreach (var pr in productsOnGoup)
            {
                GetProductStockByIdentModel pm = msSqlRepo.GetProductByIdent(pr.IDENT);
                if (pm == null) continue;
                pr.TrenutnaZaloga = (pm != null ? pm.Zaloga : 0);
                // če ima zalogo je potrebno tega dobavitleja označiti, da ima zalogo
                iHasStock = ((pm != null && pm.Zaloga > 0) ? 1 : 0);
                //preverimo ali product še ne obstaja v seznamu najbolj prodajanih v enem letu, če ne obstaja preverimo ali ima zalog, če ne obstaja in 
                // ima zalogo ugotovimo h kateri podskupini spada in mu dodamo zalogo v to podskupino
                if (lProducts.Where(p => p.IDENT == pr.IDENT).FirstOrDefault() == null)
                {
                    if (pm != null)
                    {
                        if (pm.Zaloga > 0)
                        {
                            SumSubCategoryModel sc = GetSubCategoryFromNaziv(pm.IDENT, pr.NAZIV, 0);
                            iHasStock = GetOrAddSubCategoryOnCalculateSum(sc, lSubCategoryBySum, pr);
                            //sc.VsotaZaloge += pm.Zaloga;
                        }
                    }
                }

                pr.DOBAVITELJ = pr.DOBAVITELJ.Trim();
                pr.DOBAVITELJ_ALTER = pr.DOBAVITELJ.Trim();
                pr.DOBAVITELJ = pr.DOBAVITELJ.Contains("SAPPI") ? "SAPPI" : pr.DOBAVITELJ;

                AddOrUpdateSupplierHasStock(pr.DOBAVITELJ.Trim(), pr.DOBAVITELJ_ALTER, hlpOptimalStock, iHasStock, pm.Zaloga, 0);
            }









            SumSubCategoryModel scBigestSum = lSubCategoryBySum.OrderByDescending(sc => sc.VsotaLetneProdaje).First();




            hlpOptimalStock.lAllSubCategory = lSubCategoryBySum;

            // sortiramo še dobavitelje
            hlpOptimalStock.Suppliers = hlpOptimalStock.Suppliers.OrderByDescending(s => s.StockNumber).ToList().OrderBy(s => s.HasProdaja).ToList();

            return scBigestSum;
        }

        /// <summary>
        /// sestavimo podgrupo
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private SumSubCategoryModel GetSubCategoryFromNaziv(string sIdent, string sNaziv, decimal dLetnaProdaja)
        {
            SumSubCategoryModel sc = new SumSubCategoryModel();

            if (sIdent != null)
            {
                sIdent = sIdent.Trim().ToUpper();
                string sLast = sIdent.Substring(sIdent.Length - 1);
                sc.Pefc = (sLast == "P") ? "PEFC CERTIFIED" : "";
            }

            if (sIdent != null)
            {
                sIdent = sIdent.Trim().ToUpper();
                string sLast = sIdent.Substring(sIdent.Length - 1);
                sc.Fsc = (sLast == "F") ? "FSC" : "";
            }

            sc.VsotaLetneProdaje = dLetnaProdaja;

            if (sNaziv != null)
            {
                sNaziv = sNaziv.Trim().ToUpper();


                string[] split = sNaziv.Split(' ');
                foreach (var item in split)
                {
                    if (item.Contains("GLOSS"))
                    {
                        sc.Gloss = "GLOSS";
                    }

                    if (item.Contains("MATT") || item.Contains("SATIN") || item.Contains("SILK"))
                    {
                        sc.Gloss = "MAT";
                    }

                    // weight
                    if (item.Contains("g") || item.Contains("G"))
                    {
                        string[] splWeight = item.Split('G');
                        if (splWeight.Length == 2 && DataTypesHelper.IsNumeric(splWeight[0].ToString()))
                        {
                            sc.Gramatura = item;
                        }
                    }

                    // size
                    if (item.Contains("x") || item.Contains("X"))
                    {
                        string[] splSize = item.Split('X');
                        if (splSize.Length == 2 && DataTypesHelper.IsNumeric(splSize[0].ToString()) && DataTypesHelper.IsNumeric(splSize[1].ToString()))
                        {
                            sc.Velikost = item;
                        }
                    }

                    // tek
                    if ((item == "BB") || (item == "SB"))
                    {
                        sc.Tek = item;
                    }

                    // paket
                    if ((item == "PK") || (item == "pk"))
                    {
                        sc.Paket = item;
                    }
                }
            }
            sc.NazivPodKategorije = sc.Gramatura + " " + sc.Velikost + " " + sc.Tek;
            sc.NazivPodKategorije += (sc.Pefc.Length > 0) ? " " + sc.Pefc : "";
            sc.NazivPodKategorije += (sc.Fsc.Length > 0) ? " " + sc.Fsc : "";
            sc.NazivPodKategorije += (sc.Paket != null) ? " " + sc.Paket : "";
            sc.NazivPodKategorijeFilter += sc.Gloss + " " + sc.NazivPodKategorije;
            return sc;
        }



        /// <summary>
        /// posodobili bome vse izbrane podkategorije z produkti za posametnega dobavitelja
        /// </summary>
        /// <param name="list"></param>
        /// <param name="color"></param>
        /// <param name="hlpOptimalStock"></param>
        /// <returns></returns>
        public hlpOptimalStockOrderModel UpdateSubCategoriesWithProductsForSelectedNodes(List<OptimalStockTreeHierarchy> list, string color, hlpOptimalStockOrderModel hlpOptimalStock)
        {
            try
            {
                //var mainGroupProducts = msSqlRepo.GetMainProducts();
                List<OptimalStockColumnsModel> lGetDimIdentOptList = msSqlRepo.GetDIMIdentiOPTList();
                List<GetProductsByOptimalStockValuesModel> productsOnGroup = null;

                var listOfLeafs = list.Where(ost => ost.IsLeaf).ToList();
                int maxID = list.Max(m => m.ID);

                foreach (var leaf in listOfLeafs)
                {
                    var columnValues = GetColumnValuesForOptimalStock(list, leaf, color);

                    productsOnGroup = msSqlRepo.GetProductsByOptimalStockValues(columnValues);

                    productsOnGroup = hlpOptimalStock.bRefreshTreeLastMonth ? productsOnGroup.Where(p => p.DATUMZAP >= DateTime.Now && p.DATUMZAP <= DateTime.Now.AddMonths(-1)).ToList() : productsOnGroup;
                    // pridobimo vse artikle za skupino in pa najbolj prodajano podskupino primer Skupina, ki jo potem prikažemo na treview 130G 45X64 SB PEFC 
                    // preverimo še vse produkte v skupini ali obstaja dobavitelj, katerega artikel je na zalogi in ni med najbolj prodajani
                    // če je zahteva po vseh artikleh za enega dobavitelja, potem se izvede ta koda, ki doda vse artikle za izbranega dobavitlja
                    if (hlpOptimalStock.sSelectedSupplier != null && hlpOptimalStock.sSelectedSupplier.Length > 0)
                    {
                        productsOnGroup = productsOnGroup.Where(p => p.DOBAVITELJ.Trim() == hlpOptimalStock.sSelectedSupplier.Trim()).ToList();
                        foreach (var pr in productsOnGroup)
                        {


                            SumSubCategoryModel sc = GetSubCategoryFromNaziv(pr.IDENT, pr.NAZIV, 0);
                            pr.DOBAVITELJ = pr.DOBAVITELJ.Trim();
                            List<OptimalStockTreeHierarchy> selListProductForUpdate = list.Where(l => (l.NazivPodkategorije == sc.NazivPodKategorije && l.Gloss == sc.Gloss)).ToList();

                            foreach (var item in selListProductForUpdate)
                            {

                                var cp = item.Product.ChildProducts.FirstOrDefault(p => p.IDENT.Trim().Contains(pr.IDENT.Trim()));
                                if (cp == null)
                                {
                                    item.Product.ChildProducts.Add(pr);
                                }
                            }


                            //iHasStock = GetOrAddSubCategoryOnCalculateSum(sc, lSubCategoryBySum, pr);
                        }
                    }
                }

                hlpOptimalStock.SubCategoryWithProducts = list;

                return hlpOptimalStock;
            }
            catch (Exception ex)
            {
                throw new Exception("Get products for selected optimal stock error!", ex);
            }
        }


        public hlpOptimalStockOrderModel GetProductsForSelectedOptimalStock(List<OptimalStockTreeHierarchy> list, string color, hlpOptimalStockOrderModel hlpOptimalStock)
        {
            try
            {
                //var mainGroupProducts = msSqlRepo.GetMainProducts();
                List<OptimalStockColumnsModel> lGetDimIdentOptList = msSqlRepo.GetDIMIdentiOPTList();

                var listOfLeafs = list.Where(ost => ost.IsLeaf).ToList();
                int maxID = list.Max(m => m.ID);

                foreach (var leaf in listOfLeafs)
                {
                    var columnValues = GetColumnValuesForOptimalStock(list, leaf, color);
                    // TODO: dobiti moramo pravo Groupo, lahko iz ColumnValues ali pa če prenesem izbrano skupino na server
                    OptimalStockColumnsModel dimGroup = lGetDimIdentOptList.Where(i => i.Gloss == columnValues.Gloss && i.Gramatura == columnValues.Gramatura && i.Velikost == columnValues.Velikost && i.Tek == columnValues.Tek).FirstOrDefault();
                    if (dimGroup == null) throw new Exception("Napačna izbrana skupina");
                    int iGroupID = dimGroup.ID;
                    // pridobimo vse artikle za skupino in pa najbolj prodajano podskupino primer Skupina, ki jo potem prikažemo na treview 130G 45X64 SB PEFC 
                    SumSubCategoryModel scSubGroupSales = GetSubGroupBySales(iGroupID, hlpOptimalStock, columnValues);

                    if (scSubGroupSales != null)
                    {
                        // TODO: Martin preveri ali se bo zgradil treview   
                        GetProductsByOptimalStockValuesModel mainSubGroup = new GetProductsByOptimalStockValuesModel();
                        mainSubGroup.NAZIV = scSubGroupSales.NazivPodKategorije;
                        mainSubGroup.AllSubCategories = hlpOptimalStock.lAllSubCategory.Where(op => op.NazivPodKategorije != scSubGroupSales.NazivPodKategorije).ToList();
                        mainSubGroup.ChildProducts = scSubGroupSales.ChildProducts;

                        int cntGrp = mainSubGroup.AllSubCategories.Count;

                        //v drevo dodamo nosilni izdelek
                        list.Add(new OptimalStockTreeHierarchy
                        {
                            ID = ++maxID,
                            IsLeaf = false,
                            IsProcessed = true,
                            IsProduct = true,
                            Name = (cntGrp > 0) ? mainSubGroup.NAZIV + " (" + cntGrp + " podskupin)" : mainSubGroup.NAZIV,
                            NazivPodkategorije = mainSubGroup.NAZIV,
                            NazivPodkategorijeFilter = scSubGroupSales.NazivPodKategorijeFilter,
                            Gloss = columnValues.Gloss,
                            ParentID = leaf.ID,
                            Product = mainSubGroup,
                            KolicinaOptimalna = 0,
                            KolicinaZaloga = scSubGroupSales.VsotaZaloge,
                            //VsotaZalNarRazlikaOpt = 0
                            VsotaZalNarRazlikaOpt = ((leaf.KolicinaZaloga + leaf.KolicinaNarocenoVTeku) - leaf.KolicinaOptimalna)
                        });

                    }
                }

                // poiščemo tistega dobavitelja, ki je bil zadnji naročeni - velja samo za subsuplier 
                foreach (ClientSimpleModel supp in hlpOptimalStock.Suppliers)
                {
                    
                    if (supp.NazivPrvi == "SAPPI")
                    {
                        supp.NazivPrvi = supp.NazivPrvi.ToUpper().Trim();
                        // get all suupiler like %sappi%
                        supp.SubSupplier = new List<ClientSimpleModel>();
                        supp.SubSupplier = msSqlRepo.GetSupplierListByNameLike(supp.NazivPrvi);
                    }

                    if (supp.SubSupplier != null && supp.SubSupplier.Count > 0)
                    {
                        foreach (ClientSimpleModel subSupplier in supp.SubSupplier)
                        {
                            string supName = msSqlRepo.GetLastSupplierByName(supp.NazivPrvi);
                            if (supName.Length > 0)
                            {
                                subSupplier.LastSupplier = (subSupplier.NazivPrvi.Trim() == supName.Trim() ? 1 : 0);
                            }
                        }
                    }
                }


                hlpOptimalStock.SubCategoryWithProducts = list;
                return hlpOptimalStock;
            }
            catch (Exception ex)
            {
                throw new Exception("Get products for selected optimal stock error!", ex);
            }
        }

        private OptimalStockColumnsModel GetColumnValuesForOptimalStock(List<OptimalStockTreeHierarchy> list, OptimalStockTreeHierarchy leaf, string color)
        {
            OptimalStockColumnsModel item = new OptimalStockColumnsModel();
            item.Barva = color;
            int indeks = 1;
            while (leaf != null && leaf.ID > 0)
            {
                switch (indeks)
                {
                    case 1:
                        item.Tek = leaf.Name;
                        break;
                    case 2:
                        item.Velikost = leaf.Name;
                        break;
                    case 3:
                        item.Gramatura = leaf.Name;
                        break;
                    case 4:
                        item.Gloss = leaf.Name;
                        break;
                    case 5:
                        item.Kategorija = leaf.Name;
                        break;
                }

                leaf = list.Where(ost => ost.ID == leaf.ParentID).FirstOrDefault();
                indeks++;
            }

            return item;
        }

        public List<OptimalStockOrderStatusModel> GetOptimalStockStatuses()
        {
            try
            {
                var query = from stat in context.StatusNarocilaOptimalnihZalog
                            select new OptimalStockOrderStatusModel
                            {
                                Koda = stat.Koda,
                                Naziv = stat.Naziv,
                                Opis = stat.Opis,
                                StatusNarocilaOptimalnihZalogID = stat.StatusNarocilaOptimalnihZalogID,
                                ts = stat.ts.HasValue ? stat.ts.Value : DateTime.MinValue,
                                tsIDOseba = stat.tsIDOseba.HasValue ? stat.tsIDOseba.Value : 0,
                                tsUpdate = stat.tsUpdate.HasValue ? stat.tsUpdate.Value : DateTime.MinValue,
                                tsUpdateUserID = stat.tsUpdateUserID.HasValue ? stat.tsUpdateUserID.Value : 0
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public OptimalStockOrderStatusModel GetOptimalStockStatusByID(int id)
        {
            try
            {
                var query = from stat in context.StatusNarocilaOptimalnihZalog
                            where stat.StatusNarocilaOptimalnihZalogID == id
                            select new OptimalStockOrderStatusModel
                            {
                                Koda = stat.Koda,
                                Naziv = stat.Naziv,
                                Opis = stat.Opis,
                                StatusNarocilaOptimalnihZalogID = stat.StatusNarocilaOptimalnihZalogID,
                                ts = stat.ts.HasValue ? stat.ts.Value : DateTime.MinValue,
                                tsIDOseba = stat.tsIDOseba.HasValue ? stat.tsIDOseba.Value : 0,
                                tsUpdate = stat.tsUpdate.HasValue ? stat.tsUpdate.Value : DateTime.MinValue,
                                tsUpdateUserID = stat.tsUpdateUserID.HasValue ? stat.tsUpdateUserID.Value : 0
                            };

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        private string GetXMLForOrderNOZ(OptimalStockOrderModel model)
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
                var employee = employeeNOZRepo.GetEmployeeByID(iUsrID);
                string ReferentID = (employee.PantheonUsrID != null && employee.PantheonUsrID.Length > 0) ? employee.PantheonUsrID : "";
                string OddelekID = "15 - SKLADIŠČE MALOPRODAJA";
                ClientFullModel SupplierNOZ = clientNOZRepo.GetClientByID(model.StrankaID);
                string Dobavitelj = (SupplierNOZ != null && SupplierNOZ.NazivPrvi.Length > 0) ? SupplierNOZ.NazivPrvi : "";
                xml.WriteElementString("ReferentId", ReferentID);
                xml.WriteElementString("Department", OddelekID); // FIX: "15 - TRANZIT"
                xml.WriteElementString("Supplier", Dobavitelj);
                xml.WriteElementString("Buyer", ConfigurationManager.AppSettings["PantheonCreateOrderDefBuyer"].ToString());
                xml.WriteElementString("OrderDate", DateTime.Now.ToString());
                xml.WriteElementString("LoadDate", DateTime.Now.ToString());
                xml.WriteElementString("DeliveryDate", DateTime.Now.ToString());
                xml.WriteElementString("Route", "");

                // define printtype
                string printType = (true) ? Enums.PrintType.A0Q.ToString() : Enums.PrintType.A0U.ToString();

                xml.WriteElementString("PrintType", printType);


                xml.WriteElementString("OrderPDFPath", ConfigurationManager.AppSettings["ServerOrderPDFPath"].ToString());
                xml.WriteElementString("OrderNote", model.Opombe);
                xml.WriteStartElement("Products");

                foreach (OptimalStockOrderPositionModel pos in model.NarociloOptimalnihZalogPozicija)
                {
                    decimal dKolicina = 0;
                    pos.Opombe = pos.Opombe != null ? pos.Opombe : "";
                    dKolicina = (pos.Kolicina > 0) ? pos.Kolicina : 0;
                    xml.WriteStartElement("Product");
                    xml.WriteElementString("DeliveryDate", DateTime.Now.ToString());
                    xml.WriteElementString("Department", OddelekID);
                    xml.WriteElementString("Ident", pos.IdentArtikla_P);
                    xml.WriteElementString("Name", pos.NazivArtikla);
                    xml.WriteElementString("Qty", dKolicina.ToString());
                    xml.WriteElementString("Price", "0");
                    xml.WriteElementString("Rabat", "0");
                    xml.WriteElementString("Note", pos.Opombe.ToString());
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

        public void CreateXMLForPantheonNOZ(OptimalStockOrderModel model)
        {
            string xml = GetXMLForOrderNOZ(model);

            DataTypesHelper.LogThis(xml);

            DataTypesHelper.LogThis("Run Create order procedure _upJM_CreateSupplierOrder");
            // run store procedure _upJM_CreateSupplierOrder
            CreateOrderDocument coData = msSqlRepo.GetOrderDocumentData(xml);

            DataTypesHelper.LogThis("Update Create order Recall Data");
            // update odpoklic - uspešno kreirana naročilnica v pantheonu


            OptimalStockOrderStatusModel stat = GetOptimalneZalogeStatusByCode(Enums.StatusOfOptimalStock.USTVARJENO_NAROCILO.ToString());
            if (stat != null)
            {
                model.StatusID = stat.StatusNarocilaOptimalnihZalogID;
            }


            model.NarociloID_P = coData.PDFFile.ToString();
            model.ts = DateTime.Now;
            model.NarociloOptimalnihZalogPozicija = null;
            SaveOptimalStockOrder(model, true);

        }

        public OptimalStockOrderStatusModel GetOptimalneZalogeStatusByCode(string statusCode)
        {
            try
            {
                var query = from type in context.StatusNarocilaOptimalnihZalog
                            where type.Koda == statusCode
                            select new OptimalStockOrderStatusModel
                            {
                                Koda = type.Koda,
                                Naziv = type.Naziv,
                                Opis = type.Opis,
                                StatusNarocilaOptimalnihZalogID = type.StatusNarocilaOptimalnihZalogID,
                                ts = type.ts.HasValue ? type.ts.Value : DateTime.MinValue,
                                tsUpdate = type.tsUpdate.HasValue ? type.tsUpdate.Value : DateTime.MinValue,
                                tsIDOseba = type.tsIDOseba.HasValue ? type.tsIDOseba.Value : 0,
                                tsUpdateUserID = type.tsIDOseba.HasValue ? type.tsIDOseba.Value : 0
                            };

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_06, ex);
            }
        }

        public bool CopyOptimalStockOrderByID(int optimalStockOrderID)
        {
            try
            {
                OptimalStockOrderModel model = GetOptimalStockOrderByID(optimalStockOrderID);
                SaveOptimalStockOrder(model, false, true);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ValidationExceptionError.res_07, ex);
            }
        }
    }
}