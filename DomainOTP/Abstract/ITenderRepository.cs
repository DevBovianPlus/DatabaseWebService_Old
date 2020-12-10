using DatabaseWebService.Models;
using DatabaseWebService.ModelsOTP.Tender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainOTP.Abstract
{
    public interface ITenderRepository
    {
        List<TenderFullModel> GetTenderList(string dtFrom, string dtTo, string FilterString);
        List<TenderPositionModel> GetTenderListPositionByTenderID(int tenderID);
        TenderFullModel GetTenderModelByID(int tenderID);
        TenderModel GetTenderSimpleModelByID(int tenderID);

        int SaveTender(TenderFullModel model, bool updateRecord = true);
        bool DeleteTender(int tenderID);
        List<TenderPositionModel> GetTenderListByRouteID(int routeID);
        List<TenderPositionModel> GetTenderListByRouteIDAndTonsID(int routeID, int tonsID, bool bShowZeroTenders);
        List<TenderPositionModel> GetTenderListByRouteIDAndTenderDate(int routeID, string TenderDate);
        void SaveTenders(List<TenderFullModel> model);
        List<TenderPositionModel> GetTenderPositionModelByID(int tenderID, DateTime ? dtFrom, DateTime ? dtTo);
        int SaveTenderPosition(TenderPositionModel model, bool updateRecord = true);
        bool DeleteTenderPosition(int tenderPosID);
        int SaveTenderWithTenderPositions(TenderFullModel model, bool updateRecord = true);
        void SaveTenderPositions(List<TenderPositionModel> positions, int tenderID);

        void DeleteTenderPositions(List<int> tenderPositionsID);
        List<TransportCountModel> GetTransportCounByTransporterAndRoute(List<TransportCountModel> model);
        decimal GetLowestAndMostRecentPriceByRouteID(int routeID);
        decimal GetLowestAndMostRecentPriceByRouteIDandZbirnikTonsID(int routeID, int ZbirnikTonID);
        TransportCountModel GetTransportCounByTransporterAndRoute(TransportCountModel model);

        List<TenderPositionModel> GetTenderListByRouteIDAndRecallID(int routeID, int recallID);

        List<TonsModel> GetAllTons();

        hlpTenderTransporterSelection PrepareDataForTenderTransport(hlpTenderTransporterSelection vTTModel);
    }
}
