using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Models;

namespace NotePlot.Components
{
    public class MonitorTotalParams : ViewComponent
    {
        IRepositoryMonitor repo;
        public MonitorTotalParams(IRepositoryMonitor r)
        {
            repo = r;
        }

        public async Task<IViewComponentResult> InvokeAsync(long? MonitorID, long LoginID)
        {
            ViewBag.LoginID = LoginID;
            return View("MonitorTotalParamList", await repo.GetMonitorTotalParametersAsync(MonitorID));
        }

    }
}
