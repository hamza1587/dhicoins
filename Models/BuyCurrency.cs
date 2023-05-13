#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dhicoin.Models
{
    public class BuyCurrency
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public decimal MvrToUsdAmount { get; set; }
        [Required]
        public string BtcAmount { get; set; }

        public string CurrenyChain { get; set; }
        [Required]
        public string ReceiveWalletAddress { get; set; }

        public string RefernceNote { get; set; }
        [Required]
        public string translationpicture { get; set; }

        [NotMapped]

        public IFormFile Picture { get; set; }  

        public string SelectCurrency { get; set; }

        public string Status { get; set; }

        public string userId { get;set; } 

        public DateTime OrderDate { get; set; }
    }
}
