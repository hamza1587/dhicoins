using Org.BouncyCastle.Asn1.Mozilla;

namespace Dhicoin.ViewModel
{
    public class CryptoReceiveAdressViewModel
    {
        
        public int Id { get;set ; }
        public string ChainName { get;set ; }
        public string CurrencyName { get;set ; }
        public string ReceiveAddress { get;set ; }
        public string Status { get;set ; }
    }
}
