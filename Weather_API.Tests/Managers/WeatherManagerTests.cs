using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http.Json;
using Weather_API.Managers;
using Weather_API.Models;
using Weather_API.Services.Interfaces;

namespace Weather_API.Tests.Managers
{
    public class WeatherManagerTests
    {
        private readonly Mock<IWeatherRepository> _mockRepo;
        private readonly HttpClient _httpClient;
        private readonly Mock<HttpMessageHandler> _httpHandlerMock;
        private readonly IConfiguration _config;

        public WeatherManagerTests()
        {
            _mockRepo = new Mock<IWeatherRepository>();

            var settings = new Dictionary<string, string?>
            {
                { "WeatherAPI:ApiKey", "test_api_key" }
            };
            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            _httpHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpHandlerMock.Object);
        }

        [Fact]
        public async Task GetWeatherByCity_ShouldReturnCachedRecord_BeforeTimePeriodExpired()
        {
            var city = "Colombo";
            var existing = new WeatherRecord
            {
                Id = 1,
                City = city,
                Temperature = 20,
                LastUpdated = DateTime.Now.AddMinutes(-10)
            };

            _mockRepo.Setup(r => r.GetByCity(city))
                     .ReturnsAsync(existing);

            var manager = new WeatherManager(_mockRepo.Object, _config, _httpClient);

            var result = await manager.GetWeatherByCity(city);

            result.Should().BeEquivalentTo(existing);
            _mockRepo.Verify(r => r.GetByCity(city), Times.Once);
            _mockRepo.Verify(r => r.Insert(It.IsAny<WeatherRecord>()), Times.Never);
            _mockRepo.Verify(r => r.Update(It.IsAny<WeatherRecord>()), Times.Never);
        }

        [Fact]
        public async Task GetWeatherByCity_ShouldGetFromApi_AfterTimePeriodExpired()
        {
            var city = "Paris";
            var existing = new WeatherRecord
            {
                Id = 5,
                City = city,
                LastUpdated = DateTime.Now.AddHours(-1)
            };
            _mockRepo.Setup(r => r.GetByCity(city)).ReturnsAsync(existing);

            var apiResponse = new OpenWeatherResponse
            {
                Name = "Paris",
                Sys = new() { Country = "FR" },
                Main = new() { Temp = 25, Pressure = 1000, Humidity = 50 },
                Weather = new List<Weather> { new Weather { Description = "Sunny" } },
                Wind = new() { Speed = 3.2 },
                Clouds = new() { All = 5 }
            };

            _httpHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(apiResponse)
                });

            var manager = new WeatherManager(_mockRepo.Object, _config, _httpClient);

            var result = await manager.GetWeatherByCity(city);

            result.City.Should().Be("Paris");
            result.Country.Should().Be("FR");
            result.Temperature.Should().Be(25);
            result.Description.Should().Be("Sunny");

            _mockRepo.Verify(r => r.Update(It.Is<WeatherRecord>(w => w.Id == existing.Id)), Times.Once);
        }

        [Fact]
        public async Task GetWeatherByCity_ShouldInsertNewRecord_WhenNoCacheExists()
        {
            var city = "Tokyo";
            _mockRepo.Setup(r => r.GetByCity(city)).ReturnsAsync((WeatherRecord?)null);

            var apiResponse = new OpenWeatherResponse
            {
                Name = "Tokyo",
                Sys = new() { Country = "JP" },
                Main = new() { Temp = 22, Pressure = 1012, Humidity = 55 },
                Weather = new List<Weather> { new Weather { Description = "Cloudy" } },
                Wind = new() { Speed = 2.1 },
                Clouds = new() { All = 10 }
            };

            _httpHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(apiResponse)
                });

            var manager = new WeatherManager(_mockRepo.Object, _config, _httpClient);

            var result = await manager.GetWeatherByCity(city);

            result.City.Should().Be("Tokyo");
            _mockRepo.Verify(r => r.Insert(It.Is<WeatherRecord>(w => w.City == "Tokyo")), Times.Once);
        }

        [Fact]
        public async Task GetWeatherByCity_ShouldThrow_WhenApiResponseFails()
        {
            var city = "InvalidCity";
            _mockRepo.Setup(r => r.GetByCity(city)).ReturnsAsync((WeatherRecord?)null);

            _httpHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            var manager = new WeatherManager(_mockRepo.Object, _config, _httpClient);

            var act = async () => await manager.GetWeatherByCity(city);

            await act.Should().ThrowAsync<System.ComponentModel.DataAnnotations.ValidationException>()
                     .WithMessage("Please enter valid city name.");
        }

        [Fact]
        public async Task GetWeatherByCity_ShouldThrow_WhenApiResponseIsNull()
        {
            var city = "Berlin";
            _mockRepo.Setup(r => r.GetByCity(city)).ReturnsAsync((WeatherRecord?)null);

            _httpHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create<object?>(null)
                });

            var manager = new WeatherManager(_mockRepo.Object, _config, _httpClient);

            var act = async () => await manager.GetWeatherByCity(city);

            await act.Should().ThrowAsync<System.ComponentModel.DataAnnotations.ValidationException>()
                     .WithMessage("Invalid api response.");
        }
    }
}
