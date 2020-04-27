using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWebService.DomainOTP.Abstract
{
    public interface ISystemEmailMessageRepository_OTP
    {
        List<SystemEmailMessage_OTP> GetUnprocessedEmails();
        void UpdateFailedMessges();
        void SaveEmail(SystemEmailMessage_OTP model);
    }
}
