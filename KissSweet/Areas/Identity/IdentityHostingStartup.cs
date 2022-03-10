using System;
using KissSweet.Areas.Identity.Data;
using KissSweet.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(KissSweet.Areas.Identity.IdentityHostingStartup))]
namespace KissSweet.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<KissSweetUserContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("KissSweetUserContextConnection")));

                services.AddDefaultIdentity<KissSweetUser>(options =>
                {
                    options.Password.RequiredLength = 4;             //密碼長度
                    options.Password.RequireLowercase = false;       //包含小寫英文
                    options.Password.RequireUppercase = false;       //包含大寫英文
                    options.Password.RequireNonAlphanumeric = false; //包含符號
                    options.Password.RequireDigit = false;           //包含數字
                })
                .AddEntityFrameworkStores<KissSweetUserContext>();
            });
        }
    }
}