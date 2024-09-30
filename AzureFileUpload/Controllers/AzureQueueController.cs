using AzureFileUpload.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureFileUpload.Controllers
{
    public class AzureQueueController : ControllerBase
    {
        private readonly QueueStorageService _queueStorageService;

        public AzureQueueController(QueueStorageService queueStorageService)
        {
            _queueStorageService = queueStorageService;
        }
              
        [HttpPost("create")]
        public async Task<IActionResult> CreateQueue([FromQuery] string queueName)
        {
            await _queueStorageService.CreateQueueAsync(queueName);
            return Ok(new { message = $"Queue '{queueName}' created or already exists." });
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromQuery] string queueName, [FromQuery] string message)
        {
            await _queueStorageService.SendMessageAsync(queueName, message);
            return Ok(new { message = $"Message sent to queue '{queueName}'." });
        }

        [HttpGet("receive")]
        public async Task<IActionResult> ReceiveMessage([FromQuery] string queueName)
        {
            var message = await _queueStorageService.ReceiveMessageAsync(queueName);
            if (message == null)
            {
                return NotFound("No messages available in the queue.");
            }
            return Ok(new { message });
        }
    }
}
