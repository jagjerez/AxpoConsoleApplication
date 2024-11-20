using CsvHelper;
using CsvHelper.Configuration;
using LogicApplication.Domain;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace LogicApplication.Infrastructure
{
    public class ExportDataToCsvFormatServices(IConfiguration configuration) : IExportDataToCsvFormatServices
    {
        public bool Execute(IFileData fileData)
        {
            bool result = false;
            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";"
                };

                using var writer = new StreamWriter(Path.Combine(configuration.GetSection("Configuration:RouteFile").Value ?? throw new DomainException("Not set RouteFile"), fileData.FileName));
                using var csv = new CsvWriter(writer, config);

                csv.WriteRecords(fileData.PowersByHour);
                result = true;
            }
            catch (Exception ex)
            {
                throw new DomainException(ex.Message, ex);
            }
            return result;
        }
    }
}
