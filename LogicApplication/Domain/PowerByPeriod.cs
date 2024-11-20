using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicApplication.Domain
{
    /// <summary>
    /// Volumen information in a period
    /// </summary>
    public struct PowerByPeriod
    {
        public int Period { get; }
        public double Volume { get; }

        public PowerByPeriod(int period, double? volume)
        {
            Period = period;
            Volume = volume ?? 0.0;
        }
    }
}
