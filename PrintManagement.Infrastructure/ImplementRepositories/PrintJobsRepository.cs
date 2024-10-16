using Microsoft.EntityFrameworkCore;
using PrintManagement.Domain.Entities;
using PrintManagement.Domain.InterfaceRepositories;
using PrintManagement.Infrastructure.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Infrastructure.ImplementRepositories
{
    public class PrintJobsRepository : IPrintJobsRepository
    {
        private readonly ApplicationDbContext _context;

        

        public PrintJobsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PrintJobs> getPrintJobByDesignid(int designId)
        {
            var printJob = await _context.PrintJobs.Where(x => x.DesignId == designId).FirstOrDefaultAsync();
            return printJob;
        }

        public async Task<IEnumerable<PrintJobs>> GetPrintJobsByDesignid(int designId)
        {
            var printJobs = await _context.PrintJobs.Where(x => x.DesignId == designId).ToListAsync();
            return printJobs;
        }

        public async Task<IEnumerable<ResourcePropertyDetail>> GetResourceDetailsByPrintJob(PrintJobs printJob)
        {
            // Truy xuất danh sách ResourcePropertyDetail dựa trên PrintJob
            var resourceDetails = await _context.ResourceForPrintJobs
                .Where(rfp => rfp.PrintJobId == printJob.Id)              // Lọc theo PrintJobId
                .Include(rfp => rfp.ResourcePropertyDetail)                // Bao gồm ResourcePropertyDetail
                .Select(rfp => rfp.ResourcePropertyDetail)                 // Chọn ResourcePropertyDetail
                .ToListAsync();

            return resourceDetails;
        }
    }
}
