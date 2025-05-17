using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ck.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using ck.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;

namespace ck
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<ckContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("ckContext") ?? 
                throw new InvalidOperationException("Connection string 'ckContext' not found.")));
            builder.Services.AddDbContext<ckContext>(options =>
                options.UseNpgsql("ckContext")
           .EnableSensitiveDataLogging()
           .EnableDetailedErrors());
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDistributedMemoryCache(); // THÊM DÒNG NÀY TRƯỚC AddSession
            builder.Services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });
            builder.Services.AddSession(); // Bật session
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // thời gian sống của session
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            var configuration = builder.Configuration;

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
             .AddCookie(options =>
             {
                 // Nếu chưa đăng nhập, chuyển về Login
                 options.LoginPath = "/Users/Login";
                 options.AccessDeniedPath = "/Users/AccessDenied";
             })
             .AddGoogle(googleOptions =>
             {
                 googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]
                     ?? throw new InvalidOperationException("Google ClientId is not configured.");
                 googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]
                     ?? throw new InvalidOperationException("Google ClientSecret is not configured.");

                 googleOptions.Events.OnCreatingTicket = async context =>
                {
                    var email = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                    var name = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;


                    // Lấy DbContext từ DI container
                    var db = context.HttpContext.RequestServices.GetRequiredService<ckContext>();

                    // Kiểm tra xem user đã tồn tại chưa
                    var user = await db.User.FirstOrDefaultAsync(u => u.Email == email);
                    if (user == null)
                    {
                        // Tạo user mới nếu chưa tồn tại
                        user = new User
                        {
                            Name = name,
                            Email = email,
                            Role = "user",  // Bạn có thể đặt mặc định là 'user'
                            Username = email, // Có thể dùng email làm Username
                            Password = "", // Không cần mật khẩu khi đăng nhập bằng Google

                        };

                        db.User.Add(user);
                        await db.SaveChangesAsync();
                    }

                    // Lưu thông tin người dùng vào session
                    context.HttpContext.Session.SetString("Username", user.Username ?? string.Empty);
                    context.HttpContext.Session.SetString("Role", user.Role ?? string.Empty);
                    // ✅ Lưu UserId vào Session
                    context.HttpContext.Session.SetInt32("UserId", user.Id);
                    System.Diagnostics.Debug.WriteLine($"user.Id sau khi lấy/tạo: {user.Id}");
                    System.Diagnostics.Debug.WriteLine($"user.Id.ToString(): {user.Id.ToString()}");
                    // Tạo claims và thực hiện đăng nhập
                    var claims = new List<System.Security.Claims.Claim>
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Name ?? string.Empty),
new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString() ?? string.Empty),
new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.Role ?? string.Empty)

                };

                    var identity = new System.Security.Claims.ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new System.Security.Claims.ClaimsPrincipal(identity);

                    await context.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                };
            });


            var app = builder.Build();
            

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseRouting();
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
