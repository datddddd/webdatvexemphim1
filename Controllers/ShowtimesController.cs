using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ck.Data;
using ck.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;

namespace ck.Controllers
{
    public class ShowtimesController : Controller
    {
        private readonly ckContext _context;

        public ShowtimesController(ckContext context)
        {
            _context = context;
        }

        // GET: Showtimes
        public async Task<IActionResult> Index()
        {
            var ckContext = _context.Showtime.Include(s => s.Movie);
            return View(await ckContext.ToListAsync());
        }
        // GET: Showtimes
        [Authorize(Roles = "Admin")] // Chỉ Admin mới xem được danh sách suất chiếu
        public async Task<IActionResult> Adminview()
        {
            var showtimes = await _context.Showtime
                .Include(s => s.Movie)
                .ToListAsync();

            return View(showtimes);
        }


        // GET: Showtimes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var showtime = await _context.Showtime
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (showtime == null)
            {
                return NotFound();
            }

            return View(showtime);
        }

        // GET: Showtimes/Create
        [Authorize(Roles ="Admin")] // Yêu cầu đăng nhập để tạo lịch chiếu
        public IActionResult Create()
        {
            ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "MovieName");
            return View(); // hoặc: return View("Create");
        }

        // POST: Showtimes/Create
        [HttpPost]
        [Authorize(Roles ="Admin")] // Yêu cầu đăng nhập để tạo lịch chiếu
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MovieId,Date,ShowTimes,Price,Capacity")] Showtime showtime)
        {
            if (ModelState.IsValid)
            {
                _context.Add(showtime);
                await _context.SaveChangesAsync(); // Save the showtime first

                // Create and add 50 seats for the new showtime
                for (int i = 1; i <= showtime.Capacity; i++)
                {
                    var seat = new Seat
                    {
                        ShowtimeId = showtime.Id,
                        SeatNumber = i,
                        IsAvailable = true
                    };
                    _context.Seat.Add(seat);
                }
                await _context.SaveChangesAsync(); // Save the seats

                return RedirectToAction("Adminview", "Showtimes");
            }
            ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "MovieName", showtime.MovieId);
            return View(showtime);
        }

        // GET: Showtimes/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var showtime = await _context.Showtime.FindAsync(id);
            if (showtime == null)
            {
                return NotFound();
            }

            ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "MovieName", showtime.MovieId);

            return View("Edit", showtime); // hoặc chỉ cần: return View(showtime);
        }


        // POST: Showtimes/Edit/5
        // POST: Showtimes/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MovieId,Date,Price,Capacity")] Showtime showtime)
        {
            // Xử lý trước khi validate
            var showTimesString = Request.Form["ShowTimesString"];
            if (!StringValues.IsNullOrEmpty(showTimesString))
            {
                try
                {
                    showtime.ShowTimes = showTimesString.ToString()
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => TimeSpan.Parse(s.Trim()))
                        .ToList();
                }
                catch
                {
                    ModelState.AddModelError("ShowTimes", "Định dạng giờ không hợp lệ. VD: 09:00, 13:30");
                }
            }

            if (id != showtime.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(showtime);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Adminview", "Showtimes");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShowtimeExists(showtime.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "MovieName", showtime.MovieId);
            return View(showtime);
        }



        // GET: Showtimes/Delete/5
        [Authorize(Roles = "Admin")] // Yêu cầu đăng nhập để xóa lịch chiếu
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var showtime = await _context.Showtime
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (showtime == null)
            {
                return NotFound();
            }

            return View(showtime);
        }

        // POST: Showtimes/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")] // Yêu cầu đăng nhập để xóa lịch chiếu
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var showtime = await _context.Showtime.FindAsync(id);
            if (showtime != null)
            {
                _context.Showtime.Remove(showtime);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Adminview");
        }

        private bool ShowtimeExists(int id)
        {
            return _context.Showtime.Any(e => e.Id == id);
        }

        [Authorize] // Yêu cầu đăng nhập để xem trang đặt vé
        public IActionResult GetTicket(int movieId)
        {
            var showtimes = _context.Showtime
                .Where(s => s.MovieId == movieId)
                .ToList();

            ViewBag.MovieId = movieId;
            return View("Index", showtimes); // Hoặc bạn có thể tạo một View riêng cho việc đặt vé
        }

        [HttpGet]
        public IActionResult GetShowtimesByDate(DateOnly date, int movieId)
        {
            var showtimes = _context.Showtime
                .Where(s => s.Date == date && s.MovieId == movieId)
                .ToList();

            var result = showtimes.Select(s => new
            {
                Date = s.Date.ToString("yyyy-MM-dd"),
                Times = s.ShowTimes?.Select(t => t.ToString(@"hh\:mm")).ToList() ?? new List<string>(),
                Id = s.Id
            }).ToList();

            return Json(result);
        }


        [HttpGet]
        public IActionResult GetAvailableDates(int movieId)
        {
            var dates = _context.Showtime
                .Where(s => s.MovieId == movieId)
                .Select(s => s.Date)
                .Distinct()
                .OrderBy(d => d)
                .Select(d => d.ToString("yyyy-MM-dd")) // Chuyển DateOnly sang string để trả về JSON
                .ToList();

            return Json(dates);
        }

        [HttpGet]
        public IActionResult GetSeats(int showtimeId)
        {
            var showtime = _context.Showtime.FirstOrDefault(s => s.Id == showtimeId); // Lấy thông tin Showtime
            if (showtime == null)
            {
                return NotFound(); // Trả về lỗi nếu không tìm thấy Showtime
            }

            var seats = _context.Seat
                .Where(s => s.ShowtimeId == showtimeId)
                .Select(s => new
                {
                    s.Id,
                    s.SeatNumber,
                    s.IsAvailable
                })
                .OrderBy(s => s.SeatNumber)
                .ToList();

            // Trả về một object chứa cả danh sách ghế và SoLuong
            return Json(new { seats = seats, capacity = showtime.Capacity });
        }
    }
}