#nullable disable
using System.ComponentModel.DataAnnotations;

namespace Dhicoin.Models
{
    public class SellCurrency
    {
        [Key]
        public int Id { get; set; }

        public string btcAmount { get; set; }

        public decimal MvrAmount { get; set; }

        public string SelectCurrency { get; set; }

        public string CurrenyChain { get; set; }

        public string UserID { get; set; }

        public DateTime CurrencySellDate { get; set; }

        public string CurrencyTakeFrom { get; set; }    
        public string SellRemarks { get; set; }

        public string SellStatus { get; set; }

    }
}
