using BookStore.Models;
using BookStore.Service.Implements;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<BookStoreDbContext>(opt =>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<BookStoreDbContext>()
                .AddDefaultTokenProviders();

            // Đăng ký Services
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
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
                pattern: "{controller=Home}/{action=Index}/{id?}");

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roleNames = ["Customer", "Admin", "Staff", "Manager"];
                foreach (var roleName in roleNames)
                {
                    if (!roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                        roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
            }

            app.Run();
        }
    }
}