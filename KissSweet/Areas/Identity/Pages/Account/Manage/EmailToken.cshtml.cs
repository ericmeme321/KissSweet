using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using KissSweet.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MailKit.Net.Smtp;
using MimeKit;

namespace KissSweet.Areas.Identity.Pages.Account.Manage
{
    public class EmailTokenModel : PageModel
    {
        private readonly UserManager<KissSweetUser> _userManager;
        private readonly SignInManager<KissSweetUser> _signInManager;
        public EmailTokenModel(
            UserManager<KissSweetUser> userManager,
            SignInManager<KissSweetUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public bool DbEmailConfirmed { get; set; }
        [TempData]
        public string EmailTokenStatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required]
            public string EmailToken { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            EmailTokenStatusMessage = "";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user.EmailToken.Equals(Input.EmailToken))
            {
                user.EmailConfirmed = true;
                DbEmailConfirmed = true;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    EmailTokenStatusMessage = "Error" + "�L�w�������~�A�Э��s�q�l�H�c";
                    return RedirectToPage();
                }

                EmailTokenStatusMessage = "�q�l�H�c�{�Ҧ��\";

                return Page();
            }
            else
            {
                EmailTokenStatusMessage = "Error" + "�{�ҽX���~";

                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostSendMailTokenAsync()
        {
            try
            {
                var user =   await _userManager.GetUserAsync(User);
                user.EmailToken = GetToken();
                var result =  await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    try
                    {
                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress("KissSweet�k���b�����ҽX", "allen860812@gmail.com"));
                        message.To.Add(new MailboxAddress(user.Name, user.Email));
                        message.Subject = "";
                        message.Body = new TextPart("plain")
                        {
                            Text = "�z�n�w��[�J�k���|��!�@�z���b�����ҽX�� " + user.EmailToken + " �жi��b�����ҡA���¡C"
                        };
                        using (var client = new SmtpClient())
                        {
                            client.Connect("smtp.gmail.com", 587, false);
                            client.Authenticate("allen860812@gmail.com", "allenisme123");
                            client.Send(message);
                            client.Disconnect(true);
                        }
                        System.Diagnostics.Debug.WriteLine("success");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("fail about "+ex);
                    }
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("fail");
            }

            return RedirectToPage();
        }
        private string GetToken()
        {
            string PhoneToken = "";
            Random myObject = new Random();
            int count = 3;
            while (count > 0)
            {
                int ranNum = myObject.Next(10, 99);
                PhoneToken += ranNum.ToString();
                count--;
            }
            return PhoneToken;
        }
    }
}
