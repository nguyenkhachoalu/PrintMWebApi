using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintManagement.Application.InterfaceServices;
using PrintManagement.Application.Payloads.RequestModels.ResourceForPrintJobRequest;
using PrintManagement.Application.Payloads.RequestModels.ResourcePropertyDetailRequest;
using PrintManagement.Application.Payloads.ResponseModels.DataResourceForPrintJob;
using PrintManagement.Application.Payloads.ResponseModels.DataResourcePropertyDetail;
using PrintManagement.Application.Payloads.Responses;
using PrintManagement.Constants;
using PrintManagement.Domain.Entities;

namespace PrintManagement.Controllers
{
    [Route(Constant.DefaultValue.DEFAULTCONTROLLER_ROUTE)]
    [ApiController]
    [Authorize]
    public class PrintJobController : Controller
    {
        private readonly IPrintJobService _printJobService;

        public PrintJobController(IPrintJobService printJobService)
        {
            _printJobService = printJobService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateResourceForPrintJobs([FromForm] Request_CreateResourceForPrintJob request)
        {
            return Ok(await _printJobService.CreateResourceForPrintJobs(request));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateResourcePropertyDetailAsync(int id,[FromForm] Request_UpdateResourcePropertyDetail request)
        {
            return Ok(await _printJobService.UpdateResourcePropertyDetailAsync(id, request));
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteResourcePropertyDetailAsync(int id)
        {
            return Ok(await _printJobService.DeleteResourcePropertyDetailAsync(id));
        }
        [HttpPut]
        public async Task<IActionResult> UpdatePrintJobStatus(int printJobId, PjStatus newStatus)
        {
            return Ok(await _printJobService.UpdatePrintJobStatus(printJobId, newStatus));
        }
        [HttpGet]
        public async Task<IActionResult> GetResourcePrintJobByDesign(int designId)
        {
            return Ok(await _printJobService.GetResourcePrintJobByDesign(designId));
        }
        [HttpGet]
        public async Task<IActionResult> GetResourceDetailByPrintJob(int printJobId)
        {
            return Ok(await _printJobService.GetResourceDetailByPrintJob(printJobId));
        }

    }
}
