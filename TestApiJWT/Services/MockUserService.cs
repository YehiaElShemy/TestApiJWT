using TestApiJWT.Helper;

namespace TestApiJWT.Services
{
    public class MockUserService
    {
        private readonly List<User> _users;

        public MockUserService()
        {
            _users = new List<User>
        {
            new User { Id = 1, Name = "John Doe", Email = "john@example.com" },
            new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com" }
        };
        }

        public IEnumerable<User> GetUsers() => _users;

        public void AddUser(User user) => _users.Add(user);
    }



}
