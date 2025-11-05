using Azure.Messaging.ServiceBus;
using Galeriq.Data.Entities;
using Galeriq.Model;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CollectionsAPI.Services
{
    public class ServiceBusQueueService : IAsyncDisposable
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;

        public ServiceBusQueueService(IOptions<ServiceBusSettings> options)
        {
            var settings = options.Value;
            _client = new ServiceBusClient(settings.ConnectionString);
            _sender = _client.CreateSender(settings.QueueName);
        }

        public async Task SendMessageAsync(PhotoProcessingMessage message)
        {
            var json = JsonSerializer.Serialize(message);
            var sbMessage = new ServiceBusMessage(json)
            {
                ContentType = "application/json"
            };
            await _sender.SendMessageAsync(sbMessage);
        }

        public async ValueTask DisposeAsync()
        {
            await _sender.DisposeAsync();
            await _client.DisposeAsync();
        }
    }

    public class ServiceBusSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
    }
}
