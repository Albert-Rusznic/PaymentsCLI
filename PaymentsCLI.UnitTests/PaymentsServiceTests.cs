using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using PaymentsCLI.Payments;
using PaymentsCLI.Payments.Models;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace PaymentsCLI.UnitTests
{
    public class PaymentsServiceTests
    {
        [Fact]
        public async Task MakePayment_ReturnsValidResponse_WhenApiReturnsSuccess()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"TransactionId\":123,\"Status\":\"Success\"}")
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var options = Options.Create(new PaymentsOptions
            {
                BaseUrl = "https://test/api",
                Endpoint = "/payments"
            });
            var loggerMock = new Mock<ILogger<PaymentsService>>();

            var service = new PaymentsService(httpClient, options, loggerMock.Object);
            var result = await service.MakePayment(new PaymentRequest());

            Assert.Equal(123, result.TransactionId);
            Assert.Equal("Success", result.Status);
        }

        [Fact]
        public async Task MakePayment_ThrowsHttpRequestException_WhenApiReturnsForbidden()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Forbidden,
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var options = Options.Create(new PaymentsOptions
            {
                BaseUrl = "https://test/api",
                Endpoint = "/payments"
            });
            var loggerMock = new Mock<ILogger<PaymentsService>>();

            var service = new PaymentsService(httpClient, options, loggerMock.Object);
            
            var ex = await Assert.ThrowsAsync<HttpRequestException>(() =>
                service.MakePayment(new PaymentRequest())
            );
            Assert.Contains("403", ex.Message);
        }

        [Fact]
        public async Task MakePayment_ThrowsJsonException_WhenApiReturnsBadContent()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("\n  \t")
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var options = Options.Create(new PaymentsOptions
            {
                BaseUrl = "https://test/api",
                Endpoint = "/payments"
            });
            var loggerMock = new Mock<ILogger<PaymentsService>>();

            var service = new PaymentsService(httpClient, options, loggerMock.Object);

            await Assert.ThrowsAsync<JsonException>(() =>
                service.MakePayment(new PaymentRequest())
            );
        }

        [Fact]
        public async Task MakePayment_ThrowsValidationException_WhenApiReturnsIncompleteContent()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"Status\":\"Success\"}")
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var options = Options.Create(new PaymentsOptions
            {
                BaseUrl = "https://test/api",
                Endpoint = "/payments"
            });
            var loggerMock = new Mock<ILogger<PaymentsService>>();

            var service = new PaymentsService(httpClient, options, loggerMock.Object);

            await Assert.ThrowsAsync<ValidationException>(() =>
                service.MakePayment(new PaymentRequest())
            );
        }
    }
}