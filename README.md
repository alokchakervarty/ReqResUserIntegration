
# ReqRes User Integration - Console App (.NET Core)

This is a console-based .NET Core application that demonstrates the use of:
- HTTP Client Factory to call public APIs
- Dependency Injection
- In-memory caching (`IMemoryCache`)
- Polly for transient fault handling (retry policies)
- Configuration using `appsettings.json`
- xUnit-based unit testing with `Moq`

The app fetches user information from the [https://reqres.in](https://reqres.in) public API and caches responses.

---

## 🚀 Features

- Get user by ID (with cache support)
- Get all users
- Console menu interface
- Smart retry logic using Polly
- Caching with expiration (5 minutes)
- Unit-tested service layer using Moq

---

## 📦 Project Structure

```
ReqResUserIntegrationSolution/
│
├── ReqResUserAPI/
		├──	ApiConfiguration/
		│	└── ApiConfiguration.cs
		├── Clients/
		│   └── IReqResApiClient.cs, ReqResApiClient.cs
		├── Services/
		│   └── ExternalUserService.cs, IExternalUserService.cs
		├── Models/
		│   └── UserDto.cs,User.cs,UserDtoWrapper.cs
		|── appsettings.json
├── ReqResUserApi.ConsoleDemo/
│   └── Program.cs
├── ReqResUserApi.Tests/
│   ├── UnitTest1.cs

└── README.md
```

---

## ⚙️ Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- Visual Studio 2022+ or VS Code
- Internet access to call the ReqRes API

---

## 🛠️ Setup & Run

1. **Clone the repo:**

```
git clone https://github.com/alokchakervarty/ReqResUserIntegration.git
```OR
Use UI to Clone the project And then click on sln file to open the project.

2. **Build the solution:** ctrl + Shift + B
	
3. **Run the application:** F5


## 🧪 Run Unit Tests ctrl + R,A


> The tests use **Moq** to verify the behavior of `ExternalUserService` in isolation.

---

## ⚙️ Configuration

Edit the `appsettings.json` file:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://reqres.in/api/",
	"UserCacheExpirationMinutes": 5
  }
}
```

The `ApiConfiguration` class is bound to this section using the Options pattern.

---

## 📋 Sample Output

```
Menu:
1. Get user by ID
2. Get all users
0. Exit
Enter your choice (0, 1 or 2): 1
Enter User ID to fetch: 5
[CACHE MISS] Fetching user 5 from API.
[INFO] HTTP request sent to https://reqres.in/api/users/5
[CACHE STORE] User 5 cached for 5 minutes.

User: Charles Morris (charles.morris@reqres.in)
```

On repeated fetch:
```
[CACHE HIT] Returning user 5 from memory.
```

---

## 🧠 Design Decisions

- **IMemoryCache**: Added to avoid unnecessary API hits; improves performance.
- **Polly Retry Policy**: Handles transient errors with exponential backoff.
- **Service Abstraction**: Separation of concerns allows easy testing and mocking.
- **Testable Architecture**: Tests do not rely on real HTTP calls; everything is mocked.

## 📄 License

MIT License – Feel free to use and extend this app!

---

## 👤 Author

Alok Chakervarty  
.NET Core Developer | SQL Expert | Full-stack Consultant  
