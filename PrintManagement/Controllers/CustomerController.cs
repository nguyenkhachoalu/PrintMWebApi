using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintManagement.Application.InterfaceServices;
using PrintManagement.Application.Payloads.RequestModels.CustomerRequest;
using PrintManagement.Constants;
using PrintManagement.Domain.Entities;

namespace PrintManagement.Controllers
{
    [Route(Constant.DefaultValue.DEFAULTCONTROLLER_ROUTE)]
    [ApiController]
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomer()
        {
            return Ok(await _customerService.GetAllCustomer());
        }
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody]Request_Customer request)
        {
            return Ok(await _customerService.CreateCustomer(request));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody]Request_Customer request)
        {
            return Ok(await _customerService.UpdateCustomer(id,request));
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            return Ok(await _customerService.DeleteCustomer(id));
        }
    }
}
