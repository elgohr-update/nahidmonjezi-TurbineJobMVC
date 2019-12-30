using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurbineJobMVC.Models.ViewModels;

namespace TurbineJobMVC.Services
{
    public interface IService
    {
        long addWorkOrder(JobViewModel JobModel);
        WorkOrderViewModel GetSingleWorkOrder(string Wono);
        TahvilFormsViewModel GetTahvilForm(string amval);
        IList<TahvilFormsViewModel> GetTahvilForms(string regNo);
    }
}
