using System;
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
        string _filepath;
        public HomeController(IDepositsFilePath depositsFilePath)
        {
            _filepath = depositsFilePath.FilePath;
        }
        public IActionResult Index()
        {
            return View(GetDeposits(_filepath));
        }

        /// <summary>
        /// Добавляет новый депозит в список
        /// </summary>
        /// <param name="name">Название вклада</param>
        /// <param name="interestRate">Процентная ставка</param>
        /// <param name="capitalization">Капитализация процентов</param>
        /// <returns>Редирект на главную страницу</returns>
        [HttpPost]
        public IActionResult Add(string name, string interestRate, string capitalization)
        {
            var deposits = GetDeposits(_filepath);
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

        /// <summary>
        /// Удаление депозита из списка
        /// </summary>
        /// <param name="guid">Гуид вклада</param>
        /// <returns>Редирект на главную страницу</returns>
        [HttpPost]
        public IActionResult Delete(string guid)
        {
            var deposits = GetDeposits(_filepath);
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

        /// <summary>
        /// Получаем список депозитов из XML файла
        /// </summary>
        /// <returns></returns>
        public static List<Deposit> GetDeposits(string filepath)
        {
            Stream stream;
            XDocument document = null;
            List<Deposit> deposits = new List<Deposit>();

            try
            {
                using (stream = System.IO.File.Open(filepath, FileMode.Open))
                {
                    document = new XDocument();
                    document = XDocument.Load(stream);
                }
            }
            catch (FileNotFoundException)
            {
                // Если файла нет, создаем его
                using (stream = System.IO.File.Create(filepath)) { }
                return deposits;
            }
            catch (Exception)
            {
                // Если не удалось прочитать, т.к. файл пуст, или другая ошибка, выдаем пустой список
                return deposits;
            }

            // Получаем список депозитов из файла через LinQ2XML
            deposits = document.Root.Elements("deposit")
                .Select(dep => new Deposit
                {
                    Name = dep.Element("name").Value,
                    InterestRate = Convert.ToDecimal(dep.Element("interest_rate").Value.Replace(".", ",")),
                    Capitalization = Convert.ToBoolean(dep.Element("capitalization").Value),
                    Guid = Guid.Parse(dep.Element("guid").Value)
                }).ToList();
                 

            return deposits;
        }

        /// <summary>
        /// Сохранение списка депозитов в XML файл
        /// </summary>
        /// <param name="deposits"></param>
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

            document.Save(_filepath);
        }

    }
}
