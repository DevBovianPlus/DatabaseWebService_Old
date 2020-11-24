using DatabaseWebService.Models;
using DatabaseWebService.ModelsOTP.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainOTP.Abstract
{
    public interface IRouteRepository
    {
        List<RouteModel> GetAllRoutes();
        hlpViewRoutePricesModel GetAllRoutesTransportPricesByViewType(hlpViewRoutePricesModel vRPModel);
        RouteModel GetRouteByID(int routeID);
        int SaveRoute(RouteModel model, bool updateRecord = true);
        bool DeleteRoute(int routeID);
        List<RouteModel> GetRoutesByCarrierID(int carrierID);
        List<RouteModel> GetRoutesByCarrierIDAndRouteID(int carrierID, int routeID);
    }
}
