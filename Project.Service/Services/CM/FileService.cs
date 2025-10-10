using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Project.Core;
using Project.Core.Entities.CM;
using Project.Service.Common;
using Project.Service.Dtos.CM;

namespace Project.Service.Services.CM
{
    public interface IFileService : IGenericService<CmFile, FileDto>
    {
        Task<List<FileDto>> UploadFilesToMinio(List<IFormFile> files);
    }

    public class FileService(AppDbContext dbContext, IMapper mapper, IMinioClient minioClient, IOptions<MinioConfigDto> minioOptions) : GenericService<CmFile, FileDto>(dbContext, mapper), IFileService
    {
        private readonly MinioConfigDto _minioSettings = minioOptions.Value;
        private readonly IMinioClient _minioClient = minioClient;

        public async Task<List<FileDto>> UploadFilesToMinio(List<IFormFile> files)
        {
            try
            {

                var bucketName = _minioSettings.BucketName;
                bool found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));

                if (!found)
                {
                    await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
                }

                var responseList = new List<FileDto>();

                var protocol = _minioSettings.UseSSL ? "https" : "http";

                var baseUrl = $"{protocol}://{_minioSettings.Endpoint}:{_minioSettings.Port}";

                foreach (var file in files)
                {
                    if (file == null || file.Length == 0)
                    {
                        continue;
                    }
                    var fileId = Guid.NewGuid().ToString();

                    var ext = Path.GetExtension(file.FileName);

                    var contentType = file.ContentType;

                    using (var stream = file.OpenReadStream())
                    {
                        var putObjectArgs = new PutObjectArgs()
                            .WithBucket(bucketName)
                            .WithObject(fileId)
                            .WithStreamData(stream)
                            .WithObjectSize(stream.Length)
                            .WithContentType(contentType);

                        await _minioClient.PutObjectAsync(putObjectArgs);
                    }

                    responseList.Add(new FileDto
                    {
                        Id = fileId,
                        FileName = file.FileName,
                        FileSize = file.Length,
                        MimeType = contentType,
                        Extention = ext,
                        Icon = GetFileIcon(contentType)
                    });
                }

                return responseList;
            }
            catch (Exception ex)
            {
                Status = false;
                Exception = ex;
                return new List<FileDto>();
            }
        }
        public string GetFileIcon(string fileType)
        {
            fileType = (fileType ?? "").ToLower();

            if (fileType == "application/pdf")
                return "img/pdf.png";

            if (fileType == "application/msword" || fileType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                return "img/word.png";

            if (fileType == "application/vnd.ms-excel" || fileType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                return "img/excel.png";

            if (fileType == "application/vnd.ms-powerpoint" || fileType == "application/vnd.openxmlformats-officedocument.presentationml.presentation")
                return "img/powerpoint.png";

            if (fileType.StartsWith("video/"))
                return "img/multimedia.png";

            if (fileType.StartsWith("audio/"))
                return "img/audio.png";

            if (fileType.StartsWith("image/"))
                return "img/image.png";

            return "img/file.png";
        }

    }
}
