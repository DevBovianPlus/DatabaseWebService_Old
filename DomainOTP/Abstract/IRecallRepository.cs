
using DatabaseWebService.ModelsOTP.Recall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainOTP.Abstract
{
    public interface IRecallRepository
    {
        List<RecallModel> GetAllRecalls();
        RecallFullModel GetRecallFullModelByID(int recallID);
        List<RecallPositionModel> GetRecallPositionsByID(int recallID);
        RecallPositionModel GetRecallPositionByID(int recallPositionID);
        int SaveRecall(RecallFullModel model, bool updateRecord = true);
        bool DeleteRecall(int recallID);

        int SaveRecallPosition(RecallPositionModel model, bool updateRecord = true);
        void SaveRecallPosition(List<RecallPositionModel> model, int recallID = 0, int ownStockWarehouseID = 0);

        bool DeleteRecallPosition(int recallPosID);

        RecallType GetRecallTypeByID(int typeId);
        RecallType GetRecallTypeByCode(string typeCode);
        List<RecallType> GetRecallTypes();

        List<RecallStatus> GetRecallStatuses();
        RecallStatus GetRecallStatusByID(int statusID);
        RecallStatus GetRecallStatusByCode(string statusCode);
        decimal GetLatestKolicinaOTPForProduct(string materialIdent);
        List<MaterialModel> GetLatestKolicinaOTPForProduct(List<MaterialModel> model);
        List<RecallPositionModel> GetRecallPosFromPartialOverTakeRecalls(List<int> recallIDs);
        bool ResetSequentialNumInRecallPos();

        List<RecallModel> GetAllTakeOverRecalls();
        List<RecallModel> GetAllNonTakeOverRecalls();


        string IsPriceSubmittingStillValid(int prijavaPrevoznikaID);
        bool SubmitPriceToPrijavaPrevoznika(int prijavaPrevoznikaID, decimal newPrice);

        List<CarrierInquiryModel> GetCarriersInquiry(int recallID);
        void ManualSelectCarrierForTransport(int prijavaPrevoznikaID);
        bool DeleteCarrierInquiry(int carrierInquiryID);

        void ResetRecallStatusByID(int iRecallID);
    }
}
