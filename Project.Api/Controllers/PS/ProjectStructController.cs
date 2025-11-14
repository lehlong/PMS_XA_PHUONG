using Microsoft.AspNetCore.Mvc;
using Project.Service.Common;
using Project.Service.Dtos.MD;
using Project.Service.Dtos.PS;
using Project.Service.Services.PS;

namespace Project.Api.Controllers.PS
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectStructController(IProjectStructService service) : ControllerBase
    {
        public readonly IProjectStructService _service = service;


        [HttpGet("CheckCodeExists")]
        public async Task<IActionResult> CheckCodeExists([FromQuery] string code)
        {
            var res = new TransferObject();
            bool data = await _service.ValidateCodeExists(code);
            res.Data = data;
            res.Status = true;
            return Ok(res);
        }

        [HttpGet("GetProjectStruct/{projectId}")]
        public async Task<IActionResult> Detail([FromRoute] string projectId)
        {
            var res = new TransferObject();
            var data = await _service.GetProjectStruct(projectId);
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

        //[HttpGet("Search")]
        //public async Task<IActionResult> Search([FromQuery] ProjectStructDto filter)
        //{
        //    var res = new TransferObject();
        //    var data = await _service.Search(filter);
        //    if (_service.Status)
        //    {
        //        res.Data = data;
        //    }
        //    else
        //    {
        //        await res.GetMessage("0001", _service);
        //    }
        //    return Ok(res);
        //}
        [HttpGet("GetTaskWorkflow/{projectId}")]
        public async Task<IActionResult> GetTaskWorkflow([FromRoute] string projectId)
        {
            var res = new TransferObject();
            var data = await _service.GetTaskWorkflow(projectId);
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
        public async Task<IActionResult> Insert([FromBody] ProjectStructDto request)
        {
            var res = new TransferObject();
            await _service.Insert(request);
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
    }
}
