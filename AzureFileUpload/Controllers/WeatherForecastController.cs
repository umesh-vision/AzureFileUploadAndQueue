using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure.Storage;
using Microsoft.AspNetCore.Mvc;
using AzureFileUpload.Services;

namespace AzureFileUpload.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {     
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly AzureFileService _blobStorageService;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, AzureFileService blobStorageService)
        {
            _logger = logger;
            _blobStorageService = blobStorageService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

         
        [HttpPost]
        public async Task<string> blobUpload()
        {
            string containerName = "testcontainer";
            string blobName = "sample.txt";
            string localFilePath = "sample.txt";
            string downloadFilePath = "downloaded_sample.txt";

            // Try creating or opening the file and writing data to it
            try
            {
                using (StreamWriter writer = new StreamWriter(localFilePath, false))
                {
                    writer.WriteAsync("This is a test file for Azurite Blob Storage.");
                }
                Console.WriteLine($"File '{localFilePath}' created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating file: {ex.Message}");
            }

            // Create BlobServiceClient using the connection string to Azurite
            BlobServiceClient blobServiceClient = new BlobServiceClient("AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;");

            // Create or get a reference to a container
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);
            Console.WriteLine($"Container '{containerName}' created or exists.");

            // Upload a blob
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            Console.WriteLine($"Uploading blob '{blobName}'...");
            await blobClient.UploadAsync(localFilePath, overwrite: true);
            Console.WriteLine($"Blob '{blobName}' uploaded.");

            // List blobs in the container
            Console.WriteLine("Listing blobs in the container:");
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                Console.WriteLine($"\t{blobItem.Name}");
            }

            // Download the blob
            BlobDownloadInfo download = await blobClient.DownloadAsync();
            using (FileStream downloadFileStream = new FileStream(downloadFilePath, FileMode.Create, FileAccess.Write))
            {
                await download.Content.CopyToAsync(downloadFileStream);
            }
            Console.WriteLine($"Blob downloaded to '{downloadFilePath}'.");

            return "";
        }
    }
    
}
