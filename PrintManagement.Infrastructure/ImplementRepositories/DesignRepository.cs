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
    public class DesignRepository : IDesignRepository
    {
        private readonly ApplicationDbContext _context;

        public DesignRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Design>> GetDesignWithStatusByProjectId(DeStatus? deStatus, int projectId)
        {
            // Tạo query cơ bản để lấy thiết kế theo projectId
            IQueryable<Design> query = _context.Designs.Where(d => d.ProjectId == projectId);

            if (deStatus.HasValue)
            {
                query = query.Where(d => d.DesignStatus == deStatus.Value);
            }

            return await query.ToListAsync();
        }

    }
}
