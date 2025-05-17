using ck.Data;
using Microsoft.AspNetCore.Mvc;

namespace ck.Models
{
    public class NavBar:ViewComponent
    {
        private readonly ckContext _context;

        public NavBar(ckContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            return View(_context.Genre.ToList());
        }
    }
}
