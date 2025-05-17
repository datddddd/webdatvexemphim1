using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ck.Data;
using ck.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.AspNetCore.Session;
using System.Text.RegularExpressions;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;

namespace ck.Controllers
{
    public class UsersController : Controller
    {
        private readonly ckContext _context;
        private readonly EmailSettings _emailSettings;

        public UsersController(ckContext context, IOptions<EmailSettings> emailOptions)
        {
            _context = context;
            _emailSettings = emailOptions.Value;
        }
        //search
        [HttpGet]
        public async Task<IActionResult> Search1(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                // Nếu không nhập gì thì trả lại toàn bộ danh sách người dùng
                var allUsers = await _context.User.ToListAsync();
                return View("Index", allUsers); // Giả sử bạn có view Index để hiển thị người dùng
            }

            var result = await _context.User
     .Where(u => (u.Name != null && u.Name.Contains(query)) ||
                 (u.Username != null && u.Username.Contains(query)) ||
                 (u.Email != null && u.Email.Contains(query))) // Tìm kiếm theo tên, username, email
     .ToListAsync();


            return View("Index", result); // Dùng lại view hiển thị danh sách người dùng
        }
        // Chỉ Admin mới xem được danh sách
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, bool RememberMe)
        {
            var user = _context.User.FirstOrDefault(u => u.Username == username); // Lấy user theo username

            if (user == null)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
                return View();
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password)) // Xác thực mật khẩu
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
                return View();
            }

            // Tạo claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Name ?? ""),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Role, user.Role ?? "")
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // ✅ Set IsPersistent dựa trên checkbox RememberMe
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = RememberMe,
                ExpiresUtc = RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(7) // Lưu cookie 7 ngày nếu RememberMe là true
                    : DateTimeOffset.UtcNow.AddMinutes(30) // Ngược lại, chỉ 30 phút
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties
            );

            HttpContext.Session.SetString("Username", user.Username ?? string.Empty);
            // ✅ Lưu UserId vào Session
            HttpContext.Session.SetInt32("UserId", user.Id);
            TempData["SuccessMessage"] = "Đăng nhập thành công!";
            return RedirectToAction("Index", "Home");
        }

        //Login bằng Google
        [HttpGet]
        public IActionResult ExternalLogin(string provider, string returnUrl = null )
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Users", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);
        }

        // Callback sau khi đăng nhập thành công
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return RedirectToAction(nameof(Login));
            }

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (email == null)
            {
                TempData["Error"] = "Không thể xác thực người dùng qua Google.";
                return RedirectToAction("Login");
            }

            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                user = new User
                {
                    Name = name,
                    Email = email,
                    Username = email,
                    Role = "User"
                };
                _context.User.Add(user);
                await _context.SaveChangesAsync();
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Name ?? ""),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email ?? ""),
        new Claim(ClaimTypes.Role, user.Role ?? "")
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            HttpContext.Session.SetString("Username", user.Username ?? string.Empty);
            HttpContext.Session.SetInt32("UserId", user.Id);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() => View();


        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                var existingEmail = _context.User.FirstOrDefault(u => u.Email == user.Email);
                var existingUser = _context.User.FirstOrDefault(u => u.Username == user.Username);

                if (existingUser != null)
                {
                    ViewBag.Error = "Username đã tồn tại. Vui lòng chọn username khác";
                    return View(user);
                }

                if (existingEmail != null)
                {
                    ViewBag.Error = "Email đã tồn tại. Vui lòng chọn email khác";
                    return View(user);
                }

                // Kiểm tra độ mạnh mật khẩu
                var password = user.Password;
                // Ensure the password is not null before performing Regex checks
                if (string.IsNullOrEmpty(password))
                {
                    ViewBag.Error = "Mật khẩu không được để trống.";
                    return View(user);
                }

                var hasUpperCase = Regex.IsMatch(password, "[A-Z]");
                var hasSpecialChar = Regex.IsMatch(password, @"[!@#$%^&*(),.?""{}|<>]");
                var hasMinimumLength = password.Length >= 6;


                if (!hasUpperCase || !hasSpecialChar || !hasMinimumLength)
                {
                    ViewBag.Error = "Mật khẩu phải có ít nhất 6 ký tự, bao gồm chữ in hoa và ký tự đặc biệt.";
                    return View(user);
                }

                try
                {
                    user.Role = "User";
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    _context.User.Add(user);
                    _context.SaveChanges();

                    // ✅ Gửi email thông báo
                    string body = $@"
Xin chào {user.Name},

Bạn đã đăng ký tài khoản thành công tại Movie!

Chúng tôi rất vui khi có bạn đồng hành cùng cộng đồng yêu điện ảnh.

Nếu bạn không thực hiện hành động này, vui lòng bỏ qua email này.

Trân trọng,
Movie Team";

                    await SendEmailAsync(user.Email ?? string.Empty, "Đăng ký thành công tại MovieTheme", body);

                    TempData["SuccessMessage"] = "Đăng ký thành công!";
                    return RedirectToAction("Login");
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi đăng ký. Vui lòng thử lại sau.");
                }
            }

            return View(user);
        }


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = _context.User.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Message = "Email không tồn tại trong hệ thống.";
                return View();
            }

            // Tạo link reset password giả lập (thực tế bạn nên tạo token và gửi link chứa token)
            var resetLink = Url.Action("ResetPassword", "Users", new { email = user.Email }, Request.Scheme);

            // Gửi email
            var message = $"Click vào link sau để đặt lại mật khẩu: {resetLink}";

            await SendEmailAsync(email, "Reset your password", message);

            ViewBag.Message = "Email đặt lại mật khẩu đã được gửi.";

            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string email, string newPassword)
        {
            var user = _context.User.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Message = "Email không tồn tại.";
                return View();
            }

            // Kiểm tra độ mạnh mật khẩu (giống như hàm Register)
            var hasUpperCase = Regex.IsMatch(newPassword, "[A-Z]");
            var hasSpecialChar = Regex.IsMatch(newPassword, @"[!@#$%^&*(),.?""{}|<>]");
            var hasMinimumLength = newPassword.Length >= 6;

            if (!hasUpperCase || !hasSpecialChar || !hasMinimumLength)
            {
                ViewBag.Message = "Mật khẩu phải có ít nhất 6 ký tự, bao gồm chữ in hoa và ký tự đặc biệt.";
                return View();
            }

            // Hash lại mật khẩu mới
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            _context.SaveChanges();

            ViewBag.Message = "Đổi mật khẩu thành công!";
            return View();
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Username,Password,Email,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.Role = "User";
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.User.FindAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Username,Password,Email,Role")] User user)
        {
            if (id != user.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.User.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
                    user.Role = existingUser?.Role;
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password); // Cập nhật lại mật khẩu nếu đổi
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user != null) _context.User.Remove(user);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id) => _context.User.Any(e => e.Id == id);
        public IActionResult Profile()
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null) return RedirectToAction("Login");

            var user = _context.User.FirstOrDefault(u => u.Username == username);
            return View(user);
        }

        [HttpPost]
        public IActionResult Profile(User updatedUser)
        {
            if (!ModelState.IsValid)
            {
                return View(updatedUser);
            }

            var user = _context.User.Find(updatedUser.Id);
            if (user == null) return NotFound();

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;

            if (!string.IsNullOrEmpty(updatedUser.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);
            }

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            HttpContext.Session.SetString("Username", updatedUser.Username ?? string.Empty);
            return RedirectToAction("Profile");
        }


        [HttpPost]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var user = _context.User.Find(id);
            if (user != null)
            {
                _context.User.Remove(user);
                _context.SaveChanges();
                HttpContext.Session.Clear();
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            return RedirectToAction("Login");
        }
        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailSettings.FromEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = body
            };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.FromEmail, _emailSettings.AppPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi gửi email: " + ex.Message);
                
            }
        }


    }
}
