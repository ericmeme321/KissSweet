﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KissSweet.Models
{
    public class OrderViewModel
    {
        public Order Order { get; set; }
        public List<CartItem> CartItems { get; set; }
    }
}
