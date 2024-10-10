using Backend_Harkka.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Harkka.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MessageServiceContext _context;
        public MessageRepository(MessageServiceContext context)
        {
            _context = context;
        }
        public async Task<bool> DeleteMessageAsync(Message message)
        {
            if (message == null)
            {
                return false;
            }
            else
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<Message?> GetMessageAsync(long id)
        {
            return await _context.Messages.FindAsync(id);
        }
        public async Task<IEnumerable<Message>> GetMessagesAsync()
        {
            Range r = new Range(0,20);
            return await _context.Messages.Include(s=>s.Sender).Where(x => x.Recipient == null).OrderByDescending(x => x.SendTime).Take(r).ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMyReceivedMessagesAsync(User user)
        {
            Range r = new Range(0, 20);
            return await _context.Messages.Include(s => s.Sender).Where(x => x.Recipient == user).OrderByDescending(x => x.SendTime).Take(r).ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMySentMessagesAsync(User user)
        {
            Range r = new Range(0, 20);
            return await _context.Messages.Include(s => s.Recipient).Where(x => x.Sender == user).OrderByDescending(x => x.SendTime).Take(r).ToListAsync();
        }

        public async Task<Message> NewMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<bool> UpdateMessageAsync(Message message)
        {
            _context.Entry(message).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();                
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
