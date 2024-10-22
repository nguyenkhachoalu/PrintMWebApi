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
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Project>> GetAllProjectAsync(string? projectName, PStatus? status)
        {
            var query = _context.Projects.AsQueryable();

            // Tìm kiếm theo tên dự án nếu projectName không null
            if (!string.IsNullOrEmpty(projectName))
            {
                query = query.Where(p => p.ProjectName.Contains(projectName));
            }

            // Lọc theo trạng thái nếu status không null
            if (status.HasValue)
            {
                query = query.Where(p => p.ProjectStatus == status.Value);
            }

            return await query.ToListAsync();
        }

    }
}
