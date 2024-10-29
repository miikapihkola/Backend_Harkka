using Backend_Harkka.Middleware;
using Backend_Harkka.Models;
using Backend_Harkka.Repositories;

namespace Backend_Harkka.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IUserAuthenticationService _userAuthenticationService;
        public UserService(IUserRepository repository, IUserAuthenticationService userAuthenticationService)
        {
            _repository = repository;
            _userAuthenticationService = userAuthenticationService;
        }

        public async Task<bool> DeleteUserAsync(string username)
        {
            User? user = await _repository.GetUserAsync(username);
            if (user != null)
            {
                //Tähän kohtaa message delete loop
                return await _repository.DeleteUserAsync(user);
            }
            return false;
        }
        public async Task<bool> SoftDeleteUserAsync(string username)
        {
            User? user = await _repository.GetUserAsync(username);
            if (user != null)
            {
                //Tähän kohtaa message delete loop
                return await _repository.SoftDeleteUserAsync(user);
            }
            return false;
        }

        public async Task<UserDTO?> GetUserAsync(string username)
        {
            User? user = await _repository.GetUserAsync(username);
            if (user == null)
            {
                return null;
            }
            return UserToDTO(user);
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            IEnumerable<User> users = await _repository.GetUsersAsync();
            List<UserDTO> result = new List<UserDTO>();
            foreach (User user in users)
            {
                result.Add(UserToDTO(user));
            }
            return result;
        }

        public async Task<UserDTO?> NewUserAsync(User user)
        {
            User? dbUser = await _repository.GetUserAsync(user.UserName);
            if (dbUser != null || user.UserName.ToLower().Contains("deleted"))
            {
                return null;
            }
            user.MessagesSent = 0;
            user.MessagesReceived = 0;
            user.IsDeleted = false;

            User? newUser =_userAuthenticationService.CreateUserCredentials(user);

            if (newUser != null)
            {
                return UserToDTO(await _repository.NewUserAsync(user));
            }
            return null;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            User? dbUser = await _repository.GetUserAsync(user.UserName);
            if (dbUser != null)
            {
                dbUser.FirstName = user.FirstName;
                dbUser.LastName = user.LastName;
                dbUser.Email = user.Email;
                dbUser.Password = user.Password;
                dbUser.LastLogin = DateTime.Now;
                return await _repository.UpdateUserAsync(dbUser);
            }
            return false;
        }
        private UserDTO UserToDTO(User user)
        {
            UserDTO dto = new UserDTO();
            dto.UserName = user.UserName;
            dto.FirstName = user.FirstName;
            dto.LastName = user.LastName;
            dto.Email = user.Email;
            dto.UserCreated = user.UserCreated;
            dto.LastLogin = user.LastLogin;
            dto.MessagesSent = user.MessagesSent;
            dto.MessagesReceived = user.MessagesReceived;
            return dto;
        }
    }
}
