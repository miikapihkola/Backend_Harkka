using Backend_Harkka.Models;

namespace Backend_Harkka.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetUsersAsync();
        Task<UserDTO?> GetUserAsync(string username);
        Task<UserDTO?> NewUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(string username);
        Task<bool> SoftDeleteUserAsync(string username);
    }
}
