using DatabaseWebService.DomainNOZ.Abstract;
using DatabaseWebService.Models.Client;
using DatabaseWebService.Models.Employee;
using DatabaseWebService.ModelsNOZ;
using DatabaseWebService.ModelsNOZ.OptimalStockOrder;
using DatabaseWebService.ModelsOTP.Recall;
using DatabaseWebService.ModelsPDO.Inquiry;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Web;

namespace DatabaseWebService.DomainNOZ.Concrete
{
    public class MSSQLNOZFunctionRepository : IMSSQLNOZFunctionRepository
    {
        GrafolitNOZEntities context;

        public MSSQLNOZFunctionRepository(GrafolitNOZEntities _context)
        {
            context = _context;
        }

        public List<ProductColor> GetColorListByCategory(string categoryName)
        {
            if (String.IsNullOrEmpty(categoryName)) return null;

            int tempNum = 0;
            var query = from b in context.GetColorListByCategory(categoryName)
                        select new ProductColor
                        {
                            Naziv = b
                        };

            var list = query.ToList();
            foreach (var item in list)
            {
                item.TempID = ++tempNum;
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
                item.TempID = ++tempNum;
            }

            return list;
        }

        public List<GetListOptimalnaZaloga_Result> GetListOptimalnaZaloga()
        {
            var query = from s in context.GetListOptimalnaZaloga()
                        select s;

            var list = query.ToList();

            return list;
        }

        public List<GetProductsByOptimalStockValuesModel> GetProductsByOptimalStockValues(OptimalStockColumnsModel model)
        {
            var query = from p in context.GetProductsByOptimalStockValues(model.Kategorija, model.Gloss, model.Gramatura, model.Velikost, model.Tek, model.Barva)
                        select new GetProductsByOptimalStockValuesModel
                        {
                            DOBAVITELJ = p.DOBAVITELJ,
                            IDENT = p.IDENT,
                            NAZIV = p.NAZIV,
                            DATUMZAP = p.DATUMZAP.HasValue ? p.DATUMZAP.Value : DateTime.MinValue
                        };

            return query.ToList();
        }

        public GetProductStockByIdentModel GetProductByIdent(string Ident)
        {
            var query = from p in context.GetProductStockByIdent(Ident)
                        select new GetProductStockByIdentModel
                        {
                            IDENT = p.acIdent,
                            Zaloga = p.anStock.HasValue ? p.anStock.Value : 0,
                            ZalogaVrednost = p.anValue.HasValue ? p.anValue.Value : 0,
                        };

            return query.FirstOrDefault();
        }

        public string GetLastSupplierByName(string SupplierName)
        {
            var query = from s in context.GetLastSupplierByName(SupplierName) select s;                        
            return query.FirstOrDefault();
        }

        public List<GetProductsByOptimalStockValuesModel> GetProductSalesQtyByGroupID(int GroupID)
        {
            var query = from p in context.ProductSalesQtyByGroupID(GroupID)
                        select new GetProductsByOptimalStockValuesModel
                        {
                            DOBAVITELJ = p.Dobavitelj,
                            IDENT = p.acIdent,
                            NAZIV = p.Naziv,
                            LetnaProdaja = p.Qty.HasValue ? p.Qty.Value : 0,
                        };

            return query.ToList();
        }

        public List<ClientSimpleModel> GetSupplierList()
        {
            int tempNum = 0;
            var query = from s in context.GetSupplierList()
                        select new ClientSimpleModel
                        {
                            NazivPrvi = s.Dobavitelj,
                            Drzava = s.Drzava
                        };

            var list = query.ToList();
            foreach (var item in list)
            {
                item.TempID = tempNum++;
            }

            return list;
        }

        public List<ClientSimpleModel> GetSupplierListByNameLike(string SupplierName)
        {
            int tempNum = 0;
            var query = from s in context.GetSupplierListByNameLike(SupplierName)
                        select new ClientSimpleModel
                        {
                            NazivPrvi = s.Dobavitelj,
                            Drzava = s.Drzava
                        };

            var list = query.ToList();
            foreach (var item in list)
            {
                item.TempID = tempNum++;
            }

            return list;
        }

        public List<GetProductsByOptimalStockValuesModel> GetMainProducts()
        {
            var query = from q in context.IdentByOPTGroup()
                        select new GetProductsByOptimalStockValuesModel
                        {
                            IDENT = q.acIdent
                        };

            return query.ToList();
        }

        public List<OptimalStockColumnsModel> GetDIMIdentiOPTList()
        {
            var query = from p in context.GetDIMIdentiOPTList()
                        select new OptimalStockColumnsModel
                        {
                            ID = p.ID,
                            Gloss = p.Gloss,
                            Gramatura = p.Gramatura,
                            Velikost = p.Velikost,
                            Tek = p.Tek,
                            Barva = p.Barva
                        };

            return query.ToList();
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

        public List<PantheonUsers> GetPantheonUsers()
        {
            int tempNum = 0;
            var query = from c in context.GetPantheonUsersNOZ()
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
    }
}