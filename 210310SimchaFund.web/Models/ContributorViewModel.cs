using _210310SimchaFund.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _210310SimchaFund.web.Models
{
    public class ContributorViewModel
    {
        public List<Contributor> Contributors { get; set; }
        public decimal TotalContributed { get; set; }
    }
}
