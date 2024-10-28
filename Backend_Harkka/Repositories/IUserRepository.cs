using Backend_Harkka.Models;

namespace Backend_Harkka.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User?> GetUserAsync(long id);
        Task<User?> GetUserAsync(string username);
        Task<User?> NewUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(User user);
        Task<bool> SoftDeleteUserAsync(User user);
    }
}
