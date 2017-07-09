using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Authentication;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NotePlot.Controllers
{
    public class LoginController : Controller
    {
        IRepositoryLogin repo;
        public LoginController(IRepositoryLogin r)
        {
            repo = r;
        }
        [HttpGet]
        public ActionResult LoginInput()
        {
            //LoginViewModel lg = new LoginViewModel();
            return PartialView("Views/LogIn/LoginView.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginInput(LoginViewModel lg) // аутентификация TO DO: сделать асинхронным
        {
            if (ModelState.IsValid)
            {
                try
                {
                    UserAccount us = repo.LogIn(lg.LoginName, lg.Password); // аутентификация TO DO: сделать асинхронным
                    if (us != null)
                    {
                        try
                        {
                            await Authenticate(lg.LoginName, lg.RememberMe); // аутентификация TO DO: сделать асинхронным

                            return RedirectToAction("Index", "Home");
                            //return Ok();
                        }
                        catch (Exception ex)
                        {
                            return BadRequest(ex.Message);
                            //ViewBag.ErrMessage = ex.Message;
                            //return PartialView("LoginView", lg);
                        }
                    }
                    else
                    {
                        return BadRequest("Пользователь с таким именем и паролем не найден!");
                        //ViewBag.ErrMessage = "Пользователь с таким именем и паролем не найден!";
                        //return PartialView("LoginView", lg);

                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                    //ViewBag.ErrMessage = ex.Message;
                    //return PartialView("LoginView", lg);
                }
            }
            else
            {
                //ViewBag.ErrMessage = "Пользователь с таким именем и паролем не найден!";
                //return PartialView("LoginView", lg);
                return BadRequest("Пользователь с таким именем и паролем не найден!");
            }
        }

        private async Task Authenticate(string userName, bool IsPers)
        {
            // создаем один claim
            var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, userName) };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.Authentication.SignInAsync("NotePlotCookies", new ClaimsPrincipal(id), new AuthenticationProperties
            {
                IsPersistent = IsPers,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(3)
            });
        }

        public async Task<IActionResult> LoginOut()
        {
            await HttpContext.Authentication.SignOutAsync("NotePlotCookies");
            return RedirectToAction("Index", "Home");
        }
    }
}
