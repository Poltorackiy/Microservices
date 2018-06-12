﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microservices.Deposits.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Xml.Linq;

namespace Microservices.Deposits.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(GetDeposits());
        }

        [HttpPost]
        public IActionResult Add(string name, string interestRate, string capitalization)
        {
            var deposits = GetDeposits();
            Deposit deposit = new Deposit();
            deposit.Name = name;
            // Проверка на стороне клиента. 
            // В идеале сделать проверку на стороне сервера и выдать сообщение об ошибке
            deposit.InterestRate = Convert.ToDecimal(interestRate.Replace(".", ",")); 
            deposit.Capitalization = (capitalization == "on") ? true : false;
            deposit.Guid = Guid.NewGuid();
            deposits.Add(deposit);
            SaveDeposits(deposits);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(string guid)
        {
            var deposits = GetDeposits();
            var deposit = deposits.Where(d => d.Guid == Guid.Parse(guid)).First();
            if (deposit != null)
            {
                deposits.Remove(deposit);
                SaveDeposits(deposits);
            }

            return RedirectToAction("Index");
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

        private List<Deposit> GetDeposits()
        {
            Stream stream;
            XDocument document = null;
            List<Deposit> deposits = new List<Deposit>();

            try
            {
                using (stream = System.IO.File.Open("Deposits.xml", FileMode.Open))
                {
                    document = new XDocument();
                    document = XDocument.Load(stream);
                }
            }
            catch (FileNotFoundException)
            {
                using (stream = System.IO.File.Create("Deposits.xml")) { }
                return new List<Deposit>();
            }
            catch (Exception)
            {
                return new List<Deposit>();
            }

            deposits = document.Root.Elements("deposit")
                .Select(dep => new Deposit
                {
                    Name = dep.Element("name").Value,
                    InterestRate = Convert.ToDecimal(dep.Element("interest_rate").Value.Replace(".", ",")),
                    Capitalization = Convert.ToBoolean(dep.Element("capitalization").Value),
                    Guid = Guid.Parse(dep.Element("guid").Value)
                }).ToList();
                 
                



            /*foreach (var element in document.XPathSelectElements("deposits/deposit"))
            {
                Deposit deposit = new Deposit();
                var xName = element.XPathSelectElement("/deposit/name");
                deposit.Name = element.XPathSelectElement("/deposit/name").Value;
                deposit.InterestRate = Convert.ToDecimal(element.XPathSelectElement("/deposit/interest_rate").Value.Replace(".", ","));
                if (element.XPathSelectElement("/deposit/capitalization").Value == "True")
                    deposit.Capitalization = true;
                else
                    deposit.Capitalization = false;

                deposits.Add(deposit);
            }*/

            return deposits;
        }

        private void SaveDeposits(List<Deposit> deposits)
        {
            XDocument document = new XDocument();
            XElement root = new XElement("deposits");
            document.AddFirst(root);
            foreach (var deposit in deposits)
            {
                XElement xDeposit = new XElement("deposit",
                    new XElement("guid", deposit.Guid.ToString()),
                    new XElement("name", deposit.Name),
                    new XElement("interest_rate", deposit.InterestRate),
                    new XElement("capitalization", deposit.Capitalization.ToString()));

                root.Add(xDeposit);
            }

            document.Save("Deposits.xml");
        }

    }
}
