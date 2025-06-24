using Microsoft.AspNetCore.Mvc.Filters;

namespace eAgenda.WebApp.ActionFilters;

public class ExemploAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Lógica ANTES da execução do método de ação
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        // Lógica APÓS a execução do método de ação
    }

    public override void OnResultExecuting(ResultExecutingContext context)
    {
        // Lógica ANTES do resultado da view ser gerado (ex: return View())
    }
    public override void OnResultExecuted(ResultExecutedContext context)
    {
        // Lógica APÓS do resultado da view ser gerado (ex: return View())
    }
}
