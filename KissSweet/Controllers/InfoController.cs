﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KissSweet.Controllers
{
    public class InfoController : Controller
    {
        public IActionResult Index()
        {
            return PartialView();
        }
    }
}
