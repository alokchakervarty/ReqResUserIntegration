
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using ReqResUserApi.Clients;
using ReqResUserApi.Configuration;
using ReqResUserApi.Models;
using ReqResUserApi.Services;
namespace ReqResUserApi.Tests
{
    public class ExternalUserServiceTests
    {
        private readonly Mock<IReqResApiClient> _mockApiClient;
        private readonly IMemoryCache _memoryCache;
        private readonly ExternalUserService _service;
        public ExternalUserServiceTests()
        {
            _mockApiClient = new Mock<IReqResApiClient>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            var config = Options.Create(new ApiConfiguration
            {
                BaseUrl = "https://reqres.in/api/",
                UserCacheExpirationMinutes = 5
            });
            _service = new ExternalUserService(_mockApiClient.Object, _memoryCache, config);
        }
        [Fact]
        public async Task GetUserByIdAsync_ReturnsMappedUser()
        {
            // Arrange  
            var userDto = new UserDto { id = 2, email = "janet@example.com", first_name = "Janet", last_name = "Weaver" };
            _mockApiClient.Setup(c => c.GetUserByIdAsync(2)).ReturnsAsync(userDto);

            // Act  
            var user = await _service.GetUserByIdAsync(2);

            // Assert  
            Assert.NotNull(user);
            Assert.Equal("Janet Weaver", user.FullName);
            Assert.Equal("janet@example.com", user.Email);
        }
        [Fact]
        public async Task GetUserByIdAsync_ThrowsException_WhenApiReturnsNull()
        {
            var userId = 2;
            _mockApiClient.Setup(c => c.GetUserByIdAsync(userId)).ReturnsAsync((UserDto)null);
            await Assert.ThrowsAsync<NullReferenceException>(() => _service.GetUserByIdAsync(userId));
        }
    }
}