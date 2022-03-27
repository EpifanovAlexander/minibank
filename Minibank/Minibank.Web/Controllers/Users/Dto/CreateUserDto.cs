namespace Minibank.Web.Controllers.Users.Dto
{
    public class CreateUserDto
    {
        public string Login { get; set; }
        public string Email { get; set; }

        public CreateUserDto(string login, string email)
        {
            Login = login;
            Email = email;
        }
    }
}
