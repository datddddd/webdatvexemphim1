using ck.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ck.Models
{
    public class Movielikethis : ViewComponent
    {
        private readonly ckContext _context;

        public Movielikethis(ckContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(int genreId, int excludeMovieId)
        {
            var movies = _context.Movie
                .Include(m => m.genre)
                .Where(m => m.GenreId == genreId && m.Id != excludeMovieId)
                .Take(4)
                .ToList();

            return View(movies);
        }
    }

}
