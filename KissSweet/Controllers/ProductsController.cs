using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KissSweet.Data;
using KissSweet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.IO;

namespace KissSweet.Controllers
{
    //[Authorize(Roles = "Administrator")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public IActionResult Index(int? cId)
        {
            List<DetailViewModel> dvm = new List<DetailViewModel>();
            List<Product> products = new List<Product>();

            //判斷如果有傳入類別編號，就篩選那個類別的商品出來
            try
            {
                if (cId != null)
                {
                    var result = _context.Category.Single(x => x.Id.Equals(cId));
                    products = _context.Entry(result).Collection(x => x.Products).Query().ToList();
                }
                else
                {
                    products = _context.Product.Include(p => p.Category).ToList();
                }
            }
            catch(System.InvalidOperationException e)
            {
                products = _context.Product.Include(p => p.Category).ToList();
            }
            //把取出來的資料加入ViewModel  
            foreach (var product in products)
            {
                DetailViewModel item = new DetailViewModel
                {
                    product = product,
                    imgsrc = ViewImage(product.Image)
                };
                dvm.Add(item);
            }
            ViewBag.count = dvm.Count;

            return View(dvm);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DetailViewModel dvm = new DetailViewModel();
            var product = await _context.Product
                .Include(p => p.Category)
                .Include(c => c.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            else
            {
                dvm.product = product;
                if (product.Image != null)
                {
                    dvm.imgsrc = ViewImage(product.Image);
                }
            }
            return View(dvm);
        }

        private string ViewImage(byte[] arrayImage)
        {
            // 二進位圖檔轉字串
            if (arrayImage != null)
            {
                string base64String = Convert.ToBase64String(arrayImage, 0, arrayImage.Length);
                return "data:image/png;base64," + base64String;
            }
            else
            {
                return "";
            }
        }

        [HttpPost]
        [Authorize]  //一定要登入才能留言
        public async Task<IActionResult> AddComment(int Id, string myComment)
        {
            var comment = new Comment()
            {
                ProductID = Id,
                Content = myComment,
                UserName = HttpContext.User.Identity.Name,  //取得登入中的帳號
                Time = DateTime.Now  //取得當下時間
            };
            _context.Add(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = Id });
        }
    }
}
