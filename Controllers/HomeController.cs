using Dhicoin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Dhicoin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult UserMaster()
        {
            return View();
        }
        public IActionResult CurrenciesMaster()
        {
            return View();
        }
        public IActionResult CommissionMaster()
        {
            return View();
        }
        public IActionResult ConversionMaster()
        {
            return View();
        }

        public IActionResult CustomerMaster()
        {
            return View();
        }
        public IActionResult CryptoReceiveAddress()
        {
            return View();
        }
        public IActionResult CryptoReceiveAddressDetail()
        {
            return View();
        }
        public IActionResult BuySellLimit()
        {
            return View();
        }
        public IActionResult CountryMaster()
        {
            return View();
        }
        public IActionResult RaiseTicket()
        {
            return View();
        }
        public IActionResult UserLoginDetail()
        {
            return View();
        }
        public IActionResult  BuyDetail()
        {
            return View();
        }
        public IActionResult BuyTransactionDetail()
        {
            return View();
        }
        public IActionResult SellDetail()
        {
            return View();
        }
        public IActionResult ReferAFriend()
        {
            return View();
        }
        public IActionResult CustomerApproval()
        {
            return View();
        }
        public IActionResult PendingBuyApproval()
        {
            return View();
        }
        public IActionResult PendingBuyApprovalDetails()
        {
            return View();
        }
        public IActionResult PendingSellApproval()
        {
            return View();
        }
        public IActionResult PendingSellApprovalDetails()
        {
            return View();
        }
        public IActionResult PendingKYCApproval()
        {
            return View();
        }
        public IActionResult CustomerDetail()
        {
            return View();
        }
        public IActionResult ProfilePersonal_Info()
        {
            return View();
        }
		public IActionResult ProfileChangePassword()
		{
			return View();
		}
        public IActionResult Login()
        {
            return View();
        }
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}