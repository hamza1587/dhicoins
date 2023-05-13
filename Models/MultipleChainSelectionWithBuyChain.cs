#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dhicoin.Models
{
    public class MultipleChainSelectionWithBuyChain
    {

        //This Table for BuChain
        [Key]
        public int Id { get; set; }
     
        public int BuyChainId { get; set; }

        public int CurrencyMasterId { get; set; }
        [ForeignKey("CurrencyMasterId")]

        public CurrenciesMaster CurrenciesMaster { get; set; }
    }
}
