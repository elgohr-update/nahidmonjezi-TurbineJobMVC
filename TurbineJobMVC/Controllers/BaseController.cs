using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TurbineJobMVC.Services;
using Wangkanai.Detection;

namespace TurbineJobMVC.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ILogger<HomeController> _logger;
        protected readonly IMapper _map;
        protected readonly IService _service;
        protected readonly IDataProtectionProvider _provider;

        public BaseController(
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
    }
}