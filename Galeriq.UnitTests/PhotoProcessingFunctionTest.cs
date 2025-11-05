using Galeriq.Model;
using Galeriq.PhotoProcessing.Functions;
using Galeriq.PhotoProcessor;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace Galeriq.UnitTests
{
    public class PhotoProcessingFunctionTest
    {

        [Fact]
        public async Task Run_Should_ProcessPhoto()
        {
            // Arrange
            var mockProcessor = new Mock<IPhotoProcessor>();
            var mockLogger = new Mock<ILogger>();
            var stringId = "11111111-1111-1111-1111-111111111111";

            var function = new PhotoProcessingFunction(mockProcessor.Object);

            var photoMessage = new PhotoProcessingMessage
            {
                PhotoId = new Guid(stringId),
                GalleryId = new Guid(stringId),
            };

            string messageJson = JsonSerializer.Serialize(photoMessage);

            // Act
            await function.Run(messageJson, mockLogger.Object);

            // Assert
            mockProcessor.Verify(
                p => p.ProcessPhotoAsync(It.Is<PhotoProcessingMessage>(
                    m => m.PhotoId == new Guid(stringId) && m.GalleryId == new Guid(stringId))),
                Times.Once);

        }

    }
}