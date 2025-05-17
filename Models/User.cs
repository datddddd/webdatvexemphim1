using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ck.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get;set; }
        public string? Username { get;set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Role { get;set; }
    }
}
