using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicApplication.Domain
{
    /// <summary>
    /// Interface create to interact with an external services
    /// </summary>
    public interface ICalculationServices
    {
        /// <summary>
        /// Get all power per hour
        /// </summary>
        /// <param name="referenceDate">Reference date to make the calculate</param>
        /// <returns></returns>
        public Task<IEnumerable<ICalculationData>> CalculateData(DateTime referenceDate);
    }
}
