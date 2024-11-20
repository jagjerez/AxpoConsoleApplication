using Axpo;
using LogicApplication.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicApplication.Infrastructure
{
    public class CalculationServicesAxpoLibrary(IPowerService powerService) : ICalculationServices
    {
        public async Task<IEnumerable<ICalculationData>> CalculateData(DateTime referenceDate)
        {
            IEnumerable<PowerTrade> calculateData;
            try
            {
                calculateData = await powerService.GetTradesAsync(referenceDate);
            } 
            catch (PowerServiceException ex)
            {
                throw new DomainException(ex.Message, ex);
            }
            return GetCalculationData(calculateData);
        }

        private static IEnumerable<ICalculationData> GetCalculationData(IEnumerable<PowerTrade> powerTrades)
        {
            return powerTrades.Select(r =>
            new CalculationData(r.Date, GetDataPowerByPeriod(r.Periods)));
        }

        private static IEnumerable<PowerByPeriod> GetDataPowerByPeriod(IEnumerable<PowerPeriod> powerPeriods)
        {
            return powerPeriods.Select(x => 
            new PowerByPeriod(x.Period , x.Volume));
        }
    }
}
