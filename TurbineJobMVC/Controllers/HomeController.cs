using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using DNTCaptcha.Core;
using DNTCaptcha.Core.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TurbineJobMVC.Models;
using TurbineJobMVC.Models.ViewModels;
using TurbineJobMVC.Services;
using Wangkanai.Detection;

namespace TurbineJobMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _map;
        private readonly IDetection _detection;
        private readonly IService _service;
        private readonly IDataProtectionProvider _provider;

        private readonly string[] CompatibleBrowsers = { "Chrome", "Firefox", "Edge", "Safari" };
        public HomeController(
            ILogger<HomeController> logger,
            IMapper map,
            IDetection detection,
            IService service,
            IDataProtectionProvider provider)
        {
            _logger = logger;
            _map = map;
            _detection = detection;
            _service = service;
            _provider = provider;
        }

        public IActionResult Index()
        {
            if (CompatibleBrowsers.Contains(_detection.Browser.Type.ToString()))
                return View();
            else
                return RedirectToAction("InCompatibleBrowser", "Home");
        }

        [HttpPost, AutoValidateAntiforgeryToken]
        [ValidateDNTCaptcha(ErrorMessage = "لطفا کد امنیتی را وارد نمایید",
                    IsNumericErrorMessage = "مقدار وارد شده می بایست عددی باشد",
                    CaptchaGeneratorLanguage = Language.Persian,
                    CaptchaGeneratorDisplayMode = DisplayMode.ShowDigits)]
        public async Task<IActionResult> Index(JobViewModel JobModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var Wono = await _service.addWorkOrder(JobModel);
                    if (Wono == -1) throw new Exception();
                    else
                        return RedirectToAction("Tracking", new { id = Wono.ToString() });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
            else
            {
                return View(JobModel);
            }
        }

        public async Task<IActionResult> Tracking(string id)
        {
            var workOrder = await _service.GetSingleWorkOrder(id);
            if (workOrder != null)
            {
                ViewData["TahvilInfo"] = await _service.GetTahvilForm(workOrder.Amval);
                return View(workOrder);
            }
            else
            {
                return NotFound();
            }
        }

        [ResponseCache(Duration = 100, VaryByQueryKeys = new[] { "*" })]
        public async Task<IActionResult> Search(string id)
        {
            return View(await _service.GetTahvilForms(id));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult InCompatibleBrowser()
        {
            return View();
        }

        public async Task<IActionResult> WorkOrderReport(string WonoSearch)
        {
            WorkOrderViewModel workOrder = null;
            if (!_service.IsNumberic(WonoSearch)) return BadRequest();
            if (WonoSearch.Length == 8)
                workOrder = await _service.GetSingleWorkOrder(WonoSearch);
            else if(WonoSearch.Length>8)
            {
                workOrder = await _service.GetSingleWorkOrderByAR(WonoSearch);
            }
            if (workOrder != null)
            {
                ViewData["WorkOrderInfo"] = workOrder;
            }
            else return BadRequest();
            return View(await _service.GetWorkOrderReport(workOrder.WONo.ToString()));
        }
        public async Task<IActionResult> GetVote(long Vote_Wono)
        {
            var result = await _service.SetWonoVote(Vote_Wono);
            if (result)
                return View();
            else
                return BadRequest();
        }
    }
}
