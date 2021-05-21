using Microsoft.AspNetCore.Mvc;
using SingularitybdWeb.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SingularitybdWeb.ViewComponents
{
	public class NavigationMenuViewComponent : ViewComponent
	{
		private readonly IDataAccessService _dataAccessService;

		public NavigationMenuViewComponent(IDataAccessService dataAccessService)
		{
			_dataAccessService = dataAccessService;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var items = await _dataAccessService.GetMenuItemsAsync(HttpContext.User);

			items = items.Distinct(new ItemEqualityComparer()).ToList();
			return View(items);
		}
	}
}