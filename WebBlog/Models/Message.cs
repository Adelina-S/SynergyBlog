using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBlog.Models
{
    public class Message
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsHidden { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Tag> Tags { get; set; }
        [NotMapped]
        public bool IsSelfMessage { get; set; }
        [NotMapped]
        public bool IsHaveSubscribe { get; set; }
        public Message()
        {
            Comments = new List<Comment>();
            Tags=new List<Tag>();
        }
        public Message(User user, string title, string text, bool isHidden):this()
        {
            User = user;
            Title = title;
            Text = text;
            IsHidden = isHidden;
            CreateDate = DateTime.Now;
        }
    }
}
