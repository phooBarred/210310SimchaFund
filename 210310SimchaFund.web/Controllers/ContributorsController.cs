using _210310SimchaFund.web.Models;
using _210310SimchaFund.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _210310SimchaFund.web.Controllers
{
    public class ContributorsController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimchaFund;Integrated Security=true;";
        public IActionResult Index()
        {
            ContributorViewModel cvm = new();
            ContributorsManager cm = new(_connectionString);
            cvm.Contributors = cm.GetContributors();
            cvm.TotalContributed = cm.GetTotalContributed();

            return View(cvm);
        }

        [HttpPost]
        public IActionResult Deposit(Deposit deposit)
        {
            ContributorsManager cm = new(_connectionString);
            cm.NewDeposit(deposit);

            return RedirectToAction("index");
        }

        [HttpPost]
        public IActionResult New(Contributor contributor, decimal initialDeposit)
        {
            ContributorsManager cm = new(_connectionString);
            cm.NewContributor(contributor);
            var deposit = new Deposit
            {
                ContributorId = contributor.Id,
                Amount = initialDeposit
            };
            cm.NewDeposit(deposit);
            return RedirectToAction("index");
        }

        [HttpPost]
        public IActionResult Edit(Contributor contributor)
        {
            ContributorsManager cm = new(_connectionString);
            cm.EditContributor(contributor);
            return RedirectToAction("index");
        }

        public IActionResult History(int contributorId)
        {
            ContributorsManager mgr = new ContributorsManager(_connectionString);
            HistoryViewModel vm = new();
            vm.FullName = mgr.GetContributorName(contributorId);
            vm.Transactions = mgr.GetTransactions(contributorId);
            return View(vm);
        }
    }
}
