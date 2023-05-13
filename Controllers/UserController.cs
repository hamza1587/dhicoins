using AspNetCoreHero.ToastNotification.Abstractions;
using Dhicoin.Areas.Identity.Data;
using Dhicoin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Dhicoin.Areas.Identity.Data.ContextSeed.Enums;

namespace Dhicoin.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
	{
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotyfService _notyfService;
        public UserController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
              INotyfService notyfService)
        {
            _context = context;
            _userManager = userManager;
            _notyfService = notyfService;
        }

        public IActionResult Index()
		{
			return View();
		}
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User); var userProfile = await _context.CustomerMasterProfile
        .FirstOrDefaultAsync(up => up.UserId == user.Id);

            if (userProfile != null)
            {
                var profileView = new CustomerMasterProfile
                {
                    FirstName = userProfile.FirstName,
                    LastName = userProfile.LastName,
                    Email = userProfile.Email,
                    Gender = userProfile.Gender,
                    DateOfBirth = userProfile.DateOfBirth,
                    CountryCode = userProfile.CountryCode,
                    BankName = userProfile.BankName,
                    Account = userProfile.Account,
                    NameRegisteredInBMl = userProfile.NameRegisteredInBMl,
                    KYCStatus=userProfile.KYCStatus
                   
                };
                

                return View(profileView);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(CustomerMasterProfile profile)
        {
            var user = await _userManager.GetUserAsync(User);
          
            if (user == null)
            {
                return NotFound();
            }
            // Check if the user already has a UserProfile
            var userProfile = await _context.CustomerMasterProfile
                .FirstOrDefaultAsync(up => up.UserId == user.Id);
            

            if (userProfile == null)
            {
                // If the user does not have a UserProfile yet, create a new one and set its properties
                profile.UserId = user.Id;
                profile.KYCStatus = "Pending";
                profile.CreationStatus = "Pending";
                _context.CustomerMasterProfile.Add(profile);
            }
            else
            {
                // If the user already has a UserProfile, update its properties with the new values
                userProfile.FirstName = profile.FirstName;
                userProfile.LastName = profile.LastName;
                userProfile.Email = profile.Email;
                userProfile.Gender = profile.Gender;
                userProfile.DateOfBirth = profile.DateOfBirth;
                userProfile.CountryCode = profile.CountryCode;
                userProfile.BankName = profile.BankName;
                userProfile.Account = profile.Account;
                userProfile.NameRegisteredInBMl = profile.NameRegisteredInBMl;
                userProfile.KYCStatus = "Pending";
                userProfile.CreationStatus = "Pending";
                
            }
            _context.SaveChanges();
            return View(profile);
        }
        
        public IActionResult Buy()
        {
            return View();
        }

        public IActionResult BuyHistory()
        {
            return View();
        }
        public IActionResult BuySellTransactionDetail()
        {
            return View();
        }
       
        public IActionResult Sell()
        {
            return View();
        }
        public IActionResult SellHistory()
        {
            return View();
        }

    }
}
