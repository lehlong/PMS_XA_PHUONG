using Microsoft.AspNetCore.Mvc;
using Project.Service.Common;
using Project.Service.Services.PS;

namespace Project.Api.Controllers.PS
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectStructController(IProjectStructService service) : ControllerBase
    {
        public readonly IProjectStructService _service = service;


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
    }
}
