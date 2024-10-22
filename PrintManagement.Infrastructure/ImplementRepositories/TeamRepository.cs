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
    public class TeamRepository : ITeamRepository
    {
        ApplicationDbContext _context;

        public TeamRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Team>> GetAllTeamsAsync(string? teamName)
        {
            var query = _context.Teams.AsQueryable();
            if (!string.IsNullOrEmpty(teamName))
            {
                query = query.Where(u => u.Name.Contains(teamName));
            }

            return await query.ToListAsync();
        }

        public async Task<int> GetNumberOfMembersInTeamAsync(int teamId)
        {
            return await _context.Users.CountAsync(u => u.TeamId == teamId);
        }

        public async Task<Team> GetTeamByManager(int managerId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.ManagerId == managerId);
            return team;
        }

        public async Task<Team> GetTeamByName(string Name)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.Name.ToLower().Equals(Name.ToLower()));
            return team;
        }
    }
}
