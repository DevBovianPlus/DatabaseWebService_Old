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
        List<TenderFullModel> GetTenderList();
        TenderFullModel GetTenderModelByID(int tenderID);
        int SaveTender(TenderFullModel model, bool updateRecord = true);
        bool DeleteTender(int tenderID);
        List<TenderPositionModel> GetTenderListByRouteID(int routeID);
        void SaveTenders(List<TenderFullModel> model);
        List<TenderPositionModel> GetTenderPositionModelByID(int tenderID);
        int SaveTenderPosition(TenderPositionModel model, bool updateRecord = true);
        bool DeleteTenderPosition(int tenderPosID);
        int SaveTenderWithTenderPositions(TenderFullModel model, bool updateRecord = true);
        void SaveTenderPositions(List<TenderPositionModel> positions, int tenderID);

        void DeleteTenderPositions(List<int> tenderPositionsID);
        List<TransportCountModel> GetTransportCounByTransporterAndRoute(List<TransportCountModel> model);
        decimal GetLowestAndMostRecentPriceByRouteID(int routeID);
        TransportCountModel GetTransportCounByTransporterAndRoute(TransportCountModel model);

        List<TenderPositionModel> GetTenderListByRouteIDAndRecallID(int routeID, int recallID);
    }
}
