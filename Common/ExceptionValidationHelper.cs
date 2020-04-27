using DatabaseWebService.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace DatabaseWebService.Common
{
    public static class ExceptionValidationHelper
    {
        private static string exceptionMessage = "";
        public static string GetExceptionSource(Exception ex)
        {
            exceptionMessage = "";
            InnerExceptionExist(ex.InnerException);
            return ex.Message + "\r\n" + (ex.InnerException != null ? exceptionMessage + "\r\n" : "");
            
        }

        public static void InnerExceptionExist(Exception ex)
        {
            if (ex != null)
            {
                exceptionMessage += ex.Message;

                InnerExceptionExist(ex.InnerException);
            }
        }

        public static string DBEntityErrors(DbEntityValidationException e)
        {
            string error = "";
            foreach (var eve in e.EntityValidationErrors)
            {
                Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                foreach (var ve in eve.ValidationErrors)
                {
                    Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                        ve.PropertyName, ve.ErrorMessage);
                }
            }

            return error;
        }
    }
}