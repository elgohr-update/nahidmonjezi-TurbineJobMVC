using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TurbineJobMVC.Models.ViewModels;
using TurbineJobMVC.Services;

namespace TurbineJobMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkOrderController : BaseApiController
    {

        public WorkOrderController(
            ILogger<HomeController> logger,
            IMapper map,
            IService service,
            IDataProtectionProvider provider)
        : base(logger, map, service, provider) { }

        [HttpGet("IsDublicateActiveAR/{amval}")]
        public async Task<ActionResult<string>> IsDublicateActiveAR(string amval)
        {
            var workorder = await _service.WorkOrderService.IsDublicateActiveARAsync(amval);
            if (workorder != null)
                return Ok(workorder);
            else
                return Ok("false");
        }

        [HttpGet("IsDublicateNotRateAR/{amval}")]
        public async Task<ActionResult<string>> IsDublicateNotRateAR(string amval)
        {
            var workorder = await _service.WorkOrderService.IsDublicateNotRateARAsync(amval);
            if (workorder != null)
                return Ok(workorder);
            else
                return Ok("false");
        }

        [HttpGet("GetNotEndWorkOrder")]
        public async Task<ActionResult<NotEndWorkOrderListViewModel>> GetNotEndWorkOrder()
        {
            return Ok(await _service.WorkOrderService.GetNotEndWorkOrderList());
        }
    }
}