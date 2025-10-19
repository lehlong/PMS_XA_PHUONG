using Microsoft.AspNetCore.Mvc;
using Project.Service.Common;
using Project.Service.Dtos.PS;
using Project.Service.Services.PS;

namespace Project.Api.Controllers.PS
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController(IProjectService service) : ControllerBase
    {
        public readonly IProjectService _service = service;

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] ProjectDto filter)
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

        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] ProjectDto request)
        {
            var res = new TransferObject();
            var result = await _service.CreateProject(request);
            if (_service.Status)
            {
                res.Data = result;
                await res.GetMessage("0100", _service);
            }
            else
            {
                await res.GetMessage("0101", _service);
            }
            return Ok(res);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] ProjectDto request)
        {
            var res = new TransferObject();
            await _service.Update(request);
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

        [HttpGet("Detail/{projectId}")]
        public async Task<IActionResult> Detail([FromRoute] string projectId)
        {
            var res = new TransferObject();
            var data = await _service.GetProjectDetail(projectId);
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

        [HttpGet("GetGiaiDoan/{projectId}")]
        public async Task<IActionResult> GetGiaiDoan([FromRoute] string projectId)
        {
            var res = new TransferObject();
            var data = await _service.GetGiaiDoan(projectId);
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


        [HttpPut("TrinhDuyet")]
        public async Task<IActionResult> TrinhDuyet([FromBody] ProjectDto request)
        {
            var res = new TransferObject();
            await _service.Update(request);
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

        [HttpPut("PheDuyet")]
        public async Task<IActionResult> PheDuyet([FromBody] ProjectDto request)
        {
            var res = new TransferObject();
            await _service.Update(request);
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

        [HttpPut("XacNhan")]
        public async Task<IActionResult> XacNhan([FromBody] ProjectDto request)
        {
            var res = new TransferObject();
            await _service.Update(request);
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

        [HttpPut("TuChoi")]
        public async Task<IActionResult> TuChoi([FromBody] ProjectDto request)
        {
            var res = new TransferObject();
            await _service.Update(request);
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
