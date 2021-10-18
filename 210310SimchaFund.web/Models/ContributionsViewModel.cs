using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _210310SimchaFund.Data;

namespace _210310SimchaFund.web.Models
{
    public class ContributionsViewModel
    {
        public List<SimchaContributor> SimchaContributors { get; set; }
        public Simcha Simcha { get; set; }

    }
}
