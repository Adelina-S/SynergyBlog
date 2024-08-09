namespace WebBlog.Models
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Message> Messages { get; set; }
        public Tag()
        {
            Messages = new List<Message>();
        }
        public Tag(string name):this()
        {
            Id=Guid.NewGuid();
            Name=name;
        }
    }
}
