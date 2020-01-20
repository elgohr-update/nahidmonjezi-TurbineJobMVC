using System;


namespace TurbineJobMVC.Models.Entites
{
    public class WorkOrderDailyReportTBL
    {
        public Guid ReportID  { get; set; }
        public long? Wono { get; set; }
        public DateTime? ReportDate { get; set; }
        public string ReportComment { get; set; }
    }
}