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
using System.Security.Claims;

namespace ck.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ckContext _context;

        public TicketsController(ckContext context)
        {
            _context = context;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            var ckContext = _context.Ticket
                .Include(t => t.Seat)
                .Include(t => t.Showtime)
                .ThenInclude(s => s.Movie)
                .Include(t => t.User);
            return View(await ckContext.ToListAsync());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket
                .Include(t => t.Seat)
                .Include(t => t.Showtime)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["SeatId"] = new SelectList(_context.Seat, "Id", "Id");
            ViewData["ShowtimeId"] = new SelectList(_context.Showtime, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SeatId,ShowtimeId,UserId,BookingDate,IsPaid,Price")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SeatId"] = new SelectList(_context.Seat, "Id", "Id", ticket.SeatId);
            ViewData["ShowtimeId"] = new SelectList(_context.Showtime, "Id", "Id", ticket.ShowtimeId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", ticket.UserId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["SeatId"] = new SelectList(_context.Seat, "Id", "Id", ticket.SeatId);
            ViewData["ShowtimeId"] = new SelectList(_context.Showtime, "Id", "Id", ticket.ShowtimeId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", ticket.UserId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SeatId,ShowtimeId,UserId,BookingDate,IsPaid,Price")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SeatId"] = new SelectList(_context.Seat, "Id", "Id", ticket.SeatId);
            ViewData["ShowtimeId"] = new SelectList(_context.Showtime, "Id", "Id", ticket.ShowtimeId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", ticket.UserId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket
                .Include(t => t.Seat)
                .Include(t => t.Showtime)
                .ThenInclude(s => s.Movie) // nếu có quan hệ với phim
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket != null)
            {
                // cập nhật lại ghế nếu cần
                var seat = await _context.Seat.FindAsync(ticket.SeatId);
                if (seat != null)
                {
                    seat.IsAvailable = true;
                }

                _context.Ticket.Remove(ticket);
                await _context.SaveChangesAsync();
            }

            // nếu đang ở trang MyTickets thì quay về đó
            if (Request.Headers["Referer"].ToString().Contains("MyTickets"))
                return RedirectToAction(nameof(MyTickets));

            return RedirectToAction("Index","Home");
        }



        private bool TicketExists(int id)
        {
            return _context.Ticket.Any(e => e.Id == id);
        }
        [Authorize]
        public async Task<IActionResult> Payment(string ticketIds)
        {
            if (string.IsNullOrEmpty(ticketIds))
                return BadRequest("Không có vé nào để thanh toán.");

            var ids = ticketIds.Split(',').Select(id => int.Parse(id)).ToList();

            var tickets = await _context.Ticket
                .Include(t => t.Seat)
                .Include(t => t.Showtime)
                    .ThenInclude(s => s.Movie) // Thêm dòng này để tải thông tin Movie
                .Include(t => t.User)
                .Where(t => ids.Contains(t.Id))
                .ToListAsync();

            if (!tickets.Any())
                return NotFound("Không tìm thấy vé nào.");

            return View(tickets); // Tìm view tại: Views/Tickets/Payment.cshtml
        }
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(List<int> ticketIds)
        {
            if (ticketIds == null || !ticketIds.Any())
            {
                return BadRequest("No tickets selected.");
            }

            foreach (var ticketId in ticketIds)
            {
                var ticket = await _context.Ticket.FindAsync(ticketId);
                if (ticket != null)
                {
                    ticket.IsPaid = true;
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Home"); // hoặc view khác phù hợp
        }

        [Authorize]
        public async Task<IActionResult> MyTickets()
        {
            // Lấy ID của người dùng hiện tại từ Claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(); // nếu chưa đăng nhập
            }

            int userId = int.Parse(userIdClaim.Value);

            // Lấy danh sách vé của người dùng hiện tại
            var myTickets = await _context.Ticket
                .Include(t => t.Seat)
                .Include(t => t.Showtime)
                 .Include(s => s.Showtime.Movie) // nếu có quan hệ với phim
                .Where(t => t.UserId == userId)
                .ToListAsync();

            return View(myTickets); // Trả về view MyTickets.cshtml
        }
    }
}
