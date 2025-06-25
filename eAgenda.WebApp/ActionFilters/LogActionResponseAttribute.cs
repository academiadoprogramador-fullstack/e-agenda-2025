using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApp.ActionFilters;

public class LogActionResponseAttribute : ActionFilterAttribute
{
    private readonly ILogger<LogActionResponseAttribute> _logger;

    public LogActionResponseAttribute(ILogger<LogActionResponseAttribute> logger)
    {
        _logger = logger;
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        var result = context.Result;

        if (result is ViewResult viewResult && viewResult.Model is not null)
            _logger.LogInformation("Ação executada com sucesso! {@Modelo}", viewResult.Model);

        base.OnActionExecuted(context);
    }
}