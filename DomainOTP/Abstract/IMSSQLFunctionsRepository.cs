using DatabaseWebService.ModelsOTP.Client;
using DatabaseWebService.ModelsOTP.Order;
using DatabaseWebService.ModelsOTP.Recall;
using DatabaseWebService.ModelsOTP.Tender;
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

        TransportCountModel GetTransportCounByTransporterAndRoute(TransportCountModel model);

        CreateOrderDocument GetOrderDocumentData(string OrderDocXML);
    }
}
