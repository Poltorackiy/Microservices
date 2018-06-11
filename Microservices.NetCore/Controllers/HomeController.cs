using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microservices.NetCore.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Microservices.NetCore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            string connectionString = CheckMySQLConnection();
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string CheckMySQLConnection()
        {
            MySqlConnectionStringBuilder sb = new MySqlConnectionStringBuilder();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            IConfiguration config = builder.Build();
            sb.Server = config.GetValue<string>("MySQL:Host");
            sb.Port = config.GetValue<uint>("MySQL:Port");
            sb.UserID = config.GetValue<string>("MySQL:User");
            sb.Password = config.GetValue<string>("MySQL:Password");

            return sb.ToString();
        }
    }
}
