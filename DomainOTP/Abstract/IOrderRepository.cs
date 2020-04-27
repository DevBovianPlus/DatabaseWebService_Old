using DatabaseWebService.ModelsOTP.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainOTP.Abstract
{
    public interface IOrderRepository
    {
        List<OrderModel> GetAllOrders();
        List<OrderPositionModel> GetAllOrdersPositions();
    }
}
