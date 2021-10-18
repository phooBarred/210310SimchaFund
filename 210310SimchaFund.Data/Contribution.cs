using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _210310SimchaFund.Data
{
    public class Contribution
    {
        public int ContributorId { get; set; }
        public int SimchaId { get; set; }
        public string SimchaName { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
