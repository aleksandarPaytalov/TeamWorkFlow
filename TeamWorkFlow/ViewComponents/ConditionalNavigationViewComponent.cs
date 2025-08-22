using Microsoft.AspNetCore.Mvc;

namespace TeamWorkFlow.ViewComponents
{
    public class ConditionalNavigationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var currentPath = ViewContext.HttpContext.Request.Path.Value?.ToLower();
            var isAuthPage = IsAuthenticationPage(currentPath);

            var model = new ConditionalNavigationViewModel
            {
                IsAuthenticationPage = isAuthPage,
                ShowBackToHome = isAuthPage,
                ShowMainNavigation = !isAuthPage
            };

            return View(model);
        }
        
        private static bool IsAuthenticationPage(string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
                
            return path.Contains("/identity/account/login") || 
                   path.Contains("/identity/account/register") ||
                   path.Contains("/account/login") ||
                   path.Contains("/account/register");
        }
    }
    
    public class ConditionalNavigationViewModel
    {
        public bool IsAuthenticationPage { get; set; }
        public bool ShowBackToHome { get; set; }
        public bool ShowMainNavigation { get; set; }
    }
}
