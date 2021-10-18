using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _210310SimchaFund.Data
{
    public class Transaction
    {
        public string Action { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
