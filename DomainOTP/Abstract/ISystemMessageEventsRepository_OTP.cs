using DatabaseWebService.Models;
using DatabaseWebService.Models.EmailMessage;
using DatabaseWebService.ModelsOTP.Recall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainOTP.Abstract
{
    public interface ISystemMessageEventsRepository_OTP
    {
        List<SystemMessageEvents_OTP> GetUnProcessedMesseges();
        void CreateEmailForLeaderToApproveRecall(RecallFullModel model);
        void SaveEmailEventMessag_OTP(EmailMessageModel model);
        void CreateEmailForRecallStatusChanged(RecallFullModel model);
        void CreateEmailForCarriers(RecallFullModel recall, EmployeeFullModel inquirySubmittedByEmployee);
        void CreateEmailForCarrierOrder(RecallFullModel recall);        
        void CreateEmailCarrierSelectedOrNot(Odpoklic recall, PrijavaPrevoznika carrierSelected, bool selectedCarrier = true);
        void CreateEmailLogisticsCarrierSelected(Odpoklic recall, PrijavaPrevoznika carrierSelected);
        void CreateEmailLogisticsCarrierNotSelected(List<IGrouping<Odpoklic, PrijavaPrevoznika>> listOdpoklicPrijavaPrevoznikov);
        void CreateEmailForCarriers(List<CarrierInquiryModel> carriers);
        void CreateEmailForAdmin_NoPDFForOrderOTP(string sOdobritevKomentar, string sStevilkaDokumenta, string sStevilkaNarocilnice);
    }
}
