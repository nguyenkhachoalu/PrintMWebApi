using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.InterfaceRepositories
{
    public interface ITeamRepository
    {
        Task<Team> GetTeamByName(string Name);
        Task<int> GetNumberOfMembersInTeamAsync(int teamId);
        Task<Team> GetTeamByManager(int managerId);
        Task<IEnumerable<Team>> GetAllTeamsAsync(string? teamName);

    }
}
