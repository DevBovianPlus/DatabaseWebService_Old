using DatabaseWebService.Common.Enums;
using DatabaseWebService.Domain.Abstract;
using DatabaseWebService.Models.EmailMessage;
using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Domain.Concrete
{
    public class SystemEmailMessageRepository : ISystemEmailMessageRepository
    {
        AnalizaProdajeEntities context = new AnalizaProdajeEntities();

        public List<SystemEmailMessage> GetUnprocessedEmails()
        {
            return context.SystemEmailMessage.Where(sem => sem.Status.Value == (int)Enums.SystemEmailMessageStatus.UnProcessed).ToList();
        }

        public void SaveEmail(SystemEmailMessage model)
        {
            if (model.SystemEmailMessageID != 0)
            {
                model.Status = (int)Enums.SystemEmailMessageStatus.Processed;
                var original = context.SystemEmailMessage.Where(sem => sem.SystemEmailMessageID == model.SystemEmailMessageID).FirstOrDefault();
                context.Entry(original).CurrentValues.SetValues(model);
                context.SaveChanges();
            }
            
        }

        public void UpdateFailedMessges()
        {
            List<SystemEmailMessage> errorList = context.SystemEmailMessage.Where(sem => sem.Status == (int)Enums.SystemEmailMessageStatus.Error).ToList();
            foreach (var item in errorList)
            {
                item.Status = (int)Enums.SystemEmailMessageStatus.UnProcessed;
            }
            context.SaveChanges();
        }
    }
}