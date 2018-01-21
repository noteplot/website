﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Models;
using System.Security.Claims;
//using Microsoft.AspNetCore.Http.Authentication; // 1.0
using System.Threading;
using Microsoft.AspNetCore.Authentication;          // 2.0
using Microsoft.AspNetCore.Session;          // 2.0
using Microsoft.AspNetCore.Authentication.Cookies;  // 2.0
using Microsoft.AspNetCore.Http; // для сессий CORE 2.0

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

        [HttpGet]
        public ActionResult LoginRegister()
        {
            //LoginViewModel lg = new LoginViewModel();
            return PartialView("Views/Login/LoginRegistration.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginInput(LoginViewModel lg) // аутентификация TO DO: сделать асинхронным
        {
            //Thread.Sleep(7000);
            if (ModelState.IsValid)
            {
                try
                {
                    //UserAccount us = repo.LogIn(lg.LoginName, lg.Password); // аутентификация TO DO: сделать асинхронным
                    UserAccount us = await repo.LogInAsync(lg.LoginName, lg.Password);
                    if (us != null)
                    {
                        try
                        {
                            await Authenticate(us.LoginID, lg.LoginName, lg.RememberMe); // аутентификация TO DO: сделать асинхронным

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

        private async Task Authenticate(long userId, string userName, bool IsPers)
        {
            // создаем один claim
            var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, userName) };
            //claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.ToString(), ClaimValueTypes.String));            
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            id.AddClaim(new Claim("LoginID", userId.ToString(), ClaimValueTypes.UInteger64));
            // установка аутентификационных куки
            await HttpContext.SignInAsync("NotePlotCookies"/*CookieAuthenticationDefaults.AuthenticationScheme*/, new ClaimsPrincipal(id), new AuthenticationProperties // CORE 2.0
            {
                IsPersistent = IsPers,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(3)
            });            
            /* // CORE 1.0
            await HttpContext.Authentication.SignInAsync("NotePlotCookies", new ClaimsPrincipal(id), new AuthenticationProperties
            {
                IsPersistent = IsPers,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(3)
            });
            */
        }

        public async Task<IActionResult> LoginOut()
        {
            await HttpContext.SignOutAsync("NotePlotCookies"/*CookieAuthenticationDefaults.AuthenticationScheme*/);// CORE 2.0
            //await HttpContext.Authentication.SignOutAsync("NotePlotCookies"); //CORE 1.0
            return RedirectToAction("Index", "Home");
        }

        public static long GetLogin(ClaimsPrincipal cp) //HttpContext.User
        {
            long LoginID = -1;
            if (cp.Identity.IsAuthenticated)
            {
                Claim claimLoginId = cp.Claims.FirstOrDefault(x => x.Type == "LoginID");
                if (claimLoginId != null)
                {
                    try
                    {
                        LoginID = Convert.ToInt64(claimLoginId.Value);
                    }
                    catch
                    {
                    }                    
                }
            }
            return LoginID;
        }
        
        public async Task<ActionResult> Captcha()
        {
            string code = new Random(DateTime.Now.Millisecond).Next(1111, 9999).ToString();
            HttpContext.Session.SetString("captcha", code);
            //Session["captcha"] = code;  CORE 1.0
            using (CaptchaImage captcha = new CaptchaImage(code, 70, 40))
            {
                var memStream = new System.IO.MemoryStream();
                captcha.Image.Save(memStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] imageBytes = memStream.ToArray();
                return await Task.Run(()=> File(imageBytes, "image/jpeg"));
            }            
        }
        
    }   
}
