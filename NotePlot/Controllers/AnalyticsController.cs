using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Models;

namespace NotePlot.Controllers
{
    public class AnalyticsController : Controller
    {
        AnalyticTools repo;
        public AnalyticsController(AnalyticTools r)
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
                return PartialView("MonitorParameterValues", await repo.GetReportMonitorDataAsync(MonitorID, loginID, DateFrom, DateTo, Mode)); 
            }
            else
                return BadRequest("Нет аутентификации!");
        }

        public ActionResult ReportPlot()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return View("ReportPlot");//, await repo.GetReportMonitorDataAsync(MonitorID, loginID, DateBegin, DateEnd,Mode));
            }
            else
                return BadRequest("Нет аутентификации!");
        }

        public async Task<ActionResult> SelectMonitorParam()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
            {
                return PartialView("MonitorParamSelect", await repo.GetAnalyticMonitorParamsAsync(loginID));
            }
            else
                return BadRequest("Нет аутентификации!");
        }

    }
}