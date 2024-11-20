using LogicApplication.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicApplication.Application.UseCases
{
    public class CreateReportServices(IReportServices reportServices)
    {
        public async Task Execute(DateTime executionTime, FormatReport formatReport)
        {
            try
            {
                var result = await reportServices.CreateReport(executionTime, formatReport);
                if (!result)
                {
                    throw new ApplicationException("the report haven't created");
                }
            }
            catch (DomainException ex) {
                throw new ApplicationException(ex.Message, ex);
            }
            await Task.CompletedTask;
        }
    }
}
