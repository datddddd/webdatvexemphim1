using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ck.Controllers
{
    public class Pages : Controller
    {
        

        public IActionResult Index()
        {
            return View("Faqs");
        }
        public IActionResult Contactus()
        {
            return View("Contactus");
        }
        public IActionResult About()
        {
            return View("About");
        }
        public IActionResult Terms()
        {
            return View("Terms");
        }
    }
}
