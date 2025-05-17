using ck.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ck.Models
{
    public class LatestMoviesViewComponent : ViewComponent
    {
        private readonly ckContext _context;

        public LatestMoviesViewComponent(ckContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var latestMovies = await _context.Movie
                                             .Include(m => m.genre)
                                             .OrderByDescending(m => m.CreateAt)
                                             .Take(3)
                                             .ToListAsync();

            return View(latestMovies);
        }
    }

}
