using Microsoft.AspNetCore.Mvc.Filters;

namespace RestAPI.Filter
{
    public class HeaderFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context) { }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var responseHeaders = context.HttpContext.Response.Headers;

            responseHeaders.Add("Server", "MasteryPointsStats");
        }
    }
}
