using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dhicoin.Models
{
    public class CryptoReceiverAddressDetails
    {
        public int Id { get; set; }
        [DisplayName("Chain Name")]
        public string ChainName { get; set; }
        [DisplayName("Chain Code")]
        public string ChainCode { get; set; }

        [DisplayName("Chain Status")]
        public bool ChainStatus { get; set; }

      
    }
}
