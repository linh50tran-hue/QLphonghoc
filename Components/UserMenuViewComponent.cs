using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using aznews.Models;

namespace aznews.Components
{
    [ViewComponent(Name = "UserMenu")]
    public class UserMenuViewComponent : ViewComponent
    {
        private readonly DataContext _context;

        public UserMenuViewComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var mnList = (from mn in _context.UserMenus
                          where mn.IsActive == true
                          orderby mn.ItemOrder
                          select mn).ToList();

            return await Task.FromResult((IViewComponentResult)View("Default", mnList));
        }
    }
}
