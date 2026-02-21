using FinTrack.Infrastructure.Data;
using FinTrack.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FinTrack.Core.Interfaces;
using FinTrack.Core.Interfaces.Services;
using FinTrack.Core.Services;
using FinTrack.Infrastructure.Extensions;
using FinTrack.Infrastructure.Services;
using System.Globalization;

namespace FinTrack.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var culture = new CultureInfo("en-US"); // Usa $ como símbolo
            culture.NumberFormat.CurrencySymbol = "$";

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            builder.Services.AddInfrastructureServices(builder.Configuration);


            // Identity with custom ApplicationUser and password settings
            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false; 
                options.Password.RequireDigit = false;         
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Add services for controllers and views
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddScoped<IFinancialService, FinancialService>();



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Dashboard}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
