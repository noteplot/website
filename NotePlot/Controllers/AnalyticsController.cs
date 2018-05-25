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

        [HttpPost]
        // для Get-запросов культура НЕ определяется
        public async Task<ActionResult> MonitorParamPlotDataGet(List<ReportParam> Params, DateTime? DateFrom, DateTime? DateTo)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                string jsonArray = "";
                int id = 1;
                long loginID = LoginController.GetLogin(HttpContext.User);
                var MonitorParamsXML = ToolKit.SerializeToStringXML(Params, "Report");
                try
                {
                    List<ReportJsonArray> jList = await repo.GetReportJsonDataAsync(loginID, MonitorParamsXML, DateFrom, DateTo);
                    // строим массив для Flot
                    if (jList.Count() > 0)
                    {
                        foreach (var ja in jList)
                        {
                            if (id == 1)
                                jsonArray += ja.JsonArray;
                            else
                                jsonArray += ',' + ja.JsonArray;
                            id += 1;
                        }
                        jsonArray = "[" + jsonArray + "]";
                    }
                    return Ok(jsonArray);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                //return Ok("LOAD TEST");                
                //return PartialView("PlotParameterValues", await repo.GetReportPlotDataAsync(loginID, MonitorParamsXML, DateFrom, DateTo));
            }
            else
                return BadRequest("Нет аутентификации!");
        }

    }
}