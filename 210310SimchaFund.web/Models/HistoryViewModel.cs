using _210310SimchaFund.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _210310SimchaFund.web.Models
{

    public class HistoryViewModel
    {
        public List<Transaction> Transactions { get; set; }
        public string FullName { get; set; }
    }
}
