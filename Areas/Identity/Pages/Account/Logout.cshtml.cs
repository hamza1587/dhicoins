// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Dhicoin.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Dhicoin.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnPost(string userId, string returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                user.IsOnline = false;
                var login = _context.UserLoginDetail.FirstOrDefault(x => x.UserId == user.Id);

                if (login != null)
                {
                    login.IsOnline = "Logged out";
                    _context.UserLoginDetail.Update(login);
                    _context.SaveChanges();
                }
               
            }
          
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToPage();
            }
        }
    }
}
