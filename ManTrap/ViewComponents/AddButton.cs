using Microsoft.AspNetCore.Mvc;

namespace ManTrap.ViewComponents
{
    public class AddButton : ViewComponent
    {
        [HttpGet]
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("_AddButton");
        }
    }
}
