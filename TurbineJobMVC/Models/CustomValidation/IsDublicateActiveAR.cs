using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TurbineJobMVC.Models.CustomValidation
{
    public class IsDublicateActiveAR : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return new ValidationResult(!string.IsNullOrEmpty(ErrorMessage) ? ErrorMessage : "شماره اموال وجود ندارد");
            var db = (PCStockDBContext)validationContext.GetService(typeof(PCStockDBContext));
            if (!db.WorkOrder.Any(q => q.Amval == value.ToString() && String.IsNullOrEmpty(q.EndJobDate)))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(!string.IsNullOrEmpty(ErrorMessage) ? ErrorMessage : "برای اموال یک درخواست ثبت شده که همچنان در دست اقدام است");
            }
        }

    }
}
