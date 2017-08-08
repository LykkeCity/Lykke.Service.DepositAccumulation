using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Common;
using Common.Log;

namespace Lykke.Service.Limitations.Web
{
    public class GlobalErrorHandler
    {
        private readonly ILog _log;
        private readonly RequestDelegate _next;

        public GlobalErrorHandler(RequestDelegate next, ILog log)
        {
            _log = log;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await LogError(context, ex);

                await SendError(context);
            }
        }

        private async Task LogError(HttpContext context, Exception ex)
        {
            using (var ms = new MemoryStream())
            {
                context.Request.Body.CopyTo(ms);

                ms.Seek(0, SeekOrigin.Begin);

                await _log.LogPartFromStream(ms, nameof(Limitations), context.Request.GetUri().AbsoluteUri, ex);
            }
        }

        private static async Task SendError(HttpContext ctx)
        {
            ctx.Response.ContentType = "application/json";
            ctx.Response.StatusCode = 500;

            var response = new 
            {
                Code = "RuntimeProblem",
                Message = "Technical problems"
            };

            await ctx.Response.WriteAsync(response.ToJson());
        }
    }
}
