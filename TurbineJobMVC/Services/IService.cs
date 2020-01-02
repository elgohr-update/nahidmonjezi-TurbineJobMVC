using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurbineJobMVC.Models.ViewModels;

namespace TurbineJobMVC.Services
{
    public interface IService
    {
        Task<long> addWorkOrder(JobViewModel JobModel);
        Task<WorkOrderViewModel> GetSingleWorkOrder(string Wono);
        Task<TahvilFormsViewModel> GetTahvilForm(string amval);
        Task<IList<TahvilFormsViewModel>> GetTahvilForms(string regNo);
    }
}
