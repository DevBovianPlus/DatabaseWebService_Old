using DatabaseWebService.ModelsOTP.Client;
using DatabaseWebService.ModelsOTP.Order;
using DatabaseWebService.ModelsOTP.Recall;
using DatabaseWebService.ModelsOTP.Tender;
using DatabaseWebService.ModelsPDO.Inquiry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainOTP.Abstract
{
    public interface IMSSQLFunctionsRepository
    {

        List<SupplierModel> GetListOfSupplier();

        List<OrderPositionModelNew> GetListOfOpenedOrderPositions(string supplier, int clientID = 0);
        List<OrderPositionModelNew> GetListOfOrderNumber10();

        TransportCountModel GetTransportCounByTransporterAndRoute(TransportCountModel model);

        CreateOrderDocument GetOrderDocumentData(string OrderDocXML);

        List<ProductCategory> GetCategoryList();
    }
}
