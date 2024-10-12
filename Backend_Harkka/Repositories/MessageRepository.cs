using Backend_Harkka.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Backend_Harkka.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        const int itemsPerPage = 20;

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
        public async Task<IEnumerable<Message>> GetMessagesAsync(int page)
        {
            
            if (page < 1) page = 1;
            int maxPage = await GetLastPageAsync();
            if (page > maxPage) page = maxPage;
            page--; // Testaa tarviiko näitä
            Range r = new Range(page, page + itemsPerPage);
            return await _context.Messages.Include(s=>s.Sender).Where(x => x.Recipient == null).OrderByDescending(x => x.SendTime).Take(r).ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMyReceivedMessagesAsync(User user, int page)
        {
            Range r = new Range(0, 20);
            return await _context.Messages.Include(s => s.Sender).Where(x => x.Recipient == user).OrderByDescending(x => x.SendTime).Take(r).ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMySentMessagesAsync(User user, int page)
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
        public async Task<int> GetLastPageAsync()
        {
            // Miksi await ei toimi?
            long lastId = _context.Messages.OrderByDescending(x => x.SendTime).LastOrDefault().Id;
            double lastPage = lastId / itemsPerPage;
            if (lastPage - Convert.ToInt32(lastPage) == 0) return Convert.ToInt32(lastPage); 
            return Convert.ToInt32(lastPage) + 1;
        }
    }
}
