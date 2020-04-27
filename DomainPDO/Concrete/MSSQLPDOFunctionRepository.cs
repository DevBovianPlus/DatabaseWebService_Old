using DatabaseWebService.DomainPDO.Abstract;
using DatabaseWebService.Models.Client;
using DatabaseWebService.Models.Employee;
using DatabaseWebService.ModelsOTP.Recall;
using DatabaseWebService.ModelsPDO.Inquiry;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Web;

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
                            Dobavitelj = p.DOBAVITELJ
                        };

            var list = query.ToList();
            foreach (var item in list)
            {
                item.TempID = tempNum++;
            }

            return list;
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