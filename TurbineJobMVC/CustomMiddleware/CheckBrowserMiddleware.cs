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
        private RequestDelegate next;
        private readonly string[] CompatibleBrowsers = { "Chrome", "Firefox", "Edge", "Safari" };
        public CheckBrowserMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext,
                                  IDetection detection)
        {
            if (!CompatibleBrowsers.Contains(detection.Browser.Type.ToString()))
            {
                httpContext.Response.Redirect("/InCompatibleBrowser.html");
            }
            else
            {
                await this.next.Invoke(httpContext);
            }
        }
    }
}
