using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TurbineJobMVC.Services;

namespace TurbineJobMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected readonly ILogger<HomeController> _logger;
        protected readonly IMapper _map;
        protected readonly IService _service;
        protected readonly IDataProtectionProvider _provider;
        protected readonly IUserService _userService;

        public BaseApiController(
            ILogger<HomeController> logger,
            IMapper map,
            IService service,
            IDataProtectionProvider provider,
            IUserService userService)
        {
            _logger = logger;
            _map = map;
            _service = service;
            _provider = provider;
            _userService = userService;
        }
    }
}