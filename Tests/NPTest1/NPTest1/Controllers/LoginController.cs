using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NPTest1.Models;
using System.Security.Claims;

namespace NPTest1.Controllers
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
        public IActionResult LoginInput(LoginViewModel lg) // аутентификация TO DO: сделать асинхронным
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
                            Authenticate(lg.LoginName); // аутентификация TO DO: сделать асинхронным

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

        private void /*async Task*/ Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>{new Claim(ClaimsIdentity.DefaultNameClaimType, userName)};
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);            
            // установка аутентификационных куки
            //await 
                HttpContext.Authentication.SignInAsync("NotePlotCookies", new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> LoginOut()
        {
            await HttpContext.Authentication.SignOutAsync("Cookies");
            return RedirectToAction("Login", "Account");
        }
    }
}