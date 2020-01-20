using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using DNTCaptcha.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raven.Client.Documents;
using Raven.Client.Http;
using Raven.StructuredLog;
using System;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TurbineJobMVC.Models;
using TurbineJobMVC.Services;

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
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(@"DataProtectionKeys/"))
                .SetApplicationName("TurbineJobMVC");
            services.AddAutoMapper(typeof(PCStockDBMappingProfiles));
            services
                .AddDbContext<PCStockDBContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("PCStockDBConnectionString"));
                })
                .AddUnitOfWork<PCStockDBContext>();
            services.AddSession();
            services.AddResponseCaching();
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

            services.AddScoped<IService, Service>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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
            app.UseResponseCaching();
            app.UseStatusCodePagesWithReExecute("/Home/Error{0}");
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
