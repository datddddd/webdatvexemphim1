using Microsoft.EntityFrameworkCore;

namespace ck.Models
{
    [Keyless]
    public class BookingRequest
    {
        public int MovieId { get; set; }
        public int ShowtimeId { get; set; }
        public List<int> ?SelectedSeats { get; set; }
    }

}
