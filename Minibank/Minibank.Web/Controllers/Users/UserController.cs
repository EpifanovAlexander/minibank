using Microsoft.AspNetCore.Mvc;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Services;
using Minibank.Web.Controllers.Users.Dto;

namespace Minibank.Web.Controllers.Users
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet("{id}")]
        public async Task<UserDto> GetUserById(int id)
        {
            var model = await _userService.GetById(id);
            return new UserDto(model.Id, model.Login, model.Email);
        }


        [HttpGet]
        public async IAsyncEnumerable<UserDto> GetAllUsers()
        {
            var users = _userService.GetAll();

            await foreach (var user in users)
            {
                yield return new UserDto(user.Id, user.Login, user.Email);
            }
        }


        [HttpPost]
        public async Task CreateUser(CreateUserDto model)
        {
            await _userService.Create(new CreateUser(model.Login, model.Email));
        }


        [HttpPut("{id}")]
        public async Task UpdateUser(int id, CreateUserDto model)
        {
            await _userService.Update(new User(id, model.Login, model.Email));
        }
 

        [HttpDelete("{id}")]
        public async Task DeleteUserById(int id)
        {
            await _userService.DeleteById(id);
        }

    }
}
