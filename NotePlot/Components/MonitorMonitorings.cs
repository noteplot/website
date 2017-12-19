using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Models;

namespace NotePlot.Components
{
    public class MonitorMonitorings : ViewComponent
    {
        IRepositoryMonitoring repo;
        public MonitorMonitorings(IRepositoryMonitoring r)
        {
            repo = r;
        }

        public IViewComponentResult Invoke(long MonitorID, int tops)
        {
            //ViewBag.LoginID = LoginID;
            return View("MonitorMonitoringList", repo.GetMonitorings(MonitorID,tops));
        }

    }
}
