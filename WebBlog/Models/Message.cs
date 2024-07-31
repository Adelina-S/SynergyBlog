namespace WebBlog.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsHidden { get; set; }
    }
}
