using AspNetCoreHero.ToastNotification.Abstractions;
using Dhicoin.Areas.Identity.Data;
using Dhicoin.Models;
using Dhicoin.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Net.Http.Headers;
using System.Net;
using System.Security.Claims;
using Microsoft.Web.Administration;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.Web.CodeGeneration;
using System.Drawing;
using System.IO;
using QRCoder;
using ZXing.QrCode.Internal;
using System.Collections;

namespace Dhicoin.Controllers
{
   
	public class UserController : Controller
	{
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotyfService _notyfService;
        private readonly CurrencyConverter _converter;
        private readonly IWebHostEnvironment _hostEnvironment;
       
        public UserController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
              INotyfService notyfService,
              CurrencyConverter converter,
              IWebHostEnvironment hostEnvironment )
        {
            _context = context;
            _userManager = userManager;
            _notyfService = notyfService;
            _converter = converter;            _hostEnvironment = hostEnvironment;

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
                    NameRegisteredInBMl = userProfile.NameRegisteredInBMl
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
            // Save changes to the database
            _context.SaveChanges();
            return View(profile);
        }

        public IActionResult Buy()
        {
            //await  _converter.ConvertCurrency();

            ViewBag.CurrenyList = new SelectList(_context.CurrencyMaster, "CoinId", "CurrencyName");
            return View();
        }

        [HttpPost]
        public IActionResult BuyAmount(BuyCurrency buyCurrency)
        {
            string userId = _userManager.GetUserId(User);

            if (buyCurrency != null)
            {
                string dateTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string webRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(buyCurrency.Picture.FileName);
                string extension = Path.GetExtension(buyCurrency.Picture.FileName);
                string uniqueFileName = fileName + "_" + dateTime + extension;

                buyCurrency.translationpicture = uniqueFileName;
                string filePath = Path.Combine(webRootPath, "Transaction", uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    buyCurrency.Picture.CopyTo(fileStream);
                }
                buyCurrency.Status = "Pending";
                buyCurrency.userId = userId;
                buyCurrency.OrderDate = System.DateTime.Now;
                _context.BuyCurrencies.Add(buyCurrency);
                 _context.SaveChanges();
                return Json(new { success = true });


            }

            return new JsonResult("System Error! Please try Again Later");
        }

      

        public IActionResult reverseconversion(string currency, string selectTargetAmount)
        {
            string a = ""; decimal b = 0;
            decimal ConversionRate = 0;
            decimal AmtinUSD = 0;
            decimal Commission = 0;
            decimal CommissionValue = 0;
            decimal NetworkFee = 0;
            decimal ValueinUSD = 0;
            decimal FinalValue = 0;
            decimal ValueAfterDeduction = 0;
            decimal amtinbtc = 0;
            int Conversion_ID = 0;
            decimal ValueinMVR = 0;
            decimal roundValue = 0;
            var BuyRate = _context.CurrencyMaster.Where(x => x.CoinId == selectTargetAmount).FirstOrDefault();
         
            decimal rate = Convert.ToDecimal(BuyRate.BuyConversionRate);

            ConversionRate = rate;

            if (BuyRate != null)
            {           
                decimal convertcurrency = Convert.ToDecimal(currency);

                string vsCurrencies = "usd";
                string NewUrl = "https://api.coingecko.com/api/v3/simple/price?ids=" + selectTargetAmount + "&vs_currencies=" + vsCurrencies + "&include_market_cap=false&include_24hr_vol=false&include_24hr_change=false&include_last_updated_at=false";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //GET Method
                    HttpResponseMessage response = client.GetAsync(NewUrl).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        a = response.Content.ReadAsStringAsync().Result;
                        var result1 = JObject.Parse(a);
                        b = decimal.Parse(result1[selectTargetAmount]["usd"].ToString());
                    }
                }
                if (b != 0)
                {
                   
                    AmtinUSD = Convert.ToDecimal(convertcurrency) * b;
                    Commission = BuyRate.BuyCommission;
                    CommissionValue = (AmtinUSD * Commission) / 100;
                    NetworkFee = 0;
                    ValueinUSD = AmtinUSD;
                    decimal ValinUSDafterDeduction = 0;
                    ValinUSDafterDeduction = (AmtinUSD * 100) / (100 - Commission);
                    ValueAfterDeduction = ValinUSDafterDeduction;

                    FinalValue = ValueAfterDeduction;
                    ValueinMVR = (ValueAfterDeduction) / ConversionRate;

                     roundValue = Math.Round(ValueinMVR, 2);

                    return Json(new { success = true, roundValue});
                }
                else
                {
                    ValueinMVR = 0;
                    return Json(new { success = false });
                }
            }
            return Json(new { success = false });
        }


        public IActionResult GetMaxRate(decimal currency, string selectTargetAmount)
        {
            var BuyRate = _context.CurrencyMaster.Where(x => x.CoinId == selectTargetAmount).FirstOrDefault();

            if (BuyRate != null)
            {
                var getChainList = (from mc in _context.multipleChainSelectionWithBuyChains
                                    join cn in _context.CryptoReceiverAddressDetails
                                    on mc.BuyChainId equals cn.Id
                                    join curreny in _context.CurrencyMaster on
                                    mc.CurrencyMasterId equals curreny.Id
                                    where curreny.CoinId == selectTargetAmount
                                    select cn.ChainName).ToList();
                decimal buyminAmount = BuyRate.BuyLimit;
                decimal buyMaxLimit = BuyRate.MaxBuyLimit;
                int DecimalAmount = BuyRate.Buydigit;

                if (currency >= buyminAmount && currency <= buyMaxLimit)
                {

                    return Json(new { success = true, getChainList, buyminAmount, buyMaxLimit, DecimalAmount });
                }
                else
                {
                    return Json(new { success = false, buyminAmount, buyMaxLimit });
                }
            }

            return Json(new { success = false, message = "System Error : " });


        }

        public IActionResult GetConversionRate(string currency, string selectTargetAmount)
        {
            var BuyRate = _context.CurrencyMaster.Where(x => x.CoinId == selectTargetAmount).FirstOrDefault();

            if(BuyRate!=null)
            {
                var getChainList = (from mc in _context.multipleChainSelectionWithBuyChains
                                    join cn in _context.CryptoReceiverAddressDetails
                                    on mc.BuyChainId equals cn.Id
                                    join curreny in _context.CurrencyMaster on
                                    mc.CurrencyMasterId equals curreny.Id
                                    where curreny.CoinId == selectTargetAmount
                                    select cn.ChainName).ToList();

                int decimalNumber = BuyRate.Buydigit;
                string a = ""; decimal b = 0;

                decimal BuyValue = Convert.ToDecimal(currency);
                decimal ConversionRate = 0;
                decimal AmtinUSD = 0;
                decimal Commission = 0;
                decimal CommissionValue = 0;
                decimal NetworkFee = 0;
                decimal ValueinUSD = 0;
                decimal FinalValue = 0;
                decimal ValueAfterDeduction = 0;
                decimal amtinbtc = 0;
                int Conversion_ID = 0;

                 decimal rate = Convert.ToDecimal(BuyRate.BuyConversionRate);

                ConversionRate = rate;

                AmtinUSD = Convert.ToDecimal(currency) * ConversionRate;

                Commission = BuyRate.BuyCommission;
                CommissionValue = (AmtinUSD * Commission) / 100;
              
                NetworkFee = 0;
                ValueinUSD = AmtinUSD - CommissionValue - NetworkFee;
              
                string vsCurrencies = "usd";
                string ConversionCurrency = selectTargetAmount;
                string NewUrl = "https://api.coingecko.com/api/v3/simple/price?ids=" + ConversionCurrency + "&vs_currencies=" + vsCurrencies + "&include_market_cap=false&include_24hr_vol=false&include_24hr_change=false&include_last_updated_at=false";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //GET Method

                    HttpResponseMessage response = client.GetAsync(NewUrl).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        a = response.Content.ReadAsStringAsync().Result;
                        var result = JObject.Parse(a);
                        b = decimal.Parse(result[ConversionCurrency]["usd"].ToString());
                        FinalValue = ValueinUSD / b;
                        ValueAfterDeduction = (AmtinUSD - CommissionValue - NetworkFee) / b;

                       amtinbtc =  Math.Round(ValueAfterDeduction, decimalNumber);
                    }
                }
                decimal buyminAmount = BuyRate.BuyLimit;
                decimal buyMaxLimit = BuyRate.MaxBuyLimit;

                return Json(new { success = true, amtinbtc, getChainList, buyminAmount, buyMaxLimit });
            }

         
            return View();
        }


        public IActionResult ChainConversion(string currency, string selectTargetAmount,string selectChain)
        {

            var getChain = (from crd in _context.CryptoReceiverAddressDetails 
                        join cr in _context.CryptoReceiverAddress
                        on crd.Id equals cr.ChainId
                        where crd.ChainName == selectChain 
                        select cr.CommesionOnChain).FirstOrDefault();
            if(getChain > 0)
            {
                decimal CommesionOnChain = Convert.ToDecimal(getChain);

                decimal PerstangeCommesion = CommesionOnChain / 100;

                var BuyRate = _context.CurrencyMaster.Where(x => x.CoinId == selectTargetAmount).FirstOrDefault();
                if (BuyRate != null)
                {
                    int decimalNumber = BuyRate.Buydigit;
                    string a = ""; decimal b = 0;

                    decimal BuyValue = Convert.ToDecimal(currency);
                    decimal ConversionRate = 0;
                    decimal AmtinUSD = 0;
                    decimal Commission = 0;
                    decimal CommissionValue = 0;
                    decimal NetworkFee = 0;
                    decimal ValueinUSD = 0;
                    decimal FinalValue = 0;
                    decimal ValueAfterDeduction = 0;
                    decimal AmountinChain = 0;
                    int Conversion_ID = 0;

                    decimal rate = Convert.ToDecimal(BuyRate.BuyConversionRate);

                    ConversionRate = rate;

                    AmtinUSD = Convert.ToDecimal(currency) * ConversionRate;

                    Commission = BuyRate.BuyCommission;
                    CommissionValue = (AmtinUSD * Commission) / 100;
                    NetworkFee = PerstangeCommesion;
                    ValueinUSD = AmtinUSD - CommissionValue - NetworkFee;

                    string vsCurrencies = "usd";
                    string ConversionCurrency = selectTargetAmount;
                    string NewUrl = "https://api.coingecko.com/api/v3/simple/price?ids=" + ConversionCurrency + "&vs_currencies=" + vsCurrencies + "&include_market_cap=false&include_24hr_vol=false&include_24hr_change=false&include_last_updated_at=false";
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        //GET Method

                        HttpResponseMessage response = client.GetAsync(NewUrl).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            a = response.Content.ReadAsStringAsync().Result;
                            var result = JObject.Parse(a);
                            b = decimal.Parse(result[ConversionCurrency]["usd"].ToString());

                            //FinalValue = ValueinUSD / b;
                            ValueAfterDeduction = (ValueinUSD - CommissionValue - NetworkFee) / b;

                            AmountinChain = Math.Round(ValueAfterDeduction, decimalNumber);
                        }
                    }
              
                    return Json(new { success = true, AmountinChain });
                }
            }
            else
            {
                var BuyRate = _context.CurrencyMaster.Where(x => x.CoinId == selectTargetAmount).FirstOrDefault();

                if (BuyRate != null)
                {
                    var getChainList = (from mc in _context.multipleChainSelectionWithBuyChains
                                        join cn in _context.CryptoReceiverAddressDetails
                                        on mc.BuyChainId equals cn.Id
                                        join curreny in _context.CurrencyMaster on
                                        mc.CurrencyMasterId equals curreny.Id
                                        where curreny.CoinId == selectTargetAmount
                                        select cn.ChainName).ToList();

                    int decimalNumber = BuyRate.Buydigit;
                    string a = ""; decimal b = 0;

                    decimal BuyValue = Convert.ToDecimal(currency);
                    decimal ConversionRate = 0;
                    decimal AmtinUSD = 0;
                    decimal Commission = 0;
                    decimal CommissionValue = 0;
                    decimal NetworkFee = 0;
                    decimal ValueinUSD = 0;
                    decimal FinalValue = 0;
                    decimal ValueAfterDeduction = 0;
                    decimal AmountinChain = 0;
                    int Conversion_ID = 0;

                    decimal rate = Convert.ToDecimal(BuyRate.BuyConversionRate);

                    ConversionRate = rate;

                    AmtinUSD = Convert.ToDecimal(currency) * ConversionRate;

                    Commission = BuyRate.BuyCommission;
                    CommissionValue = (AmtinUSD * Commission) / 100;

                    NetworkFee = 0;
                    ValueinUSD = AmtinUSD - CommissionValue - NetworkFee;

                    string vsCurrencies = "usd";
                    string ConversionCurrency = selectTargetAmount;
                    string NewUrl = "https://api.coingecko.com/api/v3/simple/price?ids=" + ConversionCurrency + "&vs_currencies=" + vsCurrencies + "&include_market_cap=false&include_24hr_vol=false&include_24hr_change=false&include_last_updated_at=false";
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        //GET Method

                        HttpResponseMessage response = client.GetAsync(NewUrl).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            a = response.Content.ReadAsStringAsync().Result;
                            var result = JObject.Parse(a);
                            b = decimal.Parse(result[ConversionCurrency]["usd"].ToString());
                            FinalValue = ValueinUSD / b;
                            ValueAfterDeduction = (AmtinUSD - CommissionValue - NetworkFee) / b;

                            AmountinChain = Math.Round(ValueAfterDeduction, decimalNumber);
                        }
                    }
                   

                    return Json(new { success = true, AmountinChain, getChainList });
                }
                
            }
            return Json(new { success = false});
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
            ViewBag.SellCurrenyList = new SelectList(_context.CurrencyMaster, "CoinId", "CurrencyName");

            return View();
        }


        public IActionResult Sellreverseconversion(string currency, string selectTargetAmount)
        {
            string a = ""; decimal b = 0;
            decimal ConversionRate = 0;
            decimal AmtinUSD = 0;
            decimal Commission = 0;
            decimal CommissionValue = 0;
            decimal NetworkFee = 0;
            decimal ValueinUSD = 0;
            decimal FinalValue = 0;
            decimal ValueAfterDeduction = 0;
            decimal amtinbtc = 0;
            int Conversion_ID = 0;
            decimal ValueinMVR = 0;
            decimal roundValue = 0;
            var BuyRate = _context.CurrencyMaster.Where(x => x.CoinId == selectTargetAmount).FirstOrDefault();

            decimal rate = Convert.ToDecimal(BuyRate.SellConversionRate);

            ConversionRate = rate;

            if (BuyRate != null)
            {
                var getChainList = (from mc in _context.multipleChainSelectionWithBuyChains
                                    join cn in _context.CryptoReceiverAddressDetails
                                    on mc.BuyChainId equals cn.Id
                                    join curreny in _context.CurrencyMaster on
                                    mc.CurrencyMasterId equals curreny.Id
                                    where curreny.CoinId == selectTargetAmount
                                    select cn.ChainName).ToList();
             

                decimal convertcurrency = Convert.ToDecimal(currency);

                string vsCurrencies = "usd";
                string NewUrl = "https://api.coingecko.com/api/v3/simple/price?ids=" + selectTargetAmount + "&vs_currencies=" + vsCurrencies + "&include_market_cap=false&include_24hr_vol=false&include_24hr_change=false&include_last_updated_at=false";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //GET Method
                    HttpResponseMessage response = client.GetAsync(NewUrl).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        a = response.Content.ReadAsStringAsync().Result;
                        var result1 = JObject.Parse(a);
                        b = decimal.Parse(result1[selectTargetAmount]["usd"].ToString());
                    }
                }
                if (b != 0)
                {

                    AmtinUSD = Convert.ToDecimal(convertcurrency) * b;
                    Commission = BuyRate.BuyCommission;
                    CommissionValue = (AmtinUSD * Commission) / 100;
                    NetworkFee = 0;
                    ValueinUSD = AmtinUSD;
                    decimal ValinUSDafterDeduction = 0;
                    ValinUSDafterDeduction = (AmtinUSD * 100) / (100 - Commission);
                    ValueAfterDeduction = ValinUSDafterDeduction;

                    FinalValue = ValueAfterDeduction;
                    ValueinMVR = (ValueAfterDeduction) / ConversionRate;

                    roundValue = Math.Round(ValueinMVR, 2);

                    decimal cur = Convert.ToDecimal(currency);

                    string ReciveAdress = "";
                    string code = ReciveAdress + "?Amount=" + 0; 
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);

                    int pixelsPerModule = 20;
                    int width = qrCodeData.ModuleMatrix.Count > 0 ? qrCodeData.ModuleMatrix[0].Count * pixelsPerModule : 0;
                    int height = qrCodeData.ModuleMatrix.Count * pixelsPerModule;
                    Bitmap qrCodeImage = new Bitmap(width, height);

                    using (Graphics graphics = Graphics.FromImage(qrCodeImage))
                    {
                        graphics.Clear(Color.White);
                        using (Brush brush = new SolidBrush(Color.Black))
                        {
                            for (int x = 0; x < qrCodeData.ModuleMatrix.Count; x++)
                            {
                                BitArray row = qrCodeData.ModuleMatrix[x];
                                for (int y = 0; y < row.Count; y++)
                                {
                                    if (row[y])
                                    {
                                        graphics.FillRectangle(brush, y * pixelsPerModule, x * pixelsPerModule, pixelsPerModule, pixelsPerModule);
                                    }
                                }
                            }
                        }
                    }

                    int resizedWidth = 150;  // Set the desired width
                    int resizedHeight = 150; // Set the desired height
                    string image;
                    using (Bitmap resizedImage = new Bitmap(qrCodeImage, resizedWidth, resizedHeight))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            resizedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            byte[] byteImage = ms.ToArray();
                            string base64Image = Convert.ToBase64String(byteImage);
                            string imgSrc = $"data:image/png;base64,{base64Image}";
                            image = imgSrc;
                        }
                    }


                    List<object> d = GetSellMaxRate(selectTargetAmount, roundValue);

                    if(d.Count==4)
                    {
                        return Json(new { success = true, roundValue,getChainList,selectTargetAmount,currency, image, code, ReciveAdress});
                    }
                    else
                    {
                        return Json(new { success = false,d });
                    }  

                }
                else
                {
                    ValueinMVR = 0;
                    return Json(new { success = false });
                }
            }
            return Json(new { success = false });
        }

        private List<object> GetSellMaxRate(string currencyTpye, decimal roundValue)
        {
            var buyRate = _context.CurrencyMaster.FirstOrDefault(x => x.CoinId == currencyTpye);

            if (buyRate != null)
            {

                decimal buyminAmount = buyRate.SellLimit;
                decimal buyMaxLimit = buyRate.MaxSellLimit;
                int decimalAmount = buyRate.Buydigit;

                if (roundValue >= buyminAmount && roundValue <= buyMaxLimit)
                {
                    return new List<object> { buyminAmount, buyMaxLimit, decimalAmount, true };
                }
                else
                {
                    return new List<object> { buyminAmount, buyMaxLimit, false };
                }
            }

            return null;
        }
        public IActionResult GetSellConversionRateReserse(string currency, string selectTargetAmount)
        {
            var BuyRate = _context.CurrencyMaster.Where(x => x.CoinId == selectTargetAmount).FirstOrDefault();

            if (BuyRate != null)
            {
                var getChainList = (from mc in _context.multipleChainSelectionWithBuyChains
                                    join cn in _context.CryptoReceiverAddressDetails
                                    on mc.BuyChainId equals cn.Id
                                    join curreny in _context.CurrencyMaster on
                                    mc.CurrencyMasterId equals curreny.Id
                                    where curreny.CoinId == selectTargetAmount
                                    select cn.ChainName).ToList();

                int decimalNumber = BuyRate.Buydigit;
                string a = ""; decimal b = 0;

                decimal BuyValue = Convert.ToDecimal(currency);
                decimal ConversionRate = 0;
                decimal AmtinUSD = 0;
                decimal Commission = 0;
                decimal CommissionValue = 0;
                decimal NetworkFee = 0;
                decimal ValueinUSD = 0;
                decimal FinalValue = 0;
                decimal ValueAfterDeduction = 0;
                decimal amtinbtc = 0;
                int Conversion_ID = 0;

                decimal rate = Convert.ToDecimal(BuyRate.SellConversionRate);

                ConversionRate = rate;

                AmtinUSD = Convert.ToDecimal(currency) * ConversionRate;

                Commission = BuyRate.BuyCommission;
                CommissionValue = (AmtinUSD * Commission) / 100;

                NetworkFee = 0;
                ValueinUSD = AmtinUSD - CommissionValue - NetworkFee;
                
                string vsCurrencies = "usd";
                string ConversionCurrency = selectTargetAmount;
                string NewUrl = "https://api.coingecko.com/api/v3/simple/price?ids=" + ConversionCurrency + "&vs_currencies=" + vsCurrencies + "&include_market_cap=false&include_24hr_vol=false&include_24hr_change=false&include_last_updated_at=false";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //GET Method

                    HttpResponseMessage response = client.GetAsync(NewUrl).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        a = response.Content.ReadAsStringAsync().Result;
                        var result = JObject.Parse(a);
                        b = decimal.Parse(result[ConversionCurrency]["usd"].ToString());
                        FinalValue = ValueinUSD / b;
                        ValueAfterDeduction = (AmtinUSD - CommissionValue - NetworkFee) / b;

                        amtinbtc = Math.Round(ValueAfterDeduction, decimalNumber);
                    }
                }             

                return Json(new { success = true, amtinbtc, getChainList });
            }


            return Json(new { success = false });
        }
        public IActionResult SellChainConversion(decimal currency, string selectTargetAmount, string selectChain)
        {

            var getChain = (from crd in _context.CryptoReceiverAddressDetails
                            join cr in _context.CryptoReceiverAddress
                            on crd.Id equals cr.ChainId
                            where crd.ChainName == selectChain
                            select cr.CommesionOnChain).FirstOrDefault();
            var getRecervicerAdress = (from crd in _context.CryptoReceiverAddressDetails
                            join cr in _context.CryptoReceiverAddress
                            on crd.Id equals cr.ChainId
                            where crd.ChainName == selectChain
                            select cr.ReceiveAddress).FirstOrDefault();
            if (getChain > 0)
            {



                decimal CommesionOnChain = Convert.ToDecimal(getChain);

                decimal PerstangeCommesion = CommesionOnChain / 100;

                var BuyRate = _context.CurrencyMaster.Where(x => x.CoinId == selectTargetAmount).FirstOrDefault();
                if (BuyRate != null)
                {
                    int decimalNumber = BuyRate.Buydigit;
                    string a = ""; decimal b = 0;

                    decimal BuyValue = Convert.ToDecimal(currency);
                    decimal ConversionRate = 0;
                    decimal AmtinUSD = 0;
                    decimal Commission = 0;
                    decimal CommissionValue = 0;
                    decimal NetworkFee = 0;
                    decimal ValueinUSD = 0;
                    decimal FinalValue = 0;
                    decimal ValueAfterDeduction = 0;
                    decimal AmountinChain = 0;
                    int Conversion_ID = 0;
                    decimal ValueinMVR = 0;
                    decimal roundValue = 0;
                               

                    string vsCurrencies = "usd";
                    string ConversionCurrency = selectTargetAmount;
                    string NewUrl = "https://api.coingecko.com/api/v3/simple/price?ids=" + ConversionCurrency + "&vs_currencies=" + vsCurrencies + "&include_market_cap=false&include_24hr_vol=false&include_24hr_change=false&include_last_updated_at=false";
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        //GET Method

                        HttpResponseMessage response = client.GetAsync(NewUrl).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            a = response.Content.ReadAsStringAsync().Result;
                            var result = JObject.Parse(a);
                            b = decimal.Parse(result[ConversionCurrency]["usd"].ToString());
                        }
                    }
                    if (b != 0)
                    {                 
                        decimal rate = Convert.ToDecimal(BuyRate.SellConversionRate);

                        ConversionRate = rate;

                        AmtinUSD = Convert.ToDecimal(currency) * b;
                        Commission = BuyRate.SellCommission;
                        CommissionValue = (AmtinUSD * Commission) / 100;
                        NetworkFee = PerstangeCommesion;
                        ValueinUSD = AmtinUSD - CommissionValue - NetworkFee;
                        decimal ValinUSDafterDeduction = 0;
                        ValinUSDafterDeduction = (ValueinUSD * 100) / (100 - Commission);
                        ValueAfterDeduction = ValinUSDafterDeduction;

                        FinalValue = ValueAfterDeduction;
                        ValueinMVR = (ValueAfterDeduction) / ConversionRate;

                        roundValue = Math.Round(ValueinMVR, 2);



                        decimal cur = Convert.ToDecimal(currency);
                        string code = getRecervicerAdress + "?Amount=" + currency;
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);

                        int pixelsPerModule = 20;
                        int width = qrCodeData.ModuleMatrix.Count > 0 ? qrCodeData.ModuleMatrix[0].Count * pixelsPerModule : 0;
                        int height = qrCodeData.ModuleMatrix.Count * pixelsPerModule;
                        Bitmap qrCodeImage = new Bitmap(width, height);

                        using (Graphics graphics = Graphics.FromImage(qrCodeImage))
                        {
                            graphics.Clear(Color.White);
                            using (Brush brush = new SolidBrush(Color.Black))
                            {
                                for (int x = 0; x < qrCodeData.ModuleMatrix.Count; x++)
                                {
                                    BitArray row = qrCodeData.ModuleMatrix[x];
                                    for (int y = 0; y < row.Count; y++)
                                    {
                                        if (row[y])
                                        {
                                            graphics.FillRectangle(brush, y * pixelsPerModule, x * pixelsPerModule, pixelsPerModule, pixelsPerModule);
                                        }
                                    }
                                }
                            }
                        }

                        int resizedWidth = 150;  // Set the desired width
                        int resizedHeight = 150; // Set the desired height
                        string image;
                        using (Bitmap resizedImage = new Bitmap(qrCodeImage, resizedWidth, resizedHeight))
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                resizedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                byte[] byteImage = ms.ToArray();
                                string base64Image = Convert.ToBase64String(byteImage);
                                string imgSrc = $"data:image/png;base64,{base64Image}";
                                image = imgSrc;
                            }
                        }
                        return Json(new { success = true, roundValue,image, getRecervicerAdress });
                    }
                    else
                    {
                        ValueinMVR = 0;
                        return Json(new { success = false });
                    }    
                }
            }
            else
            {
                var BuyRate = _context.CurrencyMaster.Where(x => x.CoinId == selectTargetAmount).FirstOrDefault();

                if (BuyRate != null)
                {
                  

                    int decimalNumber = BuyRate.Buydigit;
                    string a = ""; decimal b = 0;

                    decimal BuyValue = Convert.ToDecimal(currency);
                    decimal ConversionRate = 0;
                    decimal AmtinUSD = 0;
                    decimal Commission = 0;
                    decimal CommissionValue = 0;
                    decimal NetworkFee = 0;
                    decimal ValueinUSD = 0;
                    decimal FinalValue = 0;
                    decimal ValueAfterDeduction = 0;
                    decimal AmountinChain = 0;
                    int Conversion_ID = 0;

                    decimal rate = Convert.ToDecimal(BuyRate.SellConversionRate);

                    ConversionRate = rate;

                    AmtinUSD = Convert.ToDecimal(currency) * ConversionRate;

                    Commission = BuyRate.BuyCommission;
                    CommissionValue = (AmtinUSD * Commission) / 100;

                    NetworkFee = 0;
                    ValueinUSD = AmtinUSD - CommissionValue - NetworkFee;

                    string vsCurrencies = "usd";
                    string ConversionCurrency = selectTargetAmount;
                    string NewUrl = "https://api.coingecko.com/api/v3/simple/price?ids=" + ConversionCurrency + "&vs_currencies=" + vsCurrencies + "&include_market_cap=false&include_24hr_vol=false&include_24hr_change=false&include_last_updated_at=false";
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        //GET Method

                        HttpResponseMessage response = client.GetAsync(NewUrl).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            a = response.Content.ReadAsStringAsync().Result;
                            var result = JObject.Parse(a);
                            b = decimal.Parse(result[ConversionCurrency]["usd"].ToString());
                            FinalValue = ValueinUSD / b;
                            ValueAfterDeduction = (AmtinUSD - CommissionValue - NetworkFee) / b;

                            AmountinChain = Math.Round(ValueAfterDeduction, decimalNumber);
                        }
                    }


                    return Json(new { success = true, AmountinChain, selectTargetAmount });
                }

            }
            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult SellCoin(SellCurrency sellCurrency)
        {
            if(sellCurrency!=null)
            {
                string userId = _userManager.GetUserId(User);
                sellCurrency.SellStatus = "Pending";
                sellCurrency.CurrencySellDate = System.DateTime.Now;
                sellCurrency.UserID = userId;
                _context.SellCurrencies.Add(sellCurrency);
                _context.SaveChanges();
                return Json(new { success = true });
            }

            return new JsonResult("System Error! Please try Again Later");
        }

        [HttpPost]
        public IActionResult AddressMatch(string WalletAddress, string Chain)
        {

            var Adress = (from crd in _context.CryptoReceiverAddressDetails
                            join cr in _context.CryptoReceiverAddress
                            on crd.Id equals cr.ChainId
                            where crd.ChainName == Chain
                            select cr.ReceiveAddress).FirstOrDefault();

            if(Adress != null)
            {

                if(Adress==WalletAddress)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }


            }

            return Json(new { success = false });
        }


        public IActionResult SellHistory()
        {
            return View();
        }

    }
}
