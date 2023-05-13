using System.ComponentModel.DataAnnotations.Schema;

namespace Dhicoin.Models
{
    public class CryptoReceiveAddress
    {
        public int Id { get; set; }
        //public string ChainName { get; set; }
        public string CurrencyName { get; set; }
        public string ReceiveAddress { get; set; }
        public bool Status { get; set; }
        [NotMapped]
        public int[] ChainNameId { get; set; }

    }
}
