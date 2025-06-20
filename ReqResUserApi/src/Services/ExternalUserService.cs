using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ReqResUserApi.Clients;
using ReqResUserApi.Configuration;
using ReqResUserApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReqResUserApi.Services
{
    public class ExternalUserService : IExternalUserService
    {
        private readonly IReqResApiClient _apiClient;
        private readonly IMemoryCache _cache;
        private readonly int _expirationMinutes;
        public ExternalUserService(IReqResApiClient apiClient, IMemoryCache cache, IOptions<ApiConfiguration> config)
        {
            _apiClient = apiClient;
            _cache = cache;
            _expirationMinutes = config.Value.UserCacheExpirationMinutes > 0 ? config.Value.UserCacheExpirationMinutes : 5; // Default to 5 minutes if not set
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var cacheKey = $"user_{userId}";

            // Check if data is already cached
            if (_cache.TryGetValue<User>(cacheKey, out var cachedUser))
            {
                Console.WriteLine($"[CACHE HIT] User {userId} served from memory.");
                return cachedUser;
            }

            Console.WriteLine($"[CACHE MISS] Fetching user {userId} from API.");

            //If not cached, fetch and cache it
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_expirationMinutes);

                var dto = await _apiClient.GetUserByIdAsync(userId);

                Console.WriteLine($"[CACHE STORE] User {userId} cached for {_expirationMinutes} minutes.");

                return new User
                {
                    Id = dto.id,
                    Email = dto.email,
                    FullName = $"{dto.first_name} {dto.last_name}"
                };
            });          
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            List<User> allUsers = new();
            int page = 1;
            while (true)
            {
                var users = await _apiClient.GetUsersByPageAsync(page);
                if (users == null || !users.Any()) break;

                allUsers.AddRange(users.Select(dto => new User
                {
                    Id = dto.id,
                    Email = dto.email,
                    FullName = $"{dto.first_name} {dto.last_name}"
                }));

                page++;
            }
            return allUsers;
        }
    }
}