using Backend_Harkka.Models;

namespace Backend_Harkka.Repositories
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetMessagesAsync(int page);
        Task<IEnumerable<Message>> GetMySentMessagesAsync(User user, int page);
        Task<IEnumerable<Message>> GetMyReceivedMessagesAsync(User user, int page);
        Task<Message?> GetMessageAsync(long id);
        Task<Message> NewMessageAsync(Message message);
        Task<bool> UpdateMessageAsync(Message message);
        Task<bool> DeleteMessageAsync(Message message);
        Task<bool> SoftDeleteMessageAsync(Message message);
    }
}
