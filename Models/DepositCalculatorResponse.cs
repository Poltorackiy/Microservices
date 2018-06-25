using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservices.Deposits.Models
{
    /// <summary>
    /// Расчет выплат по вкладу
    /// </summary>
    public class DepositCalculatorResponse
    {
        Deposit _deposit;
        int _months;

        /// <summary>
        /// Создание нового экземпляра депозитного калькулятора
        /// </summary>
        /// <param name="deposit">Вид вклада</param>
        /// <param name="initialAmount">Первоначальная сумма вклада</param>
        /// <param name="months">Период, мес.</param>
        public DepositCalculatorResponse(Deposit deposit, decimal initialAmount, int months)
        {
            _deposit = deposit;
            _months = months;
            InitialAmount = initialAmount;

            // Расчет выплат сделан в конструкторе, а не в свойствах для того, 
            // чтобы не запускать вычисления несколько раз, для каждых свойств
            CalculatePayments();
        }

        /// <summary>
        /// Первоначальная сумма вклада
        /// </summary>
        public decimal InitialAmount { get; private set; }

        /// <summary>
        /// Выплаты процентов итого
        /// </summary>
        public decimal TotalPercents { get; private set; }

        /// <summary>
        /// Конечная сумма выплаты по закрытии вклада
        /// </summary>
        public decimal TotalAmount
        {
            get
            {
                if (_deposit.Capitalization)
                {
                    return DepositPayments.Max(dep => dep.TotalAmount);
                }
                else
                {
                    return InitialAmount + TotalPercents;
                }
            }
        }

        /// <summary>
        /// Список выплат по месяцам
        /// </summary>
        public ICollection<DepositPayment> DepositPayments { get; private set; }

        /// <summary>
        /// Расчет выплат по вкладу
        /// </summary>
        private void CalculatePayments()
        {
            // Переменная lastTotalAmount вынесена за цикл для экономии памяти 
            // и для того, чтобы не получать каждую итерацию значение из коллекции
            decimal lastTotalAmount = InitialAmount;
            DepositPayments = new List<DepositPayment>();

            for (int i = 0; i < _months; i++)
            {
                DepositPayment depositPayment = new DepositPayment();
                depositPayment.InitialAmount = lastTotalAmount;
                depositPayment.Fee = lastTotalAmount * _deposit.InterestRate / 100 / 12;

                if (_deposit.Capitalization)
                {
                    lastTotalAmount = depositPayment.TotalAmount;
                }

                DepositPayments.Add(depositPayment);
            }

            TotalPercents = DepositPayments.Sum(dep => dep.Fee);
        }
    }
}
