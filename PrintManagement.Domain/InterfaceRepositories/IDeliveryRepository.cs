using PrintManagement.Domain.Entities;
using PrintManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Domain.InterfaceRepositories
{
    public interface IDeliveryRepository
    {
        Task<IEnumerable<DeliveryModels>> GetAllDeliveryByStatus(DStatus? dStatus);
        Task<int?> GetShipperIdByDeliveryAddressAsync(string deliveryAddress);
        Task<IEnumerable<Project>> GetCompletedProjectsNotInDeliveryAsync();
        Task<IEnumerable<User>> GetEmployeesInDeliveryTeamAsync();
        Task<IEnumerable<DeliveryModels>> GetAllDeliveryByDeliverIdAndStatus(int deliverId, DStatus? dStatus);
    }
}
