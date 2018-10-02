using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microservices.Deposits.Models;
using Microservices.Deposits.Models.Exceptions;

namespace Microservices.Deposits.Controllers
{
    [Produces("application/json")]
    public class ApiController : Controller
    {
        string _filepath;
        public ApiController(IDepositsFilePath depositsFilePath)
        {
            _filepath = depositsFilePath.FilePath;
        }

        [HttpGet]
        [Route("api/deposits")]
        public IResponse GetDeposits()
        {
            var depositsResponse = new DepositsResponse
            {
                Deposits = HomeController.GetDeposits(_filepath)
            };

            var response = new Response();
            response.Body = depositsResponse;
            return response;
        }

        /* Здесь в качестве примера показал возможность отлавливать исключения через фильтры,
         * этот код закомментирован, также для примера создан код в папке Models\Exceptions,
         * но отправка сообщения вида Status-Message-Body без фильтрации по исключениям мне 
         * показалась проще, к тому же многие API так делают, и наверно не зря*/

        [HttpGet]
        [Route("api/deposits/payments/{guid}/{amount}/{months}")]
        //[DepositsIncorrectDataExceptionFilter]
        //[DepositsNotFountExceptionFilter]
        public IResponse GetCalculatorResponse(string guid, decimal amount, int months)
        {
            var deposits = HomeController.GetDeposits(_filepath);
            Guid depositGuid;

            if (amount < 0 || months < 0 || Guid.TryParse(guid, out depositGuid))
            {
                BadRequest();
                var errorResponse = new Response();
                errorResponse.StatusCode = "500";
                errorResponse.Message = "Введены некорректные данные";
            }

            if (!deposits.Exists(dep => dep.Guid == depositGuid))
            {
                NotFound();
                var errorResponse = new Response();
                errorResponse.StatusCode = "404";
                errorResponse.Message = "Вид вклада не найден";
            }

            var deposit = deposits.Find(dep => dep.Guid == depositGuid);
            var response = new Response();
            response.Body = new DepositCalculatorResponse(deposit, amount, months);
            return response; 
        }

        /* Продолжение обработки исключений через фильтры
        // Отлавливаем исключения
        [Route("api/deposits/500")]
        public ErrorResponse ErrorIncorrectData()
        {
            BadRequest();
            return new ErrorResponse { ErrorCode = "400", ErrorMessage = "Введены некорректные данные" };
        }

        [Route("api/deposits/404")]
        public ErrorResponse ErrorDepositNotFound()
        {
            NotFound();
            return new ErrorResponse { ErrorCode = "404", ErrorMessage = "Вид депозита не найден" };
        }
        */
    }
}