using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using DNTCaptcha.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Raven.Client.Documents;
using Raven.Client.Http;
using Raven.StructuredLog;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TurbineJobMVC.CustomMiddleware;
using TurbineJobMVC.Models;
using TurbineJobMVC.Services;
using Wangkanai.Detection;

namespace TurbineJobMVC
{
    public class Startup
    {
        public Startup(
            IConfiguration configuration,
            IHostEnvironment host)
        {
            Configuration = configuration;
            hostEnvironment = host;
        }

        public IConfiguration Configuration { get; }
        private IHostEnvironment hostEnvironment { get; }
        private X509Certificate2 logServerCertificate;

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
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });
            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.AddResponseCompression(action =>
            {
                action.EnableForHttps = true;
                action.Providers.Add<BrotliCompressionProvider>();
                action.Providers.Add<GzipCompressionProvider>();
            });
            if (Convert.ToBoolean(Configuration.GetSection("RavenDBSettings:Enabled").Value))
                services.AddLogging(builder => builder.AddRavenStructuredLogger(this.CreateRavenDocStore()));
            
            services.AddScoped<IWorkOrderService, WorkOrderService>();
            services.AddScoped<IDateTimeService, DateTimeService>();
            services.AddScoped<IService, Service>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddControllersWithViews()
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var mimeTypeProvider = new FileExtensionContentTypeProvider();

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
            app.UseResponseCompression();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 60 * 60 * 24;
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                        "public,max-age=" + durationInSeconds;

                    var headers = ctx.Context.Response.Headers;
                    var contentType = headers["Content-Type"];

                    if (contentType != "application/x-gzip" && !ctx.File.Name.EndsWith(".gz"))
                    {
                        return;
                    }

                    var fileNameToTry = ctx.File.Name.Substring(0, ctx.File.Name.Length - 3);

                    if (mimeTypeProvider.TryGetContentType(fileNameToTry, out var mimeType))
                    {
                        headers.Add("Content-Encoding", "gzip");
                        headers["Content-Type"] = mimeType;
                    }
                }
            });
            app.UseSession();
            app.UseResponseCaching();
            app.UseStatusCodePagesWithRedirects("/Home/Error");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<CheckBrowserMiddleware>();
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
            if (hostEnvironment.IsDevelopment())
                logServerCertificate = new X509Certificate2($"{hostEnvironment.ContentRootPath}/wwwroot/certificate/free.aiki.client.certificate.with.password.pfx", "D7511C44414CAA552B425F39DAE8CA6");
            else
                logServerCertificate = new X509Certificate2($"{hostEnvironment.ContentRootPath}/wwwroot/certificate/TurbineJobLogs.pfx", "Mveyma6303$");
            var docStore = new DocumentStore
            {
                Urls = new[] { Configuration.GetSection("RavenDBSettings:Server").Value },
                Database = Configuration.GetSection("RavenDBSettings:CollectionName").Value,
                Certificate = logServerCertificate
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
