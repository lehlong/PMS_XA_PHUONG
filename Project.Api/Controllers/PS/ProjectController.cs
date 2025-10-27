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

        [HttpGet("GetCurrentStep/{stepId}")]
        public async Task<IActionResult> GetCurrentStep([FromRoute] string stepId)
        {
            var res = new TransferObject();
            var data = await _service.GetCurrentStep(stepId);
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


        [HttpPut("TrinhDuyet/{projectId}")]
        public async Task<IActionResult> TrinhDuyet([FromRoute] string projectId)
        {
            var res = new TransferObject();
            await _service.TrinhDuyet(projectId);
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

        [HttpPut("PheDuyet/{projectId}")]
        public async Task<IActionResult> PheDuyet([FromRoute] string projectId)
        {
            var res = new TransferObject();
            await _service.PheDuyet(projectId);
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

        [HttpPut("XacNhan/{projectId}")]
        public async Task<IActionResult> XacNhan([FromRoute] string projectId)
        {
            var res = new TransferObject();
            await _service.XacNhan(projectId);
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

        [HttpPut("TuChoi/{projectId}")]
        public async Task<IActionResult> TuChoi([FromRoute] string projectId)
        {
            var res = new TransferObject();
            await _service.TuChoi(projectId);
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

        [HttpPut("YeuCauChinhSua/{projectId}")]
        public async Task<IActionResult> YeuCauChinhSua([FromRoute] string projectId)
        {
            var res = new TransferObject();
            await _service.YeuCauChinhSua(projectId);
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
