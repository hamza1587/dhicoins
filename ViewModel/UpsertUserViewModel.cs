#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Dhicoin.ViewModel
{
    public class UpsertUserViewModel
    {
        public string Id { get; set; }
        [DisplayName("User Name")]
        public string UserName { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public bool Status { get; set; }
        [DisplayName("User Type")]
        public string UserType { get; set; }

        public string Email { get;set; }
    }
}
