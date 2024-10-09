using System.ComponentModel.DataAnnotations;

namespace Backend_Harkka.Models
{
    public class Message
    {
        public long Id { get; set; }
        [StringLength(100, MinimumLength = 1)]
        [Required]
        public string Title { get; set; }
        [StringLength(1000, MinimumLength = 1)]
        [Required]
        public string Body { get; set; }
        public User Sender { get; set; }
        public User? Recipient { get; set; }
        public Message? PrevMessage { get; set; }
        public DateTime? SendTime { get; set; }
        public DateTime? EditTime { get; set; }
    }
    public class MessageDTO
    {
        public long Id { get; set; }
        [StringLength(100, MinimumLength = 1)]
        public string Title { get; set; }
        [StringLength(1000, MinimumLength = 1)]
        public string Body { get; set; }
        public string Sender { get; set; }
        public string? Recipient { get; set; }
        public long? PrevMessageId { get; set; }
        public DateTime? SendTime { get; set; }
        public DateTime? EditTime { get; set; }
    }
}
