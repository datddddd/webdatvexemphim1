using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ck.Data;
using ck.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ck.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ckContext _context;

        public BookingsController(ckContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            var userId = GetLoggedInUserId();
            var claims = User.Claims.ToList();
            foreach (var claim in claims)
            {
                System.Diagnostics.Debug.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
            }
            if (request == null || request.SelectedSeats == null || !request.SelectedSeats.Any())
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });

            var unavailableSeats = await _context.Ticket
                .Where(t => request.SelectedSeats.Contains(t.SeatId) && t.ShowtimeId == request.ShowtimeId)
                .Select(t => t.SeatId)
                .ToListAsync();

            if (unavailableSeats.Any())
            {
                return Json(new
                {
                    success = false,
                    message = "Một số ghế đã có người đặt: " + string.Join(", ", unavailableSeats)
                });
            }

            var showtime = await _context.Showtime.FindAsync(request.ShowtimeId);
            if (showtime == null)
            {
                return Json(new { success = false, message = "Không tìm thấy suất chiếu." });
            }

            var bookingDate = DateTime.UtcNow;
            var seatPrice = showtime.Price; // Lấy giá từ suất chiếu

            _context.Database.BeginTransaction(); // Bắt đầu transaction để đảm bảo tính toàn vẹn

            try
            {
                foreach (var seatId in request.SelectedSeats)
                {
                    var ticket = new Ticket
                    {
                        SeatId = seatId,
                        ShowtimeId = request.ShowtimeId,
                        UserId = userId,
                        BookingDate = bookingDate,
                        IsPaid = false,
                        Price = seatPrice
                    };
                    _context.Ticket.Add(ticket);

                    var seat = await _context.Seat.FindAsync(seatId);
                    if (seat != null)
                    {
                        seat.IsAvailable = false;
                    }
                }

                await _context.SaveChangesAsync();
                _context.Database.CommitTransaction(); // Commit transaction khi thành công

                var ticketIds = await _context.Ticket
                    .Where(t => t.UserId == userId && t.BookingDate == bookingDate)
                    .Select(t => t.Id)
                    .ToListAsync();

                return Json(new { success = true, ticketIds = string.Join(",", ticketIds) }); // Trả về JSON thành công với ticketIds
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction(); // Rollback transaction nếu có lỗi
                return Json(new { success = false, message = "Đã xảy ra lỗi khi đặt vé: " + ex.Message });
            }
        }


        private int GetLoggedInUserId()
        {
            if (!User.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");
            }

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                throw new InvalidOperationException("Không tìm thấy ID người dùng trong token.");
            }

            if (!int.TryParse(userIdString, out int userId))
            {
                throw new InvalidOperationException("ID người dùng không hợp lệ.");
            }

            return userId;
        }
    }
}
