using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ck.Models
{
    [Table("Genres")]
    public class Genre
    {
        [Key]
        public int Id { get; set; }
        public string ?Name { get; set; }
    }
}
