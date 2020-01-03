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
using Microsoft.AspNetCore.ResponseCompression;
using Raven.Client.Documents;
using Raven.StructuredLog;
using Raven.Client.Http;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Microsoft.AspNetCore.DataProtection;

namespace TurbineJobMVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment host)
        {
            Configuration = configuration;
            hostEnvironment = host;
        }

        public IConfiguration Configuration { get; }
        private IHostEnvironment hostEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDetection();
            services.AddDataProtection();
            services.AddAutoMapper(typeof(PCStockDBMappingProfiles));
            services
                .AddEntityFrameworkSqlServer()
                .AddDbContext<PCStockDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("PCStockDBConnectionString")))
                .AddUnitOfWork<PCStockDBContext>();
            services.AddScoped<IService, Service>();
            services.AddSession();
            services.AddDNTCaptcha(options => options.UseCookieStorageProvider());
            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = System.IO.Compression.CompressionLevel.Optimal;
            });

            services.AddResponseCompression(action =>
            {
                action.EnableForHttps = true;
                action.Providers.Add<BrotliCompressionProvider>();
            });
            if (Convert.ToBoolean(Configuration.GetSection("RavenDBSettings:Enabled").Value))
                services.AddLogging(builder => builder.AddRavenStructuredLogger(this.CreateRavenDocStore()));
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
            app.UseSession();
            app.UseResponseCompression();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private IDocumentStore CreateRavenDocStore()
        {
            RequestExecutor.RemoteCertificateValidationCallback += CertificateCallback;
            var docStore = new DocumentStore
            {
                Urls = new[] { Configuration.GetSection("RavenDBSettings:Server").Value },
                Database = Configuration.GetSection("RavenDBSettings:CollectionName").Value,
                Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2($"{hostEnvironment.ContentRootPath}/wwwroot/certificate/free.aiki.client.certificate.with.password.pfx", "D7511C44414CAA552B425F39DAE8CA6")
            };
            docStore.Initialize();
            return docStore;
        }

        private bool CertificateCallback(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
    }
}
