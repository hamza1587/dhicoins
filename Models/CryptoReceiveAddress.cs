#nullable disable
using System.ComponentModel.DataAnnotations.Schema;

namespace Dhicoin.Models
{
    public class CryptoReceiveAddress
    {
        public int Id { get; set; }
      
        public string ReceiveAddress { get; set; }
        public bool Status { get; set; }
        
        public int ChainId { get; set; }
        [ForeignKey("ChainId")]

        public CryptoReceiverAddressDetails CryptoReceiverAddressDetails { get; set; }

        public decimal CommesionOnChain { get; set; }




    }
}
