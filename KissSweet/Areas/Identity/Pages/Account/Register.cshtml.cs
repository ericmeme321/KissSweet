using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using KissSweet.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using KissSweet.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace KissSweet.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<KissSweetUser> _signInManager;
        private readonly UserManager<KissSweetUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<KissSweetUser> userManager,
            SignInManager<KissSweetUser> signInManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Full name")]
            public string Name { get; set; }
            public DateTime RegistrationDate { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {

                var user = new KissSweetUser
                {
                    Email = Input.Email,
                    UserName = Input.Email,
                    Name = Input.Name,
                    RegistrationDate = DateTime.Today,
                    PhoneNumberToken = GetToken(),
                    EmailToken = GetToken(),
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    //sendPhoneToken(user.PhoneNumber, user.PhoneNumberToken);
                    //sendMailToken(user.Email, user.EmailToken);

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return Redirect("/Identity/Account/Manage/EmailToken");
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private string GetToken()
        {
            string PhoneToken = "";
            Random myObject = new Random();
            int count = 3;
            while (count > 0)
            {
                int ranNum = myObject.Next(0, 99);
                PhoneToken += ranNum.ToString();
                count--;
            }
            return PhoneToken;
        }

        private void sendMailToken(string UserMail, string UserMailToken)
        {
            try
            {
                MailMessage mm = new MailMessage();
                mm.To.Add(UserMail);
                mm.Body = "您的信箱驗證碼為: " + UserMailToken;
                mm.From = new MailAddress("allen860812@gmail.com");
                mm.IsBodyHtml = false;
                SmtpClient smtp = new SmtpClient("smtp@gmail.com");
                smtp.Port = 587;
                smtp.UseDefaultCredentials = true;
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential("allen860812@gmail.com", "allenisme123");
                smtp.Send(mm);
                System.Diagnostics.Debug.WriteLine("success");
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("fail");
            }
        }
        //private void sendPhoneToken(string UserPhoneNumber, string UserPhoneToken)
        //{
        //    try
        //    {
        //        WebClient client = new WebClient();

        //        var url = string.Format("http://202.39.48.216/kotsmsapi-1.php?username={0}&password={1}&dstaddr={2}&smbody={3}",
        //           "allen860812real",
        //           "allen860812",
        //           UserPhoneNumber,
        //           HttpUtility.UrlEncode("驗證碼為" + UserPhoneToken + "請驗證您的帳號", Encoding.GetEncoding(950))
        //           );
        //        var result = client.DownloadString(url);
        //        System.Diagnostics.Debug.WriteLine(result);
        //    }
        //    catch
        //    {
        //        System.Diagnostics.Debug.WriteLine("錯誤");
        //    }
        //}
    }
}
