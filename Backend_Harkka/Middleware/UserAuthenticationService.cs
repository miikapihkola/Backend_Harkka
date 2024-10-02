using Backend_Harkka.Models;
using Backend_Harkka.Repositories;

namespace Backend_Harkka.Middleware
{
    public interface IUserAuthenticationService
    {
        Task<User?> Authenticate(string username, string password);
    }
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly IUserRepository _userRepository;

        public UserAuthenticationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<User?> Authenticate(string username, string password)
        {
            User? user;

            user = await _userRepository.GetUserAsync(username);
            if (user == null)
            {
                return null;
            }
            if (password != user.Password)
            {
                return null;
            }
            return user;
        }
    }
}
