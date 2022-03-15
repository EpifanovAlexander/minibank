namespace Minibank.Web.Controllers.Users
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }

        public UserDto(int id, string login, string email)
        {
            Id = id;
            Login = login;
            Email = email;
        }
    }
}
