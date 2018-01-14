using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Models;

namespace NotePlot.Components
{
    public class MonitoringParams : ViewComponent
    {
        IRepositoryMonitoring repo;
        public MonitoringParams(IRepositoryMonitoring r)
        {
            repo = r;
        }
        /*
        public IViewComponentResult Invoke(long? MonitoringID, long MonitorID)
        {
            //ViewBag.LoginID = LoginID;
            return View("MonitoringParamList", repo.GetMonitoringParams(MonitoringID,MonitorID));
        }
        */
        public async Task<IViewComponentResult> InvokeAsync(long? MonitoringID, long MonitorID)
        {
            return View("MonitoringParamList", await repo.GetMonitoringParamsAsync(MonitoringID, MonitorID));
        }

    }
}
