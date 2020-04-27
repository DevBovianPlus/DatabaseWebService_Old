using DatabaseWebService.ModelsPDO.Inquiry;
using DatabaseWebService.ModelsPDO.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainPDO.Abstract
{
    public interface IOrderPDORepository
    {
        List<OrderPDOFullModel> GetOrderList();
        OrderPDOFullModel GetOrderByID(int oID);
        List<OrderPDOPositionModel> GetOrderPositionsByOrderID(int oId);
        InquiryFullModel GetOrderPositionsByInquiryIDForNewOrder(int iId);

        

        void SaveOrderPositionsModel(List<OrderPDOPositionModel> iOrderpos, int orderID, bool updateRecord = true);
        bool DeleteOrderPosition(int orderPosId);

        int SaveOrder(OrderPDOFullModel order, bool updateRecord = true, bool CreateXMLDoc = true);
        InquiryFullModel CheckPantheonArtikles(InquiryFullModel order, bool updateRecord = true);
        bool DeleteOrder(int orderId);

        IEnumerable<IGrouping<int, OrderPDOPositionModel>> GroupOrderPositionsBySupplier(OrderPDOFullModel model);
        

        void CreatePDFAndSendPDOOrdersMultiple();
        void ResetOrderStatusByID(int iPDOOrderID);
        void LaunchPantheonCreatePDF(string file, string sParam);

        List<InquiryModel> GetAllPurchases();
    }
}
