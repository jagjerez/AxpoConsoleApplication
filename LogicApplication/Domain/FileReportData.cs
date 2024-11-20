using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicApplication.Domain
{
    public class FileReportData(string fileName, IEnumerable<IPowerByHour> powerByHours) : IFileData
    {
        public string FileName { get; private set; } = fileName;

        public IEnumerable<IPowerByHour> PowersByHour { get; private set; } = powerByHours;
    }
}
