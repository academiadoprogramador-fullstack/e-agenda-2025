using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApp.ActionFilters;

public class ValidarModeloAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var modelState = filterContext.ModelState;

        if (!modelState.IsValid)
        {
            var controller = (Controller)filterContext.Controller;

            var viewModel = filterContext.ActionArguments.First().Value;

            filterContext.Result = controller.View(viewModel);
        }
    }
}