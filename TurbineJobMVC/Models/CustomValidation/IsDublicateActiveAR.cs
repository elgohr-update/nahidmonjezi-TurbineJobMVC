using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TurbineJobMVC.Models.CustomValidation
{
    public class IsDublicateActiveAR : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return new ValidationResult(ErrorMessage!=null ? ErrorMessage : "شماره اموال وجود ندارد");
            var db = (PCStockDBContext)validationContext.GetService(typeof(PCStockDBContext));
            var workorder = db.WorkOrder.Where(q => q.Amval == value.ToString() && String.IsNullOrEmpty(q.EndJobDate)).FirstOrDefault();
            if (workorder==null)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(ErrorMessage != null ? ErrorMessage : $"برای اموال یک درخواست به کد رهگیری {workorder.WONo} ثبت شده که همچنان در دست اقدام است");
            }
        }

    }
}
