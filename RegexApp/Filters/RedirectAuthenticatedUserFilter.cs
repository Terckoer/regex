using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RegexApp.Filters
{
    public class RedirectAuthenticatedUserFilter: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string? cookie = context.HttpContext.Request.Cookies["username"];
            if(cookie != null && cookie != "" && context.HttpContext.User.Identity != null && context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Index", "User", null);
            }

            base.OnActionExecuting(context);
        }

    }
}
