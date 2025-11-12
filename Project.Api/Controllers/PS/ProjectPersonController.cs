using Microsoft.AspNetCore.Mvc;
using Project.Service.Common;
using Project.Service.Dtos.MD;
using Project.Service.Dtos.PS;
using Project.Service.Services.PS;

namespace Project.Api.Controllers.PS
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectPersonController(IProjectPersonService service) : ControllerBase
    {
        public readonly IProjectPersonService _service = service;


        [HttpGet("GetProjectPerson/{projectId}")]
        public async Task<IActionResult> GetProjectPerson([FromRoute] string projectId)
        {
            var res = new TransferObject();
            var data = await _service.GetProjectPerson(projectId);
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

        [HttpGet("GetProjectPersonByOrg/{projectId}/{orgId}")]
        public async Task<IActionResult> GetProjectPersonByOrg([FromRoute] string projectId, [FromRoute] string orgId)
        {
            var res = new TransferObject();
            var data = await _service.GetProjectPersonByOrg(projectId, orgId);
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

        [HttpPut("UpdateProjectPerson")]
        public async Task<IActionResult> UpdateProjectPerson([FromBody] List<ProjectPersonDto> request)
        {
            var res = new TransferObject();
            await _service.UpdateProjectPerson(request);
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

        [HttpPut("UpdateInfoProjectPerson")]
        public async Task<IActionResult> UpdateInfoProjectPerson([FromBody] List<ProjectPersonDto> request)
        {
            var res = new TransferObject();
            await _service.UpdateInfoProjectPerson(request);
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

        [HttpPost("DeleteProjectPerson")]
        public async Task<IActionResult> DeleteProjectPerson([FromBody] List<string> ids)
        {
            var res = new TransferObject();
            await _service.DeleteProjectPerson(ids);
            if (_service.Status)
            {
                await res.GetMessage("0105", _service);
            }
            else
            {
                await res.GetMessage("0106", _service);
            }
            return Ok(res);
        }
    }
}
