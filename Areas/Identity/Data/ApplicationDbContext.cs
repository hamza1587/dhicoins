#nullable disable
using Dhicoin.Areas.Identity.Data;
using Dhicoin.Models;
using Dhicoin.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dhicoin.Areas.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<UserLoginDetail> UserLoginDetail { get; set; }
    public DbSet<CryptoReceiveAddress> CryptoReceiverAddress { get; set; }
    public DbSet<CryptoReceiverAddressDetails> CryptoReceiverAddressDetails { get; set; }
    public DbSet<CurrenciesMaster> CurrencyMaster { get; set; }

    public DbSet<MultipleChainSelection> multipleChainSelections { get; set; }
  
    public DbSet<CustomerMasterProfile> CustomerMasterProfile { get; set; }
  

    public DbSet<CurrencyExchange> CurrencyExchanges { get; set; }

    public DbSet<BuyCurrency> BuyCurrencies { get; set; }

    public DbSet<MultipleChainSelectionWithBuyChain> multipleChainSelectionWithBuyChains { get; set; }  

    public DbSet<SellCurrency> SellCurrencies { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
