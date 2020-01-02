using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using DNTCaptcha.Core;
using DNTCaptcha.Core.Providers;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TurbineJobMVC.Models;
using TurbineJobMVC.Models.Entites;
using TurbineJobMVC.Models.ViewModels;
using TurbineJobMVC.Services;
using Wangkanai.Detection;

namespace TurbineJobMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitofwork;
        private readonly IMapper _map;
        private readonly IDetection _detection;
        private readonly IService _service;

        private readonly string[] CompatibleBrowsers = { "Chrome", "Firefox", "Edge", "Safari" };
        public HomeController(
            ILogger<HomeController> logger, 
            IUnitOfWork unitofwork,
            IMapper map,
            IDetection detection,
            IService service)
        {
            _logger = logger;
            _unitofwork = unitofwork;
            _map = map;
            _detection = detection;
            _service = service;
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
                    if (Wono == -1) return BadRequest();
                    else 
                        return RedirectToAction("Tracking", new { id = Wono.ToString() });
                }
                catch(Exception)
                {
                    return BadRequest();
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
            if (workOrder!=null)
            {
                ViewData["TahvilInfo"] = await _service.GetTahvilForm(workOrder.Amval);
                return View(workOrder);
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Search(string id)
        {
            return View(await _service.GetTahvilForms(id));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("500")]
        [Route("404")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult InCompatibleBrowser()
        {
            return View();
        }

    }
}
