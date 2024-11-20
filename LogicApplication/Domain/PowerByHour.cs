using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicApplication.Domain
{
    public class PowerByHour(DateTime localDateTime, double volumen) : IPowerByHour
    {
        private readonly TimeZoneInfo GmtTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        public string DateTime => TimeZoneInfo.ConvertTimeToUtc(localDateTime, GmtTimeZoneInfo).ToString("O");

        public string Volume => volumen.ToString(CultureInfo.InvariantCulture);
    }
}
