using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using WebBlog.Models;
namespace WebBlog.Database
{

    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public ApplicationContext()
        { 
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=base.db");
        }
        //Метод, который заполняет базу данных при её создании
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            User adelina = new User();
            adelina.Login = "Adelina";
            adelina.Password = "Synergy";
            adelina.Id = Guid.NewGuid();
            modelBuilder.Entity<User>().HasData(new User[] { adelina });
        }
        //Метод, который добавляет в базу данных нового пользователя
        public static User AddUser(string login, string password)
        {
            User user=new User();
            user.Login=login;
            user.Password=password;
            user.Id=Guid.NewGuid();

            using (ApplicationContext database = new ApplicationContext())
            {
                database.Users.Add(user);
                database.SaveChanges();
            }
            return user;
        }
        public static User CheckUser(string login, string password)
        {
            using (ApplicationContext database = new ApplicationContext())
            {
                User result = database.Users.FirstOrDefault(user => user.Login == login && user.Password == password);
                return result;
            }
        }
        public static User GetUser(string login)
        {
            using (ApplicationContext database = new ApplicationContext())
            {
                User result = database.Users.FirstOrDefault(user => user.Login == login);
                return result;
            }
        }
        //Метод, который добавляет в базу данных новый пост
        public static void CreateMessage(string Title, string Text, bool IsHidden, string userLogin)
        {
            var message = new Message(); 
            message.Title=Title;
            message.Text=Text;
            message.IsHidden=IsHidden;
            message.Id=Guid.NewGuid();
            message.CreateDate=DateTime.Now;
            using (ApplicationContext database = new ApplicationContext())
            {
                User user = database.Users.First(user => user.Login == userLogin);
                message.UserId=user.Id;
                database.Messages.Add(message);
                database.SaveChanges();
            }

        }
    }
}

