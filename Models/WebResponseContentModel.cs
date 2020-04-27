using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Http.Results;

namespace DatabaseWebService.Models
{
    public class WebResponseContentModel<T>
    {
        private T responseContent;

        public bool IsRequestSuccesful
        {
            get;
            set;
        }

        public string ValidationError { get; set; }

        public string ValidationErrorAppSide { get; set; }

        public T Content
        {
            get { return responseContent; }

            set { responseContent = value; }
        }
        
    }
}