using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicApplication.Domain
{
    /// <summary>
    /// All information of report
    /// </summary>
    public interface ICalculationData
    {
        /// <summary>
        /// Get Datetime
        /// </summary>
        public DateTime DateTime { get; }
        
        /// <summary>
        /// Get Periods
        /// </summary>
        public IEnumerable<PowerByPeriod> Periods { get; }
    }
}
