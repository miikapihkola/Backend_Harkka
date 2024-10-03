using Backend_Harkka.Models;
using Backend_Harkka.Repositories;

namespace Backend_Harkka.Middleware
{
    public interface IUserAuthenticationService
    {
        Task<User?> Authenticate(string username, string password);
        Task<bool> isMyMessage(string username, long messageId);
    }
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;

        public UserAuthenticationService(IUserRepository userRepository, IMessageRepository messageRepository)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
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

        public async Task<bool> isMyMessage(string username, long messageId)
        {
            User? user = await _userRepository.GetUserAsync(username);
            if (user == null)
            {
                return false;
            }
            Message? message = await _messageRepository.GetMessageAsync(messageId);
            if (message == null)
            {
                return false;
            }
            if(message.Sender == user)
            {
                return true;
            }
            return false;

        }
    }
}
