using AspNetCoreHero.ToastNotification.Abstractions;
using Dhicoin.Areas.Identity.Data;
using Dhicoin.Models;
using Dhicoin.Utility;
using Dhicoin.Utility.Repositories;
using Dhicoin.ViewModel;
using MailKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Shyjus.BrowserDetection;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;

namespace Dhicoin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly INotyfService _notyfService;
        private readonly IMailSender _emailSender;
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;
        private readonly BrowserDetector _browserDetector;

      
        public AdminController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            INotyfService notyfService,
            IMailSender emailSender, IWebHostEnvironment env,
            ApplicationDbContext context, BrowserDetector browserDetector)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notyfService = notyfService;
            _emailSender = emailSender;
            _env = env;
            _context = context;
            _browserDetector = browserDetector;
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
        #region API CALLS
        [HttpGet]
        public IActionResult GetUserMaster()
        {
            var usersList = _context.ApplicationUsers.ToList();

            var user = (from users in _context.Users
                        join usrRole in _context.UserRoles
                      on users.Id equals usrRole.UserId
                        join role in _context.Roles
                      on usrRole.RoleId equals role.Id
                        select new UserMasterViewModel
                        {
                            Email = users.Email,
                            Status = users.Status ? "Active" : "Inactive",
                            Id = users.Id,
                            UserName = users.FullName,
                            UserType = role.Name,

                        }).ToList();
            return Json(new { data = user });
        }
        #endregion


        public async Task<IActionResult> UpsertUser(UpsertUserViewModel upsertView)
        {
            string userId = upsertView.Id;

            if (userId == null)
            {
                var user = new ApplicationUser()
                {


                    Email = upsertView.Email,
                    FullName = upsertView.UserName,
                    Status = upsertView.Status,
                    UserName = upsertView.Email,
                };

                IdentityResult result = await _userManager.CreateAsync(user, upsertView.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, upsertView.UserType);
                    //// send confirmation email to the user
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Page(
                    //        "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //        values: new { area = "Identity", code, email = insertView.Email },
                    //        protocol: Request.Scheme);

                    //var pathToFile = _env.WebRootPath
                    //        + Path.DirectorySeparatorChar.ToString()
                    //        + "Templates"
                    //        + Path.DirectorySeparatorChar.ToString()
                    //        + "VerificationTemplate.html";
                    //var builder = new BodyBuilder();
                    //using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                    //{
                    //    builder.HtmlBody = SourceReader.ReadToEnd();
                    //}
                    //var url = HtmlEncoder.Default.Encode(callbackUrl);
                    //string messageBody = string.Format(builder.HtmlBody, url);
                    //var message = new Message(insertView.Email, "Confirm your email", messageBody);
                    //_emailSender.SendEmail(message);

                }
                _notyfService.Success("User Created Successfully!");
            }
            else
            {
                var user = await _userManager.FindByIdAsync(upsertView.Id);
                if (user != null)
                {
                    user.Email = upsertView.Email;
                    user.UserName = upsertView.Email;
                    user.FullName = upsertView.UserName;
                    user.Status = upsertView.Status;

                    // update user role
                    var currentRole = await _userManager.GetRolesAsync(user);
                    if (currentRole != null && currentRole.Count > 0)
                    {
                        await _userManager.RemoveFromRoleAsync(user, currentRole[0]);
                    }
                    await _userManager.AddToRoleAsync(user, upsertView.UserType);

                    // save changes
                    var result = await _userManager.UpdateAsync(user);
                    _notyfService.Success("User Updated Successfully");
                }
            }
            return View("UserMaster");
        }
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {

            if (!string.IsNullOrEmpty(id))
            {

                var obj = await (from usr in _context.Users join
                               userRole in _context.UserRoles on usr.Id
                               equals userRole.UserId join role in _context.Roles on userRole.RoleId equals role.Id where usr.Id == id select new UpsertUserViewModel
                               {
                                   UserName = usr.FullName,
                                   Email = usr.Email,
                                   Status = usr.Status,
                                   UserType = role.Name,
                                   Id = usr.Id,
                               }).FirstOrDefaultAsync();


                return Json(new { success = true, obj });
            }
            return Json("");
        }
        public async Task<IActionResult> ResetUserPassword(string? id)
        {
            var user = await _userManager.FindByIdAsync(id);

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                   "/Account/ResetPassword",
               pageHandler: null,
                   values: new { area = "Identity", code, email = user.Email },
                   protocol: Request.Scheme);
            var pathToFile = _env.WebRootPath
                    + Path.DirectorySeparatorChar.ToString()
                    + "Templates"
                    + Path.DirectorySeparatorChar.ToString()
                    + "ResetPasswordTemplate.html";
            var builder = new BodyBuilder();
            using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
            {
                builder.HtmlBody = SourceReader.ReadToEnd();
            }
            var url = HtmlEncoder.Default.Encode(callbackUrl);
            string messageBody = string.Format(builder.HtmlBody, url);
            var message = new Message(user.Email, "Reset Password", messageBody);
            _emailSender.SendEmail(message);
            return Json(new { success = true });
        }
        [HttpGet]
        public IActionResult DeleteUser(string id)
        {
            // Get the user from the database
            var user = _context.Users.SingleOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            // Remove the user from the database and save changes
            _context.Users.Remove(user);
            _context.SaveChanges();
            _notyfService.Success("User Deleted Successfully");
            return Json(new { success = true });
        }




        public IActionResult CurrenciesMaster()
        {
            ViewBag.Currency = new SelectList(_context.CryptoReceiverAddressDetails, "Id", "ChainName");
            return View();
        }
        [HttpPost]
        public IActionResult CurrenciesMaster(CurrenciesMaster model)
        {
         
            if (model.Id == 0)
            {

               string lower =  model.Symbol.ToLower();
                model.Symbol = lower;
                // Add new currency
                _context.CurrencyMaster.Add(model);
                _context.SaveChanges();

                for (int i=0; i<model.SellChainId.Count(); i++)
                {
                    MultipleChainSelection multipleChain = new MultipleChainSelection();
                    multipleChain.SellChainId = model.SellChainId[i];
                    multipleChain.CurrencyMasterId = model.Id;
                    _context.multipleChainSelections.Add(multipleChain);
                    _context.SaveChanges();  
                }
                for(int i=0; i<model.BuyChainId.Count();i++)
                {
                    MultipleChainSelectionWithBuyChain multipleChainSelectionWithBuy = new MultipleChainSelectionWithBuyChain();
                    multipleChainSelectionWithBuy.BuyChainId = model.BuyChainId[i];
                    multipleChainSelectionWithBuy.CurrencyMasterId = model.Id;
                    _context.multipleChainSelectionWithBuyChains.Add(multipleChainSelectionWithBuy);
                    _context.SaveChanges();
                }
            }
            else
            {

                // Update currency
                string loweronedit = model.Symbol.ToLower();
                model.Symbol = loweronedit;

                _context.CurrencyMaster.Update(model);

                // Remove existing multiple chain selections form sell  the currency
                var existingMultipleChains = _context.multipleChainSelections.Where(m => m.CurrencyMasterId == model.Id).ToList();
               
                foreach (var item in existingMultipleChains)
                {
                    _context.multipleChainSelections.Remove(item);
                }
                _context.SaveChanges();

                // Remove existing multiple chain selections form BUy  the currency

                var existingMultipleChainsforBuyChian = _context.multipleChainSelectionWithBuyChains.Where(m => m.CurrencyMasterId == model.Id).ToList();

                foreach (var item in existingMultipleChainsforBuyChian)
                {
                    _context.multipleChainSelectionWithBuyChains.Remove(item);
                }
                _context.SaveChanges();


                for (int i = 0; i < model.SellChainId.Count(); i++)
                {
                    MultipleChainSelection multipleChain = new MultipleChainSelection();
                    multipleChain.SellChainId = model.SellChainId[i];                   
                    multipleChain.CurrencyMasterId = model.Id;
                    _context.multipleChainSelections.Add(multipleChain);
                    _context.SaveChanges();
                }

                for (int i = 0; i < model.BuyChainId.Count(); i++)
                {
                    MultipleChainSelectionWithBuyChain multipleChainSelectionWithBuy = new MultipleChainSelectionWithBuyChain();
                    multipleChainSelectionWithBuy.BuyChainId = model.BuyChainId[i];
                    multipleChainSelectionWithBuy.CurrencyMasterId = model.Id;
                    _context.multipleChainSelectionWithBuyChains.Add(multipleChainSelectionWithBuy);
                    _context.SaveChanges();
                }

            }
                
                 return RedirectToAction("CurrenciesMaster");
         }
        #region API CALLS
        public IActionResult GetCurrenciesMaster()
        {

            var currencymaster = _context.CurrencyMaster.ToList();

            var currency = (from curruncey in currencymaster select new CurrencyMasterViewModel
            {
                Id = curruncey.Id,
                CurrencyName = curruncey.CurrencyName,
                Symbol = curruncey.Symbol,
                SellStatus = curruncey.SellStatus ? "Active" : "Inactive",
                BuyStatus = curruncey.BuyStatus ? "Active" : "Inactive",
                
            }).ToList();
            return Json(new { data = currency });
        }
        #endregion
        public async Task<IActionResult> EditCurrenciesMaster(int id)
        {

            var obj = await _context.CurrencyMaster.FindAsync(id);

            // Retrieve the multiple chain selections for the currency
            var multipleChains = _context.multipleChainSelections.Where(m => m.CurrencyMasterId == obj.Id).ToList();

            var multibuyChains = _context.multipleChainSelectionWithBuyChains.Where(x => x.CurrencyMasterId == obj.Id).ToList();
            // Create a list of selected sell chain IDs and buy chain IDs
            var selectedSellChains = multipleChains.Select(m => m.SellChainId).ToList();

            var selectedBuyChains = multibuyChains.Select(m => m.BuyChainId).ToList();

            // Pass the selected sell and buy chain IDs to the view
            
            ViewBag.SelectedSellChains = selectedSellChains;
            ViewBag.SelectedBuyChains = selectedBuyChains;
            obj.SellChainId = selectedSellChains.ToArray();
            obj.BuyChainId = selectedBuyChains.ToArray();
           

            return Json(new { data = obj });

        }
        public IActionResult DeleteCurrenciesMaster(int id)
        {
            // Get the crypto address from the database
            var currency = _context.CurrencyMaster.Find(id);
            if (currency == null)
            {
                return NotFound();
            }
            // Remove the crypto address from the database and save changes
            _context.CurrencyMaster.Remove(currency);
            _context.SaveChanges();
            _notyfService.Information("Currency Deleted Successfully");
            return Json(new { success = true });
        }
        public IActionResult CustomerMaster()
        {
            return View();
        }
        #region API CALLS
        [HttpGet]
        public async Task<IActionResult> GetCustomerMaster()
        {
            var usersList = await (from u in _context.CustomerMasterProfile
                             select new
                             {
                                 u.Id, u.FirstName, u.LastName, u.Email, u.CreationStatus, u.KYCStatus
                             }).ToListAsync();
            return Json(new { data = usersList });
        }
        #endregion
        public IActionResult CryptoReceiveAddress()
        {
            return View();
        }
       

        #region API CALLS

        [HttpGet]
        public IActionResult GetCryptoReceiveAddress()
        {

            var crypto = _context.CryptoReceiverAddress.Include(x=>x.CryptoReceiverAddressDetails).ToList();
        
            return Json(new { data = crypto });

        }
        #endregion
        [HttpPost]
        public IActionResult AddCryptoAddress(CryptoReceiveAddress model)
        {
            if(model!=null)
            {
                _context.CryptoReceiverAddress.Add(model);
                _context.SaveChanges();

                return RedirectToAction("CryptoReceiveAddress");
            }
            return View(model);
        }
        public async Task<IActionResult> EditCryptoAddress(int id)
        {

            var cryptoaddres =await _context.CryptoReceiverAddress.FindAsync(id);
            ViewBag.Chains = new SelectList(_context.CryptoReceiverAddressDetails, "Id", "ChainName");
            return View(cryptoaddres);

        }
        [HttpPost]
        public IActionResult EditCryptoAddress(CryptoReceiveAddress model)
        {

            _context.CryptoReceiverAddress.Update(model);
            _context.SaveChanges();
            return RedirectToAction("CryptoReceiveAddress");

        }
        public IActionResult DeleteCryptoAddress(int id)
        {
            // Get the crypto address from the database
            var cryptoaddress = _context.CryptoReceiverAddress.Find(id);
            if (cryptoaddress == null)
            {
                return NotFound();
            }
            // Remove the crypto address from the database and save changes
            _context.CryptoReceiverAddress.Remove(cryptoaddress);
            _context.SaveChanges();
            _notyfService.Information("Crypto Address Deleted Successfully");
            return Json(new { success = true });
        }
       
        public IActionResult CryptoReceiveAddressDetail()
        {

            ViewBag.Chains = new SelectList(_context.CryptoReceiverAddressDetails, "Id", "ChainName");
            return View();
        }
        [HttpPost]
        public IActionResult CryptoReceiveAddressDetail(CryptoReceiverAddressDetails model)
        {
            // Check if chain name already exists in the database
            if (_context.CryptoReceiverAddressDetails.Any(x => x.ChainName == model.ChainName))
            {
                ModelState.AddModelError("ChainName", "Chain name already exists");
            }
            else
            {
                var cryptoReceiveAddress = new CryptoReceiverAddressDetails
                {
                    ChainName = model.ChainName,
                    ChainCode = model.ChainCode,
                    ChainStatus = model.ChainStatus,
                };
                _context.CryptoReceiverAddressDetails.Add(cryptoReceiveAddress);
                _context.SaveChanges();
                _notyfService.Success("Chain added Successfully");
            }

            // Get the chains again to update the dropdown
            var chains = _context.CryptoReceiverAddressDetails
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ChainName
                })
                .ToList();
            ViewBag.Chains = chains;
            return View();
        }

        public IActionResult GetChains()
        {
            var chains = _context.CryptoReceiverAddressDetails
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ChainName
                })
                .ToList();

            return PartialView("_ChainsDropdown", chains);
        }

        public IActionResult DeleteChain(int id)
        {
            var chain = _context.CryptoReceiverAddressDetails.Find(id);
            if (chain != null)
            {
                _context.CryptoReceiverAddressDetails.Remove(chain);
                _context.SaveChanges();
                _notyfService.Information("Chain Deleted Successfully");
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }


        public IActionResult UserLoginDetail()
        {
            return View();
        }
        #region API CALLS
        [HttpGet]

        public IActionResult GetUserLoginDetail()
        {
            var loginuser = _context.UserLoginDetail.ToList();
            return Json(new { data = loginuser });
        }
       
        #endregion
        [HttpPost]
        public async Task<IActionResult> ForceLogout(string userId, bool isChecked)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (isChecked)
            {
                // Log out the user
                await _userManager.UpdateSecurityStampAsync(user);
                user.IsOnline = false;
                await _userManager.UpdateAsync(user);

                var login = _context.UserLoginDetail.FirstOrDefault(x=>x.UserId == user.Id);

                if(login != null)
                {
                    login.IsOnline = "Logged out";
                    _context.UserLoginDetail.Update(login);
                    _context.SaveChanges();
                } 
            }
            _notyfService.Success("User Force Logout Successfully");
            return Json(new { success = true });
        }
       
            public IActionResult BuyDetail()
            {

              var getProductCurrency = _context.BuyCurrencies.ToList();

                return View(getProductCurrency);
            }
            public IActionResult BuyTransactionDetail(int?id)
            {

            var getCurrenyDetail = _context.BuyCurrencies.Find(id);
            if (getCurrenyDetail != null)
            {             

                string userId =  getCurrenyDetail.userId;

                var getUserEmail = (from ur in _context.Users 
                                    where ur.Id==userId
                                    select ur.Email).FirstOrDefault();

                var getBuyCommession= (from byCurrency in _context.BuyCurrencies 
                                       join CurrenyMaster in _context.CurrencyMaster
                                       on byCurrency.SelectCurrency equals CurrenyMaster.CoinId
                                       where CurrenyMaster.CoinId==getCurrenyDetail.SelectCurrency 
                                       select CurrenyMaster.BuyConversionRate).FirstOrDefault();

                var getCommessionChain = (from currenyDetail in _context.CryptoReceiverAddressDetails
                                          join currency in _context.CryptoReceiverAddress
                                          on currenyDetail.Id equals currency.ChainId
                                          where currenyDetail.ChainName == getCurrenyDetail.CurrenyChain
                                          select currency.CommesionOnChain).FirstOrDefault();

                decimal Result1 = Convert.ToDecimal(getBuyCommession);
                decimal Result2 = Convert.ToDecimal(getCommessionChain);

                decimal TotalCommession = Result1 + Result2;

                ViewBag.TotalCommession = TotalCommession;
                ViewBag.UserEmail = getUserEmail;
                return View(getCurrenyDetail);

            }

              
                return View();
            }
            public IActionResult SellDetail()
            {
            var getSellCurrency = _context.SellCurrencies.ToList();

            return View(getSellCurrency);
        }
            public IActionResult ReferAFriend()
            {
                return View();
            }
            public IActionResult CustomerApproval()
            {
                return View();
            }
        #region API CALLS
        [HttpGet]

        public IActionResult GetCustomerApproval()
        {
            var pendingcustomers = (from u in _context.CustomerMasterProfile
                                         select new
                                         {
                                             u.Id,
                                             u.FirstName,
                                             u.Email,
                                             u.CreationStatus,
                                         }).Where(c=>c.CreationStatus=="Pending").ToList();
            return Json(new { data = pendingcustomers });
        }

        #endregion
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
            public IActionResult CustomerDetail(int id)
            {
            var profile =  _context.CustomerMasterProfile.Find(id);
            if (profile == null)
            {
                return NotFound();
            }
            return View(profile);
        
            }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CustomerDetail(int id, CustomerMasterProfile customer)
        {

            if (ModelState.IsValid)
            {
                var profile = _context.CustomerMasterProfile.Find(id);
                if (profile == null)
                {
                    return NotFound();
                }
                profile.FirstName = customer.FirstName;
                profile.LastName = customer.LastName;
                profile.Email = customer.Email;
                profile.Gender = customer.Gender;
                profile.DateOfBirth = customer.DateOfBirth;
                profile.CountryCode = customer.CountryCode;
                profile.BankName = customer.BankName;
                profile.Account = customer.Account;
                profile.NameRegisteredInBMl = customer.NameRegisteredInBMl;
                profile.CreationStatus = "Pending";
                profile.KYCStatus = "Pending";
                profile.RejectionRemarks = customer.RejectionRemarks;

                var result = _context.CustomerMasterProfile.Update(profile);
                _context.SaveChanges();
                return View(customer);

            }
            return View(customer);
        }
        public IActionResult ApproveCustomerMasterProfile(int id)
        {
            var userProfile = _context.CustomerMasterProfile.Find(id);
            if (userProfile != null)
            {
                userProfile.KYCStatus = "Verified";
                userProfile.CreationStatus = "Verified";
                userProfile.RejectionRemarks = null;
                _context.CustomerMasterProfile.Update(userProfile);
                _context.SaveChanges();
                _notyfService.Information("Profile Approved Successfully");
                return RedirectToAction("CustomerMaster");
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult PendingCustomerMasterProfile(int id,string remarks)
        {
            var userProfile = _context.CustomerMasterProfile.Find(id);
            if (userProfile != null)
            {
                userProfile.KYCStatus = "Pending";
                userProfile.CreationStatus = "Pending";
                userProfile.RejectionRemarks = remarks;
                _context.CustomerMasterProfile.Update(userProfile);
                _context.SaveChanges();
                _notyfService.Information("Profile Pended Successfully");
                return RedirectToAction("CustomerMaster");
            }
            else
            {
                return NotFound();
            }
        }
        public IActionResult DeleteCustomerMasterProfile(int id)
        {
            var userProfile = _context.CustomerMasterProfile.Find(id);
            if (userProfile != null)
            {
                userProfile.KYCStatus = "Pending";
                userProfile.CreationStatus = "Pending";
                _context.CustomerMasterProfile.Remove(userProfile);
                _context.SaveChanges();
                _notyfService.Information("Profile Deleted Successfully");
                return RedirectToAction("CustomerMaster");
            }
            else
            {
                return NotFound();
            }
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

       

        }
    }


