using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.InterfaceRepositories
{
    public interface IPrintJobsRepository
    {
        Task<PrintJobs> getPrintJobByDesignid(int designId);
        Task<IEnumerable<ResourcePropertyDetail>> GetResourceDetailsByPrintJob(PrintJobs printJob);
        Task<IEnumerable<PrintJobs>> GetPrintJobsByDesignid(int designId);
    }
}
