using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TurbineJobMVC.Models.CustomValidation
{
    public class IsDublicateNotRateAR : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return new ValidationResult(ErrorMessage!=null ? ErrorMessage : "شماره اموال وجود ندارد");
            var db = (PCStockDBContext)validationContext.GetService(typeof(PCStockDBContext));
            var workorder = db.WorkOrder.Where(q => q.Amval == value.ToString() && String.IsNullOrEmpty(q.EndJobDate) && q.CustomerRate == null).FirstOrDefault();
            if (workorder==null)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(ErrorMessage != null ? ErrorMessage : $"برای اموال یک درخواست با کد رهگیری {workorder.WONo} ثبت شده که تاییدیه اتمام کار آن توسط شما اعلان نشده است");
            }
        }

    }
}
