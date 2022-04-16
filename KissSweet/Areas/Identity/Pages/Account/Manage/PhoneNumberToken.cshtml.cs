using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Threading;
using KissSweet.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using KissSweet.Areas.Identity.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KissSweet.Areas.Identity.Pages.Account.Manage
{
    public class PhoneNumberTokenModel : PageModel
    {
        private readonly UserManager<KissSweetUser> _userManager;
        private readonly SignInManager<KissSweetUser> _signInManager;
        public PhoneNumberTokenModel(
            UserManager<KissSweetUser> userManager,
            SignInManager<KissSweetUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public bool DbPhoneNumberConfirmed { get; set; }
        public int seconds { get; set; }
        [TempData]
        public string PhoneNumberTokenStatusMessage { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required]
            public string PhoneNumberToken { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user.PhoneNumberToken.Equals(Input.PhoneNumberToken))
            {
                user.PhoneNumberConfirmed = true;
                DbPhoneNumberConfirmed = user.PhoneNumberConfirmed;
                var result = await _userManager.UpdateAsync(user);
                SessionHelper.SetObjectAsJson(HttpContext.Session, "PhoneConfirmed", true);
                if (!result.Succeeded)
                {
                    PhoneNumberTokenStatusMessage = "Error" + "無預期的錯誤，請重新認證手機";
                    return RedirectToPage();
                }

                PhoneNumberTokenStatusMessage = "手機驗證成功";

                return Page();
            }
            else
            {
                PhoneNumberTokenStatusMessage = "Error" + "認證碼錯誤";

                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);

            return Page();
        }
    }
}
