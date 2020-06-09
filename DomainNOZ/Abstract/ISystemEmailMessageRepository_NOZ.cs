using DatabaseWebService.ModelsNOZ;
using DatabaseWebService.ModelsNOZ.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainNOZ.Abstract
{
    public interface ISystemEmailMessageRepository_NOZ
    {
        void CreateEmailForUserCreateNewCodeForProduct(CreateNewCodeMailModel model);
        void SaveEmail(SystemEmailMessage_NOZ model);
        void UpdateFailedMessges();
        List<SystemEmailMessage_NOZ> GetUnprocessedEmails();
        List<NOZEmailModel> GetAllEmailsNOZ();
    }
}
