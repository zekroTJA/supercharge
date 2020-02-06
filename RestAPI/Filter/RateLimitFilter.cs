using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RestAPI.Modules;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;

namespace RestAPI.Filter
{
    public class RateLimitFilter : ActionFilterAttribute
    {

        private readonly ConcurrentDictionary<IPAddress, RateLimiter> limiters;
        private readonly TimeSpan limit;
        private readonly int burst;

        public RateLimitFilter(int limitSeconds = 1, int _burst = 5)
        {
            limit = TimeSpan.FromSeconds(limitSeconds);
            burst = _burst;
            limiters = new ConcurrentDictionary<IPAddress, RateLimiter>();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var remoteAddress = context.HttpContext.Connection.RemoteIpAddress;

            if (remoteAddress == null)
            {
                context.Result = new BadRequestObjectResult(null);
                return;
            }

            RateLimiter limiter;

            if (limiters.ContainsKey(remoteAddress))
            {
                limiter = limiters[remoteAddress];
            }
            else
            {
                limiter = new RateLimiter(limit, burst);
                limiters[remoteAddress] = limiter;
            }

            var reservation = limiter.Reserve();

            AddReservationHeader(context, reservation);

            if (!reservation.Success)
            {
                var result = new ObjectResult(null);
                result.StatusCode = 429;
                context.Result = result;
                return;
            }
        }

        private void AddReservationHeader(ActionContext context, Reservation reservation)
        {
            var headers = context.HttpContext.Response.Headers;

            headers.Add("X-Ratelimit-Limit", reservation.Burst.ToString());
            headers.Add("X-Ratelimit-Remaining", reservation.Remaining.ToString());
            headers.Add("X-Ratelimit-Reset", reservation.Reset.ToString("o"));
        }
    }

}

