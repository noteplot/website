using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Models;

namespace NotePlot.Controllers
{
    public class AnaliticsController : Controller
    {
        AnaliticTools repo;
        public AnaliticsController(AnaliticTools r)
        {
            repo = r;
        }

        public ActionResult ReportMonitor()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return View("ReportMonitor");//, await repo.GetReportMonitorDataAsync(MonitorID, loginID, DateBegin, DateEnd,Mode));
            }
            else
                return BadRequest("Нет аутентификации!");
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> MonitorParameterValuesGet(long MonitorID, DateTime? DateFrom, DateTime? DateTo , short Mode = 0)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                return View("", await repo.GetReportMonitorDataAsync(MonitorID, loginID, DateFrom, DateTo, Mode)); 
            }
            else
                return BadRequest("Нет аутентификации!");
        }

    }
}