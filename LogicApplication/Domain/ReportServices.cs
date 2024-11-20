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
        private readonly TimeZoneInfo ApplicationTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(configuration.GetSection("Configuration:LocalTimeZone").Value ?? throw new DomainException("Not set LocalTimeZone"));

        public async Task<bool> CreateReport(DateTime executionTime, FormatReport formatReport)
        {
            DateTime utcDateTime = executionTime.ToUniversalTime();
            DateTime applicationDatetime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, ApplicationTimeZoneInfo);
            var referenceDate = applicationDatetime.AddDays(1);
            var calculationDataWithLocalDatetime = (await calculationServices.CalculateData(applicationDatetime)).ToList();
            var formatFile = configuration.GetSection("Configuration:FormatFileName").Value ?? throw new DomainException("Not set FormatFileName");
            var fileName = string.Format(
                                formatFile,
                                referenceDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                                utcDateTime.ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture));
            if (calculationDataWithLocalDatetime.Count() <= 1)
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
            DateTime utcDateTime = calculationDataDayAhead.DateTime.ToUniversalTime();
            DateTime firtsTimeOfPeriodUtc = new DateTime(utcDateTime.Year, utcDateTime.Month, utcDateTime.Day, utcDateTime.Hour, 0, 0, DateTimeKind.Utc);
            return new FileReportData(fileName, calculationDataDayAhead.Periods.Select(r =>
            {
                var interDayPeriod = calculationDataInterDay.Periods.First(x => x.Period == r.Period);
                return new PowerByHour(TimeZoneInfo.ConvertTimeFromUtc(firtsTimeOfPeriodUtc.AddHours(r.Period), ApplicationTimeZoneInfo), r.Volume + interDayPeriod.Volume);
            }));
        }
    }
}
