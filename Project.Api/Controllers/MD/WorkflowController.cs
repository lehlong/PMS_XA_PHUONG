using Microsoft.AspNetCore.Mvc;
using Project.Service.Common;
using Project.Service.Dtos.MD;
using Project.Service.Services.MD;

namespace Project.Api.Controllers.MD
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowController(IWorkflowService service) : ControllerBase
    {
        public readonly IWorkflowService _service = service;

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] WorkflowDto filter)
        {
            var res = new TransferObject();
            var data = await _service.Search(filter);
            if (_service.Status)
            {
                res.Data = data;
            }
            else
            {
                await res.GetMessage("0001", _service);
            }
            return Ok(res);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var res = new TransferObject();
            var data = await _service.GetAll();
            if (_service.Status)
            {
                res.Data = data;
            }
            else
            {
                await res.GetMessage("0001", _service);
            }
            return Ok(res);
        }


        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] WorkflowDto request)
        {
            var res = new TransferObject();
            await _service.InsertWorkflow(request);
            if (_service.Status)
            {
                await res.GetMessage("0100", _service);
            }
            else
            {
                await res.GetMessage("0101", _service);
            }
            return Ok(res);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] WorkflowDto request)
        {
            var res = new TransferObject();
            await _service.UpdateWorkflow(request);
            if (_service.Status)
            {
                await res.GetMessage("0103", _service);
            }
            else
            {
                await res.GetMessage("0104", _service);
            }
            return Ok(res);
        }

        [HttpGet("Detail/{workflowId}")]
        public async Task<IActionResult> Detail([FromRoute] string workflowId)
        {
            var res = new TransferObject();
            var data = await _service.GetDetail(workflowId);
            if (_service.Status)
            {
                res.Data = data;
            }
            else
            {
                await res.GetMessage("0001", _service);
            }
            return Ok(res);
        }
    }
}
