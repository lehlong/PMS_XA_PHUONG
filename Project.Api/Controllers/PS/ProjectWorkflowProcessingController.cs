using Microsoft.AspNetCore.Mvc;
using Pipelines.Sockets.Unofficial.Arenas;
using Project.Service.Common;
using Project.Service.Dtos.PS;
using Project.Service.Services.MD;
using Project.Service.Services.PS;

namespace Project.Api.Controllers.PS
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProjectWorkflowProcessingController(IProjectWorkflowProcessingService service,IProjectStructService projectStructService,IProjectService projectService) : ControllerBase
    {
        public readonly IProjectWorkflowProcessingService _service = service;
        public readonly IProjectStructService _projectStructService = projectStructService;
        public readonly IProjectService _projectService = projectService;

        [HttpGet("GetProjectWorkflowStep/{projectId}/{workflowId}")]
        public async Task<IActionResult> GetProjectWorkflowStep([FromRoute] string projectId,string workflowId)
        {
            var res = new TransferObject();
            var data = await _service.GetProjectWorkflowStep(projectId,workflowId);
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

        [HttpGet("GetTaskWorkflow/{projectId}")]
        public async Task<IActionResult> GetTaskWorkflow([FromRoute] string projectId)
        {
            var res = new TransferObject();
            var data = await _projectStructService.GetTaskWorkflow(projectId);
            if (_projectStructService.Status)
            {
                res.Data = data;
            }
            else
            {
                await res.GetMessage("0001", _projectStructService);
            }
            return Ok(res);
        }

        [HttpGet("GetProjectWorkflow/{projectId}")]
        public async Task<IActionResult> GetProjectWorkflow([FromRoute] string projectId)
        {
            var res = new TransferObject();
            var data = await _projectService.GetProjectWorkflow(projectId);
            if (_projectService.Status)
            {
                res.Data = data;
            }
            else
            {
                await res.GetMessage("0001", _projectService);
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
