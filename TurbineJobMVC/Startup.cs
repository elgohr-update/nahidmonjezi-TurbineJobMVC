using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TurbineJobMVC.Models;
using Microsoft.EntityFrameworkCore;
using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using TurbineJobMVC.Services;
using DNTCaptcha.Core;

namespace TurbineJobMVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDetection();
            services.AddAutoMapper(typeof(PCStockDBMappingProfiles));
            services
                .AddDbContext<PCStockDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("PCStockDBConnectionString")))
                .AddUnitOfWork<PCStockDBContext>();
            services.AddScoped<IService, Service>();
            services.AddSession();
            services.AddDNTCaptcha(options => options.UseCookieStorageProvider());
            services.AddResponseCompression();
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseResponseCompression();
            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();
            //app.UseStatusCodePages();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
