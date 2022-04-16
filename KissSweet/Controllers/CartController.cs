﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using KissSweet.Helpers;
using KissSweet.Models;
using KissSweet.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace KissSweet.Controllers
{
   
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            //向 Session 取得商品列表
            List<CartItem> CartItems = SessionHelper.
                GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");

            //計算商品總額
            if (CartItems != null)
            {
                ViewBag.Total = CartItems.Sum(m => m.SubTotal);
            }
            else
            {
                ViewBag.Total = 0;
            }

            return View(CartItems);
        }
        public IActionResult AddtoCart(int id)
        {
            var product = _context.Product.Single(x => x.Id.Equals(id));
            CartItem item = new CartItem()
            {
                ProductId = product.Id,
                Product = product,
                Amount = 1,
                SubTotal = product.Price,
                imageSrc = ViewImage(product.Image)
            };

            //判斷 Session 內有無購物車
            if (SessionHelper.
                GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart") == null)
            {
                //如果沒有已存在購物車: 建立新的購物車
                List<CartItem> cart = new List<CartItem>();
                cart.Add(item);
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                //如果已存在購物車: 檢查有無相同的商品，有的話只調整數量
                List<CartItem> cart = SessionHelper.
                    GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");

                int index = cart.FindIndex(m => m.Product.Id.Equals(id));
                System.Diagnostics.Debug.WriteLine("add",index.ToString());
                if (index != -1)
                {
                    cart[index].Amount += item.Amount;
                    cart[index].SubTotal += item.SubTotal;
                }
                else
                {
                    cart.Add(item);
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }

            return NoContent(); // HttpStatus 204: 請求成功但不更新畫面
        }

        public IActionResult RemoveItem(int id)
        {
            //向 Session 取得商品列表
            List<CartItem> cart = SessionHelper.
                GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");

            //用FindIndex查詢目標在List裡的位置
            int index = cart.FindIndex(m => m.Product.Id.Equals(id));
            System.Diagnostics.Debug.WriteLine("remove", index.ToString());

            if (cart.Count < 1)
            {
                SessionHelper.Remove(HttpContext.Session, "cart");
            }
            else
            {
                cart.RemoveAt(index);
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }

            return RedirectToAction("Index");
        }

        private string ViewImage(byte[] arrayImage)
        {
            string base64String = Convert.ToBase64String(arrayImage, 0, arrayImage.Length);
            return "data:image/png;base64," + base64String;
        }
    }
}
