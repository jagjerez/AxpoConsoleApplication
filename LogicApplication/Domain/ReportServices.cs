using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LogicApplication.Domain
{
    /// <summary>
    /// Services used to create report
    /// </summary>
    public class ReportServices(
        ICalculationServices calculationServices, 
        IExportDataToCsvFormatServices exportDataToCsvFormatServices,
        IConfiguration configuration) : IReportServices
    {
        private readonly TimeZoneInfo GmtTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        private readonly TimeZoneInfo ApplicationTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(configuration.GetSection("Configuration:LocalTimeZone").Value ?? throw new DomainException("You set LocalTimeZone"));

        public async Task<bool> CreateReport(DateTime executionTime, FormatReport formatReport)
        {
            DateTime executionTimeDayAhead = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(executionTime.Year, executionTime.Month, executionTime.Day, executionTime.Hour, executionTime.Minute, executionTime.Second, DateTimeKind.Utc), ApplicationTimeZoneInfo);
            var referenceDate = executionTimeDayAhead.AddDays(1);
            var calculationDataWithLocalDatetime = (await calculationServices.CalculateData(referenceDate)).ToList();
            var formatFile = configuration.GetSection("Configuration:FormatFileName").Value ?? throw new DomainException("You set FormatFileName");
            var fileName = string.Format(
                                formatFile,
                                referenceDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                                executionTimeDayAhead.ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture));
            if (calculationDataWithLocalDatetime.Count() == 2)
            {
                throw new DomainException("Error when call to external services, the calculate return data empty.");
            }
            
            return formatReport == FormatReport.CSV ?
                exportDataToCsvFormatServices.Execute(
                        GetCalculateDataHourlyAggreate(
                            (CalculationData)calculationDataWithLocalDatetime[0],
                            (CalculationData)calculationDataWithLocalDatetime[1],
                            fileName)) :
                throw new DomainException("Format not sopported.");
        }

        private IFileData GetCalculateDataHourlyAggreate(CalculationData calculationDataDayAhead, CalculationData calculationDataInterDay, string fileName)
        {
            DateTime dateTime = new DateTime(calculationDataDayAhead.DateTime.Year, calculationDataDayAhead.DateTime.Month, calculationDataDayAhead.DateTime.Day, 0, 0, 0, DateTimeKind.Unspecified);
            return new FileReportData(fileName, calculationDataDayAhead.Periods.Select(r =>
            {
                var interDayPeriod = calculationDataInterDay.Periods.First(x => x.Period == r.Period);
                return new PowerByHour(dateTime.AddHours(r.Period), r.Volume + interDayPeriod.Volume);
            }));
        }
    }
}
