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
        /// <summary>
        /// Get Datetime
        /// </summary>
        public DateTime DateTime { get; private set; } = dateTime;

        /// <summary>
        /// Get periods
        /// </summary>
        public IEnumerable<PowerByPeriod> Periods { get; private set; } = periods;
    }
}
