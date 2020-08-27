using DatabaseWebService.Models;
using DatabaseWebService.Models.Client;
using DatabaseWebService.Models.EmailMessage;
using DatabaseWebService.ModelsOTP.Recall;
using DatabaseWebService.ModelsPDO;
using DatabaseWebService.ModelsPDO.Inquiry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainPDO.Abstract
{
    public interface ISystemMessageEventsRepository_PDO
    {
        List<SystemMessageEvents_PDO> GetUnProcessedMesseges();
        void CreateEmailForSuppliers(List<GroupedInquiryPositionsBySupplier> suppliers, EmployeeFullModel inquirySubmittedByEmployee, string StevilkaPovprasevanja);
        void CreateEmailForGrafolitPurcaheDept(ClientFullModel cfmGrafolit, EmployeeFullModel InqueryEmployee, InquiryFullModel InqModel);
    }
}
