using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaDataProvider6.Extensions
{
    public static class DateTimeExtensions
    {

        public static bool IsInRange(this DateTime value, DateTime start, DateTime end)
        {
            return value >= start && value <= end;
        }
    }
}
