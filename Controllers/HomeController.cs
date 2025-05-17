using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // C?n thi?t ?? dùng Include()
using ck.Models;
using ck.Data;

namespace ck.Controllers
{
    public class HomeController : Controller
    {
        private readonly ckContext _context;

        public HomeController(ckContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // S? d?ng Include() ?? l?y c? Genre
            var movies = _context.Movie.Include(m => m.genre).ToList();
            return View(movies); // Tr? v? movies ?ã Include(Genre)
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
