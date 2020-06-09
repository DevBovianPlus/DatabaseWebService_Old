using DatabaseWebService.Models.Client;
using DatabaseWebService.Models.Employee;
using DatabaseWebService.ModelsNOZ;
using DatabaseWebService.ModelsNOZ.OptimalStockOrder;
using DatabaseWebService.ModelsOTP.Recall;
using DatabaseWebService.ModelsPDO.Inquiry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainNOZ.Abstract
{
    public interface IMSSQLNOZFunctionRepository
    {
        List<ProductColor> GetColorListByCategory(string categoryName);
        List<ProductCategory> GetCategoryList();
        List<GetListOptimalnaZaloga_Result> GetListOptimalnaZaloga();
        List<GetProductsByOptimalStockValuesModel> GetProductsByOptimalStockValues(OptimalStockColumnsModel model);
        List<ClientSimpleModel> GetSupplierList();
        List<OptimalStockColumnsModel> GetDIMIdentiOPTList();
        GetProductStockByIdentModel GetProductByIdent(string Ident);
        //CreateOrderDocument GetOrderDocumentData(string OrderDocXML);
        List<GetProductsByOptimalStockValuesModel> GetMainProducts();
        List<GetProductsByOptimalStockValuesModel> GetProductSalesQtyByGroupID(int GroupID);
        CreateOrderDocument GetOrderDocumentData(string OrderDocXML);
        List<PantheonUsers> GetPantheonUsers();
        string GetLastSupplierByName(string SupplierName);
        List<ClientSimpleModel> GetSupplierListByNameLike(string SupplierName);
    }
}
