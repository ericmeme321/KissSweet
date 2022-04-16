using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KissSweet.Helpers;
using KissSweet.Models;
using Microsoft.AspNetCore.Identity;
using KissSweet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using KissSweet.Areas.Identity.Data;

namespace KissSweet.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<KissSweetUser> _userManager;

        public AccountController(UserManager<KissSweetUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
