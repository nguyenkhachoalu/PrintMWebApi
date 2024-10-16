using PrintManagement.Application.Payloads.ResponseModels.DataTeams;
using PrintManagement.Application.Payloads.ResponseModels.DataUsers;
using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.Mappers
{
    public class TeamConverter
    {
        public DataResponseTeam EntitytoDTO(Team team)
        {
            return new DataResponseTeam()
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                NumberOfMember = team.NumberOfMember,
                CreateTime = team.CreateTime,
                UpdateTime = team.UpdateTime,
                ManagerId = team.ManagerId,
            };
        }
    }
}
