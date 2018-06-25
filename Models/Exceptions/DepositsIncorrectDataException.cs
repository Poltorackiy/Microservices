using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Microservices.Deposits.Models.Exceptions
{
    public class DepositsIncorrectDataException : Exception
    {
        public DepositsIncorrectDataException() : base("Введены некорректные данные") { }
        public DepositsIncorrectDataException(string message) : base(message) { }
        public DepositsIncorrectDataException(string message, System.Exception inner) : base(message, inner) { }
    }

    public class DepositsIncorrectDataExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is DepositsIncorrectDataException)
            {
                context.Result = new RedirectResult("/api/deposits/500");
            }
            
            base.OnException(context);
        }
    }
}
