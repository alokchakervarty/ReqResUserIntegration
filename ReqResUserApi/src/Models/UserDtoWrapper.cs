using System.Collections.Generic;

namespace ReqResUserApi.Models
{
    public class UserDtoWrapper
    {
        public int page { get; set; }
        public int total_pages { get; set; }
        public List<UserDto> data { get; set; }
    }
}