using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _210310SimchaFund.Data
{
    public class ContributionToSimcha
    {
        public int ContributorId { get; set; }
        public int SimchaId { get; set; }
        public string Amount { get; set; }
        public bool Include { get; set; }
    }
}
