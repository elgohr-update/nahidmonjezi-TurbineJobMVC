using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TurbineJobMVC.Services;

namespace TurbineJobMVC.Models.CustomValidation
{
    public class IsDublicateActiveAR : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return new ValidationResult(ErrorMessage!=null ? ErrorMessage : "شماره اموال وجود ندارد");
            var _service = (Service)validationContext.GetService(typeof(IService));
            var workorder = _service.IsDublicateActiveAR(value.ToString()).Result;
            if (workorder==null)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(ErrorMessage != null ? ErrorMessage : $"برای اموال یک درخواست با کد رهگیری {workorder.WONo} ثبت شده که همچنان در دست اقدام است");
            }
        }

    }
}
