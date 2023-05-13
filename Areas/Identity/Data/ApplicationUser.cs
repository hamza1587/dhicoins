using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Dhicoin.Models;
using Microsoft.AspNetCore.Identity;

namespace Dhicoin.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [DisplayName("User Name")]
    public string? FullName { get; set; }
    public bool Status { get; set; }
    public bool IsOnline { get; set; }
  
    public virtual CustomerMasterProfile Profile { get; set; }




}

