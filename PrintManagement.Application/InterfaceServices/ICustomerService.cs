using PrintManagement.Application.Payloads.RequestModels.CustomerRequest;
using PrintManagement.Domain.Entities;
using PrintManagement.Domain.InterfaceRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.InterfaceServices
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllCustomer();
        Task<Customer> CreateCustomer(Request_Customer request);
        Task<Customer> UpdateCustomer(int id, Request_Customer request);
        Task<bool> DeleteCustomer(int id);

    }
}
