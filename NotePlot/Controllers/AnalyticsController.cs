using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Models;
using NotePlot.Tools;
using System.Globalization;

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

        [HttpGet]
        // для Get-запросов культура НЕ определяется, поэтому передаем строкой
        public async Task<ActionResult> MonitorParamPlotDataGet(List<long> MonitorParamIDs, string DateFrom, string DateTo)        
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                CultureInfo culture = CultureInfo.CurrentCulture;
                DateTime? dDateFrom, dDateTo;
                DateTime dateResult;
                if (DateTime.TryParse(DateFrom, culture, DateTimeStyles.None, out dateResult))
                    // преобразование строки в DateTime
                    dDateFrom = dateResult;
                else
                {
                    return BadRequest("Неверный формат даты!");
                }
                if (DateTime.TryParse(DateTo, culture, DateTimeStyles.None, out dateResult))
                    // преобразование строки в DateTime
                    dDateTo = dateResult;
                else
                {
                    return BadRequest("Неверный формат даты!");
                }

                long loginID = LoginController.GetLogin(HttpContext.User);
                var MonitorParams = ToolKit.ListToStringXML(MonitorParamIDs, "MonitorParams", "MonitorParamID");
                await repo.GetReportPlotDataAsync(loginID, MonitorParams, dDateFrom, dDateTo);
                return Ok("LOAD TEST");
                //return PartialView("MonitorParameterValues", await repo.GetReportMonitorDataAsync(MonitorID, loginID, DateFrom, DateTo, Mode));
            }
            else
                return BadRequest("Нет аутентификации!");
        }

    }
}