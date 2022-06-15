using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WatchNotes.Models;

namespace WatchNotes.Controllers
{
    public class AccountController : Controller
    {

        //Sample Users Data, it can be fetched with the use of any ORM    
        public List<UserModel> users = null;
        public AccountController(IOptions<UserModel> defaultUser)
        {
           var DefaultUser = defaultUser.Value;
            users = new List<UserModel>
            {
                new()
                {
                    Username = DefaultUser.Username,
                    Password = DefaultUser.Password,
                }
            };
        }
        public IActionResult Login(string ReturnUrl = "/")
        {
            var objLoginModel = new LoginModel
            {
                ReturnUrl = ReturnUrl
            };
            return View(objLoginModel);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel objLoginModel)
        {
            if (!ModelState.IsValid) return View(objLoginModel);
            var user = users.FirstOrDefault(x => x.Username == objLoginModel.UserName && x.Password == objLoginModel.Password);
            if (user == null)
            {
                  
                ViewBag.Message = "Failed.";
                return View(objLoginModel);
            }

            var claims = new List<Claim>() {
                new(ClaimTypes.Name, user.Username),
            };
                
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
            {
                IsPersistent = true
            });
            return LocalRedirect(objLoginModel.ReturnUrl);
        }
        public async Task<IActionResult> LogOut()
        {
            //SignOutAsync is Extension method for SignOut    
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //Redirect to home page    
            return LocalRedirect("/");
        }
    }
}