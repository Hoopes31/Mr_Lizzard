using System;
using System.Linq;
using Newtonsoft.Json;
using scaffold.Models;
using DbConnection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace scaffold.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserFactory userFactory;
        public HomeController(UserFactory connect)
        {
            userFactory = connect;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register(UserViewModel model)
        {
            if(ModelState.IsValid)
            {
                User usernameFinder = userFactory.FindByusername(model.username);

                if (usernameFinder != null)
                {
                    ModelState.AddModelError("username", "username address must be unique");
                }

                PasswordHasher<User> hasher = new PasswordHasher<User>();

                User newUser = new User
                {
                    first_name = model.first_name,    
                    last_name = model.last_name,    
                    username = model.username,   
                    password = model.password,
                };
                newUser.password = hasher.HashPassword(newUser, newUser.password);
                userFactory.AddNewUser(newUser);

                User returnedUser = userFactory.FindByusername(model.username);
                HttpContext.Session.SetInt32("id", returnedUser.id);
                return RedirectToAction("Index", "Auction");
            }
            
            return View(model);
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                LoginModel user = new LoginModel
                {
                    username = model.username,
                    password = model.password
                };

                User returnedUser = userFactory.Login(user);
                if (returnedUser != null)
                {
                    var Hasher = new PasswordHasher<User>();
                    if (0 != Hasher.VerifyHashedPassword(returnedUser, returnedUser.password, user.password))
                    {
                        HttpContext.Session.SetInt32("id", returnedUser.id);
                        return RedirectToAction("Index", "Auction");
                    }
                }
                ModelState.AddModelError("username", "Invalid Login Information");
            }
            return View(model);
        }
        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            return RedirectToAction("Index");
        }
    }
}
