using System.ComponentModel.DataAnnotations;

namespace Backend_Harkka.Models
{
    public class User
    {
        public long Id { get; set; }
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; }
        [MaxLength(25)]
        public string? FirstName { get; set; }
        [MaxLength(25)]
        public string? LastName { get; set; }
        [EmailAddress, MaxLength(100)]
        public string? Email { get; set; }
        [StringLength(100, MinimumLength = 4)]
        public string Password { get; set; }
        public DateTime? UserCreated { get; set; }
        public DateTime? LastLogin { get; set; }
        public long? MessagesSent { get; set; }
        public long? MessagesReceived { get; set; }
        public long? UnreadMessages {  get; set; }
    }
    public class UserDTO
    {
        public string UserName { get; set; }
        [MaxLength(25)]
        public string? FirstName { get; set; }
        [MaxLength(25)]
        public string? LastName { get; set; }
        [EmailAddress, MaxLength(100)]
        public string? Email { get; set; }
        [StringLength(100, MinimumLength = 4)]
        public DateTime? UserCreated { get; set; }
        public DateTime? LastLogin { get; set; }
        public long? MessagesSent { get; set; }
        public long? MessagesReceived { get; set; }
        public long? UnreadMessages { get; set; }
    }
}
