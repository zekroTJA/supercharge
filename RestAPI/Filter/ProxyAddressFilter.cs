using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.Net;

namespace RestAPI.Filter
{
    public class ProxyAddressFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpCtx = context.HttpContext;

            httpCtx.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues headerValues);

            var val = headerValues.FirstOrDefault();
            if (val != null && val != "")
            {
                context.HttpContext.Connection.RemoteIpAddress = IPAddress.Parse(val);
            }
        }
    }
}
