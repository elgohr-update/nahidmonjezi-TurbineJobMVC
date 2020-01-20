using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TurbineJobMVC.Models.Entites;

namespace TurbineJobMVC.Models.EntitiesConfigure
{
    public class WorkOrderDailyReportTBLConfigure:IEntityTypeConfiguration<WorkOrderDailyReportTBL>
    {
        public void Configure(EntityTypeBuilder<WorkOrderDailyReportTBL> builder)
        {
            builder.HasKey(x => x.ReportID);
            builder.HasQueryFilter(x => !String.IsNullOrEmpty(x.ReportComment));
        }
    }
}