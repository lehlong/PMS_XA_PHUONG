using Microsoft.AspNetCore.Mvc;
using Project.Service.Common;
using Project.Service.Dtos.PS;
using Project.Service.Services.PS;

namespace Project.Api.Controllers.PS
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProjectWorkflowProcessingController(IProjectWorkflowProcessingService service) : ControllerBase
    {
        public readonly IProjectWorkflowProcessingService _service = service;

        [HttpGet("GetProjectWorkflowStep/{projectId}")]
        public async Task<IActionResult> GetProjectWorkflowStep([FromRoute] string projectId)
        {
            var res = new TransferObject();
            var data = await _service.GetProjectWorkflowStep(projectId);
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

        [HttpPut("UpdateWorkflowProject")]
        public async Task<IActionResult> UpdateWorkflowProject([FromBody] List<ProjectWorkflowProcessingDto> request)
        {
            var res = new TransferObject();
            await _service.UpdateWorkflowProject(request);
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

        [HttpPut("StartWorkflow/{projectId}")]
        public async Task<IActionResult> StartWorkflow([FromRoute] string projectId)
        {
            var res = new TransferObject();
            await _service.StartWorkflow(projectId);
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

    }
}
