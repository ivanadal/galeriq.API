using System;
using System.Text.Json;
using Galeriq.PhotoProcessor;
using Galeriq.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Galeriq.PhotoProcessing.Functions
{
    public class PhotoProcessingFunction
    {
        private readonly IPhotoProcessor _photoProcessor; // your image service

        public PhotoProcessingFunction(IPhotoProcessor photoProcessor)
        {
            _photoProcessor = photoProcessor;
        }

        [FunctionName("ProcessPhoto")]
        public async Task Run(
            [ServiceBusTrigger("photo-processing", Connection = "ServiceBusConnection")] string message,
            ILogger log)
        {
            try
            {
                var photoMessage = JsonSerializer.Deserialize<PhotoProcessingMessage>(message);

                log.LogInformation($"Processing Photo {photoMessage.PhotoId} for Gallery {photoMessage.GalleryId}");

                await _photoProcessor.ProcessPhotoAsync(photoMessage);

                log.LogInformation($"Completed Photo {photoMessage.PhotoId}");
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error processing photo message");
                throw; // ensures Service Bus retries / DLQ
            }
        }
    }
}
