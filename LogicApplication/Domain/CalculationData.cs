using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicApplication.Domain
{
    /// <summary>
    /// All information of calculate
    /// </summary>
    public class CalculationData(DateTime dateTime, IEnumerable<PowerByPeriod> periods) : ICalculationData
    {
        public DateTime DateTime { get; private set; } = dateTime;

        public IEnumerable<PowerByPeriod> Periods { get; private set; } = periods;
    }
}
