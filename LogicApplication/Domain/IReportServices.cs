using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicApplication.Domain
{
    /// <summary>
    /// Service use to create the report.
    /// </summary>
    public interface IReportServices
    {
        /// <summary>
        /// Create report
        /// </summary>
        /// <param name="executionTime">Time to create data, this time is in local time</param>
        /// <param name="formatReport">Type of format file</param>
        /// <returns>Csv file</returns>
        Task<bool> CreateReport(DateTime executionTime, FormatReport formatReport);
    }
}
