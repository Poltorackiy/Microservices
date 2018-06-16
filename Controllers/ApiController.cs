using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microservices.Deposits.Models;
using System.Xml.Serialization;

namespace Microservices.Deposits.Controllers
{
    [Produces("application/json")]
    [Route("api/deposits")]
    public class ApiController : Controller
    {
        [HttpGet]
        public IEnumerable<Deposit> GetDeposits()
        {
            return HomeController.GetDeposits();
        }
    }
}