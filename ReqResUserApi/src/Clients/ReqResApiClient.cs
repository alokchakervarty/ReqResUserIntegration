using Microsoft.Extensions.Options;
using ReqResUserApi.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using ReqResUserApi.Configuration;

namespace ReqResUserApi.Clients
{
    public class ReqResApiClient : IReqResApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;        
        public ReqResApiClient(HttpClient httpClient, IOptions<ApiConfiguration> config)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("x-api-key", "reqres-free-v1");
            _baseUrl = string.IsNullOrEmpty(config.Value.BaseUrl) 
                ? throw new ArgumentNullException(nameof(config.Value.BaseUrl), "BaseUrl cannot be null or empty.")
                : config.Value.BaseUrl.TrimEnd('/') + "/";
        }
        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}users/{userId}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var data = doc.RootElement.GetProperty("data").ToString();
                return JsonSerializer.Deserialize<UserDto>(data);
            }
            catch (HttpRequestException httpEx)
            {
                // Log error (replace with your logger if needed)
                Console.WriteLine($"HTTP Error: {httpEx.Message}");
                throw;
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"JSON Parse Error: {jsonEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled Error: {ex.Message}");
                throw;
            }
        }

        public async Task<List<UserDto>> GetUsersByPageAsync(int pageNumber)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}users?page={pageNumber}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var wrapper = JsonSerializer.Deserialize<UserDtoWrapper>(json);
                return wrapper.data;
            }

            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Error: {httpEx.Message}");
                throw;
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"JSON Parse Error: {jsonEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled Error: {ex.Message}");
                throw;
            }
        }
    }
}