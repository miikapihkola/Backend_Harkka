using Backend_Harkka.Models;
using Backend_Harkka.Repositories;
using NuGet.Packaging.Signing;

namespace Backend_Harkka.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        public MessageService(IMessageRepository messageRepository, IUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }
        public async Task<bool> DeleteMessageAsync(long id)
        {
            Message? message=await _messageRepository.GetMessageAsync(id);
            if (message != null)
            {
                await _messageRepository.DeleteMessageAsync(message);
                return true;
            }
            return false;
        }

        public async Task<MessageDTO?> GetMessageAsync(long id)
        {
           return MessageToDTO(await _messageRepository.GetMessageAsync(id));
        }

        public async Task<IEnumerable<MessageDTO>> GetMessagesAsync(int page)
        {
            IEnumerable<Message> messages = await _messageRepository.GetMessagesAsync(page);
            List<MessageDTO> result = new List<MessageDTO>();
            foreach (Message message in messages)
            {
                result.Add(MessageToDTO(message));
            }
            return result;
        }
        public async Task<IEnumerable<MessageDTO>> GetMySentMessagesAsync(string userName, int page)
        {
            IEnumerable<Message> messages = await _messageRepository.GetMySentMessagesAsync(await _userRepository.GetUserAsync(userName), page);
            List<MessageDTO> result = new List<MessageDTO>();
            foreach (Message message in messages)
            {
                result.Add(MessageToDTO(message));
            }
            return result;
        }

        public async Task<IEnumerable<MessageDTO>> GetMyReceivedMessagesAsync(string userName, int page)
        {
            IEnumerable<Message> messages = await _messageRepository.GetMyReceivedMessagesAsync(await _userRepository.GetUserAsync(userName), page);
            List<MessageDTO> result = new List<MessageDTO>();
            foreach (Message message in messages)
            {
                result.Add(MessageToDTO(message));
            }
            return result;
        }

        public async Task<MessageDTO> NewMessageAsync(MessageDTO message)
        {
            return MessageToDTO(await _messageRepository.NewMessageAsync(await DTOToMessageAsync(message)));
        }

        public async Task<bool> UpdateMessageAsync(MessageDTO message)
        {
            Message? dbMessage = await _messageRepository.GetMessageAsync(message.Id);
            if (dbMessage != null)
            {
                dbMessage.Title = message.Title;
                dbMessage.Body = message.Body;
                dbMessage.EditTime = DateTime.Now;
                return await _messageRepository.UpdateMessageAsync(dbMessage);
            }
            return false;
        }

        private MessageDTO MessageToDTO(Message message)
        {
            MessageDTO dTO = new MessageDTO();

            dTO.Id = message.Id;
            dTO.Title = message.Title;
            dTO.Body = message.Body;
            dTO.Sender = message.Sender.UserName;
            if (message.Recipient != null)
            {
                dTO.Recipient = message.Recipient.UserName;
            }
            if (message.PrevMessage != null)
            {
                dTO.PrevMessageId = message.PrevMessage.Id;
            }
            dTO.SendTime = message.SendTime;
            if (message.EditTime != null)
            {
                dTO.EditTime = message.EditTime;
            }

            return dTO;
        }
        private async Task <Message> DTOToMessageAsync (MessageDTO dTO)
        {
            Message newMessage = new Message();
            newMessage.Id = dTO.Id;
            newMessage.Title = dTO.Title;
            newMessage.Body = dTO.Body;
            newMessage.SendTime = DateTime.Now;

            User? sender = await _userRepository.GetUserAsync(dTO.Sender);
            if (sender != null)
            {
                newMessage.Sender = sender;
                sender.MessagesSent++;
                await _userRepository.UpdateUserAsync(sender);
            }
            if (dTO.Recipient != null)
            {
                User? recipient = await _userRepository.GetUserAsync(dTO.Recipient);
                if (recipient != null)
                {
                    newMessage.Recipient = recipient;
                    recipient.MessagesReceived++;
                    await _userRepository.UpdateUserAsync(recipient);
                }
            }

            if (dTO.PrevMessageId != null && dTO.PrevMessageId != 0)
            {
                Message prevMessage = await _messageRepository.GetMessageAsync((long)dTO.PrevMessageId);
                newMessage.PrevMessage = prevMessage;
            }
            return newMessage;
        }


    }
}
