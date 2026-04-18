using BookStore.Data;
using BookStore.Models;
using BookStore.Service.Implements;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
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
            builder.Services.AddScoped<IRoleService, RoleService>();

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
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                SeedRoles(roleManager);
                SeedUsers(userManager);
            }

                SeedUsers(userManager, roleManager);

                var db = scope.ServiceProvider.GetRequiredService<BookStoreDbContext>();
                SeedData.SeedCatalog(db);
            }

            app.Run();
        }

        private static void SeedRoles(RoleManager<ApplicationRole> roleManager)
        {
            string[] roleNames = ["Customer", "Admin", "Staff", "Manager", "Shipper"];

            foreach (var roleName in roleNames)
            {
                if (!roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = roleName,
                        IsSystemRole = roleName == "Admin",
                        Status = true,
                        CreatedDate = DateTime.Now
                    }).GetAwaiter().GetResult();
                }
                else
                {
                    // Ensure existing roles (created before migration) have correct values
                    var existing = roleManager.FindByNameAsync(roleName).GetAwaiter().GetResult();
                    if (existing != null)
                    {
                        existing.Status = true;
                        existing.IsSystemRole = roleName == "Admin";
                        if (existing.CreatedDate == default)
                            existing.CreatedDate = DateTime.Now;
                        roleManager.UpdateAsync(existing).GetAwaiter().GetResult();
                    }
                }
            }
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            var seedUsers = new[]
            {
                (UserName: "admin",    Email: "admin@test.com",          Name: "Administrator", Password: "Admin@123",    Role: "Admin"),
                (UserName: "manager",  Email: "manager@test.com",   Name: "Manager",       Password: "Manager@123",  Role: "Manager"),
                (UserName: "staff",    Email: "staff@test.com",     Name: "Staff",         Password: "Staff@123",    Role: "Staff"),
                (UserName: "customer", Email: "customer@test.com",  Name: "Customer",      Password: "Customer@123", Role: "Customer"),
                (UserName: "shipper",  Email: "shipper@test.com",   Name: "Shipper",       Password: "Shipper@123",  Role: "Shipper"),
            };

            foreach (var seed in seedUsers)
            {
                if (userManager.FindByEmailAsync(seed.Email).GetAwaiter().GetResult() != null) continue;

                var user = new ApplicationUser
                {
                    UserName = seed.UserName,
                    Email = seed.Email,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    Name = seed.Name,
                    Status = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var result = userManager.CreateAsync(user, seed.Password).GetAwaiter().GetResult();
                if (result.Succeeded)
                    userManager.AddToRoleAsync(user, seed.Role).GetAwaiter().GetResult();
            }
        }
    }
}