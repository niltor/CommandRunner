using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CommandRunner.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace CommandRunner.Controllers
{
    [Authorize(Policy = "Admin")]
    public class HomeController : Controller
    {

        /// <summary>
        /// Login page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            String username = HttpContext.Session.GetString("username");

            if (username != null)
            {
                return RedirectToAction(controllerName: "WebHook", actionName: "Index");
            }
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(String username, String password)
        {

            UserInfo userInfo = await UserHelper.GetUserAsync();

            password = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password)));
            if (username == userInfo.UserName && password == userInfo.Password)
            {
                // identity field infomatioin
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,username),
                    new Claim(ClaimTypes.Role,"Admin")
                };

                //create identity
                var claimsIdentity = new ClaimsIdentity(claims, "Identity");


                var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.Authentication.SignInAsync("CommandRunnerCookies", claimsPrinciple);

                HttpContext.Session.SetString("username", username);

                return RedirectToAction("Index", "WebHook");

            }
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.Authentication.SignOutAsync("CommandRunnerCookies");
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}
