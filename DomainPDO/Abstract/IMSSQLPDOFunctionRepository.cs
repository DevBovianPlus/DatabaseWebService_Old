using DatabaseWebService.Models.Client;
using DatabaseWebService.Models.Employee;
using DatabaseWebService.ModelsOTP.Recall;
using DatabaseWebService.ModelsPDO.Inquiry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainPDO.Abstract
{
    public interface IMSSQLPDOFunctionRepository
    {
        List<ClientSimpleModel> GetBuyerList();
        List<ProductCategory> GetCategoryList();
        List<PantheonUsers> GetPantheonUsers();
        List<ClientSimpleModel> GetSupplierByName(string name);
        List<ProductModel> GetProductByName(string name);
        List<ProductModel> GetProductBySupplierAndName(string supplier, string name);
        CreateOrderDocument GetOrderDocumentData(string OrderDocXML);
    }
}
