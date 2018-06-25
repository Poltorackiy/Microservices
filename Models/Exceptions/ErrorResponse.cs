using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservices.Deposits.Models.Exceptions
{
    public class ErrorResponse
    {
        /// <summary>
        /// Код ошибки
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Текст ошибки
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
