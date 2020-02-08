using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurbineJobMVC.CustomMiddleware;

namespace TurbineJobMVC.BuilderExtensions
{
    public static  class BuilderExtensions
    {
        public static IApplicationBuilder UseSitemapMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SitemapMiddleware>();
        }

        public static IApplicationBuilder UseCheckBrowserMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CheckBrowserMiddleware>();
        }
    }
}
