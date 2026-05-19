using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Futelo.Server.Filters;

public class ApiExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = context.Exception switch
        {
            KeyNotFoundException      => new NotFoundResult(),
            UnauthorizedAccessException => new ForbidResult(),
            InvalidOperationException ex => new BadRequestObjectResult(ex.Message),
            _ => null
        };

        if (context.Result != null)
            context.ExceptionHandled = true;
    }
}
