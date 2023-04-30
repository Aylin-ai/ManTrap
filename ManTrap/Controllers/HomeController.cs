using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ManTrap.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SelectionFromUser(string selectedValue)
        {
            if (User.Identity.IsAuthenticated)
            {
                switch (selectedValue)
                {
                    case "settings":
                        return RedirectToPage("/UserProfile");
                    case "logout":
                        await HttpContext.SignOutAsync("MyCookieAuthenticationScheme");
                        return RedirectToPage("/Authorization");
                    default:
                        return RedirectToPage("/Error");
                }
            }
            else
                return RedirectToPage("/Authorization");
        }
    }
}
