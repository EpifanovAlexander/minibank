namespace Minibank.Core.Domains.Users
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }

        public User(int id, string login, string email)
        {
            Id = id;
            Login = login;
            Email = email;
        }
    }
}
