using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TurbineJobMVC.Services
{
    public class Service:IService
    {

        public IWorkOrderService WorkOrderService { get; private set; }
        public Service(IWorkOrderService workOrderService)
        {
            WorkOrderService = workOrderService;
        }
    }
}
