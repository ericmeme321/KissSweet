using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Linq;
using System.Threading.Tasks;
using KissSweet.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KissSweet.Areas.Identity.Pages.Account.Manage
{
    public partial class EmailModel : PageModel
    {
        private readonly UserManager<KissSweetUser> _userManager;
        private readonly SignInManager<KissSweetUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public EmailModel(
            UserManager<KissSweetUser> userManager,
            SignInManager<KissSweetUser> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string EmailConfirmedStatusMessage { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }
        }

        private async Task LoadAsync(KissSweetUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;

            Input = new InputModel
            {
                NewEmail = email,
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            if (!IsEmailConfirmed)
            {
                EmailConfirmedStatusMessage = "Error" + "電子信箱尚未進行認證，請先認證手機才能使用其他功能";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                user.Email = Input.NewEmail;
                user.UserName = Input.NewEmail;
                user.EmailConfirmed = false;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    StatusMessage = "Error" + "無預期的錯誤，請重新更改資料";
                    return RedirectToPage();
                }
                else
                {
                    StatusMessage = "您的電子郵件已更改";
                }
                //var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                //var callbackUrl = Url.Page(
                //    "/Account/ConfirmEmailChange",
                //    pageHandler: null,
                //    values: new { userId = userId, email = Input.NewEmail, code = code },
                //    protocol: Request.Scheme);
                //await _emailSender.SendEmailAsync(
                //    Input.NewEmail,
                //    "Confirm your email",
                //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return RedirectToPage();
            }

            StatusMessage = "Error" + "電子信箱不可相同";
            return RedirectToPage();
        }

        //public async task<iactionresult> onpostsendverificationemailasync()
        //{
        //    var user = await _usermanager.getuserasync(user);
        //    if (user == null)
        //    {
        //        return notfound($"unable to load user with id '{_usermanager.getuserid(user)}'.");
        //    }

        //    if (!modelstate.isvalid)
        //    {
        //        await loadasync(user);
        //        return page();
        //    }

        //    var userid = await _usermanager.getuseridasync(user);
        //    var email = await _usermanager.getemailasync(user);
        //    var code = await _usermanager.generateemailconfirmationtokenasync(user);
        //    code = webencoders.base64urlencode(encoding.utf8.getbytes(code));
        //    var callbackurl = url.page(
        //        "/account/confirmemail",
        //        pagehandler: null,
        //        values: new { area = "identity", userid = userid, code = code },
        //        protocol: request.scheme);
        //    await _emailsender.sendemailasync(
        //        email,
        //        "confirm your email",
        //        $"please confirm your account by <a href='{htmlencoder.default.encode(callbackurl)}'>clicking here</a>.");

        //    statusmessage = "verification email sent. please check your email.";
        //    return redirecttopage();
        //}
    }
}
