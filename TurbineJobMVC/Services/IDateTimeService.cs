using MD.PersianDateTime.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TurbineJobMVC.Services
{
    public interface IDateTimeService
    {
        PersianDateTime ConvertToPersianDateTime(DateTime? nullableDateTime);
        string ConvertToLongJalaliDateTimeString(DateTime? nullableDateTime);
        string ConvertToShortJalaliDateString(DateTime? nullableDateTime);
    }
}
