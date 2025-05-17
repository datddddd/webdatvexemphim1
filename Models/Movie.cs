namespace ck.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Movie")]
    public class Movie
    {
        [Key]
        public int Id { get; set; } // Khóa chính
        public string? MovieName { get; set; } // Tên phim
       
        [DisplayName("Time")]
        public int Capacity { get; set; } // Số ghế tối đa
        public string? Status { get; set; } // Trạng thái (đang chiếu / ngừng chiếu)
        public string? MovieImage { get; set; } // Ảnh phim
        public DateTime? CreateAt { get; set; } // Ngày tạo

        [ForeignKey("Genre")]
        public int GenreId { get; set; } // Thể loại phim
        public string? Descripton { get; set; }
        public string? MovieImage2 { get; set; }
        public string? Director { get; set; }
        public string? Video { get; set; }
        public virtual Genre? genre { get; set; }


    }


}
