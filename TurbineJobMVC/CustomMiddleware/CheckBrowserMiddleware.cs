using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wangkanai.Detection;

namespace TurbineJobMVC.CustomMiddleware
{
    public class CheckBrowserMiddleware
    {
        private RequestDelegate _next;
        private readonly string[] CompatibleBrowsers = { "Chrome", "Firefox", "Edge", "Safari" };
        public CheckBrowserMiddleware(RequestDelegate next)
        {
            this._next = next;
        }
        public async Task InvokeAsync(HttpContext context,
                                  IDetection detection)
        {
            if (!context.Request.Path.StartsWithSegments("/api"))
            {
                if (!CompatibleBrowsers.Contains(detection.Browser.Type.ToString()))
                {
                    context.Response.Redirect("/InCompatibleBrowser.html");
                }
                else
                {
                    await this._next.Invoke(context);
                }
            }
            else { await this._next.Invoke(context); }
            
        }
    }
}
