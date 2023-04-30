using ManTrap.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace ManTrap.ViewComponents
{
    public class UserButton : ViewComponent
    {
        [HttpGet]
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("_UserButton");
        }
    }
}
