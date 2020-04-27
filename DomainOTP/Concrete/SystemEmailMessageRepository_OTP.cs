using DatabaseWebService.Common.Enums;
using DatabaseWebService.DomainOTP.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatabaseWebService.DomainOTP.Concrete
{
    public class SystemEmailMessageRepository_OTP : ISystemEmailMessageRepository_OTP
    {
        GrafolitOTPEntities context;

        public SystemEmailMessageRepository_OTP(GrafolitOTPEntities _context)
        {
            context = _context;
        }

        public List<SystemEmailMessage_OTP> GetUnprocessedEmails()
        {
            return context.SystemEmailMessage_OTP.Where(sem => sem.Status.Value == (int)Enums.SystemEmailMessageStatus.UnProcessed).ToList();
        }

        public void SaveEmail(SystemEmailMessage_OTP model)
        {
            if (model.SystemEmailMessageID != 0)
            {
                model.Status = (int)Enums.SystemEmailMessageStatus.Processed;
                var original = context.SystemEmailMessage_OTP.Where(sem => sem.SystemEmailMessageID == model.SystemEmailMessageID).FirstOrDefault();
                context.Entry(original).CurrentValues.SetValues(model);
                context.SaveChanges();
            }

        }

        public void UpdateFailedMessges()
        {
            List<SystemEmailMessage_OTP> errorList = context.SystemEmailMessage_OTP.Where(sem => sem.Status == (int)Enums.SystemEmailMessageStatus.Error).ToList();
            foreach (var item in errorList)
            {
                item.Status = (int)Enums.SystemEmailMessageStatus.UnProcessed;
            }
            context.SaveChanges();
        }
    }
}