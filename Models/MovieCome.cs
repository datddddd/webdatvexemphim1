using ck.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ck.Models
{
    public class MovieCome : ViewComponent
    {
        private readonly ckContext _context;

        public MovieCome(ckContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var latestMovies = await _context.Movie
                                             .Include(m => m.genre)
                                             .Where(m=>m.Status=="Coming soon")
                                             .ToListAsync();

            return View(latestMovies);
        }
    }

}
