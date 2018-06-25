using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Microservices.Deposits.Models.Exceptions
{
    public class DepositsNotFountException : Exception
    {
        public DepositsNotFountException() : base("Вид вклада не найден") { }
        public DepositsNotFountException(string message) : base(message) { }
        public DepositsNotFountException(string message, System.Exception inner) : base(message, inner) { }

    }

    public class DepositsNotFountExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is DepositsNotFountException)
            {
                context.Result = new RedirectResult("/api/deposits/404");
            }
            base.OnException(context);
        }
    }
}
