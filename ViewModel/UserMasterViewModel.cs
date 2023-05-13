#nullable disable
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Dhicoin.ViewModel
{
    public class UserMasterViewModel
    {
        public string Id { get; set; }
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [DisplayName("Email ID")]
        public string Email { get; set; }
        [DisplayName("User Type")]
        public string UserType { get; set; }
        public string Status { get; set; }
    }
}
