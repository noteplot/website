using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NPTest1.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Authentication;

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
        public async Task<IActionResult> LoginInput(LoginViewModel lg) // �������������� TO DO: ������� �����������
        {
            if (ModelState.IsValid)
            {
                try
                {
                    UserAccount us = repo.LogIn(lg.LoginName, lg.Password); // �������������� TO DO: ������� �����������
                    if (us != null)
                    {
                        try
                        {
                            await Authenticate(lg.LoginName,lg.RememberMe); // �������������� TO DO: ������� �����������

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
                        return BadRequest("������������ � ����� ������ � ������� �� ������!");
                        //ViewBag.ErrMessage = "������������ � ����� ������ � ������� �� ������!";
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
                //ViewBag.ErrMessage = "������������ � ����� ������ � ������� �� ������!";
                //return PartialView("LoginView", lg);
                return BadRequest("������������ � ����� ������ � ������� �� ������!");
            }
        }

        private async Task Authenticate(string userName, bool IsPers)
        {
            // ������� ���� claim
            var claims = new List<Claim>{new Claim(ClaimsIdentity.DefaultNameClaimType, userName)};
            // ������� ������ ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // ��������� ������������������ ����
            await HttpContext.Authentication.SignInAsync("NotePlotCookies", new ClaimsPrincipal(id), new AuthenticationProperties{
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