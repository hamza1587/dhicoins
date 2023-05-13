using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dhicoin.Models
{
    public class UserLoginDetail
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        [DisplayName("User Name")]
        public string? FullName { get; set; }
        public string Email { get; set; }

        public string IsOnline { get; set; }
        public string IpAddress { get; set; }
        public string BrowserName { get; set; }
        public string BrowserOS { get; set; }
        public string BrowserVersion { get; set; }
        public string DeviceType { get; set; }

        public string? Country { get; set; }

        public DateTime Date { get; set; }
    }
}
