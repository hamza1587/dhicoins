using Dhicoin.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Dhicoin.Models
{
    [Table("CustomerMasterProfile")]
    public class CustomerMasterProfile
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        [DisplayName("Email")]
        public string Email { get; set; }
        [Required]
        [DisplayName("Gender")]
        public string Gender { get; set; }
        [Required]
        [DisplayName("Date Of Birth")]
        public string DateOfBirth { get; set; }
        [Required]
        [DisplayName("Country Code")]
        public string CountryCode { get; set; }
        [Required]
        [DisplayName("Bank Name")]
        public string BankName { get; set; }
        [Required]
        [DisplayName("Account")]
        public string Account { get; set; }
        [Required]
        [DisplayName("Name Registered In BML")]
        public string NameRegisteredInBMl { get; set; }
        [DisplayName("KYC Verification Status")]

        public string KYCStatus { get; set; }
        [DisplayName("Customer Creation Status")]
        public string CreationStatus { get; set; }
        public string? RejectionRemarks { get; set; }
        public string UserId { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }
}
