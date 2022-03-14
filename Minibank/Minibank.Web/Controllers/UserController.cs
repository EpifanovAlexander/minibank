using Microsoft.AspNetCore.Mvc;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Services;
using Minibank.Web.Dtos;

namespace Minibank.Web.Controllers
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


        [HttpGet("GetUser/{id}")]
        public UserDto Get(int id)
        {
            var model = _userService.Get(id);
            return new UserDto(model.Id, model.Login, model.Email);
        }

        [HttpGet("GetAllUsers")]
        public IEnumerable<UserDto> GetAll()
        {
            return _userService.GetAll()
                .Select(user => new UserDto(user.Id, user.Login, user.Email));
        }


        [HttpPost("CreateUser")]
        public void Create(UserDto model)
        {
            _userService.Create(new User(model.Id, model.Login, model.Email));
        }



        [HttpPut("UpdateUser/{id}")]
        public void Update(int id, UserDto model)
        {
            _userService.Update(new User(id, model.Login, model.Email));
        }

 

        [HttpDelete("DeleteUser/{id}")]
        public void Delete(int id)
        {
            _userService.Delete(id);
        }

    }
}
