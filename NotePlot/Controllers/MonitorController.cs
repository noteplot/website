using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using NotePlot.Models;


namespace NotePlot.Controllers
{
    public class MonitorController : Controller
    {
        IRepositoryMonitor repo;
        public MonitorController(IRepositoryMonitor r)
        {
            repo = r;
        }

        // GET:
        public ActionResult MonitorList()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
                return View("MonitorList", repo.GetMonitors(loginID));
            else
                return BadRequest("Нет аутентификации!");
        }
    }
}