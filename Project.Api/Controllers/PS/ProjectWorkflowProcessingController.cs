using Microsoft.AspNetCore.Mvc;
using Project.Service.Common;
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

    }
}
