using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBlog.Models
{
    public class Comment
    {
        public Guid Id { get; set; } 
        [Required]
        public User User { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public Message Message { get; set; }
        [ForeignKey("Message")]
        public int MessageId { get; set; }
        [Required]
        public string Text { get; set; }
        public DateTime CreateDate { get; set; }
        public Comment()
        {

        }
        public Comment(User user, Message message, string text)
        {
            Id = Guid.NewGuid();
            User = user;
            Message = message;
            Text = text;
            CreateDate = DateTime.Now;
        }
    }
}
