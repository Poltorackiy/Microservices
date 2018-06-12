﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservices.Deposits.Models
{
    public class Deposit
    {
        private decimal _interestRate;

        /// <summary>
        /// Гуид депозита
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Название вклада
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Процентная ставка
        /// </summary>
        public decimal InterestRate
        {
            get
            {
                return _interestRate;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Процентая ставка не может быть отрицательной");

                _interestRate = value;
            }
        }

        /// <summary>
        /// Капитализация процентов
        /// </summary>
        public bool Capitalization { get; set; }

        /// <summary>
        /// Количество средств после закрытия вклада
        /// </summary>
        /// <param name="StartCapital">Первоначальная сумма вложения</param>
        /// <param name="Months">Количество месяцев</param>
        /// <returns></returns>
        public decimal TotalAmount(decimal StartCapital, int Months)
        {
            decimal result = StartCapital;
            for (int i = 0; i < Months; i++)
            {
                if (Capitalization == true)
                    result = result + result * InterestRate / 12;
                else
                    result = result + StartCapital * InterestRate / 12;
            }
            return result;
        }

    }
}
