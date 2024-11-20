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
        public string DateTime => localDateTime.ToString("O");

        public string Volume => volumen.ToString(CultureInfo.InvariantCulture);
    }
}
