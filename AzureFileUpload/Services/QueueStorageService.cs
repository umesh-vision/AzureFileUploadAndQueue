using Azure.Storage.Queues.Models;
using Azure.Storage.Queues;

namespace AzureFileUpload.Services
{
    public class QueueStorageService
    {
        private readonly QueueServiceClient _queueServiceClient;

        public QueueStorageService(IConfiguration configuration)
        {
            string connectionString = configuration["AzureBlobStorage"];
            _queueServiceClient = new QueueServiceClient(connectionString);
        }

        public async Task CreateQueueAsync(string queueName)
        {
            QueueClient queueClient = _queueServiceClient.GetQueueClient(queueName);
            await queueClient.CreateIfNotExistsAsync();
        }

        public async Task SendMessageAsync(string queueName, string message)
        {
            QueueClient queueClient = _queueServiceClient.GetQueueClient(queueName);
            await queueClient.SendMessageAsync(message);
        }

        public async Task<string> ReceiveMessageAsync(string queueName)
        {
            QueueClient queueClient = _queueServiceClient.GetQueueClient(queueName);
            QueueMessage[] messages = await queueClient.ReceiveMessagesAsync(1);
            if (messages.Length > 0)
            {
                string messageText = messages[0].MessageText;
                await queueClient.DeleteMessageAsync(messages[0].MessageId, messages[0].PopReceipt);
                return messageText;
            }
            return null;
        }
    }
}
