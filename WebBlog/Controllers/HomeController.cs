using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebBlog.Database;
using WebBlog.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebBlog.Controllers
{
    public class HomeController : Controller
    {

        // онструктор - вызываетс€ об€зательно, если есть
        public HomeController()
        {
            
        }
        [HttpGet] //атрибут - дополнительно описывает метод. HttpGet - вызываетс€, если у нас запрос из командной строки
        [Authorize] //атрибут, показывает что сюда можно перейти только после успешной авторизации
        public IActionResult Index()
        {
            var name = HttpContext.User.Identity.Name;
            var user = ApplicationContext.GetUser(name);
            ViewData["Name"] = user.Login;
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost] //HttpPost - вызываетс€, если указано что у нас Post запрос
        public async Task<IActionResult> Login(string login, string password)
        {
            User result = ApplicationContext.CheckUser(login, password);
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
        [Authorize] //атрибут, показывает что сюда можно перейти только после успешной авторизации
        public IActionResult CreateMessage()
        {
            return View();
        }
        [HttpPost]
        [Authorize] //атрибут, показывает что сюда можно перейти только после успешной авторизации
        public IActionResult CreateMessage(string Title, string Text, bool IsHidden)
        {
            var login = HttpContext.User.Identity.Name;
            ApplicationContext.CreateMessage(Title, Text, IsHidden, login);
            return View();
        }
        //¬ыход из учЄтной записи
        [HttpGet]
        public async Task<IActionResult> Logout ()
        {
            var name = HttpContext.User.Identity.Name;
            var user = ApplicationContext.GetUser(name);
            if (user!=null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            return Redirect("/Home/Login");
        }
        //–егистраци€ нового пользовател€
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost] 
        public string Register(string login, string password)
        {
            var user=ApplicationContext.GetUser(login);
            if (user!=null) 
                return "ѕользователь с таким Loginом уже зарегистрирован!";
            ApplicationContext.AddUser(login, password);
            return "";
        }
    }
}
