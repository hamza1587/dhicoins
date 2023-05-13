#nullable disable
using System.ComponentModel.DataAnnotations.Schema;

namespace Dhicoin.Models
{
    public class CurrenciesMaster
    {
        public int Id { get; set; }
        public string CurrencyName { get; set; }
        public string Symbol { get; set; }
        public string CoinId { get; set; }
        public bool CommissionApplicable { get; set; }
        public decimal BuyCommission { get; set; }
        public decimal SellCommission { get; set; }
        public bool BuyStatus { get; set; }
        public bool SellStatus { get; set; }

        public decimal MaxBuyLimit { get; set; }
        public decimal BuyLimit { get; set; }
        public decimal MaxSellLimit { get; set; }
        public decimal SellLimit { get; set; }
        public string BuyConversionRate { get; set; }
        public string SellConversionRate { get; set; }

        public int Buydigit { get; set; }

      
        [NotMapped]
        public int[] BuyChainId { get; set; }
        [NotMapped]
        public int[] SellChainId { get; set; }
       
    }
}
