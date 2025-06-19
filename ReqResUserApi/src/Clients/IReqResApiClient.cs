using ReqResUserApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReqResUserApi.Clients
{
    public interface IReqResApiClient
    {
        Task<UserDto> GetUserByIdAsync(int userId);
        Task<List<UserDto>> GetUsersByPageAsync(int pageNumber);
    }
}