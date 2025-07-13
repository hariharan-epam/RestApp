using System.Net;
using Moq;
using RestApp.ThirdParty;

namespace RestApp.Tests
{
    public class RestClientWithRetryTests
    {
        [Fact]
        public async Task Should_Retry_On_WebException_And_Return_Default_On_Final_Failure()
        {
            var restClientMock = new Mock<IRestClient>();
            var loggerMock = new Mock<ILogger>();

            restClientMock.Setup(x => x.Get<object>(It.IsAny<string>()))
                .ThrowsAsync(new WebException("Simulated network failure"));

            var retryClient = new RestClientWithRetry(restClientMock.Object, loggerMock.Object, maxRetries: 3, retryDelay: TimeSpan.FromMilliseconds(10));

            var result = await retryClient.Get<object>("http://test");

            Assert.Null(result); 

            restClientMock.Verify(x => x.Get<object>(It.IsAny<string>()), Times.Exactly(3));
            loggerMock.Verify(x => x.Error(It.IsAny<Exception>()), Times.Once);
        }

        [Fact]
        public async Task Should_FailFast_On_NonWebException()
        {
            var restClientMock = new Mock<IRestClient>();
            var loggerMock = new Mock<ILogger>();

            restClientMock.Setup(x => x.Get<object>(It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("Some other error"));

            var retryClient = new RestClientWithRetry(restClientMock.Object, loggerMock.Object, maxRetries: 3);

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await retryClient.Get<object>("http://test");
            });

            restClientMock.Verify(x => x.Get<object>(It.IsAny<string>()), Times.Once); 
            loggerMock.Verify(x => x.Error(It.IsAny<Exception>()), Times.Once);
        }

        [Fact]
        public async Task Should_Return_Success_On_Second_Attempt()
        {
            var restClientMock = new Mock<IRestClient>();
            var loggerMock = new Mock<ILogger>();

            var callCount = 0;

            restClientMock.Setup(x => x.Get<string>(It.IsAny<string>()))
                .ReturnsAsync(() =>
                {
                    callCount++;
                    if (callCount == 1)
                        throw new WebException("First attempt fails");
                    return "Success";
                });

            var retryClient = new RestClientWithRetry(restClientMock.Object, loggerMock.Object, maxRetries: 3, retryDelay: TimeSpan.FromMilliseconds(10));

            var result = await retryClient.Get<string>("http://test");

            Assert.Equal("Success", result);

            restClientMock.Verify(x => x.Get<string>(It.IsAny<string>()), Times.Exactly(2));
            loggerMock.Verify(x => x.Error(It.IsAny<Exception>()), Times.Never); 
        }
    }
}
