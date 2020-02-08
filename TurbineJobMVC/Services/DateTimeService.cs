using MD.PersianDateTime.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TurbineJobMVC.Services
{
    public class DateTimeService:IDateTimeService
    {
        public DateTimeService()
        {

        }
        public string ConvertToLongJalaliDateTimeString(DateTime? nullableDateTime)
        {
            return ConvertToPersianDateTime(nullableDateTime).ToLongDateTimeString();
        }
        public string ConvertToShortJalaliDateString(DateTime? nullableDateTime)
        {
            return ConvertToPersianDateTime(nullableDateTime).ToShortDateString();
        }

        public PersianDateTime ConvertToPersianDateTime(DateTime? nullableDateTime)
        {
            return new PersianDateTime(nullableDateTime);
        }

    }
}
