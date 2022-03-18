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
        public UserDto GetUserById(int id)
        {
            var model = _userService.GetById(id);
            return new UserDto(model.Id, model.Login, model.Email);
        }


        [HttpGet]
        public IEnumerable<UserDto> GetAllUsers()
        {
            return _userService.GetAll()
                .Select(user => new UserDto(user.Id, user.Login, user.Email));
        }


        [HttpPost]
        public void CreateUser(CreateUserDto model)
        {
            _userService.Create(new CreateUser(model.Login, model.Email));
        }


        [HttpPut("{id}")]
        public void UpdateUser(int id, UserDto model)
        {
            _userService.Update(new User(id, model.Login, model.Email));
        }
 

        [HttpDelete("{id}")]
        public void DeleteUserById(int id)
        {
            _userService.DeleteById(id);
        }

    }
}
