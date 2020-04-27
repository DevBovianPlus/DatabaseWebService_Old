using DatabaseWebService.ModelsOTP;
using DatabaseWebService.ModelsOTP.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainOTP.Abstract
{
    public interface IUtilityServiceRepository
    {
        void CheckForOrderTakeOver();
        void CheckForOrderTakeOver2();
        void CreateAndSendOrdersMultiple();
        //void CreatePDFAndSendPDOOrdersMultiple();
        void CheckRecallsForCarriersSubmittingPrices();
        void CheckForRecallsWithNoSubmitedPrices();
        void CreateOrderTransport(CreateOrderModel model);
        void LaunchPantheonCreatePDF();
        string GetOrderPDFFile(int iRecallID);
    }
}
