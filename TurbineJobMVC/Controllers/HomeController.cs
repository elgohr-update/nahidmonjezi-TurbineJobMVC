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
    public class HomeController : BaseController
    {
        public HomeController(ILogger<HomeController> logger,
            IMapper map,
            IService service,
            IDataProtectionProvider provider) : base(logger, map, service, provider) { }
        public IActionResult Index()
        {
            return View();
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
                    var Wono = await _service.WorkOrderService.addWorkOrder(JobModel);
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
            var workOrder = await _service.WorkOrderService.GetSingleWorkOrder(id);
            if (workOrder != null)
            {
                ViewData["TahvilInfo"] = await _service.WorkOrderService.GetTahvilForm(workOrder.Amval);
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
            return View(await _service.WorkOrderService.GetTahvilForms(id));
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

        public async Task<IActionResult> WorkOrderReport(string WonoSearch)
        {
            WorkOrderViewModel workOrder = null;
            if (!_service.WorkOrderService.IsNumberic(WonoSearch)) return BadRequest();
            if (WonoSearch.Length == 8)
                workOrder = await _service.WorkOrderService.GetSingleWorkOrder(WonoSearch);
            else if (WonoSearch.Length > 8)
            {
                workOrder = await _service.WorkOrderService.GetSingleWorkOrderByAR(WonoSearch);
            }
            if (workOrder != null)
            {
                ViewData["WorkOrderInfo"] = workOrder;
            }
            else return BadRequest();
            return View(await _service.WorkOrderService.GetWorkOrderReport(workOrder.WONo.ToString()));
        }
        public async Task<IActionResult> GetVote(long Vote_Wono)
        {
            var result = await _service.WorkOrderService.SetWonoVote(Vote_Wono);
            if (result)
                return View();
            else
                return BadRequest();
        }
    }
}
