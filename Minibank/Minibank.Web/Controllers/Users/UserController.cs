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
        public async Task<UserDto> GetUserById(int id, CancellationToken cancellationToken)
        {
            var model = await _userService.GetById(id, cancellationToken);
            return new UserDto(model.Id, model.Login, model.Email);
        }


        [HttpGet]
        public async Task<List<UserDto>> GetAllUsers(CancellationToken cancellationToken)
        {
            return (await _userService.GetAll(cancellationToken))
                .Select(user => new UserDto(user.Id, user.Login, user.Email))
                .ToList();
        }


        [HttpPost]
        public async Task CreateUser(CreateUserDto model, CancellationToken cancellationToken)
        {
            await _userService.Create(new CreateUser(model.Login, model.Email), cancellationToken);
        }


        [HttpPut("{id}")]
        public async Task UpdateUser(int id, CreateUserDto model, CancellationToken cancellationToken)
        {
            await _userService.Update(new User(id, model.Login, model.Email), cancellationToken);
        }
 

        [HttpDelete("{id}")]
        public async Task DeleteUserById(int id, CancellationToken cancellationToken)
        {
            await _userService.DeleteById(id, cancellationToken);
        }

    }
}
