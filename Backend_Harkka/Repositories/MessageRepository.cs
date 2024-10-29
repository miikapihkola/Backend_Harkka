using Azure;
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
        public async Task<bool> SoftDeleteMessageAsync(Message message)
        {
            if (message == null)
            {
                return false;
            }
            else
            {
                message.Title = "Deleted Message";
                message.Body = "Deleted Message";
                message.EditTime = DateTime.Now;
                message.IsDeleted = true;
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
            long itemCount = (await _context.Messages.Where(x => x.Recipient == null).Where(x => x.IsDeleted == false).OrderByDescending(x => x.SendTime).ToListAsync()).Count;
            int pageOut = ConfirmPage(itemCount, page);            
            return await _context.Messages.Include(s=>s.Sender).Where(x => x.Recipient == null).Where(x => x.IsDeleted == false).OrderByDescending(x => x.SendTime).Skip((pageOut - 1) * itemsPerPage).Take(itemsPerPage).ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMyReceivedMessagesAsync(User user, int page)
        {
            long itemCount = (await _context.Messages.Where(x => x.Recipient == user).Where(x => x.IsDeleted == false).OrderByDescending(x => x.SendTime).ToListAsync()).Count;
            int pageOut = ConfirmPage(itemCount, page);
            return await _context.Messages.Include(s => s.Sender).Where(x => x.Recipient == user).Where(x => x.IsDeleted == false).OrderByDescending(x => x.SendTime).Skip((pageOut - 1) * itemsPerPage).Take(itemsPerPage).ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMySentMessagesAsync(User user, int page)
        {
            long itemCount = (await _context.Messages.Where(x => x.Sender == user).Where(x => x.IsDeleted == false).OrderByDescending(x => x.SendTime).ToListAsync()).Count;
            int pageOut = ConfirmPage(itemCount, page);
            return await _context.Messages.Include(s => s.Recipient).Where(x => x.Sender == user).Where(x => x.IsDeleted == false).OrderByDescending(x => x.SendTime).Skip((pageOut - 1) * itemsPerPage).Take(itemsPerPage).ToListAsync();
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
        public int ConfirmPage(long itemCount, int page)
        {
            int maxPage;
            if (page < 1) page = 1;
            double lastPage = Convert.ToDouble(itemCount) / itemsPerPage;
            if (lastPage - Convert.ToInt32(lastPage) == 0)  maxPage = Convert.ToInt32(lastPage); 
            else maxPage = Convert.ToInt32(lastPage) + 1;
            if (page > maxPage) page = maxPage;
            return page;
        }
    }
}
