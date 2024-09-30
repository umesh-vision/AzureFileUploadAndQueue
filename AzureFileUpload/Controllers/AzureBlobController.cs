using AzureFileUpload.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureFileUpload.Controllers
{
    public class AzureBlobController : ControllerBase
    {
        private readonly AzureFileService _blobStorageService;

        public AzureBlobController(AzureFileService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }
        // GET api/blob/list?containerName={containerName}
        [HttpGet("list")]
        public async Task<IEnumerable<string>> ListFiles([FromQuery] string containerName)
        {
            return await _blobStorageService.ListFilesAsync(containerName);
        }
        // POST api/blob/upload
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] string containerName, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File not provided.");
            }

            using (var stream = file.OpenReadStream())
            {
                await _blobStorageService.UploadFileAsync(containerName, file.FileName, stream);
            }

            return Ok(new { message = $"File {file.FileName} uploaded successfully." });
        }

        // GET api/blob/download?containerName={containerName}&fileName={fileName}
        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile([FromQuery] string containerName, [FromQuery] string fileName)
        {
            var stream = await _blobStorageService.DownloadFileAsync(containerName, fileName);

            return File(stream, "application/octet-stream", fileName);
        }

    }
}
