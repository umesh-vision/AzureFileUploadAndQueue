using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureFileUpload.Services
{
    public class AzureFileService
    {
        private readonly BlobServiceClient _blobServiceClient;
        public AzureFileService(IConfiguration configuration)
        {
            string connectionString = configuration["AzureBlobStorage"];
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task UploadFileAsync(string containerName, string fileName, Stream fileStream)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, true);
        }
        public async Task<Stream> DownloadFileAsync(string containerName, string fileName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            BlobDownloadInfo download = await blobClient.DownloadAsync();
            return download.Content;
        }

        public async Task<IEnumerable<string>> ListFilesAsync(string containerName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            List<string> files = new List<string>();
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                files.Add(blobItem.Name);
            }
            return files;
        }
    }
}
