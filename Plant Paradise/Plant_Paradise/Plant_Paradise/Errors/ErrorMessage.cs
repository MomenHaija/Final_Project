using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plant_Paradise.Errors
{
    public class ErrorMessage
    {
        private string message;
        private string pageName;
        public ErrorMessage(string message, string pageName)
        {
            this.message = message;
            this.pageName = pageName;
        }
        public string notFound() { return message;}

    }
}