using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Sqlite;
using WebBlog.Models;
namespace WebBlog.Database
{

    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Subscribe> Subscribes { get; set; }  
        public DbSet<Tag> Tags { get; set; }
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
            //modelBuilder.Entity<Subscribe>().HasOne(t=>t.Creator).WithMany().OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<Subscribe>().HasOne(t=>t.Target).WithMany().OnDelete(DeleteBehavior.Restrict);

            User adelina = new User("Adelina", "Synergy", "Аделина");
            User jack = new User("Jack", "123", "Евгений");
            User vika = new User("Vika", "123", "Виктория");
            modelBuilder.Entity<User>().HasData(new User[] { adelina, jack, vika });

            //modelBuilder.Entity<User>().Navigation(t => t.Subscribes).AutoInclude();
            modelBuilder.Entity<Message>().Navigation(t => t.User).AutoInclude();
            modelBuilder.Entity<Message>().Navigation(t => t.Comments).AutoInclude();
            modelBuilder.Entity<Message>().Navigation(t => t.Tags).AutoInclude();
            modelBuilder.Entity<Comment>().Navigation(t => t.User).AutoInclude();
            modelBuilder.Entity<Comment>().Navigation(t => t.Message).AutoInclude();
            modelBuilder.Entity<Subscribe>().Navigation(t => t.Target).AutoInclude();
            modelBuilder.Entity<Subscribe>().Navigation(t => t.Creator).AutoInclude();
            //modelBuilder.Entity<Tag>().Navigation(t => t.Messages).AutoInclude();
        }
        //Метод, который добавляет в базу данных нового пользователя
        public User AddUser(string login, string password, string name)
        {
            User user = new User(login, password, name);
            Users.Add(user);
            SaveChanges();
            return user;
        }
        public User CheckUser(string login, string password)
        {
                User result = Users.FirstOrDefault(user => user.Login == login && user.Password == password);
                return result;
        }
        public User GetUser(string login)
        {
                User result = Users.FirstOrDefault(user => user.Login == login);
                return result;
        }
        //Метод, который добавляет в базу данных новый пост и тэги поста
        public void CreateMessage(string Title, string Text, List<string> tagsList, bool IsHidden, User user)
        {
            var message = new Message(user, Title, Text, IsHidden);
            var sameTags = Tags.Where(t => tagsList.Contains(t.Name)).ToList();
            if (sameTags.Count!=tagsList.Count)
            {
                var sameTagsNames = sameTags.Select(t => t.Name).ToList();
                foreach (string tagName in tagsList.Where(t=>sameTagsNames.Contains(t)==false))
                {
                    Tag tag=new Tag(tagName);
                    tag.Messages.Add(message);
                    Tags.Add(tag);
                }
            }
            foreach (var tag in sameTags)
                tag.Messages.Add(message);
            //Messages.Add(message);
            SaveChanges();
        }
        //Метод возвращает все посты пользователя из базы
        public List<Message> GetMessages(User user, int page)
        {
            var query = Messages.Where(message => message.UserId == user.Id);
                return query
                .OrderByDescending(t=>t.CreateDate)
                .Skip(page*20)
                .Take(20)
                .ToList();
        }
        //Метод возвращает последние посты из базы, отсортированные по дате
        public List<Message> GetAllMessages(int page, DateTime startSearchDate, User user)
        {
                var messages = Messages
                    .Where(t=>t.IsHidden==false && t.CreateDate<startSearchDate)
                    .OrderByDescending(t=>t.CreateDate)
                    .Skip(page*20)
                    .Take(20)
                    .ToList();
            AddSubscribes(messages, user);
            /*var messageUsers = messages.Select(t => t.User).ToList();
            var subscribes = Subscribes
                .Where(t => t.Creator == user && messageUsers.Contains(t.Target))
                .Select(t=>t.Target)
                .ToList();
            foreach (var message in messages)
            {
                if (message.User == user) message.IsSelfMessage = true;
                if (subscribes.Contains(message.User)) message.IsHaveSubscribe = true;
            }*/
            return messages;

        }
        private void AddSubscribes(List<Message> messages, User user)
        {
            var messageUsers = messages.Select(t => t.User).ToList();
            var subscribes = Subscribes
                .Where(t => t.Creator == user && messageUsers.Contains(t.Target))
                .Select(t => t.Target)
                .ToList();
            foreach (var message in messages)
            {
                if (message.User == user) message.IsSelfMessage = true;
                if (subscribes.Contains(message.User)) message.IsHaveSubscribe = true;
            }
        }
        public List<Message> GetMessagesFiltered(int page, DateTime startSearchDate, User user, string filterUser, List<string> filterTags)
        {
            IQueryable<Message> messages;
            if (filterTags.Count>0)
            {
                messages = Tags.Include(t => t.Messages).Where(t => filterTags.Contains(t.Name)).SelectMany(t => t.Messages).Distinct()
                    .Where(t => t.IsHidden == false && t.CreateDate < startSearchDate);
            }
            else
            {
                messages = Messages.Where(t => t.IsHidden == false && t.CreateDate < startSearchDate);
            }
            if (string.IsNullOrEmpty(filterUser)==false)
                messages = messages.Where(t => t.User.Name.Contains(filterUser));
            var result = messages.OrderByDescending(t => t.CreateDate)
                    .Skip(page * 20)
                    .Take(20)
                    .ToList();
            if (user!=null) AddSubscribes(result, user);
            return result;
        }
        //Возвращет сообщение по его Id
        public Message GetMessage(int id)
        {
            return Messages.FirstOrDefault(t => t.Id == id);
        }
        //Добавляет новый комментарий к сообщению
        public void AddComment(User user, string textComment, Message message)
        {
            var comment=new  Comment(user, message, textComment);
            Comments.Add(comment);
            message.Comments.Add(comment);
            SaveChanges();
        }
        //Добавляет новую подписку
        public void AddSubscribe(User user, User target)
        {
            var subscribe=new Subscribe(user, target);
            Subscribes.Add(subscribe);
            SaveChanges();
        }
        //Снимает подписку
        public void RemoveSubscribe(User user, User target)
        {
            var subscribe = Subscribes.FirstOrDefault(t=>t.Creator== user && t.Target == target);
            if (subscribe != null) 
            Subscribes.Remove(subscribe);
            SaveChanges();
        }
        //Возвращает список пользователей на которые подписан человек и их последние сообщения
        public List<UserWithMessage> GetSubscribes(User user, int page)
        {
            var users = Subscribes.Where(t => t.Creator == user)
                  .Select(t => t.Target)
                  .Join(Messages, u => u.Id, m => m.UserId, (u, m) => new UserWithMessage { User = u, Message = m })
                  .OrderByDescending(t => t.Message.CreateDate)
                  .Select(t=>t.User)
                  .Distinct()
                  .Skip(page * 20)
                  .Take(20)
                  .ToList();
            var messages = from m in Messages
                         group m by m.UserId into g
                         select g.OrderByDescending(x => x.CreateDate).FirstOrDefault();
            return users.Join(messages, u => u.Id, m => m.UserId, (u, m) => new UserWithMessage { User = u, Message = m }).ToList();


        }
    }
}

