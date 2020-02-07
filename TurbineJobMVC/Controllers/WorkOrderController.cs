using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TurbineJobMVC.Services;

namespace TurbineJobMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkOrderController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _map;
        private readonly IService _service;
        private readonly IDataProtectionProvider _provider;
        public WorkOrderController(
            ILogger<HomeController> logger,
            IMapper map,
            IService service,
            IDataProtectionProvider provider)
        {
            _logger = logger;
            _map = map;
            _service = service;
            _provider = provider;
        }

        [HttpGet("IsDublicateActiveAR/{amval}")]
        public async Task<ActionResult<string>> IsDublicateActiveAR(string amval)
        {
            var workorder = await _service.WorkOrderService.IsDublicateActiveAR(amval);
            if (workorder != null)
                return Ok(workorder);
            else
                return Ok("false");
        }

        [HttpGet("IsDublicateNotRateAR/{amval}")]
        public async Task<ActionResult<string>> IsDublicateNotRateAR(string amval)
        {
            var workorder = await _service.WorkOrderService.IsDublicateNotRateAR(amval);
            if (workorder != null)
                return Ok(workorder);
            else
                return Ok("false");
        }
    }
}