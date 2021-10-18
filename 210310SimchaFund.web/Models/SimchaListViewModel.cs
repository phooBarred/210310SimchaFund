using _210310SimchaFund.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _210310SimchaFund.web.Models
{
    public class SimchaListViewModel
    {
        public List<Simcha> Simchas { get; set; } = new List<Simcha>();
        public int ContributorCount { get; set; }

    }
}
