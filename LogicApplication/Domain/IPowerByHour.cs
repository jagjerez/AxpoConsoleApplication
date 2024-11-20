using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicApplication.Domain
{
    public interface IPowerByHour
    {
        public string DateTime { get; }
        public string Volume { get; }
    }
}
