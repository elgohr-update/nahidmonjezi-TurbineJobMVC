using MD.PersianDateTime.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TurbineJobMVC.Services
{
    public interface IService
    {
        IWorkOrderService WorkOrderService { get; }
        IDateTimeService DateTimeService { get; }
    }
}
