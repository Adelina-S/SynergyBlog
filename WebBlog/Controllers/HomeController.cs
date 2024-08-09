using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebBlog.Database;
using WebBlog.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace WebBlog.Controllers
{
    public class HomeController : Controller
    {
        private User User { get; set; }
        private static ApplicationContext Database;
        //����������� - ���������� �����������, ���� ����
        public HomeController(ApplicationContext database)
        {
            Database = database;
        }
        //����� ���������� ��������� � ������ ������, ��� ������ �������� ������������ � ��������� ������ ��������
        private void CheckUser()
        {
            if (HttpContext == null) return;
            var name = HttpContext.User?.Identity?.Name;
            if (name != null)
            {
                User = Database.GetUser(name);
                ViewData["Name"] = User.Name;
                ViewData["IsHaveUser"] = true;
            }
            else
                ViewData["IsHaveUser"] = false;
        }
        //������� �������� ����� ����������
        [HttpGet] //������� - ������������� ��������� �����. HttpGet - ����������, ���� � ��� ������ �� ��������� ������
        public IActionResult Index()
        {
            CheckUser();
            Response.Cookies.Append("PostDate", DateTime.Now.Ticks.ToString());
            return View();
        }
        [HttpGet]
        public IActionResult Filter(string filterUser, string filterTags)
        {
            CheckUser();
            Response.Cookies.Append("PostDate", DateTime.Now.Ticks.ToString());
            ViewData["filterUser"] = filterUser;
            ViewData["filterTags"] = filterTags;
            return View();
        }
        [HttpPost]
        public IActionResult Filter(string filterUser, string filterTags, int page)
        {
            CheckUser();
            //��������� �� ����� ���� � �����, ����� ������������ ������ ��� ����� �� ��������.
            string dateTicks = Request.Cookies.ContainsKey("PostDate") ? Request.Cookies["PostDate"] : null;
            DateTime postDate = DateTime.Now;
            if (dateTicks != null)
            {
                long ticks;
                if (long.TryParse(dateTicks, out ticks))
                    postDate = new DateTime(ticks);
            }
            //��������� �� �� ����� ���������, � ������������ �� ��������� � �����, � ������� ��������� �����, � ����� ��������
            List<string> splittedtags= new List<string>();
            Common.SplitTags(filterTags, out splittedtags);
            var messages = Database.GetMessagesFiltered(page, postDate, User, filterUser, splittedtags);
            return View("/Views/Home/Ajax/AllMessages.cshtml", messages);
        }
        [HttpGet]
        public IActionResult Login()
        {
            CheckUser();
            return View();
        }
        [HttpPost] //HttpPost - ����������, ���� ������� ��� � ��� Post ������
        public async Task<IActionResult> Login(string login, string password)
        {
            CheckUser();
            User result = Database.CheckUser(login, password);
            if (result == null)
                return View("Error");
            else
            {
                List<Claim> claims = new List<Claim> { new Claim(ClaimTypes.Name, result.Login) };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return Redirect("/Home/Index");
            }
        }
        [HttpGet]
        [Authorize] //�������, ���������� ��� ���� ����� ������� ������ ����� �������� �����������
        public IActionResult CreateMessage()
        {
            CheckUser();
            return View();
        }
        [HttpPost]
        [Authorize] //�������, ���������� ��� ���� ����� ������� ������ ����� �������� �����������
        public string CreateMessage(string Title, string Text, bool IsHidden, string Tags)
        {
            CheckUser();
            List<string> tagsList = null;
            var splitResult = Common.SplitTags(Tags, out tagsList);
            if (string.IsNullOrEmpty(splitResult) == false)
                return splitResult;
            Database.CreateMessage(Title, Text, tagsList, IsHidden, User);
            return "";
        }
        [HttpGet]
        public IActionResult Message (int id)
        {
            CheckUser();
            var message=Database.GetMessage(id);
            if (message == null)
                return View("Error", "��������� �� ������� ��� �������");
            return View("Message", message);
        }
        [HttpPost, Authorize]
        public IActionResult CreateComment(int messageId, string textComment)
        {
            CheckUser();
            var message=Database.GetMessage(messageId);
            if (message == null)
                return View("Error", "��������� �� ������� ��� �������");
            Database.AddComment(User, textComment, message);
            return Redirect($"/Home/Message?id={messageId}");
        }
        //����� ��������� �������� �� ������ �����
        [HttpPost, Authorize]
        public string Subscribe(int messageId)
        {
            CheckUser();
            var message = Database.GetMessage(messageId);
            if (message == null)
                return "��������� �� ������� ��� �������";
            if (User == message.User)
                return "���������� ����������� �� ����";
            Database.AddSubscribe(User, message.User);
            return "";
        }
        //����� ������� �������� �� ������ �����
        [HttpPost, Authorize]
        public string UnSubscribe(int messageId)
        {
            CheckUser();
            var message = Database.GetMessage(messageId);
            if (message == null)
                return "��������� �� ������� ��� �������";
            if (User == message.User)
                return "���������� ���������� �� ����";
            Database.RemoveSubscribe(User, message.User);
            return "";
        }
        //����� ���������� �������� � ����������
        [HttpGet, Authorize]
        public IActionResult MySubscribes()
        {
            CheckUser();
            return View("MySubscribes");
        }
        //����� ���������� ��������
        [HttpPost, Authorize]
        public IActionResult GetMySubscribes(int page)
        {
            CheckUser();
            var subscibes = Database.GetSubscribes(User, page);
            return View("/Views/Home/Ajax/SubscribeList.cshtml", subscibes);
        }
        //����������� ��������� �� ������ �������
        [HttpGet, Authorize]
        public IActionResult MyMessages()
        {
            CheckUser();
            return View("MyMessages");
        }
        //����� �������� ������ ���� ����� � ���������� ��
        [HttpPost, Authorize]
        public IActionResult GetMyMessages(int page)
        {
            CheckUser();
            var messages = Database.GetMessages(User, page);
            foreach (var sm in messages)
                sm.IsSelfMessage = true;
            return View("/Views/Home/Ajax/AllMessages.cshtml", messages);
        }
        //����� ���������� �������� � �����������
        [HttpPost]
        public IActionResult GetMessages(int page)
        {
            CheckUser();
            //��������� �� ����� ���� � �����, ����� ������������ ������ ��� ����� �� ��������.
            string dateTicks = Request.Cookies.ContainsKey("PostDate") ? Request.Cookies["PostDate"] : null;
            DateTime postDate= DateTime.Now;
            if (dateTicks!=null)
            {
                long ticks;
                if (long.TryParse(dateTicks, out ticks))
                    postDate=new DateTime(ticks);
            }
            //��������� �� �� ����� ���������, � ������������ �� ��������� � �����, � ������� ��������� �����
            var messages = Database.GetAllMessages(page, postDate, User);
            return View("/Views/Home/Ajax/AllMessages.cshtml", messages);
        }
        [HttpPost]
        public IActionResult FilterMessages(string userFilter, string tagFilter)
        {
            CheckUser();

            return View();
        }
        //����� ���������� �������� � �������������
        //����� �� ������� ������
        [HttpGet]
        public async Task<IActionResult> Logout ()
        {
            CheckUser();
            if (User!=null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            return Redirect("/Home/Index");
        }
        //����������� ������ ������������
        [HttpGet]
        public IActionResult Register()
        {
            CheckUser();
            return View();
        }
        [HttpPost] 
        public string Register(string login, string password, string name)
        {
            CheckUser();
            var user=Database.GetUser(login);
            if (user!=null) 
                return "������������ � ����� Login�� ��� ���������������!";
            Database.AddUser(login, password, name);
            return "";
        }
    }
}
