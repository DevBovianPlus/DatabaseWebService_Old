using DatabaseWebService.Common;
using DatabaseWebService.DomainPDO.Abstract;
using DatabaseWebService.Models.Client;
using DatabaseWebService.Models.Employee;
using DatabaseWebService.ModelsOTP.Recall;
using DatabaseWebService.ModelsPDO.Inquiry;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Drawing.Text;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace DatabaseWebService.DomainPDO.Concrete
{
    public class MSSQLPDOFunctionRepository : IMSSQLPDOFunctionRepository
    {
        GrafolitPDOEntities context;

        public MSSQLPDOFunctionRepository(GrafolitPDOEntities _context)
        {
            context = _context;
        }

        public List<ClientSimpleModel> GetBuyerList()
        {
            int tempNum = 0;
            var query = from b in context.GetBuyerList()
                        select new ClientSimpleModel
                        {
                            NazivPrvi = b
                        };

            var list = query.ToList();
            foreach (var item in list)
            {
                item.TempID = tempNum++;
            }

            return list;
        }

        public List<ProductCategory> GetCategoryList()
        {
            int tempNum = 0;
            var query = from c in context.GetCategoryList()
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

        public List<PantheonUsers> GetPantheonUsers()
        {
            int tempNum = 0;
            var query = from c in context.GetPantheonUsers()
                        select new PantheonUsers
                        {
                            acSubject = c.acSubject,
                            acUserId = c.acUserId,
                            anUserID = c.anUserID,
                        };

            var list = query.ToList();
            foreach (var item in list)
            {
                item.TempID = tempNum++;
            }

            return list;
        }

        public List<ClientSimpleModel> GetSupplierByName(string name)
        {
            int tempNum = 0;
            var query = from s in context.GetSuplierBySearchStr("%" + name + "%")
                        select new ClientSimpleModel
                        {
                            NazivPrvi = s
                        };

            var list = query.ToList();
            foreach (var item in list)
            {
                item.TempID = tempNum++;
            }

            return list;
        }

        public List<ProductModel> GetProductByName(string name)
        {
            int tempNum = 0;
            var query = from p in context.GetArtikelBySearchStr("%" + name + "%")
                        select new ProductModel
                        {
                            Gloss = p.Gloss,
                            Gramatura = p.Gramatura,
                            Kategorija = p.Kategorija,
                            Naziv = p.NAZIV,
                            StevilkaArtikel = p.StArtikla,
                            Tek = p.Tek,
                            Velikost = p.Velikost
                        };

            var list = query.ToList();
            foreach (var item in list)
            {
                item.TempID = tempNum++;
            }

            return list;
        }

        public List<ProductModel> GetProductBySupplierAndName(string supplier, string name)
        {
            int tempNum = 0;
            var query = from p in context.GetArtikelByName(supplier, name)
                        select new ProductModel
                        {
                            Gloss = p.Gloss,
                            Gramatura = p.Gramatura,
                            Kategorija = p.Kategorija,
                            Naziv = p.NAZIV,
                            StevilkaArtikel = p.StArtikla,
                            Tek = p.Tek,
                            Velikost = p.Velikost,
                            Dobavitelj = p.DOBAVITELJ,
                            Poreklo = p.POREKLO
                        };

            var list = query.ToList();

            // če ne vrne artikla potem preverimo še direct v Pantheon bazi
            if (list.Count == 0)
            {
                var queryP = from p in context.GetArtikelByNamePantheonOnly(supplier, name)
                             select new ProductModel
                             {                                 
                                 Naziv = p.NAZIV,                                 
                                 StevilkaArtikel = p.StArtikla,
                                 Dobavitelj = p.DOBAVITELJ
                             };

                var listP = queryP.ToList();

                if (listP.Count == 1)
                {
                    foreach (var itemP in listP)
                    {
                        ExtractDataFromName(itemP);
                        itemP.TempID = tempNum++;
                    }

                    return listP;
                }
            }

            foreach (var item in list)
            {
                item.TempID = tempNum++;
            }

            return list;
        }

        private void ExtractDataFromName(ProductModel pItem)
        {
            string sNaziv = pItem.Naziv;

            if (sNaziv != null)
            {
                sNaziv = sNaziv.Trim().ToUpper();


                string[] split = sNaziv.Split(' ');
                foreach (var item in split)
                {
                    
                    // weight
                    if (item.Contains("g") || item.Contains("G"))
                    {
                        string[] splWeight = item.Split('G');
                        if (splWeight.Length == 2 && DataTypesHelper.IsNumeric(splWeight[0].ToString()))
                        {
                            pItem.Gramatura = item;
                        }
                    }

                    

                    // tek
                    if ((item == "BB") || (item == "SB"))
                    {
                        pItem.Tek = item;
                    }

                    
                }
            }

            pItem.Kategorija = GetCategoryFromIdentNumber(pItem.StevilkaArtikel);
        }

        /// <summary>
        /// Dobimo ven Kategorije iz začetkov šifre kot jih vodijo v Pantheonu
        /// </summary>
        /// <param name="sIdent"></param>
        /// <returns></returns>
        private string GetCategoryFromIdentNumber(string sIdent)
        {
            string sPrva2 = sIdent.Substring(0, 2);
            string sPrvi4 = sIdent.Substring(0, 4);

            switch (sPrva2)
            {
                case "14":
                    return "LEPENKA";                    
                case "05":
                    return "KARTON";                    
                default:
                    break;
            }

            switch (sPrvi4)
            {
                case "0712":
                    return "OFFSET";
                case "0718":
                    return "PREMAZ";
                case "0714":
                    return "A4";
                case "0728":
                    return "NEWSPRINT & BOOK PAPERS";
                case "0721":
                    return "SAMOKOPIRNI IN TERMALNI";
                default:
                    break;
            }
            return "NEZNAN";
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


            if (sErrorDesc.Length > 0)
            {
                throw new Exception("PDO Run Procedure _upJM_CreateSupplierOrder error:" + "<br><br>" + sErrorDesc);
            }

            _coData.ExportPath = sExportPath;
            _coData.PDFFile = sPDFFileName;
            _coData.ErrorDesc = sErrorDesc;


            return _coData;
        }
    }
}