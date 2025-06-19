using ReqResUserApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReqResUserApi.Services
{
    public interface IExternalUserService
    {
        Task<User> GetUserByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}