using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurbineJobMVC.Extensions
{
    public static class DateExtensions
    {
        public static string ConvertToWesternArbicNumerals(string input)
        {
            var result = new StringBuilder(input.Length);
            foreach (char c in input.ToCharArray())
            {
                if (char.IsNumber(c))
                {
                    result.Append(char.GetNumericValue(c));
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }
    }
}
