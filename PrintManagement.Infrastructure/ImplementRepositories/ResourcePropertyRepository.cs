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
    public class ResourcePropertyRepository : IResourcePropertyRepository
    {
        private readonly ApplicationDbContext _context;

        public ResourcePropertyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ResourceProperty>> GetResourcePropertiesByResourceId(int resourceId)
        {
            return await _context.ResourceProperties
            .Where(rp => rp.ResourceId == resourceId)
            .ToListAsync();
        }
    }
}
