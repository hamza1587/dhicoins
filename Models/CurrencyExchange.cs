#nullable disable
using System.ComponentModel.DataAnnotations;

namespace Dhicoin.Models
{
    public class CurrencyExchange
    {
        [Key]
        public int Id { get; set; }

        public string MVRTOUSD { get; set; }
    }
}
