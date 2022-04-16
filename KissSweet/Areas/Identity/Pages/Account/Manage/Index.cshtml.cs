using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KissSweet.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KissSweet.Helpers;

namespace KissSweet.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<KissSweetUser> _userManager;
        private readonly SignInManager<KissSweetUser> _signInManager;

        public IndexModel(
            UserManager<KissSweetUser> userManager,
            SignInManager<KissSweetUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }
        public bool PhoneNumberConfirmed { get; set; }

        [TempData]
        public string PhoneNumberConfirmedStatusMessage { get; set; }

        [TempData]
        public string ProfileStatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Birth Date")]
            [DataType(DataType.Date)]
            public DateTime DOB { get; set; }

            [Display(Name = "Name")]
            [DataType(DataType.Text)]
            public string Name { get; set; }

            [Display(Name = "Gender")]
            public GenderType Gender { get; set; }          //性別
        }

        private async Task LoadAsync(KissSweetUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                DOB = user.DOB,
                Name = user.Name,
                Gender = user.Gender,
                PhoneNumber = phoneNumber,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            if (!PhoneNumberConfirmed)
            {
                PhoneNumberConfirmedStatusMessage = "Error" + "手機尚未進行認證，請先認證手機才能使用其他功能";
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
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

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            
            if (Input.PhoneNumber != phoneNumber || Input.DOB != user.DOB || Input.Name != user.Name || Input.Gender != user.Gender)
            {
                user.DOB = Input.DOB;
                user.Name = Input.Name;
                user.PhoneNumber = Input.PhoneNumber;
                user.Gender = Input.Gender;

                if (Input.PhoneNumber != phoneNumber)
                {
                    user.PhoneNumberConfirmed = false;
                }
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    ProfileStatusMessage = "Error" + "無預期的錯誤，請重新更改資料";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            ProfileStatusMessage = "您的資料已更改成功";
            return RedirectToPage();
        }
    }
}
