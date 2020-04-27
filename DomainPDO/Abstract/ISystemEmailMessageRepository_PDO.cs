using DatabaseWebService.ModelsPDO.Order;
using DatabaseWebService.ModelsPDO.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainPDO.Abstract
{
    public interface ISystemEmailMessageRepository_PDO
    {
        List<SystemEmailMessage_PDO> GetUnprocessedEmails();
        List<PDOEmailModel> GetAllEmails();
        void UpdateFailedMessges();
        void SaveEmail(SystemEmailMessage_PDO model);
        void CreateEmailForSupplierOrder(OrderPDOFullModel order);
        void CreateEmailForAdmin_NoPDFForOrderPDO(string sOdobritevKomentar, string sStevilkaDokumenta, string sStevilkaNarocilnice, bool bOdpoklic);
    }
}
