using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.InterfaceRepositories
{
    public interface IResourcesRepository
    {
        Task<Resources> GetResourcesByName (string name);
        Task UpdateAvailableQuantityAsync(int resourceId);

    }
}
