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

        [HttpGet("Detail/{code}")]
        public async Task<IActionResult> Detail([FromRoute] string code)
        {
            var res = new TransferObject();
            var data = await _service.GetProjectDetail(code);
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

        [HttpDelete("Delete/{code}")]
        public async Task<IActionResult> Delete([FromRoute] string code)
        {
            var res = new TransferObject();
            await _service.Delete(code);
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
