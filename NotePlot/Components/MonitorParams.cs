using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Models;

namespace NotePlot.Components
{
    public class MonitorParams : ViewComponent
    {
        IRepositoryMonitor repo;
        public MonitorParams(IRepositoryMonitor r)
        {
            repo = r;
        }

        public IViewComponentResult Invoke(long? MonitorID, long LoginID)
        {
            ViewBag.LoginID = LoginID;
            return View("MonitorParamList", repo.GetMonitorParameters(MonitorID));
        }

    }
}
