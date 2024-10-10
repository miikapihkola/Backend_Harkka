using Backend_Harkka.Models;

namespace Backend_Harkka.Services
{
    public interface IMessageService
    {
        Task<IEnumerable<MessageDTO>> GetMessagesAsync();
        Task<IEnumerable<MessageDTO>> GetMySentMessagesAsync(User user); //muuta user as username
        Task<IEnumerable<MessageDTO>> GetMyReceivedMessagesAsync(User user); //muuta user as username
        Task<MessageDTO?> GetMessageAsync(long id);
        Task<MessageDTO> NewMessageAsync(MessageDTO message);
        Task<bool> UpdateMessageAsync(MessageDTO message);
        Task<bool> DeleteMessageAsync(long id);

    }
}
