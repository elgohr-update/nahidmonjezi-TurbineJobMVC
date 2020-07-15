using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using MD.PersianDateTime.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurbineJobMVC.Extensions;
using TurbineJobMVC.Models.Entities;
using TurbineJobMVC.Models.ViewModels;

namespace TurbineJobMVC.Services
{
    public class WorkOrderService : IWorkOrderService
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly IMapper _map;
        private readonly IDateTimeService _dateTimeService;
        public WorkOrderService(IUnitOfWork unitofwork, IMapper map, IDateTimeService dateTimeService)
        {
            _unitofwork = unitofwork;
            _map = map;
            _dateTimeService = dateTimeService;
        }
        public async Task<long> addWorkOrder(JobViewModel JobModel)
        {
            var repo = _unitofwork.GetRepository<WorkOrderTBL>();
            var newWorkOrder = _map.Map<WorkOrderTBL>(JobModel);
            newWorkOrder.WOTime = DateTime.Now.ToString("HH:MM");
            newWorkOrder.WODate = _dateTimeService.ConvertToShortJalaliDateString(DateTime.Now);
            newWorkOrder.RequestDate = newWorkOrder.WODate;
            var recordsExists = await repo.ExistsAsync(q => q.WODate.Substring(0, 4) == newWorkOrder.WODate.Substring(0, 4));
            if (recordsExists)
                newWorkOrder.WONo = Convert.ToInt64(await repo.MaxAsync(predicate: q => q.WODate.Substring(0, 4) == newWorkOrder.WODate.Substring(0, 4), q => q.WONo) + 1);
            else
                newWorkOrder.WONo = Convert.ToInt64($"{(newWorkOrder.WODate.Substring(0, 4))}000001");
            newWorkOrder.ConsComment = 45;
            newWorkOrder.OprCode = null;
            newWorkOrder.WoType = 1;
            var ARInfo = _map.Map<TahvilFormsViewModel>(await _unitofwork.GetRepository<TahvilForms>().GetFirstOrDefaultAsync(predicate: q => q.AmvalNo.ToString() == JobModel.AR));
            if (ARInfo != null)
            {
                newWorkOrder.AskerCode = ARInfo.DevilerCodeOrigin;
                await repo.InsertAsync(newWorkOrder);
                await _unitofwork.SaveChangesAsync();
                await AddDailyReport(newWorkOrder.WONo, "درخواست کاربر دریافت گردید");
            }
            else
            {
                newWorkOrder.WONo = -1;
            }
            return newWorkOrder.WONo;
        }

        public async Task<WorkOrderViewModel> ChooseSingleWorkOrderByAROrWono(string code)
        {
            var founded = await GetSingleWorkOrder(code).ConfigureAwait(false);
            if (founded != null)
            { return founded; }
            return await GetSingleWorkOrderByAR(code);
        }

        public async Task<WorkOrderViewModel> GetSingleWorkOrder(string Wono) =>
            _map.Map<WorkOrderViewModel>(await _unitofwork.GetRepository<WorkOrder>().GetFirstOrDefaultAsync(predicate: q => q.WONo == Convert.ToInt64(Wono)));
        public async Task<WorkOrderViewModel> GetSingleWorkOrderByAR(string AR) =>
            _map.Map<WorkOrderViewModel>(await _unitofwork.GetRepository<WorkOrder>().GetFirstOrDefaultAsync(predicate: q => q.Amval == AR && string.IsNullOrEmpty(q.EndJobDate)));
        public async Task<TahvilFormsViewModel> GetTahvilForm(string amval)=>
            _map.Map<TahvilFormsViewModel>(await _unitofwork.GetRepository<TahvilForms>().GetFirstOrDefaultAsync(predicate: q => q.AmvalNo.ToString() == amval));
        public async Task<IList<TahvilFormsViewModel>> GetTahvilForms(string regNo) =>
            _map.Map<IList<TahvilFormsViewModel>>(await _unitofwork.GetRepository<TahvilForms>().GetAllAsync(predicate: q => q.RegisterNo == regNo, orderBy: q => q.OrderByDescending(c => c.AmvalNo)));
        public async Task<IList<WorkOrderDailyReportViewModel>> GetWorkOrderReport(string Wono) =>
            _map.Map<IList<WorkOrderDailyReportViewModel>>(await _unitofwork.GetRepository<WorkOrderDailyReportTBL>().GetAllAsync(predicate: q => q.Wono == Convert.ToInt64(Wono), orderBy: q => q.OrderByDescending(c => c.ReportDate)));
        public WorkOrderTBL IsDublicateActiveAR(string amval)=>
            _unitofwork.GetRepository<WorkOrderTBL>().GetFirstOrDefault(predicate: q => q.Amval == amval && String.IsNullOrEmpty(q.EndJobDate));
        public async Task<WorkOrderTBL> IsDublicateActiveARAsync(string amval) =>
            await _unitofwork.GetRepository<WorkOrderTBL>().GetFirstOrDefaultAsync(predicate: q => q.Amval == amval && String.IsNullOrEmpty(q.EndJobDate));
        public WorkOrderTBL IsDublicateNotRateAR(string amval) =>
             _unitofwork.GetRepository<WorkOrderTBL>().GetFirstOrDefault(predicate: q => q.Amval == amval && String.IsNullOrEmpty(q.EndJobDate) && q.CustomerRate == null);
        public async Task<WorkOrderTBL> IsDublicateNotRateARAsync(string amval)=>
            await _unitofwork.GetRepository<WorkOrderTBL>().GetFirstOrDefaultAsync(predicate: q => q.Amval == amval && String.IsNullOrEmpty(q.EndJobDate) && q.CustomerRate == null);
        public bool IsNumberic(string number)
        {
            return double.TryParse(number, out _);
        }

        public async Task<bool> SetWonoVote(long wono)
        {
            var workorder = await _unitofwork.GetRepository<WorkOrderTBL>().FindAsync(wono);
            if (workorder == null) return false;
            workorder.CustomerRate = 10;
            await AddDailyReport(wono, "تاییدیه انجام کار توسط کاربر ثبت گردید");
            await _unitofwork.SaveChangesAsync();
            return true;
        }

        public async Task<List<NotEndWorkOrderListViewModel>> GetNotEndWorkOrderList()
        {
            return _map.Map<List<NotEndWorkOrderListViewModel>>(await _unitofwork.GetRepository<NotEndWorkOrderList>().GetAllAsync());
        }
        private async Task<WorkOrderDailyReportViewModel> AddDailyReport(long wono, string ReportComment)
        {
            var repoWorkOrderComment = _unitofwork.GetRepository<WorkOrderDailyReportTBL>();
            var workOrderReport = new WorkOrderDailyReportTBL()
            {
                Wono = wono,
                ReportID = Guid.NewGuid(),
                ReportDate = DateTime.Now,
                ReportComment = ReportComment,
                MemberName = "سامانه مدیریت درخواست"
            };
            await repoWorkOrderComment.InsertAsync(workOrderReport);
            await _unitofwork.SaveChangesAsync();
            return _map.Map<WorkOrderDailyReportViewModel>(workOrderReport);
        }

        public async Task<IList<WorkOrderViewModel>> WorkOrderArchive(string AR)=>
        _map.Map<IList<WorkOrderViewModel>>(await _unitofwork.GetRepository<WorkOrder>().GetAllAsync(predicate: q => q.Amval == AR &&  !(string.IsNullOrEmpty(q.EndJobDate))));
        

    }
}
