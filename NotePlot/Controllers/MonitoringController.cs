using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Net.Http.Headers;
using NotePlot.Models;

namespace NotePlot.Controllers
{
    public class MonitoringController : Controller
    {
        IRepositoryMonitoring repo;
        public MonitoringController(IRepositoryMonitoring r)
        {
            repo = r;
        }

        //Get
        public IActionResult MonitoringList(long id)
        {
            /*
            IHeaderDictionary headersDictionary = HttpContext.Request.Headers;
            string urlReferrer = headersDictionary[HeaderNames.Referer].ToString();
            string url = "/" + RouteData.Values["Controller"] + "/" + RouteData.Values["Action"] + "/" + RouteData.Values["id"];
            */
            long loginID = LoginController.GetLogin(HttpContext.User); // для настроек по каждому логину в будущем
            MonitoringFilter mf = repo.GetMonitoringFilter(id, loginID);
            return View("MonitoringList", mf);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult MonitorMonitoringList(MonitoringFilter mf)
        {
            /*
            IHeaderDictionary headersDictionary = HttpContext.Request.Headers;
            string urlReferrer = headersDictionary[HeaderNames.Referer].ToString();
            string url = "/" + RouteData.Values["Controller"] + "/" + RouteData.Values["Action"] + "/" + RouteData.Values["id"];
            */
            return View("Views/Shared/Components/MonitorMonitorings/MonitorMonitoringList.cshtml", repo.GetMonitorings(mf));
        }

        //Get
        public IActionResult MonitoringNew(long MonitorID)
        {            
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                // считываем название монитора
                RepositoryMonitor repoMon = new RepositoryMonitor((repo as RepositoryMonitoring).GetConnection()); // TODO: обработать на ошибку
                Monitor mr = repoMon.GetMonitor(MonitorID, loginID);
                // новое измерение
                Monitoring mt = new Monitoring() { MonitorID = MonitorID, MonitorShortName = mr.MonitorShortName };
                return View("MonitoringEdit", mt);
            }
            else
                return BadRequest("Нет аутентификации!"); //// TODO: обработать ошибку аутентификации
        }

    }
}