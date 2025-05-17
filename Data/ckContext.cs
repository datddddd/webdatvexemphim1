using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ck.Models;

namespace ck.Data
{
    public class ckContext : DbContext
    {
        public ckContext (DbContextOptions<ckContext> options)
            : base(options)
        {
        }

        public DbSet<ck.Models.Movie> Movie { get; set; } = default!;
        public DbSet<ck.Models.Genre> Genre { get; set; } = default!;
        public DbSet<ck.Models.User> User { get; set; } = default!;
        public DbSet<ck.Models.Showtime> Showtime { get; set; } = default!;
        public DbSet<ck.Models.Seat> Seat { get; set; } = default!;
        public DbSet<ck.Models.Ticket> Ticket { get; set; } = default!;


    }

}
