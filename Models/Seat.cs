using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ck.Models
{
    [Table("Seat")] // Đặt tên bảng trong cơ sở dữ liệu là "Seat"
    public class Seat
    {
        [Key] // Đánh dấu thuộc tính Id là khóa chính
        public int Id { get; set; } // Khóa chính của ghế
        public int SeatNumber { get; set; } // Số ghế
        public bool IsAvailable { get; set; } // Trạng thái ghế (có sẵn / đã đặt)
        [ForeignKey("Showtime")] // Đánh dấu thuộc tính ShowtimeId là khóa ngoại liên kết đến bảng Showtime
        public int ShowtimeId { get; set; } // Khóa ngoại liên kết đến suất chiếu
        // Thuộc tính Navigation để truy cập thông tin suất chiếu liên quan
        public virtual Showtime? Showtime { get; set; }
        public virtual ICollection<Ticket>? Tickets { get; set; }
    }
    
}
