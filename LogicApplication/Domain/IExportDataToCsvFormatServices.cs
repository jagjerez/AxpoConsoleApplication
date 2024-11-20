using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicApplication.Domain
{
    /// <summary>
    /// Services to export data to CSV
    /// </summary>
    public interface IExportDataToCsvFormatServices
    {
        bool Execute(IFileData fileData);
    }
}
