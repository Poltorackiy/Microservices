using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservices.Deposits.Models
{
    public class DepositsResponse
    {
        public IEnumerable<Deposit> Deposits { get; set; }
        
    }
}
