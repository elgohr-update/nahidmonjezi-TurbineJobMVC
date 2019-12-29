using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurbineJobMVC.Models.Entites;
using TurbineJobMVC.Models.EntitiesConfigure;

namespace TurbineJobMVC.Models
{
    public class PCStockDBContext : DbContext
    {
        public DbSet<WorkOrderTBL> WorkOrderTBL { get; set; }
        public DbSet<WorkOrder> WorkOrder { get; set; }
        public DbSet<TahvilForms> TahvilForms { get; set; }

        public PCStockDBContext():base()
        {

        }

        public PCStockDBContext(DbContextOptions options):base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging();
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
