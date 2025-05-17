using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ck.Models
{
    [Table("Showtime")]
    public class Showtime
    {
        [Key]
        public int Id { get; set; } // Khóa chính của Showtime

        [ForeignKey("Movie")] // Thiết lập khóa ngoại liên kết đến Movie
        public int MovieId { get; set; }

        public DateOnly Date { get; set; } // Ngày chiếu

        public List<TimeSpan> ?ShowTimes { get; set; }

        // Các thuộc tính khác của suất chiếu (ví dụ: phòng chiếu, giá vé riêng nếu cần)
        public decimal Price { get; set; }
        public int? Capacity { get; set; } // Số ghế tối đa
        // Thuộc tính Navigation để truy cập thông tin Movie liên quan
        public virtual Movie? Movie { get; set; }
    }
}
