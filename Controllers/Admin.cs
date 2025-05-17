using ck.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Add this using directive

namespace ck.Controllers
{
    public class Admin : Controller
    {
        private readonly ckContext _context;

        public Admin(ckContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            // Tổng số phim
            ViewBag.TotalMovies = _context.Movie.Count();

            // Tổng số người dùng
            ViewBag.TotalUsers = _context.User.Count();

            // Tổng doanh thu
            ViewBag.TotalEarnings = _context.Ticket
                .Where(t => t.IsPaid)
                .Sum(t => (decimal?)t.Price) ?? 0;

            // Phim bán chạy nhất
            ViewBag.BestSellingMovie = _context.Ticket
                .Where(t => t.IsPaid)
                .Include(t => t.Showtime).ThenInclude(s => s.Movie) // No changes here
                .GroupBy(t => t.Showtime.Movie.MovieName)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? "Không có dữ liệu";

            // Doanh thu theo tháng
            var revenueByMonth = _context.Ticket
                .Where(t => t.IsPaid && t.BookingDate != null)
                .GroupBy(t => t.BookingDate.Month)
                .Select(g => new { Month = g.Key, Total = g.Sum(t => t.Price) })
                .OrderBy(g => g.Month)
                .ToList();

            ViewBag.Months = revenueByMonth.Select(x => $"Tháng {x.Month}").ToList();
            ViewBag.MonthlyRevenue = revenueByMonth.Select(x => x.Total).ToList();

            return View();
        }
    }
}
