using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicApplication.Domain
{
    public interface IFileData
    {
        public string FileName { get; }
        public IEnumerable<IPowerByHour> PowersByHour { get; }
    }
}
