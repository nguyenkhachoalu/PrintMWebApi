using PrintManagement.Application.InterfaceServices;
using PrintManagement.Application.Payloads.RequestModels.CustomerRequest;
using PrintManagement.Domain.Entities;
using PrintManagement.Domain.InterfaceRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.ImplementServices
{
    public class CustomerService : ICustomerService
    {
        private readonly IBaseRepository<Customer> _baseCustomerRepository;

        public CustomerService(IBaseRepository<Customer> baseCustomerRepository)
        {
            _baseCustomerRepository = baseCustomerRepository;
        }

        public async Task<Customer> CreateCustomer(Request_Customer request)
        {
            var customer = new Customer()
            {
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Address = request.Address,
            };
            await _baseCustomerRepository.CreateAsync(customer);
            return customer;
        }

        public async Task<bool> DeleteCustomer(int id)
        {
            await _baseCustomerRepository.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomer()
        {
            var customers = await _baseCustomerRepository.GetAllAsync();
            return customers;
        }
    

        public async Task<Customer> UpdateCustomer(int id, Request_Customer request)
        {
            var customer = await _baseCustomerRepository.GetByIdAsync(id);
            customer.FullName = request.FullName;
            customer.PhoneNumber = request.PhoneNumber;
            customer.Email = request.Email;
            customer.Address = request.Address;
            await _baseCustomerRepository.UpdateAsync(customer);
            return customer;
        }
    }
}
