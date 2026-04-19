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

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<BookStoreDbContext>()
                .AddDefaultTokenProviders();

            // DI Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAboutService, AboutService>();
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IHomeSliderService, HomeSliderService>();

            builder.Services.AddScoped<IBookTagService, BookTagService>();
            builder.Services.AddScoped<IShipperService, ShipperService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICartService, CartService>();


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
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roleNames = ["Customer", "Admin", "Staff", "Manager", "Shipper"];
                foreach (var roleName in roleNames)
                {
                    if (!roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                        roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }

                SeedUsers(userManager, roleManager);
            }

            app.Run();
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Admin
            var adminEmail = "admin@test.com";
            var adminPassword = "Admin@123";
            // Manager
            var managerEmail = "manager@bookstore.com";
            var managerPassword = "Manager@123";
            // Staff
            var staffEmail = "staff@bookstore.com";
            var staffPassword = "Staff@123";
            // Customer
            var customerEmail = "customer@bookstore.com";
            var customerPassword = "Customer@123";
            // Shipper
            var shipperEmail = "shipper@bookstore.com";
            var shipperPassword = "Shipper@123";

            if (userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult() == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    Name = "Administrator",
                    Status = true,
                    CreatedAt = DateTime.Now
                };

                var result = userManager.CreateAsync(adminUser, adminPassword).GetAwaiter().GetResult();
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(adminUser, "Admin").GetAwaiter().GetResult();
                }
            }

            if (userManager.FindByEmailAsync(managerEmail).GetAwaiter().GetResult() == null)
            {
                var managerUser = new ApplicationUser
                {
                    UserName = "manager",
                    Email = managerEmail,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    Name = "Manager",
                    Status = true,
                    CreatedAt = DateTime.Now
                };

                var result = userManager.CreateAsync(managerUser, managerPassword).GetAwaiter().GetResult();
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(managerUser, "Manager").GetAwaiter().GetResult();
                }
            }

            if (userManager.FindByEmailAsync(staffEmail).GetAwaiter().GetResult() == null)
            {
                var staffUser = new ApplicationUser
                {
                    UserName = "staff",
                    Email = staffEmail,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    Name = "Staff",
                    Status = true,
                    CreatedAt = DateTime.Now
                };

                var result = userManager.CreateAsync(staffUser, staffPassword).GetAwaiter().GetResult();
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(staffUser, "Staff").GetAwaiter().GetResult();
                }
            }

            if (userManager.FindByEmailAsync(customerEmail).GetAwaiter().GetResult() == null)
            {
                var customerUser = new ApplicationUser
                {
                    UserName = "customer",
                    Email = customerEmail,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    Name = "Customer",
                    Status = true,
                    CreatedAt = DateTime.Now
                };

                var result = userManager.CreateAsync(customerUser, customerPassword).GetAwaiter().GetResult();
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(customerUser, "Customer").GetAwaiter().GetResult();
                }
            }

            if (userManager.FindByEmailAsync(shipperEmail).GetAwaiter().GetResult() == null)
            {
                var shipperUser = new ApplicationUser
                {
                    UserName = "shipper",
                    Email = shipperEmail,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    Name = "Shipper",
                    Status = true,
                    CreatedAt = DateTime.Now
                };

                var result = userManager.CreateAsync(shipperUser, shipperPassword).GetAwaiter().GetResult();
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(shipperUser, "Shipper").GetAwaiter().GetResult();
                }
            }

        }
    }
}