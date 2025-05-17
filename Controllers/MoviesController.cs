using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ck.Data;
using ck.Models;
using Microsoft.AspNetCore.Authorization;

namespace ck.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ckContext _context;

        public MoviesController(ckContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Genre(int id)
        {
            var moviesByGenre = await _context.Movie
                .Include(m => m.genre)
                .Where(m => m.GenreId == id)
                .ToListAsync();

            var genreName = await _context.Genre
                .Where(g => g.Id == id)
                .Select(g => g.Name)
                .FirstOrDefaultAsync();

            ViewBag.GenreName = genreName;
            return View("MoviesByGenre", moviesByGenre);
        }
        [HttpGet]
        public async Task<IActionResult> Search(string? query)
        {
            if (string.IsNullOrEmpty(query))
            {
                // Nếu không nhập gì thì trả lại toàn bộ danh sách
                var allMovies = await _context.Movie.Include(m => m.genre).ToListAsync();
                return View("Index", allMovies);
            }

            var result = await _context.Movie
                .Include(m => m.genre)
                .Where(m => m.MovieName != null && m.MovieName.Contains(query))
                .ToListAsync();

            return View("Index", result); // Dùng lại view hiển thị danh sách phim
        }

        [HttpGet]
        public async Task<IActionResult> Search1(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                // Nếu không nhập gì thì trả lại toàn bộ danh sách
                var allMovies = await _context.Movie.Include(m => m.genre).ToListAsync();
                return View("Index", allMovies);
            }

            var result = await _context.Movie
                .Include(m => m.genre)
                .Where(m => m.MovieName != null && m.MovieName.Contains(query))
                .ToListAsync();

            return View("IndexAdmin", result); // Dùng lại view hiển thị danh sách phim
        }

        // GET: All Movies
        public async Task<IActionResult> Index()
        {
            var ckContext = _context.Movie
                .Include(m => m.genre);

            return View(await ckContext.ToListAsync());
        }
        public async Task<IActionResult> IndexAdmin()
        {
            var ckContext = _context.Movie
                .Include(m => m.genre);

            return View(await ckContext.ToListAsync());
        }
        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .Include(m => m.genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }
        public async Task<IActionResult> DetailsAdmin(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .Include(m => m.genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View("DetailAdmin", movie);
        }
        // GET: Movies/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "Id", "Name");
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,MovieName,Capacity,Status,MovieImage,CreateAt,GenreId,Descripton,MovieImage2,Director,Video")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                movie.CreateAt = DateTime.UtcNow;

                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexAdmin));
            }
            ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "Id", "Name", movie.GenreId);
            return View( movie);
        }

        // GET: Movies/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "Id", "Name", movie.GenreId);
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MovieName,Capacity,Status,MovieImage,CreateAt,GenreId,Descripton,MovieImage2,Director,Video")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("IndexAdmin");
            }
            ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "Id", "Name", movie.GenreId);
            return View(movie);
        }

        // GET: Movies/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .Include(m => m.genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("IndexAdmin");
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
        public IActionResult GetTicket(int id)
        {
            // Lấy danh sách lịch chiếu theo MovieId
            var showtimes = _context.Showtime
                .Where(s => s.MovieId == id)
                .ToList();

            // Truyền dữ liệu showtimes sang view
            return View("ChonShowtime", showtimes); // View tên khác tùy bạn đặt
        }


    }
}
