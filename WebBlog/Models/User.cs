using System.ComponentModel.DataAnnotations.Schema;

namespace WebBlog.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public User()
        {

        }
        public User(string login, string password, string name):this()
        {
            Name= name;
            Login= login;
            Password= password;
        }
    }
}
