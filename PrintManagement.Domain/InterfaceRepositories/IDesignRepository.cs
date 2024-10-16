using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.InterfaceRepositories
{
    public interface IDesignRepository
    {
        Task<IEnumerable<Design>> GetDesignWithStatusByProjectId(DeStatus? deStatus,int projectId);
    }
}
