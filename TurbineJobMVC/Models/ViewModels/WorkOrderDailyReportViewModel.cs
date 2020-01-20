using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TurbineJobMVC.Models.ViewModels
{
    public class WorkOrderDailyReportViewModel

    {
        public Guid ReportID { get; set; }
        public long Wono { get; set; }
        public DateTime ReportDate { get; set; }
        public string ReportComment { get; set; }

    }
}