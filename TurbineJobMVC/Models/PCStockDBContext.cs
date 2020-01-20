using Audit.Core;
using Audit.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using TurbineJobMVC.Models.Entites;
using TurbineJobMVC.Models.EntitiesConfigure;

namespace TurbineJobMVC.Models
{
    public class PCStockDBContext : DbContext 
    {
        private readonly ILogger<PCStockDBContext> _logger;
        private readonly IHttpContextAccessor _accessor;
        public DbSet<WorkOrderTBL> WorkOrderTBL { get; set; }
        public DbSet<WorkOrder> WorkOrder { get; set; }
        public DbSet<TahvilForms> TahvilForms { get; set; }

        public PCStockDBContext(ILogger<PCStockDBContext> logger, IHttpContextAccessor accessor) : base()
        {
            _logger = logger;
            _accessor = accessor;
        }

        public PCStockDBContext(DbContextOptions options, ILogger<PCStockDBContext> logger, IHttpContextAccessor accessor) : base(options)
        {
            _logger = logger;
            _accessor = accessor;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging(false);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration<WorkOrderTBL>(new WorkOrderTBLConfigure());
            modelBuilder.ApplyConfiguration<WorkOrder>(new BaseViewConfigure<WorkOrder>());
            modelBuilder.ApplyConfiguration<TahvilForms>(new TahvilFormsConfigure());
        }
    }
}
