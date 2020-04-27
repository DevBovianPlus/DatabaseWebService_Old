using DatabaseWebService.Models.EmailMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Domain.Abstract
{
    public interface ISystemEmailMessageRepository
    {
        List<SystemEmailMessage> GetUnprocessedEmails();

        void SaveEmail(SystemEmailMessage model);

        void UpdateFailedMessges();
    }
}