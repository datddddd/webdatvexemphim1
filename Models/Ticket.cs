using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ck.Models
{
    [Table("Ticket")] // Đặt tên bảng trong cơ sở dữ liệu là "Tickets"
    public class Ticket
    {
        [Key] // Đánh dấu thuộc tính Id là khóa chính
        public int Id { get; set; } // Khóa chính của vé
        [ForeignKey("Seat")] // Đánh dấu thuộc tính SeatId là khóa ngoại liên kết đến bảng Seat
        public int SeatId { get; set; } // Khóa ngoại liên kết đến ghế
        [ForeignKey("Showtime")] // Đánh dấu thuộc tính ShowtimeId là khóa ngoại liên kết đến bảng Showtime
        public int ShowtimeId { get; set; } // Khóa ngoại liên kết đến suất chiếu
        [ForeignKey("User")] // Đánh dấu thuộc tính UserId là khóa ngoại liên kết đến bảng User
        public int UserId { get; set; } // Khóa ngoại liên kết đến người dùng
        public DateTime BookingDate { get; set; } // Ngày đặt vé
        public bool IsPaid { get; set; } // Trạng thái thanh toán
        public decimal Price { get; set; } // Giá vé
        public virtual Seat? Seat { get; set; } // Thuộc tính Navigation để truy cập thông tin ghế liên quan
        public virtual Showtime? Showtime { get; set; } // Thuộc tính Navigation để truy cập thông tin suất chiếu liên quan
        public virtual User? User { get; set; } // Thuộc tính Navigation để truy cập thông tin người dùng liên quan


    }
}
