using DatabaseWebService.ModelsPDO.Inquiry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainPDO.Abstract
{
    public interface IInquiryRepository
    {
        List<InquiryModel> GetInquiryList();
        InquiryFullModel GetInquiryByID(int iId, bool bOnlySelected, int iSelDobaviteljID = 0);
        int SaveInquiry(InquiryFullModel model, bool updateRecord = true, bool cpyInquery = false);
        int SaveInquiryPurchase(InquiryFullModel model, bool updateRecord = true);
        bool DeleteInquiry(int iId);
        bool CopyInquiryByID(int iId);
        List<InquiryPositionModel> GetInquiryPositionByInquiryID(int iId);

        void SaveInquiryPositionsModel(List<InquiryPositionModel> ipos, int inquiry = 0, bool cpyInquery = false);
        int SaveInquiryPositionModel(InquiryPositionModel item, bool updateRecord = true);
        InquiryPositionModel GetInquiryPositionByID(int ipID);
        InquiryPositionModel CopyInquiryPositionByID(int ipID);
        bool DeleteInquiryPosition(int ipId);
        bool DeleteInquiryPositionArtikles(List<InquiryPositionArtikelModel> itemsToDelete);

        InquiryStatusModel GetInquiryStatusByID(int statusID);
        InquiryStatusModel GetInquiryStatusByCode(string statusCode);
        List<InquiryStatusModel> GetInquiryStatuses();
        bool LockInquiry(int inquiryID, int userID);
        bool UnLockInquiry(int inquiryID, int userID);
        bool IsInquiryLocked(int inquiryID);
        bool UnLockInquiriesByUserID(int userID);
        List<InquiryPositionArtikelModel> GetInquiryPositionArtikelByInquiryPositionID(int iposID);
        InquiryFullModel GetInquiryPositionArtikelByInquiryID(InquiryFullModel inquery, int iId, bool bOnlySelected, int iSelDobaviteljID = 0);
        List<GroupedInquiryPositionsBySupplier> GetInquiryPositionsGroupedBySupplier(int inquiryID);
        void SaveInquiryPositionPdfReport(GroupedInquiryPositionsBySupplier model);

        InquiryStatus GetPovprasevanjaStatusByCode(string statusCode);        
    }
}
