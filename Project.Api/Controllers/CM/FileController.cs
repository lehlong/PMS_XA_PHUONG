using Microsoft.AspNetCore.Mvc;
using Minio.DataModel.Select;
using Project.Service.Common;
using Project.Service.Dtos.CM;
using Project.Service.Services.CM;

namespace Project.Api.Controllers.CM
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController(IFileService service) : ControllerBase
    {
        public readonly IFileService _service = service;

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            var res = new TransferObject();
            if (files == null || files.Count == 0)
            {
                res.Status = false;
                res.MessageObject.Message = "Không có file được chọn";
                return Ok(res);
            }

            var result = await _service.UploadFilesToMinio(files);
            if (_service.Status)
            {
                res.Data = result;
                res.Status = true;
                await res.GetMessage("0107", _service);
            }
            else
            {
                res.Status = false;
                await res.GetMessage("0108", _service);
            }

            return Ok(res);
        }
    }

}
