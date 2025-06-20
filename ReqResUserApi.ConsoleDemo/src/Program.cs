using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Polly;
using ReqResUserApi.Clients;
using ReqResUserApi.Configuration;
using ReqResUserApi.Services;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
namespace ReqResUserApi.ConsoleDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders(); // Clear default providers
                    logging.AddConsole(); // Add console logging provider
                })               
                .ConfigureServices((context, services) =>
                {
                    services.AddMemoryCache();
                    services.AddScoped<IExternalUserService, ExternalUserService>();
                    services.Configure<ApiConfiguration>(context.Configuration.GetSection("ApiSettings"));
                    services.AddHttpClient<IReqResApiClient, ReqResApiClient>();
                    services.AddHttpClient<IReqResApiClient, ReqResApiClient>()
                            .AddTransientHttpErrorPolicy(policy =>
                                                           policy.WaitAndRetryAsync(
                                                               5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                                                onRetryAsync: async (outcome, timespan, retryAttempt, context) =>
                                                                {
                                                                    Console.WriteLine($"Retrying due to exception: {outcome.Exception.Message}. Attempt: {retryAttempt}");
                                                                    await Task.CompletedTask;
                                                                }
                                                               ));
                })
                .Build();

            using var scope = host.Services.CreateScope();

            var userService = scope.ServiceProvider.GetRequiredService<IExternalUserService>();
            Console.WriteLine("=== ReqRes User API Console Demo ===");

            bool keepRunning = true;
            while (keepRunning)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Get user by ID");
                Console.WriteLine("2. Get all users");
                Console.WriteLine("0. Exit");
                Console.Write("Enter your choice (0, 1 or 2): ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter User ID to fetch: ");
                        if (int.TryParse(Console.ReadLine(), out int userId))
                        {
                            try
                            {
                                var user = await userService.GetUserByIdAsync(userId);
                                Console.WriteLine($"\nUser: {user.FullName} ({user.Email})");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid user ID.");
                        }
                        break;

                    case "2":
                        try
                        {
                            Console.WriteLine("\nFetching all users...");
                            var users = await userService.GetAllUsersAsync();
                            foreach (var u in users)
                            {
                                Console.WriteLine($"- {u.FullName} ({u.Email})");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;

                    case "0":
                        keepRunning = false;
                        Console.WriteLine("Goodbye!");
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }
}