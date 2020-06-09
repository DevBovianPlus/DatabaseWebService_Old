using DatabaseWebService.ModelsNOZ;
using DatabaseWebService.ModelsNOZ.OptimalStockOrder;
using DatabaseWebService.ModelsPDO.Inquiry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainNOZ.Abstract
{
    public interface IOptimalStockOrderRepository
    {

        void GetOptimalStockTree(List<string> productCategory, string color);
        List<OptimalStockTreeHierarchy> GetOptimalStockTree(string productCategory, string color);

        List<OptimalStockOrderModel> GetOptimalStockOrders();
        OptimalStockOrderModel GetOptimalStockOrderByID(int ID);
        int SaveOptimalStockOrder(OptimalStockOrderModel model, bool updateRecord = true, bool copyOrder = false);
        bool DeleteOptimalStockOrder(int ID);
        void CreateXMLForPantheonNOZ(OptimalStockOrderModel model);

        List<OptimalStockOrderPositionModel> GetOptimalStockOrderPositionsByOrderID(int orderID);
        OptimalStockOrderPositionModel GetOptimalStockOrderPositionByID(int ID);
        int SaveOptimalStockPositionOrder(OptimalStockOrderPositionModel model, bool updateRecord = true);
        void SaveOptimalStockPositionOrder(List<OptimalStockOrderPositionModel> model, int orderID, bool copyOrder = false);
        bool DeleteOptimalStockPosition(int ID);

        List<ProductCategory> GetCategoryList();
        List<ProductColor> GetColorListByCategory(string category);

        hlpOptimalStockOrderModel GetProductsForSelectedOptimalStock(List<OptimalStockTreeHierarchy> list, string color, hlpOptimalStockOrderModel hlpOptimalStock);
        hlpOptimalStockOrderModel UpdateSubCategoriesWithProductsForSelectedNodes(List<OptimalStockTreeHierarchy> list, string color, hlpOptimalStockOrderModel hlpOptimalStock);

        OptimalStockOrderStatusModel GetOptimalStockStatusByID(int id);
        List<OptimalStockOrderStatusModel> GetOptimalStockStatuses();

        bool CopyOptimalStockOrderByID(int optimalStockOrderID);
    }
}
