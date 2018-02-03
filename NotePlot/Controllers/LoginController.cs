using System;
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
using System.Net;
using System.Net.Mail; // почта
using System.Web;
using Microsoft.Extensions.Configuration;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NotePlot.Controllers
{
    public class LoginController : Controller
    {
        IRepositoryLogin repo;
        IConfiguration config;
        public LoginController(IRepositoryLogin r, IConfiguration c )
        {
            repo = r;
            config = c;
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
                            if (!us.IsConfirmed)
                            {
                                throw new Exception("Логин не подтвержден!");
                            }
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

        [HttpPost]
        //public async Task<ActionResult> RegisterLogin(RegisterViewModel lg)
        public async Task<ActionResult> RegisterLogin(LoginRegistration lg)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.Session.GetString("captcha") != lg.Captcha)
                {
                    return BadRequest("Неверное число!");
                }
                if (lg.Password != lg.ConfirmPassword)
                {
                    return BadRequest("Пароль подтвержден не верно!");
                }
                try
                {
                    long loginId = await repo.CreateLoginAsync(lg.Email, lg.Password);

                    if (loginId > 0 )
                    {
                        try
                        {
                            /*
                            var mail = config["SmtpParameters:mail"];
                            var mailPassword = config["SmtpParameters:mailPassword"];
                            var host = config["SmtpParameters:host"];
                            var port = Convert.ToInt32(config["SmtpParameters:port"]);

                            // наш email с заголовком письма
                            MailAddress from = new MailAddress(mail, "Web Registration");
                            // кому отправляем
                            MailAddress to = new MailAddress(lg.Email);
                            // создаем объект сообщения
                            MailMessage m = new MailMessage(from, to);
                            // тема письма
                            m.Subject = "Email confirmation";
                            m.Body = string.Format("Для завершения регистрации перейдите по ссылке:" +
                                                       "<a href=\"{0}\" title=\"Подтвердить регистрацию\">{0}</a>",
                                        Url.Action("ConfirmLogin", "Login", new { LoginId = loginId }, Request.Scheme)); //Получение полного URL-адреса действия в ASP.NET MVC
                            m.IsBodyHtml = true;
                            // адрес smtp-сервера, с которого мы и будем отправлять письмо
                            SmtpClient smtp = new SmtpClient(host, port);
                            // логин и пароль
                            smtp.Credentials = new NetworkCredential(mail, mailPassword);
                            smtp.EnableSsl = true;
                            smtp.Send(m);
                            */
                            var subj = "Email confirmation";
                            var body =string.Format("Для завершения регистрации перейдите по ссылке:" +
                                                                   "<a href=\"{0}\" title=\"Подтвердить регистрацию\">{0}</a>",
                                                    Url.Action("ConfirmLogin", "Login", new { LoginId = loginId }, Request.Scheme)); //Получение полного URL-адреса действия в ASP.NET MVC

                            SendConfirmation(loginId, lg.Email,subj,body);

                            var mesOk = "Информация для завершения авторизации выслана на ваш e-mail.";
                            return Ok(mesOk);

                        }
                        catch (Exception ex)
                        {
                            return BadRequest(ex.Message);
                        }
                    }
                    else
                    {
                        return BadRequest("Регистрация не доступна!");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                string errmes = string.Empty;
                foreach (var error in ViewData.ModelState.Values.SelectMany(modelState => modelState.Errors))
                {
                    errmes = errmes + error.ErrorMessage + ' ';
                }
                //return BadRequest("Не все обязательные поля заполнены!");
                return BadRequest(errmes);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmLogin(long loginId)
        {
            string mesOk = "Вы авторизованы";
            try
            {
                await repo.ConfirmLoginAsync(loginId);
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(mesOk);//View("ConfirmLogin");
        }

        [NonAction]
        private void SendConfirmation(long loginId, string email, string subj, string body )
        {
            var mailFrom = config["SmtpParameters:mail"];
            var mailPassword = config["SmtpParameters:mailPassword"];
            var host = config["SmtpParameters:host"];
            var port = Convert.ToInt32(config["SmtpParameters:port"]);

            // наш email с заголовком письма
            MailAddress from = new MailAddress(mailFrom, "Web Registration");
            // кому отправляем
            MailAddress to = new MailAddress(email);
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = subj;//"Email confirmation";
            m.Body = body;
/*
                string.Format("Для завершения регистрации перейдите по ссылке:" +
                                       "<a href=\"{0}\" title=\"Подтвердить регистрацию\">{0}</a>",
                        Url.Action("ConfirmLogin", "Login", new { LoginId = loginId }, Request.Scheme)); //Получение полного URL-адреса действия в ASP.NET MVC
*/
            m.IsBodyHtml = true;
            // адрес smtp-сервера, с которого мы и будем отправлять письмо
            SmtpClient smtp = new SmtpClient(host, port);
            // логин и пароль
            smtp.Credentials = new NetworkCredential(mailFrom, mailPassword);
            smtp.EnableSsl = true;
            smtp.Send(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginForget(string id) // id - LoginName
        {
            if (id == null)
            {
                return BadRequest("Для восстановления пароля введите логин");
            }

            var mesOk  = "Информация по восстановлению пароля выслана на ваш e-mail.";
            var mesBad = "Указанный логин не зарегистрирован.";
            var mesBadReg = "Необходимо подтвердить регистрацию, ссылка для подтверждения была ранее отправлена на ваш почтовый адрес";

            try
            {
                UserAccount us = await repo.GetLoginAsync(id);
                if (us == null)
                {
                    return BadRequest(mesBad);
                }

                if (!us.IsConfirmed)
                {
                    return BadRequest(mesBadReg);
                }

                var subj = "Восстановление пароля";
                var body = string.Format("Ваш пароль: " + us.Password);
                SendConfirmation(us.LoginID, us.Email, subj, body);
                return Ok(mesOk);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
