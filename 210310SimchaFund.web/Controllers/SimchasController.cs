using _210310SimchaFund.web.Models;
using _210310SimchaFund.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace _210310SimchaFund.web.Controllers
{
    public class SimchasController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimchaFund;Integrated Security=true;";

        public IActionResult Index()
        {
            SimchaListViewModel vm = new SimchaListViewModel();
            SimchasManager manager = new(_connectionString);
            vm.Simchas = manager.GetSimchas();
            vm.ContributorCount = manager.GetContributorsCount();
            return View(vm);
        }

        public IActionResult Contributions(int simchaId)
        {
            ContributionsViewModel vm = new();
            SimchasManager manager = new(_connectionString);
            vm.Simcha = manager.GetSimcha(simchaId);
            vm.SimchaContributors = manager.GetSimchaContributors(simchaId);

            return View(vm);
        }

        [HttpPost]
        public IActionResult UpdateContributions(List<ContributionToSimcha> contributionToSimchas)
        {
            SimchasManager manager = new SimchasManager(_connectionString);
            manager.UpdateContributions(contributionToSimchas);

            return RedirectToAction("index");
        }

        public IActionResult New(Simcha simcha)
        {
            SimchasManager manager = new(_connectionString);
            manager.AddSimcha(simcha);

            return RedirectToAction("index");
        }

    }
}
