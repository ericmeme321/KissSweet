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
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<KissSweetUser> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<KissSweetUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if (GetEmailConfirmed())
            {
                List<OrderViewModel> orderVM = new List<OrderViewModel>();
                var userId = _userManager.GetUserId(User);
                var orders = await _context.Order.
                                OrderByDescending(k => k.OrderDate).  //依照日期排序
                                Where(m => m.UserId == userId).ToListAsync();

                foreach (var item in orders)
                {
                    item.OrderItem = await _context.OrderItem.
                                        Where(p => p.OrderId == item.Id).ToListAsync();
                    var vm = new OrderViewModel()
                    {
                        Order = item,
                        CartItems = GetOrderItems(item.Id)
                    };
                    orderVM.Add(vm);
                }
                return View(orderVM);
            }

            return Redirect("/Identity/Account/Manage/Email");

        }
        public async Task<IActionResult> Details(int? Id)
        {
            if (GetEmailConfirmed())
            {
                if (Id == null)
                {
                    return NotFound();
                }

                var order = await _context.Order.FirstOrDefaultAsync(m => m.Id == Id);
                if (order.UserId != _userManager.GetUserId(User))
                {
                    return NotFound();
                }
                else
                {
                    order.OrderItem = await _context.OrderItem.
                                        Where(p => p.OrderId == Id).ToListAsync();
                    ViewBag.orderItems = GetOrderItems(order.Id);
                }
                return View(order);
            }

            return Redirect("/Identity/Account/Manage/Email");
        }
        public IActionResult Checkout()
        {
            if (GetEmailConfirmed())
            {
                if (SessionHelper.GetObjectFromJson<List<OrderItem>>(HttpContext.Session, "cart") == null)
                {
                    return RedirectToAction("Index", "Cart");
                }

                var myOrder = new Order()
                {
                    UserId = _userManager.GetUserId(User),     //取得訂購人ID
                    UserName = _userManager.GetUserName(User), //取得訂購人帳號
                    OrderItem = SessionHelper.                 //取得購物車資料
                        GetObjectFromJson<List<OrderItem>>(HttpContext.Session, "cart"),
                };

                myOrder.Total = myOrder.OrderItem.Sum(m => m.SubTotal); //計算訂單總額

                ViewBag.CartItems = SessionHelper.
                    GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");

                return View(myOrder);
            }

            return Redirect("/Identity/Account/Manage/Email");
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            if (GetEmailConfirmed())
            {
                if (ModelState.IsValid)
                {
                    order.OrderDate = DateTime.Now;    //取得當前時間
                    order.isPaid = false;              //付款狀態預設為False
                    order.OrderItem = SessionHelper.   //綁定訂單內容
                        GetObjectFromJson<List<OrderItem>>(HttpContext.Session, "cart");
                    _context.Add(order);               //將訂單寫入資料庫
                    await _context.SaveChangesAsync();
                    SessionHelper.Remove(HttpContext.Session, "cart");
                    System.Diagnostics.Debug.WriteLine(order.OrderItem);

                    //完成後畫面移轉至ReviewOrder()
                    return RedirectToAction("ReviewOrder", new { Id = order.Id });
                }
                return View("Checkout");
            }

            return Redirect("/Identity/Account/Manage/Email");
        }
        public async Task<IActionResult> ReviewOrder(int? Id)
        {
            if (GetEmailConfirmed())
            {
                if (Id == null)
                {
                    return NotFound();
                }
                //從資料庫取得訂單資料
                var order = await _context.Order.FirstOrDefaultAsync(m => m.Id == Id);
                if (order.UserId != _userManager.GetUserId(User))
                {
                    return NotFound();
                }
                else
                {
                    //取得訂單內容
                    order.OrderItem = await _context.OrderItem
                        .Where(p => p.OrderId == Id).ToListAsync();
                    ViewBag.orderItems = GetOrderItems(order.Id);
                }
                return View(order);
            }

            return Redirect("/Identity/Account/Manage/Email");
        }

        // 取得商品詳細資料
        private List<CartItem> GetOrderItems(int orderId)
        {
            var OrderItems = _context.OrderItem.Where(p => p.OrderId == orderId).ToList();
            List<CartItem> orderItems = new List<CartItem>();
            foreach (var orderitem in OrderItems)
            {
                CartItem item = new CartItem(orderitem);
                item.Product = _context.Product.Single(x => x.Id == orderitem.ProductId);
                orderItems.Add(item);
            }

            return orderItems;
        }

        private bool GetEmailConfirmed()
        {
            return _userManager.GetUserAsync(User).Result.EmailConfirmed;
        }
    }
}
