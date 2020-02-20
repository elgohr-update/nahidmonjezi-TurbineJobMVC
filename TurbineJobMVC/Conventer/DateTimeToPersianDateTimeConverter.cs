using AutoMapper;
using MD.PersianDateTime.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TurbineJobMVC.Conventer
{
    public class DateTimeToPersianDateTimeConverter : ITypeConverter<DateTime, string>
    {
        public string Convert(DateTime source, string destination, ResolutionContext context)
        {
            return new PersianDateTime(source).ToLongDateString();
        }
    }
}
