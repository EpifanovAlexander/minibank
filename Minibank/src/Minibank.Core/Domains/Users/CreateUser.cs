namespace Minibank.Core.Domains.Users
{
    public class CreateUser
    {
        public string Login { get; set; }
        public string Email { get; set; }

        public CreateUser(string login, string email)
        {
            Login = login;
            Email = email;
        }
    }
}