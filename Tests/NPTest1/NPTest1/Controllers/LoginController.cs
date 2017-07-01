using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NPTest1.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult InputLogin()
        {
            //LoginViewModel lg = new LoginViewModel();
            return PartialView("Views/LogIn/LoginView.cshtml");
        }

        // ›“Œ
        public ActionResult LoginPanel()
        {
            return PartialView("LoginInputPanelView");
            /*
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                FormsIdentity id = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;
                try
                {
                    UserViewModel us = DBAuthentication.GetLoginById(Convert.ToInt32(ticket.UserData));
                    if (us == null || us.LoginViewName == null)
                        throw new Exception();
                    return PartialView("LoginNamePanelView", us.LoginViewName);
                }
                catch
                {
                    return PartialView("LoginNamePanelView", HttpContext.User.Identity.Name);
                }
            }
            else
                return PartialView("LoginInputPanelView");
            */
        }
    }
}