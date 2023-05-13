using Org.BouncyCastle.Asn1.Mozilla;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dhicoin.Models
{
    public class ChainDetail
    {
        public int Id { get; set; }
        public int CryptoId { get; set; }
        [ForeignKey("CryptoId")]
        public CryptoReceiveAddress CryptoReceiverAddress { get; set; }
        public int ChainId { get; set; }
        [ForeignKey("ChainId")]
        public CryptoReceiverAddressDetails CryptoReceiverAddressDetails { get; set; }
    }
}
