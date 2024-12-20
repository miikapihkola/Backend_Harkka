﻿using Backend_Harkka.Models;

namespace Backend_Harkka.Services
{
    public interface IMessageService
    {
        Task<IEnumerable<MessageDTO>> GetMessagesAsync(int page);
        Task<IEnumerable<MessageDTO>> GetMySentMessagesAsync(string userName, int page);
        Task<IEnumerable<MessageDTO>> GetMyReceivedMessagesAsync(string userName, int page);
        Task<MessageDTO?> GetMessageAsync(long id);
        Task<MessageDTO> NewMessageAsync(MessageDTO message);
        Task<bool> UpdateMessageAsync(MessageDTO message);
        Task<bool> DeleteMessageAsync(long id);
        Task<bool> SoftDeleteMessageAsync(long id);

    }
}
